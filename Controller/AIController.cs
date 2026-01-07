using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Controller
{
    public class AIController(GamingQueue queue, GameMap map)
    {
        private readonly GamingQueue _queue = queue;
        private readonly GameMap _map = map;

        /// <summary>
        /// AI的核心决策方法，根据当前游戏状态为角色选择最佳行动
        /// </summary>
        /// <param name="character">当前行动的AI角色</param>
        /// <param name="dp">角色的决策点</param>
        /// <param name="startGrid">角色的起始格子</param>
        /// <param name="allPossibleMoveGrids">从起始格子可达的所有移动格子（包括起始格子本身）</param>
        /// <param name="availableSkills">角色所有可用的技能（已过滤CD和EP/MP）</param>
        /// <param name="availableItems">角色所有可用的物品（已过滤CD和EP/MP）</param>
        /// <param name="allEnemysInGame">场上所有敌人</param>
        /// <param name="allTeammatesInGame">场上所有队友</param>
        /// <param name="selectableEnemys">场上能够选取的敌人</param>
        /// <param name="selectableTeammates">场上能够选取的队友</param>
        /// <returns>包含最佳行动的AIDecision对象</returns>
        public AIDecision DecideAIAction(Character character, DecisionPoints dp, Grid startGrid, List<Grid> allPossibleMoveGrids,
            List<Skill> availableSkills, List<Item> availableItems, List<Character> allEnemysInGame, List<Character> allTeammatesInGame,
            List<Character> selectableEnemys, List<Character> selectableTeammates)
        {
            // 初始化一个默认的“结束回合”决策作为基准
            AIDecision bestDecision = new()
            {
                ActionType = CharacterActionType.EndTurn,
                TargetMoveGrid = startGrid,
                Targets = [],
                Score = -1000.0
            };

            // 遍历所有可能的移动目标格子 (包括起始格子本身)
            foreach (Grid potentialMoveGrid in allPossibleMoveGrids)
            {
                // 计算移动到这个格子的代价（曼哈顿距离）
                int moveDistance = GameMap.CalculateManhattanDistance(startGrid, potentialMoveGrid);
                double movePenalty = moveDistance * 0.5; // 每移动一步扣0.5分

                if (CanCharacterNormalAttack(character, dp))
                {
                    // 计算普通攻击的可达格子
                    List<Grid> normalAttackReachableGrids = _map.GetGridsByRange(potentialMoveGrid, character.ATR, true);

                    List<Character> normalAttackReachableEnemys = [.. allEnemysInGame
                        .Where(c => normalAttackReachableGrids.SelectMany(g => g.Characters).Contains(c) && !c.IsUnselectable && selectableEnemys.Contains(c))
                        .Distinct()];
                    List<Character> normalAttackReachableTeammates = [.. allTeammatesInGame
                        .Where(c => normalAttackReachableGrids.SelectMany(g => g.Characters).Contains(c) && selectableTeammates.Contains(c))
                        .Distinct()];

                    if (normalAttackReachableEnemys.Count > 0)
                    {
                        // 将筛选后的目标列表传递给 SelectTargets
                        List<Character> targets = SelectTargets(character, character.NormalAttack, normalAttackReachableEnemys, normalAttackReachableTeammates);
                        if (targets.Count > 0)
                        {
                            double currentScore = EvaluateNormalAttack(character, targets) - movePenalty;
                            if (currentScore > bestDecision.Score)
                            {
                                bestDecision = new AIDecision
                                {
                                    ActionType = CharacterActionType.NormalAttack,
                                    TargetMoveGrid = potentialMoveGrid,
                                    SkillToUse = character.NormalAttack,
                                    Targets = targets,
                                    Score = currentScore
                                };
                            }
                        }
                    }
                }

                foreach (Skill skill in availableSkills)
                {
                    if (CanCharacterUseSkill(character, skill, dp) && _queue.CheckCanCast(character, skill, out double cost))
                    {
                        // 计算当前技能的可达格子
                        List<Grid> skillReachableGrids = _map.GetGridsByRange(potentialMoveGrid, skill.CastRange, true);

                        if (skill.IsNonDirectional)
                        {
                            AIDecision? nonDirDecision = EvaluateNonDirectionalSkill(character, skill, potentialMoveGrid, skillReachableGrids, allEnemysInGame, allTeammatesInGame, cost);

                            if (nonDirDecision != null && nonDirDecision.Score > bestDecision.Score)
                            {
                                bestDecision = nonDirDecision;
                            }
                        }
                        else
                        {
                            List<Character> skillReachableEnemys = [.. allEnemysInGame
                            .Where(c => skillReachableGrids.SelectMany(g => g.Characters).Contains(c) && !c.IsUnselectable && selectableEnemys.Contains(c))
                            .Distinct()];
                            List<Character> skillReachableTeammates = [.. allTeammatesInGame
                            .Where(c => skillReachableGrids.SelectMany(g => g.Characters).Contains(c) && selectableTeammates.Contains(c))
                            .Distinct()];

                            // 检查是否有可用的目标（敌人或队友，取决于技能类型）
                            if (skillReachableEnemys.Count > 0 || skillReachableTeammates.Count > 0)
                            {
                                // 将筛选后的目标列表传递给 SelectTargets
                                List<Character> targets = SelectTargets(character, skill, skillReachableEnemys, skillReachableTeammates);
                                if (targets.Count > 0)
                                {
                                    double currentScore = EvaluateSkill(character, skill, targets, cost) - movePenalty;
                                    if (currentScore > bestDecision.Score)
                                    {
                                        bestDecision = new AIDecision
                                        {
                                            ActionType = CharacterActionType.PreCastSkill,
                                            TargetMoveGrid = potentialMoveGrid,
                                            SkillToUse = skill,
                                            Targets = targets,
                                            Score = currentScore
                                        };
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Item item in availableItems)
                {
                    if (item.Skills.Active != null && CanCharacterUseItem(character, item, dp) && _queue.CheckCanCast(character, item.Skills.Active, out double cost))
                    {
                        Skill itemSkill = item.Skills.Active;

                        // 计算当前物品技能的可达格子
                        List<Grid> itemSkillReachableGrids = _map.GetGridsByRange(potentialMoveGrid, itemSkill.CastRange, true);

                        if (itemSkill.IsNonDirectional)
                        {
                            AIDecision? nonDirDecision = EvaluateNonDirectionalSkill(character, itemSkill, potentialMoveGrid, itemSkillReachableGrids, allEnemysInGame, allTeammatesInGame, cost);

                            if (nonDirDecision != null && nonDirDecision.Score > bestDecision.Score)
                            {
                                bestDecision = nonDirDecision;
                            }
                        }
                        else
                        {
                            List<Character> itemSkillReachableEnemys = [.. allEnemysInGame
                            .Where(c => itemSkillReachableGrids.SelectMany(g => g.Characters).Contains(c) && !c.IsUnselectable && selectableEnemys.Contains(c))
                            .Distinct()];
                            List<Character> itemSkillReachableTeammates = [.. allTeammatesInGame
                            .Where(c => itemSkillReachableGrids.SelectMany(g => g.Characters).Contains(c) && selectableTeammates.Contains(c))
                            .Distinct()];

                            // 检查是否有可用的目标
                            if (itemSkillReachableEnemys.Count > 0 || itemSkillReachableTeammates.Count > 0)
                            {
                                // 将筛选后的目标列表传递给 SelectTargets
                                List<Character> targetsForItem = SelectTargets(character, itemSkill, itemSkillReachableEnemys, itemSkillReachableTeammates);
                                if (targetsForItem.Count > 0)
                                {
                                    double currentScore = EvaluateItem(character, item, targetsForItem, cost) - movePenalty;
                                    if (currentScore > bestDecision.Score)
                                    {
                                        bestDecision = new AIDecision
                                        {
                                            ActionType = CharacterActionType.UseItem,
                                            TargetMoveGrid = potentialMoveGrid,
                                            ItemToUse = item,
                                            SkillToUse = itemSkill,
                                            Targets = targetsForItem,
                                            Score = currentScore
                                        };
                                    }
                                }
                            }
                        }
                    }
                }

                // 如果从该格子没有更好的行动，但移动本身有价值
                // 只有当当前最佳决策是“结束回合”或分数很低时，才考虑纯粹的移动
                if (potentialMoveGrid != startGrid && bestDecision.Score < 0) // 如果当前最佳决策是负分（即什么都不做）
                {
                    double pureMoveScore = -movePenalty; // 移动本身有代价

                    // 为纯粹移动逻辑重新计算综合可达敌人列表
                    List<Grid> tempAttackGridsForPureMove = _map.GetGridsByRange(potentialMoveGrid, character.ATR, true);
                    List<Grid> tempCastGridsForPureMove = [];
                    foreach (Skill skill in availableSkills)
                    {
                        tempCastGridsForPureMove.AddRange(_map.GetGridsByRange(potentialMoveGrid, skill.CastRange, true));
                    }
                    foreach (Item item in availableItems)
                    {
                        if (item.Skills.Active != null)
                        {
                            tempCastGridsForPureMove.AddRange(_map.GetGridsByRange(potentialMoveGrid, item.Skills.Active.CastRange, true));
                        }
                    }
                    List<Grid> tempAllReachableGridsForPureMove = [.. tempAttackGridsForPureMove.Union(tempCastGridsForPureMove).Distinct()];
                    List<Character> tempCurrentReachableEnemysForPureMove = [.. allEnemysInGame
                    .Where(c => tempAllReachableGridsForPureMove.SelectMany(g => g.Characters).Contains(c) && !c.IsUnselectable && selectableEnemys.Contains(c))
                    .Distinct()];

                    // 如果当前位置无法攻击任何敌人，但地图上还有敌人，尝试向最近的敌人移动
                    if (tempCurrentReachableEnemysForPureMove.Count == 0 && allEnemysInGame.Count > 0) // 使用新计算的列表
                    {
                        Character? target = allEnemysInGame
                            .OrderBy(e => GameMap.CalculateManhattanDistance(potentialMoveGrid, _map.GetCharacterCurrentGrid(e)!))
                            .FirstOrDefault();

                        if (target != null)
                        {
                            Grid? nearestEnemyGrid = _map.GetCharacterCurrentGrid(target);
                            if (nearestEnemyGrid != null)
                            {
                                // 奖励靠近敌人
                                pureMoveScore += (10 - GameMap.CalculateManhattanDistance(potentialMoveGrid, nearestEnemyGrid)) * 0.1;
                            }
                        }
                    }

                    // 如果纯粹移动比当前最佳（什么都不做）更好
                    if (pureMoveScore > bestDecision.Score)
                    {
                        bestDecision = new AIDecision
                        {
                            ActionType = CharacterActionType.Move,
                            TargetMoveGrid = potentialMoveGrid,
                            Targets = [],
                            Score = pureMoveScore,
                            IsPureMove = true
                        };
                    }
                }
            }

            return bestDecision;
        }

        // --- AI 决策辅助方法 ---

        // 检查角色是否能进行普通攻击（基于状态）
        private static bool CanCharacterNormalAttack(Character character, DecisionPoints dp)
        {
            return dp.CheckActionTypeQuota(CharacterActionType.NormalAttack) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostNormalAttack &&
                   character.CharacterState != CharacterState.NotActionable &&
                   character.CharacterState != CharacterState.ActionRestricted &&
                   character.CharacterState != CharacterState.BattleRestricted &&
                   character.CharacterState != CharacterState.AttackRestricted;
        }

        // 检查角色是否能使用某个技能（基于状态）
        private static bool CanCharacterUseSkill(Character character, Skill skill, DecisionPoints dp)
        {
            return ((skill.SkillType == SkillType.Magic && dp.CheckActionTypeQuota(CharacterActionType.PreCastSkill) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostMagic) ||
                    (skill.SkillType == SkillType.Skill && dp.CheckActionTypeQuota(CharacterActionType.CastSkill) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostSkill) ||
                    (skill.SkillType == SkillType.SuperSkill && dp.CheckActionTypeQuota(CharacterActionType.CastSuperSkill)) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostSuperSkill) &&
                   character.CharacterState != CharacterState.NotActionable &&
                   character.CharacterState != CharacterState.ActionRestricted &&
                   character.CharacterState != CharacterState.BattleRestricted &&
                   character.CharacterState != CharacterState.SkillRestricted;
        }

        // 检查角色是否能使用某个物品（基于状态）
        private static bool CanCharacterUseItem(Character character, Item item, DecisionPoints dp)
        {
            return dp.CheckActionTypeQuota(CharacterActionType.UseItem) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostItem &&
                   character.CharacterState != CharacterState.NotActionable &&
                   (character.CharacterState != CharacterState.ActionRestricted || item.ItemType == ItemType.Consumable) && // 行动受限只能用消耗品
                   character.CharacterState != CharacterState.BattleRestricted;
        }

        /// <summary>
        /// 选择技能的最佳目标
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        private static List<Character> SelectTargets(Character character, ISkill skill, List<Character> enemys, List<Character> teammates)
        {
            List<Character> targets = skill.GetSelectableTargets(character, enemys, teammates);
            int count = skill.RealCanSelectTargetCount(enemys, teammates);
            return [.. targets.OrderBy(o => Random.Shared.Next()).Take(count)];
        }

        /// <summary>
        /// 评估普通攻击的价值
        /// </summary>
        /// <param name="character"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private static double EvaluateNormalAttack(Character character, List<Character> targets)
        {
            double score = 0;
            foreach (Character target in targets)
            {
                double damage = character.NormalAttack.Damage * (1 - target.PDR);
                score += damage;
                if (target.HP <= damage) score += 100;
            }
            return score;
        }

        /// <summary>
        /// 评估技能的价值
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        /// <param name="targets"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        private static double EvaluateSkill(Character character, Skill skill, List<Character> targets, double cost)
        {
            double score = 0;
            score += targets.Sum(t => CalculateTargetValue(t, skill));
            //score -= cost * 5;
            //score -= skill.RealCD * 2;
            //score -= skill.HardnessTime * 2;
            return score;
        }

        // 非指向性技能的评估
        private AIDecision? EvaluateNonDirectionalSkill(Character character, Skill skill, Grid moveGrid, List<Grid> castableGrids, List<Character> allEnemys, List<Character> allTeammates, double cost)
        {
            double bestSkillScore = double.NegativeInfinity;
            List<Grid> bestTargetGrids = [];

            // 枚举所有可施放的格子作为潜在中心
            foreach (Grid centerGrid in castableGrids)
            {
                // 计算该中心格子下的实际影响范围格子
                List<Grid> effectGrids = skill.SelectNonDirectionalTargets(character, centerGrid, skill.SelectIncludeCharacterGrid);

                // 计算实际影响的角色
                List<Character> affected = skill.SelectTargetsByRange(character, allEnemys, allTeammates, [], effectGrids);

                if (affected.Count == 0)
                    continue;

                // 评估这些影响目标的价值
                double skillScore = affected.Sum(t => CalculateTargetValue(t, skill));

                if (skillScore > bestSkillScore)
                {
                    bestSkillScore = skillScore;
                    bestTargetGrids = effectGrids;
                }
            }

            if (bestSkillScore == double.NegativeInfinity)
                return null; // 无有效格子

            double movePenalty = GameMap.CalculateManhattanDistance(_map.GetCharacterCurrentGrid(character)!, moveGrid) * 0.5;
            double finalScore = bestSkillScore - movePenalty;

            return new AIDecision
            {
                ActionType = CharacterActionType.PreCastSkill,
                TargetMoveGrid = moveGrid,
                SkillToUse = skill,
                Targets = [],
                TargetGrids = bestTargetGrids,
                Score = finalScore
            };
        }

        /// <summary>
        /// 评估物品的价值
        /// </summary>
        /// <param name="character"></param>
        /// <param name="item"></param>
        /// <param name="targets"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        private static double EvaluateItem(Character character, Item item, List<Character> targets, double cost)
        {
            double score = Random.Shared.Next(1000);
            return score;
        }

        /// <summary>
        /// 辅助函数：计算单个目标在某个技能下的价值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        private static double CalculateTargetValue(Character target, ISkill skill)
        {
            double value = Random.Shared.Next(1000);
            return value;
        }
    }
}
