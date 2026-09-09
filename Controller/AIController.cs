using System.Collections.Concurrent;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;
using FunGame.Core.Model.Queue;

namespace FunGame.Core.Controller
{
    /// <summary>
    /// AI 决策控制器
    /// <para>为 AI 托管角色评估行动与目标，同时可在无外部答复时生成询问答复</para>
    /// <para>战棋地图模式下基于格子射程与可移动格评估；非地图模式（<paramref name="map"/> 为 null）退化为无距离限制、不产生移动的评估</para>
    /// </summary>
    /// <param name="queue">所属的游戏队列</param>
    /// <param name="map">战棋地图，非地图模式传 null</param>
    public class AIController(GamingQueue queue, GameMap? map)
    {
        private readonly GamingQueue _queue = queue;
        private readonly GameMap? _map = map;

        /// <summary>
        /// 控制器当前使用的地图，非地图模式为 null
        /// </summary>
        public GameMap? Map => _map;

        /// <summary>
        /// 是否处于战棋地图模式
        /// </summary>
        public bool IsMapMode => _map != null;

        /// <summary>
        /// 询问答复委托：模组可订阅 <see cref="ResolveInquiryEvent"/> 完全接管 AI 的询问答复
        /// </summary>
        /// <param name="ctx">询问上下文</param>
        /// <returns>返回 null 表示不介入，交由内置的默认规则处理</returns>
        public delegate InquiryResponse? ResolveInquiryDelegate(InquiryContext ctx);

        /// <summary>
        /// AI 询问答复事件：模组可根据询问内容自行给出答复
        /// </summary>
        public static event ResolveInquiryDelegate? ResolveInquiryEvent;

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
        /// <param name="startGrid">角色的起始格子，非战棋模式下为 null</param>
        /// <param name="allPossibleMoveGrids">从起始格子可达的所有移动格子（包括起始格子本身），非战棋模式下为空</param>
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
        public AIDecision DecideAIAction(Character character, DecisionPoints dp, Grid? startGrid, List<Grid> allPossibleMoveGrids,
            List<Skill> availableSkills, List<Item> availableItems, List<Character> allEnemysInGame, List<Character> allTeammatesInGame,
            List<Character> selectableEnemys, List<Character> selectableTeammates, double pUseItem, double pCastSkill, double pNormalAttack)
        {
            // 战棋模式下必须知道角色所在格子才能评估，角色不在地图上时不参与决策
            if (_map != null && startGrid is null)
            {
                return CreateDefaultDecision(startGrid);
            }

            // 非战棋模式没有格子概念，退化为一个虚拟格，保证至少完成一次决策计算
            List<Grid> moveGrids = allPossibleMoveGrids.Count > 0 ? allPossibleMoveGrids : [startGrid ?? Grid.Empty];

            // 评估范围上限：大范围移动无效格子可能较多
            // 对每个可达格做「普攻+技能+物品」全量评估会让单次 AI 决策过重，而失败重试会反复触发整段评估
            // 此处优先保留离敌人更近的候选格
            // 并始终保留当前格作为「原地行动」的基准
            int maxEvaluatedMoveGrids = 32;
            if (_map != null && moveGrids.Count > maxEvaluatedMoveGrids)
            {
                Grid? currentGrid = _map.GetCharacterCurrentGrid(character);
                List<Grid> enemyGrids = [.. allEnemysInGame.Select(_map.GetCharacterCurrentGrid).OfType<Grid>()];
                moveGrids = [.. moveGrids
                    .OrderBy(g => ReferenceEquals(g, currentGrid) ? -1 :
                        (enemyGrids.Count == 0 ? 0 : enemyGrids.Min(eg => GameMap.CalculateManhattanDistance(g, eg))))
                    .Take(maxEvaluatedMoveGrids)];
            }

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

            // 获取偏好行动类型（使用调整并归一化后的概率，与模组调整保持同一口径）
            CharacterActionType? preferredAction = GetPreferredActionType(normalizedPUseItem, normalizedPCastSkill, normalizedPNormalAttack);

            // 初始化一个默认的“结束回合”决策作为基准
            AIDecision bestDecision = CreateDefaultDecision(startGrid);

            // 候选决策
            ConcurrentBag<AIDecision> candidateDecisions = [];

            // 封装单个移动格子的决策计算逻辑为异步任务
            List<Task> decisionTasks = [];
            foreach (Grid potentialMoveGrid in moveGrids)
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
                bestDecision.HasCandidate = true;
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
            Character character, DecisionPoints dp, Grid? startGrid, Grid potentialMoveGrid,
            List<Skill> availableSkills, List<Item> availableItems, List<Character> allEnemysInGame, List<Character> allTeammatesInGame,
            List<Character> selectableEnemys, List<Character> selectableTeammates,
            double normalizedPUseItem, double normalizedPCastSkill, double normalizedPNormalAttack,
            CharacterActionType? preferredAction,
            ConcurrentBag<AIDecision> candidateDecisions)
        {
            // 计算移动惩罚（非战棋模式使用虚拟格，距离为 0）
            int moveDistance = GameMap.CalculateManhattanDistance(startGrid ?? Grid.Empty, potentialMoveGrid);
            double movePenalty = moveDistance * 0.5;

            // 计算普通攻击决策
            if (normalizedPNormalAttack > 0 && CanCharacterNormalAttack(character, dp))
            {
                List<Grid> normalAttackReachableGrids = GetReachableGrids(potentialMoveGrid, character.ATR, true);
                List<Character> normalAttackReachableEnemys = FilterReachableEnemies(normalAttackReachableGrids, allEnemysInGame, selectableEnemys);

                if (normalAttackReachableEnemys.Count > 0)
                {
                    List<Character> targets = SelectTargets(character, character.NormalAttack, allEnemysInGame, allTeammatesInGame, normalAttackReachableEnemys, []);
                    if (targets.Count > 0)
                    {
                        double currentScore = EvaluateNormalAttack(character, targets) - movePenalty;
                        // 概率权重直接采用归一化后的行动概率，使模组调整的概率真实影响排序
                        double probabilityWeight = normalizedPNormalAttack;
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
                        List<Grid> skillReachableGrids = GetReachableGrids(potentialMoveGrid, skill.CastRange, true);

                        // 非战棋模式没有格子中心可枚举，非指向性技能退化为按可选目标直接评估
                        if (skill.IsNonDirectional && _map != null)
                        {
                            AIDecision? nonDirDecision = EvaluateNonDirectionalSkill(
                                character, skill, potentialMoveGrid, skillReachableGrids,
                                allEnemysInGame, allTeammatesInGame, cost, normalizedPCastSkill);

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
                            List<Character> skillReachableEnemys = FilterReachableEnemies(skillReachableGrids, allEnemysInGame, selectableEnemys);
                            List<Character> skillReachableTeammates = FilterReachableTeammates(skillReachableGrids, allTeammatesInGame, selectableTeammates);

                            if (skillReachableEnemys.Count > 0 || skillReachableTeammates.Count > 0)
                            {
                                List<Character> targets = SelectTargets(character, skill, allEnemysInGame, allTeammatesInGame, skillReachableEnemys, skillReachableTeammates);
                                if (targets.Count > 0)
                                {
                                    double currentScore = EvaluateSkill(character, skill, targets, cost) - movePenalty;
                                    // 概率权重直接采用归一化后的行动概率，使模组调整的概率真实影响排序
                                    double probabilityWeight = normalizedPCastSkill;
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
                        List<Grid> itemSkillReachableGrids = GetReachableGrids(potentialMoveGrid, itemSkill.CastRange, true);

                        // 非战棋模式没有格子中心可枚举，非指向性技能退化为按可选目标直接评估
                        if (itemSkill.IsNonDirectional && _map != null)
                        {
                            AIDecision? nonDirDecision = EvaluateNonDirectionalSkill(
                                character, itemSkill, potentialMoveGrid, itemSkillReachableGrids,
                                allEnemysInGame, allTeammatesInGame, cost, normalizedPUseItem);

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
                            List<Character> itemSkillReachableEnemys = FilterReachableEnemies(itemSkillReachableGrids, allEnemysInGame, selectableEnemys);
                            List<Character> itemSkillReachableTeammates = FilterReachableTeammates(itemSkillReachableGrids, allTeammatesInGame, selectableTeammates);

                            if (itemSkillReachableEnemys.Count > 0 || itemSkillReachableTeammates.Count > 0)
                            {
                                List<Character> targetsForItem = SelectTargets(character, itemSkill, allEnemysInGame, allTeammatesInGame, itemSkillReachableEnemys, itemSkillReachableTeammates);
                                if (targetsForItem.Count > 0)
                                {
                                    double currentScore = EvaluateItem(character, item, targetsForItem, cost) - movePenalty;
                                    // 概率权重直接采用归一化后的行动概率，使模组调整的概率真实影响排序
                                    double probabilityWeight = normalizedPUseItem;
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

            // 计算纯移动决策，仅战棋模式存在移动概念
            if (_map != null && !ReferenceEquals(potentialMoveGrid, startGrid))
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

        // 获取偏好行动类型（传入的是归一化后的概率，总和为 1，占比过半即视为偏好）
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

                // 如果最高概率占比过半，优先考虑该类型
                if (maxProb >= 0.5)
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
            // 技能受限时，Reward 类型的技能不受限制
            && (character.CharacterState != CharacterState.SkillRestricted || skill.Source == SkillSource.Reward);
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
        private static List<Character> SelectTargets(Character character, ISkill skill, List<Character> allEnemys, List<Character> allTeammates, List<Character> enemys, List<Character> teammates)
        {
            List<Character> targets = skill.GetSelectableTargets(character, allEnemys, allTeammates, enemys, teammates);
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
        private AIDecision? EvaluateNonDirectionalSkill(Character character, Skill skill, Grid moveGrid, List<Grid> castableGrids, List<Character> allEnemys, List<Character> allTeammates, double cost, double probabilityWeight)
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

            double movePenalty = GameMap.CalculateManhattanDistance(_map?.GetCharacterCurrentGrid(character) ?? Grid.Empty, moveGrid) * 0.5;
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
                ProbabilityWeight = probabilityWeight // 与指向性技能一致，采用归一化后的行动概率
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

        /// <summary>
        /// 构造无候选的占位决策，表示 AI 未能评估出任何可行行动，交由宿主的回退逻辑处理
        /// </summary>
        /// <param name="startGrid">角色的起始格子，非战棋模式为 null</param>
        private static AIDecision CreateDefaultDecision(Grid? startGrid) => new()
        {
            ActionType = CharacterActionType.EndTurn,
            TargetMoveGrid = startGrid,
            Targets = [],
            Score = -1000.0,
            HasCandidate = false
        };

        /// <summary>
        /// 获取指定射程内的格子；非战棋模式没有格子概念，返回空列表
        /// </summary>
        /// <param name="from">中心格</param>
        /// <param name="range">射程</param>
        /// <param name="includeCharacter">是否包含角色所在的格子</param>
        private List<Grid> GetReachableGrids(Grid? from, int range, bool includeCharacter)
        {
            return _map != null && from != null ? _map.GetGridsByRange(from, range, includeCharacter) : [];
        }

        /// <summary>
        /// 过滤出实际可选的敌人
        /// <para>战棋模式按射程内的格子裁剪；非战棋模式无距离限制，直接取可选取全集</para>
        /// </summary>
        /// <param name="reachableGrids">射程内的格子</param>
        /// <param name="candidates">候选敌人</param>
        /// <param name="selectable">可选取的敌人</param>
        private List<Character> FilterReachableEnemies(List<Grid> reachableGrids, List<Character> candidates, List<Character> selectable)
        {
            HashSet<Character> selectableSet = [.. selectable];
            if (_map is null)
            {
                return [.. candidates.Where(c => !c.IsUnselectable && selectableSet.Contains(c)).Distinct()];
            }
            HashSet<Character> onGrids = [.. reachableGrids.SelectMany(g => g.Characters)];
            return [.. candidates.Where(c => onGrids.Contains(c) && !c.IsUnselectable && selectableSet.Contains(c)).Distinct()];
        }

        /// <summary>
        /// 过滤出实际可选的队友
        /// <para>战棋模式按射程内的格子裁剪；非战棋模式无距离限制，直接取可选取全集</para>
        /// </summary>
        /// <param name="reachableGrids">射程内的格子</param>
        /// <param name="candidates">候选队友</param>
        /// <param name="selectable">可选取的队友</param>
        private List<Character> FilterReachableTeammates(List<Grid> reachableGrids, List<Character> candidates, List<Character> selectable)
        {
            HashSet<Character> selectableSet = [.. selectable];
            if (_map is null)
            {
                return [.. candidates.Where(selectableSet.Contains).Distinct()];
            }
            HashSet<Character> onGrids = [.. reachableGrids.SelectMany(g => g.Characters)];
            return [.. candidates.Where(c => onGrids.Contains(c) && selectableSet.Contains(c)).Distinct()];
        }

        /// <summary>
        /// 为 AI 托管角色生成询问答复
        /// <para>模组可通过 <see cref="ResolveInquiryEvent"/> 完全接管；未接管时按询问选项内置的默认规则构造答复，保证答复一定合法</para>
        /// </summary>
        /// <param name="ctx">询问上下文</param>
        /// <returns>AI 给出的答复；非 AI 托管角色返回 null，交给框架的默认规则</returns>
        public InquiryResponse? ResolveInquiry(InquiryContext ctx)
        {
            Character? character = ctx.Trigger;
            if (character is null || !_queue.IsCharacterInAIControlling(character))
            {
                return null;
            }

            if (ResolveInquiryEvent?.Invoke(ctx) is InquiryResponse response)
            {
                // 模组接管给出的答复归为自定义来源，避免回放时误显示成 AI 决策
                response.Source = InquiryResponseSource.Custom;
                return response;
            }

            return new InquiryResponse(ctx.Options, InquiryResponseSource.AI);
        }
    }
}
