using System.Collections.Concurrent;
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

        public delegate double EvaluateSkillDelegate(Character character, Skill skill, List<Character> targets, double cost);
        public delegate double EvaluateNormalAttackDelegate(Character character, NormalAttack normalAttack, List<Character> targets);
        public delegate double EvaluateNonDirectionalSkillDelegate(Character character, Skill skill, Grid moveGrid, List<Grid> castableGrids, List<Character> allEnemys, List<Character> allTeammates, double cost);
        public delegate double EvaluateItemDelegate(Character character, Item item, List<Character> targets, double cost);
        public delegate double CalculateTargetValueDelegate(Character character, ISkill skill);
        public static event EvaluateSkillDelegate? EvaluateSkillEvent;
        public static event EvaluateNormalAttackDelegate? EvaluateNormalAttackEvent;
        public static event EvaluateNonDirectionalSkillDelegate? EvaluateNonDirectionalSkillEvent;
        public static event EvaluateItemDelegate? EvaluateItemEvent;
        public static event CalculateTargetValueDelegate? CalculateTargetValueEvent;

        /// <summary>
        /// 核心决策方法：外部同步，内部异步并行计算所有独立决策的分数
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
        /// <param name="pUseItem">使用物品的概率</param>
        /// <param name="pCastSkill">释放技能的概率</param>
        /// <param name="pNormalAttack">普通攻击的概率</param>
        /// <returns>包含最佳行动的AIDecision对象</returns>
        public AIDecision DecideAIAction(Character character, DecisionPoints dp, Grid startGrid, List<Grid> allPossibleMoveGrids,
            List<Skill> availableSkills, List<Item> availableItems, List<Character> allEnemysInGame, List<Character> allTeammatesInGame,
            List<Character> selectableEnemys, List<Character> selectableTeammates, double pUseItem, double pCastSkill, double pNormalAttack)
        {
            // 控制最大并发数
            int maxConcurrency = Math.Max(1, Environment.ProcessorCount / 2);
            SemaphoreSlim semaphore = new(maxConcurrency);

            // 动态调整概率
            double dynamicPUseItem = pUseItem;
            double dynamicPCastSkill = pCastSkill;
            double dynamicPNormalAttack = pNormalAttack;
            AdjustProbabilitiesBasedOnContext(ref dynamicPUseItem, ref dynamicPCastSkill, ref dynamicPNormalAttack, character, allEnemysInGame, allTeammatesInGame);

            // 归一化概率
            double[] normalizedProbs = NormalizeProbabilities(dynamicPUseItem, dynamicPCastSkill, dynamicPNormalAttack);
            double normalizedPUseItem = normalizedProbs[0];
            double normalizedPCastSkill = normalizedProbs[1];
            double normalizedPNormalAttack = normalizedProbs[2];

            // 获取偏好行动类型
            CharacterActionType? preferredAction = GetPreferredActionType(pUseItem, pCastSkill, pNormalAttack);

            // 初始化一个默认的“结束回合”决策作为基准
            AIDecision bestDecision = new()
            {
                ActionType = CharacterActionType.EndTurn,
                TargetMoveGrid = startGrid,
                Targets = [],
                Score = -1000.0
            };

            // 候选决策
            ConcurrentBag<AIDecision> candidateDecisions = [];

            // 封装单个移动格子的决策计算逻辑为异步任务
            List<Task> decisionTasks = [];
            foreach (Grid potentialMoveGrid in allPossibleMoveGrids)
            {
                // 捕获循环变量（避免闭包陷阱）
                Grid currentMoveGrid = potentialMoveGrid;
                Task task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        CalculateDecisionForGrid(
                            character, dp, startGrid, currentMoveGrid,
                            availableSkills, availableItems, allEnemysInGame, allTeammatesInGame,
                            selectableEnemys, selectableTeammates,
                            normalizedPUseItem, normalizedPCastSkill, normalizedPNormalAttack,
                            preferredAction, candidateDecisions
                        );
                    }
                    catch (Exception ex)
                    {
                        // 单个格子的计算异常不影响整体，记录日志
                        Console.WriteLine($"计算格子[{currentMoveGrid.X},{currentMoveGrid.Y}]决策失败：{ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                decisionTasks.Add(task);
            }

            // 等待所有异步任务完成
            Task.WaitAll(decisionTasks);

            // 从所有候选决策中选出最高分的（保留原有评分权重逻辑）
            if (!candidateDecisions.IsEmpty)
            {
                bestDecision = candidateDecisions
                    .OrderByDescending(d => d.Score * d.ProbabilityWeight)
                    .FirstOrDefault() ?? bestDecision;
            }

            return bestDecision;
        }

        /// <summary>
        /// 异步执行的核心：计算单个移动格子的所有可能决策，并添加到线程安全容器
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dp"></param>
        /// <param name="startGrid"></param>
        /// <param name="potentialMoveGrid"></param>
        /// <param name="availableSkills"></param>
        /// <param name="availableItems"></param>
        /// <param name="allEnemysInGame"></param>
        /// <param name="allTeammatesInGame"></param>
        /// <param name="selectableEnemys"></param>
        /// <param name="selectableTeammates"></param>
        /// <param name="normalizedPUseItem"></param>
        /// <param name="normalizedPCastSkill"></param>
        /// <param name="normalizedPNormalAttack"></param>
        /// <param name="preferredAction"></param>
        /// <param name="candidateDecisions"></param>
        private void CalculateDecisionForGrid(
            Character character, DecisionPoints dp, Grid startGrid, Grid potentialMoveGrid,
            List<Skill> availableSkills, List<Item> availableItems, List<Character> allEnemysInGame, List<Character> allTeammatesInGame,
            List<Character> selectableEnemys, List<Character> selectableTeammates,
            double normalizedPUseItem, double normalizedPCastSkill, double normalizedPNormalAttack,
            CharacterActionType? preferredAction,
            ConcurrentBag<AIDecision> candidateDecisions)
        {
            // 计算移动惩罚
            int moveDistance = GameMap.CalculateManhattanDistance(startGrid, potentialMoveGrid);
            double movePenalty = moveDistance * 0.5;

            // 计算普通攻击决策
            if (normalizedPNormalAttack > 0 && CanCharacterNormalAttack(character, dp))
            {
                List<Grid> normalAttackReachableGrids = _map.GetGridsByRange(potentialMoveGrid, character.ATR, true);
                List<Character> normalAttackReachableEnemys = [.. allEnemysInGame.Where(c =>
                    normalAttackReachableGrids.SelectMany(g => g.Characters).Contains(c)
                    && !c.IsUnselectable
                    && selectableEnemys.Contains(c)).Distinct()];

                if (normalAttackReachableEnemys.Count > 0)
                {
                    List<Character> targets = SelectTargets(character, character.NormalAttack, normalAttackReachableEnemys, []);
                    if (targets.Count > 0)
                    {
                        double currentScore = EvaluateNormalAttack(character, targets) - movePenalty;
                        double probabilityWeight = 1.0 + (normalizedPNormalAttack * 0.3);
                        AIDecision attackDecision = new()
                        {
                            ActionType = CharacterActionType.NormalAttack,
                            TargetMoveGrid = potentialMoveGrid,
                            SkillToUse = character.NormalAttack,
                            Targets = targets,
                            Score = currentScore,
                            ProbabilityWeight = probabilityWeight
                        };
                        // 偏好类型加分
                        if (preferredAction.HasValue && attackDecision.ActionType == preferredAction.Value)
                        {
                            attackDecision.Score *= 1.2;
                        }
                        candidateDecisions.Add(attackDecision);
                    }
                }
            }

            // 计算技能释放决策
            if (normalizedPCastSkill > 0)
            {
                foreach (Skill skill in availableSkills)
                {
                    if (CanCharacterUseSkill(character, skill, dp) && _queue.CheckCanCast(character, skill, out double cost))
                    {
                        List<Grid> skillReachableGrids = _map.GetGridsByRange(potentialMoveGrid, skill.CastRange, true);

                        if (skill.IsNonDirectional)
                        {
                            AIDecision? nonDirDecision = EvaluateNonDirectionalSkill(
                                character, skill, potentialMoveGrid, skillReachableGrids,
                                allEnemysInGame, allTeammatesInGame, cost);

                            if (nonDirDecision != null)
                            {
                                // 偏好类型加分
                                if (preferredAction.HasValue && nonDirDecision.ActionType == preferredAction.Value)
                                {
                                    nonDirDecision.Score *= 1.2;
                                }
                                candidateDecisions.Add(nonDirDecision);
                            }
                        }
                        else
                        {
                            List<Character> skillReachableEnemys = [.. allEnemysInGame.Where(c =>
                                skillReachableGrids.SelectMany(g => g.Characters).Contains(c)
                                && !c.IsUnselectable
                                && selectableEnemys.Contains(c)).Distinct()];
                            List<Character> skillReachableTeammates = [.. allTeammatesInGame.Where(c =>
                                skillReachableGrids.SelectMany(g => g.Characters).Contains(c)
                                && selectableTeammates.Contains(c)).Distinct()];

                            if (skillReachableEnemys.Count > 0 || skillReachableTeammates.Count > 0)
                            {
                                List<Character> targets = SelectTargets(character, skill, skillReachableEnemys, skillReachableTeammates);
                                if (targets.Count > 0)
                                {
                                    double currentScore = EvaluateSkill(character, skill, targets, cost) - movePenalty;
                                    double probabilityWeight = 1.0 + (normalizedPCastSkill * 0.3);
                                    AIDecision skillDecision = new()
                                    {
                                        ActionType = CharacterActionType.PreCastSkill,
                                        TargetMoveGrid = potentialMoveGrid,
                                        SkillToUse = skill,
                                        Targets = targets,
                                        Score = currentScore,
                                        ProbabilityWeight = probabilityWeight
                                    };
                                    // 偏好类型加分
                                    if (preferredAction.HasValue && skillDecision.ActionType == preferredAction.Value)
                                    {
                                        skillDecision.Score *= 1.2;
                                    }
                                    candidateDecisions.Add(skillDecision);
                                }
                            }
                        }
                    }
                }
            }

            // 计算物品使用决策
            if (normalizedPUseItem > 0)
            {
                foreach (Item item in availableItems)
                {
                    if (item.Skills.Active != null && CanCharacterUseItem(character, item, dp) && _queue.CheckCanCast(character, item.Skills.Active, out double cost))
                    {
                        Skill itemSkill = item.Skills.Active;
                        List<Grid> itemSkillReachableGrids = _map.GetGridsByRange(potentialMoveGrid, itemSkill.CastRange, true);

                        if (itemSkill.IsNonDirectional)
                        {
                            AIDecision? nonDirDecision = EvaluateNonDirectionalSkill(
                                character, itemSkill, potentialMoveGrid, itemSkillReachableGrids,
                                allEnemysInGame, allTeammatesInGame, cost);

                            if (nonDirDecision != null)
                            {
                                // 偏好类型加分
                                if (preferredAction.HasValue && nonDirDecision.ActionType == preferredAction.Value)
                                {
                                    nonDirDecision.Score *= 1.2;
                                }
                                candidateDecisions.Add(nonDirDecision);
                            }
                        }
                        else
                        {
                            List<Character> itemSkillReachableEnemys = [.. allEnemysInGame.Where(c =>
                                itemSkillReachableGrids.SelectMany(g => g.Characters).Contains(c)
                                && !c.IsUnselectable
                                && selectableEnemys.Contains(c)).Distinct()];
                            List<Character> itemSkillReachableTeammates = [.. allTeammatesInGame.Where(c =>
                                itemSkillReachableGrids.SelectMany(g => g.Characters).Contains(c)
                                && selectableTeammates.Contains(c)).Distinct()];

                            if (itemSkillReachableEnemys.Count > 0 || itemSkillReachableTeammates.Count > 0)
                            {
                                List<Character> targetsForItem = SelectTargets(character, itemSkill, itemSkillReachableEnemys, itemSkillReachableTeammates);
                                if (targetsForItem.Count > 0)
                                {
                                    double currentScore = EvaluateItem(character, item, targetsForItem, cost) - movePenalty;
                                    double probabilityWeight = 1.0 + (normalizedPUseItem * 0.3);
                                    AIDecision itemDecision = new()
                                    {
                                        ActionType = CharacterActionType.UseItem,
                                        TargetMoveGrid = potentialMoveGrid,
                                        ItemToUse = item,
                                        SkillToUse = itemSkill,
                                        Targets = targetsForItem,
                                        Score = currentScore,
                                        ProbabilityWeight = probabilityWeight
                                    };
                                    // 偏好类型加分
                                    if (preferredAction.HasValue && itemDecision.ActionType == preferredAction.Value)
                                    {
                                        itemDecision.Score *= 1.2;
                                    }
                                    candidateDecisions.Add(itemDecision);
                                }
                            }
                        }
                    }
                }
            }

            // 计算纯移动决策
            if (potentialMoveGrid != startGrid)
            {
                double pureMoveScore = -movePenalty;

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
                List<Character> tempCurrentReachableEnemysForPureMove = [.. allEnemysInGame.Where(c =>
                    tempAllReachableGridsForPureMove.SelectMany(g => g.Characters).Contains(c)
                    && !c.IsUnselectable
                    && selectableEnemys.Contains(c)).Distinct()];

                if (tempCurrentReachableEnemysForPureMove.Count == 0 && allEnemysInGame.Count > 0)
                {
                    Character? target = allEnemysInGame.OrderBy(e =>
                        GameMap.CalculateManhattanDistance(potentialMoveGrid, _map.GetCharacterCurrentGrid(e) ?? Grid.Empty)).FirstOrDefault();

                    if (target != null)
                    {
                        Grid? nearestEnemyGrid = _map.GetCharacterCurrentGrid(target);
                        if (nearestEnemyGrid != null)
                        {
                            pureMoveScore += (10 - GameMap.CalculateManhattanDistance(potentialMoveGrid, nearestEnemyGrid)) * 0.1;
                        }
                    }
                }

                AIDecision moveDecision = new()
                {
                    ActionType = CharacterActionType.Move,
                    TargetMoveGrid = potentialMoveGrid,
                    Targets = [],
                    Score = pureMoveScore,
                    IsPureMove = true,
                    ProbabilityWeight = 1.0 // 纯移动无概率权重
                };
                // 偏好类型加分（如果移动是偏好类型）
                if (preferredAction.HasValue && moveDecision.ActionType == preferredAction.Value)
                {
                    moveDecision.Score *= 1.2;
                }
                candidateDecisions.Add(moveDecision);
            }
        }

        // --- AI 决策辅助方法 ---

        // 获取偏好行动类型
        private static CharacterActionType? GetPreferredActionType(double pItem, double pSkill, double pAttack)
        {
            // 找出最高概率的行动类型
            Dictionary<CharacterActionType, double> probabilities = new()
            {
                { CharacterActionType.UseItem, pItem },
                { CharacterActionType.PreCastSkill, pSkill },
                { CharacterActionType.NormalAttack, pAttack }
            };

            double maxProb = probabilities.Values.Max();
            if (maxProb > 0)
            {
                CharacterActionType preferredType = probabilities.FirstOrDefault(kvp => kvp.Value == maxProb).Key;

                // 如果最高概率超过阈值，优先考虑该类型
                if (maxProb > 0.7)
                {
                    return preferredType;
                }
            }

            return null;
        }

        // 动态调整概率
        private static void AdjustProbabilitiesBasedOnContext(ref double pUseItem, ref double pCastSkill, ref double pNormalAttack, Character character, List<Character> allEnemysInGame, List<Character> allTeammatesInGame)
        {
            // 基于角色状态调整
            if (character.HP / character.MaxHP < 0.3 || allTeammatesInGame.Any(c => c.HP / c.MaxHP < 0.3))
            {
                // 低生命值时增加使用物品概率
                if (pUseItem > 0) pUseItem *= 1.5;
                if (pCastSkill > 0) pCastSkill *= 0.7;
            }

            // 基于敌人数量调整
            int enemyCount = allEnemysInGame.Count;
            if (enemyCount > 3)
            {
                // 敌人多时倾向于范围技能
                if (pCastSkill > 0) pCastSkill *= 1.3;
            }
            else if (enemyCount == 1)
            {
                // 单个敌人时倾向于普通攻击
                if (pNormalAttack > 0) pNormalAttack *= 1.2;
            }

            if (pUseItem > 0) pUseItem = Math.Max(0, pUseItem);
            if (pCastSkill > 0) pCastSkill = Math.Max(0, pCastSkill);
            if (pNormalAttack > 0) pNormalAttack = Math.Max(0, pNormalAttack);
        }

        // / 归一化概率分布
        private static double[] NormalizeProbabilities(double pUseItem, double pCastSkill, double pNormalAttack)
        {
            if (pUseItem <= 0 && pCastSkill <= 0 && pNormalAttack <= 0)
            {
                return [0, 0, 0];
            }

            double sum = pUseItem + pCastSkill + pNormalAttack;
            if (sum <= 0) return [0.33, 0.33, 0.34];

            return [
                pUseItem / sum,
                pCastSkill / sum,
                pNormalAttack / sum
            ];
        }

        // 检查角色是否能进行普通攻击（基于状态）
        private static bool CanCharacterNormalAttack(Character character, DecisionPoints dp)
        {
            return dp.CheckActionTypeQuota(CharacterActionType.NormalAttack)
                && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostNormalAttack
                && character.CharacterState != CharacterState.NotActionable
                && character.CharacterState != CharacterState.ActionRestricted
                && character.CharacterState != CharacterState.BattleRestricted
                && character.CharacterState != CharacterState.AttackRestricted;
        }

        // 检查角色是否能使用某个技能（基于状态）
        private static bool CanCharacterUseSkill(Character character, Skill skill, DecisionPoints dp)
        {
            return (
                (skill.SkillType == SkillType.Magic && dp.CheckActionTypeQuota(CharacterActionType.PreCastSkill) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostMagic)
                || (skill.SkillType == SkillType.Skill && dp.CheckActionTypeQuota(CharacterActionType.CastSkill) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostSkill)
                || (skill.SkillType == SkillType.SuperSkill && dp.CheckActionTypeQuota(CharacterActionType.CastSuperSkill) && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostSuperSkill)
            )
            && character.CharacterState != CharacterState.NotActionable
            && character.CharacterState != CharacterState.ActionRestricted
            && character.CharacterState != CharacterState.BattleRestricted
            && character.CharacterState != CharacterState.SkillRestricted;
        }

        // 检查角色是否能使用某个物品（基于状态）
        private static bool CanCharacterUseItem(Character character, Item item, DecisionPoints dp)
        {
            return dp.CheckActionTypeQuota(CharacterActionType.UseItem)
                && dp.CurrentDecisionPoints > dp.GameplayEquilibriumConstant.DecisionPointsCostItem
                && character.CharacterState != CharacterState.NotActionable
                && (character.CharacterState != CharacterState.ActionRestricted || item.ItemType == ItemType.Consumable)
                && character.CharacterState != CharacterState.BattleRestricted;
        }

        // 选择技能的最佳目标
        private static List<Character> SelectTargets(Character character, ISkill skill, List<Character> enemys, List<Character> teammates)
        {
            List<Character> targets = skill.GetSelectableTargets(character, enemys, teammates);
            int count = skill.RealCanSelectTargetCount(enemys, teammates);
            return [.. targets.OrderBy(o => Random.Shared.Next()).Take(count)];
        }

        // 评估普通攻击的价值
        private static double EvaluateNormalAttack(Character character, List<Character> targets)
        {
            double score = 0;
            foreach (Character target in targets)
            {
                double damage = character.NormalAttack.Damage * (1 - target.PDR);
                score += damage;
                if (target.HP <= damage) score += 100;
            }
            score += EvaluateNormalAttackEvent?.Invoke(character, character.NormalAttack, targets) ?? 0;
            return score;
        }

        // 评估技能的价值
        private static double EvaluateSkill(Character character, Skill skill, List<Character> targets, double cost)
        {
            double score = 0;
            score += targets.Sum(t => CalculateTargetValue(t, skill));
            score += EvaluateSkillEvent?.Invoke(character, skill, targets, cost) ?? 0;
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
            finalScore += EvaluateNonDirectionalSkillEvent?.Invoke(character, skill, moveGrid, castableGrids, allEnemys, allTeammates, cost) ?? 0;

            return new AIDecision
            {
                ActionType = CharacterActionType.PreCastSkill,
                TargetMoveGrid = moveGrid,
                SkillToUse = skill,
                Targets = [],
                TargetGrids = bestTargetGrids,
                Score = finalScore,
                ProbabilityWeight = 1.0 // 非指向性技能默认权重
            };
        }

        // 评估物品的价值
        private static double EvaluateItem(Character character, Item item, List<Character> targets, double cost)
        {
            double score = Random.Shared.Next(1000);
            score += EvaluateItemEvent?.Invoke(character, item, targets, cost) ?? 0;
            return score;
        }

        // 辅助函数：计算单个目标在某个技能下的价值
        private static double CalculateTargetValue(Character target, ISkill skill)
        {
            double value = Random.Shared.Next(1000);
            value += CalculateTargetValueEvent?.Invoke(target, skill) ?? 0;
            return value;
        }
    }
}
