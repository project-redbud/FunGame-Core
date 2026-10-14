using FunGame.Core.Api;
using FunGame.Core.Controller;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.Queue
{
    public partial class GamingQueue
    {
        private Character? _chainTurnActor;
        private Guid _chainTurnId;
        private bool _executingChainAction;
        private readonly Dictionary<Guid, ChainActionRequest> _chainRequests = [];
        private readonly Dictionary<Character, ChainCastContext> _chainCasts = [];

        private DecisionPoints CreateInitialDecisionPoints() => new()
        {
            GameplayEquilibriumConstant = GameplayEquilibriumConstant,
            CurrentDecisionPoints = GameplayEquilibriumConstant.InitialDecisionPoints,
            MaxDecisionPoints = GameplayEquilibriumConstant.InitialDecisionPoints,
            RecoverDecisionPointsPerRound = GameplayEquilibriumConstant.RecoverDecisionPointsPerRound
        };

        /// <summary>
        /// 由当前角色选择一份申请；返回 null 取消。AI 无选择时使用第一份有效申请
        /// </summary>
        public event Func<Character, IReadOnlyList<ChainActionRequest>, Guid?>? SelectChainActionEvent;

        /// <summary>当前正常回合标识。协同吟唱结算回合不开放申请。</summary>
        public Guid? ChainActionTurnId => _chainTurnActor is null ? null : _chainTurnId;

        /// <summary>
        /// 当前角色后方连续同队区间内可行动的协作方。同队控制态占位但不能提交
        /// </summary>
        public IReadOnlyList<Character> GetAvailableChainActors(Character turnActor)
        {
            if (!IsChainTurn(turnActor)) return [];
            List<Character> result = [];
            // NextCharacter 已将回合角色出队；同时兼容直接调用 ProcessTurn 的宿主
            int start = _queue.IndexOf(turnActor) + 1;
            foreach (Character actor in _queue.Skip(start))
            {
                if (!AreChainTeammates(turnActor, actor)) break;
                if (actor.HP > 0 && actor.CharacterState == CharacterState.Actionable && !_chainCasts.ContainsKey(actor)) result.Add(actor);
            }
            return result.AsReadOnly();
        }

        /// <summary>
        /// 在 AI 正常决策前，为连续同队区间内的队友代交原地协同申请，保留已有有效申请
        /// </summary>
        /// <param name="turnActor">当前正常回合的 AI 角色</param>
        /// <param name="allTeammates">回合角色的队友</param>
        /// <param name="allEnemies">回合角色的敌人</param>
        protected void WillSubmitChainActions(Character turnActor, List<Character> allTeammates, List<Character> allEnemies)
        {
            if (!IsChainTurn(turnActor) || !IsCharacterInAIControlling(turnActor) || AI is not AIController ai) return;
            HashSet<Character> pendingActors = [.. GetPendingChainActions(turnActor).Select(r => r.Actor)];
            foreach (Character actor in GetAvailableChainActors(turnActor))
            {
                if (pendingActors.Contains(actor) || !_decisionPoints.TryGetValue(actor, out DecisionPoints? dp) || dp.CurrentDecisionPoints <= 0) continue;
                List<Character> teammates = [.. allTeammates.Where(c => c != actor), turnActor];
                List<Character> enemies = [.. allEnemies.Where(c => c != actor)];
                (List<Character> selectableTeammates, List<Character> selectableEnemies, List<Skill> skills, List<Item> items) = GetTurnStartNeedyList(actor, teammates, enemies);
                // 当前回合角色已出队，仍可作为协同治疗等辅助动作的目标。
                if (!turnActor.IsUnselectable && !selectableTeammates.Contains(turnActor)) selectableTeammates.Add(turnActor);
                actor.NormalAttack.GamingQueue = this;
                foreach (Skill availableSkill in skills.Concat(items.Select(i => i.Skills.Active!))) availableSkill.GamingQueue = this;
                Grid? origin = _map?.GetCharacterCurrentGrid(actor);
                // 协同不允许移动；仅评估协作方当前格，避免生成移动后才能执行的申请。
                AIDecision decision = ai.DecideAIAction(actor, dp, origin, origin is null ? [] : [origin],
                    skills, items, enemies, teammates, selectableEnemies, selectableTeammates, 0.33, 0.33, 0.34, isChainAction: true);
                if (!decision.HasCandidate) continue;
                Skill? skill = decision.SkillToUse as Skill;
                Item? item = decision.ItemToUse ?? items.FirstOrDefault(i => i.Skills.Active == skill);
                CharacterActionType type = item != null ? CharacterActionType.UseItem : skill?.SkillType switch
                {
                    SkillType.Skill => CharacterActionType.CastSkill,
                    SkillType.Magic => CharacterActionType.PreCastSkill,
                    SkillType.SuperSkill => CharacterActionType.CastSuperSkill,
                    _ => decision.ActionType
                };
                if (type == CharacterActionType.NormalAttack) skill = null;
                SubmitChainAction(turnActor, actor, type, decision.Targets, out _, skill, item, decision.TargetGrids, decision.CastCenter);
            }
        }

        private bool AreChainTeammates(Character a, Character b)
        {
            Character rootA = a, rootB = b;
            HashSet<Character> visited = [];
            while (rootA.Master is Character master && visited.Add(rootA)) rootA = master;
            visited.Clear();
            while (rootB.Master is Character master && visited.Add(rootB)) rootB = master;
            return rootA == rootB || IsTeammate(rootA, rootB);
        }

        private bool IsChainTurn(Character actor) => _isInRound && !_isGameEnd && _chainTurnActor == actor &&
            actor.HP > 0 && actor.CharacterState == CharacterState.Actionable && !_chainCasts.ContainsKey(actor);

        /// <summary>
        /// 返回仍符合资格的申请。失效申请立即移除，不会在队列再次变化后复活
        /// </summary>
        public IReadOnlyList<ChainActionRequest> GetPendingChainActions(Character turnActor)
        {
            foreach (ChainActionRequest request in _chainRequests.Values.ToArray())
            {
                if (ValidateChainAction(request, out _, out _) != "") _chainRequests.Remove(request.Id);
            }
            return Array.AsReadOnly(_chainRequests.Values.Where(r => r.TurnActor == turnActor).ToArray());
        }

        /// <summary>
        /// 提交或替换该协作方的唯一申请；所有目标集合在提交时复制，失败不扣资源
        /// </summary>
        public ChainActionRequest? SubmitChainAction(Character turnActor, Character actor, CharacterActionType type,
            IEnumerable<Character> targets, out string failReason, Skill? skill = null, Item? item = null,
            IEnumerable<Grid>? targetGrids = null, Grid? castCenter = null)
        {
            ChainActionRequest request = new(_chainTurnId, TotalRound, turnActor, actor, type,
                skill ?? item?.Skills.Active, item, targets, targetGrids ?? [], castCenter);
            failReason = _executingChainAction ? "协同行动正在执行。" : ValidateChainAction(request, out _, out _);
            if (failReason != "") return null;
            foreach (ChainActionRequest previous in _chainRequests.Values.Where(r => r.Actor == actor).ToArray()) _chainRequests.Remove(previous.Id);
            _chainRequests[request.Id] = request;
            return request;
        }

        /// <summary>
        /// 只有申请的协作方可以撤回
        /// </summary>
        public bool WithdrawChainAction(Character actor, Guid requestId)
        {
            return !_executingChainAction && _chainRequests.TryGetValue(requestId, out ChainActionRequest? request) &&
                request.Actor == actor && _chainRequests.Remove(requestId);
        }

        private string ValidateChainAction(ChainActionRequest request, out List<Character> targets, out List<Grid> grids)
        {
            targets = [];
            grids = [];
            if (!IsChainTurn(request.TurnActor) || request.TurnId != _chainTurnId || request.Round != TotalRound) return "申请所属回合已结束。";
            if (!GetAvailableChainActors(request.TurnActor).Contains(request.Actor)) return "协作方不在连续同队区间或无法行动。";
            if (request.ActionType is not (CharacterActionType.NormalAttack or CharacterActionType.CastSkill or
                CharacterActionType.PreCastSkill or CharacterActionType.CastSuperSkill or CharacterActionType.UseItem)) return "不支持的协同动作类型。";
            if (!_decisionPoints.TryGetValue(request.Actor, out DecisionPoints? dp)) return "协作方没有初始决策点预算。";
            if (dp.CurrentDecisionPoints < dp.GetActionPointCost(request.ActionType, request.Skill, chainAction: true) ||
                !dp.CheckActionTypeQuota(request.ActionType)) return "协作方决策点或配额不足。";
            if (!_decisionPoints.TryGetValue(request.TurnActor, out DecisionPoints? ownerDP) ||
                ownerDP.CurrentDecisionPoints < ownerDP.GetActionPointCost(CharacterActionType.ChainAction)) return "回合角色决策点不足。";
            Character actor = request.Actor;
            List<Character> teammates = GetTeammates(actor);
            if (!teammates.Contains(request.TurnActor)) teammates.Add(request.TurnActor);
            List<Character> enemies = GetEnemies(actor);
            enemies.Remove(request.TurnActor);
            (List<Character> _, List<Character> _, List<Skill> availableSkills, List<Item> availableItems) = GetTurnStartNeedyList(actor, teammates, enemies);
            ISkill action;
            if (request.ActionType == CharacterActionType.NormalAttack)
            {
                if (request.Skill != null || request.Item != null) return "普通攻击不能携带技能或物品。";
                actor.NormalAttack.GamingQueue = this;
                action = actor.NormalAttack;
            }
            else
            {
                Skill? skill = request.Skill;
                if (skill is null) return "缺少技能。";
                skill.GamingQueue = this;
                if (request.ActionType == CharacterActionType.UseItem)
                {
                    if (request.Item is not Item item || !availableItems.Contains(item) || item.Skills.Active != skill ||
                        (item.IsReduceTimesAfterUse && item.RemainUseTimes <= 0)) return "物品不可用或次数不足。";
                    if (!CheckCanCast(actor, item, out _, out _)) return "物品资源不足。";
                }
                else
                {
                    SkillType expected = request.ActionType switch
                    {
                        CharacterActionType.CastSkill => SkillType.Skill,
                        CharacterActionType.CastSuperSkill => SkillType.SuperSkill,
                        _ => SkillType.Magic
                    };
                    if (request.Item != null || skill.SkillType != expected || !availableSkills.Contains(skill)) return "技能类型不符、未持有或冷却中。";
                }
                if (!CheckCanCast(actor, skill, out _)) return "技能资源不足。";
                action = skill;
            }
            List<Character> selectableEnemies = [.. enemies.Where(c => (_queue.Contains(c) || c == _chainTurnActor) && !c.IsUnselectable)];
            List<Character> selectableTeammates = [.. teammates.Where(c => (_queue.Contains(c) || c == _chainTurnActor) && !c.IsUnselectable)];
            List<Grid> range = [];
            if (_map != null)
            {
                Grid? origin = _map.GetCharacterCurrentGrid(actor);
                if (origin is null) return "协作方不在地图上。";
                range = _map.GetGridsByRange(origin, request.ActionType == CharacterActionType.NormalAttack ? actor.ATR : request.Skill!.CastRange, true);
                selectableEnemies = [.. selectableEnemies.Where(range.SelectMany(g => g.Characters).Contains)];
                selectableTeammates = [.. selectableTeammates.Where(range.SelectMany(g => g.Characters).Contains)];
            }
            if (request.Skill is Skill area && area.IsNonDirectional && _map != null)
            {
                grids = [.. request.TargetGrids];
                if (request.CastCenter is Grid center)
                {
                    if (!range.Contains(center)) return "施法中心超出射程。";
                    grids = area.SelectNonDirectionalTargets(actor, center, area.SelectIncludeCharacterGrid);
                    if (!grids.ToHashSet().SetEquals(request.TargetGrids)) return "范围方案已失效。";
                }
                else if (grids.Count == 0 || grids.Count > area.RealCanSelectTargetCount(selectableEnemies, selectableTeammates) ||
                    grids.Any(g => !range.Contains(g) || (!area.SelectIncludeCharacterGrid && g.Characters.Count > 0))) return "目标格子不合法或超出射程。";
                targets = area.SelectTargetsByRange(actor, enemies, teammates, [], grids);
                if (targets.Count == 0 && !area.AllowSelectNoCharacterGrid) return "目标区域没有可作用角色。";
            }
            else
            {
                List<Character> selectable = action.GetSelectableTargets(actor, enemies, teammates, selectableEnemies, selectableTeammates);
                targets = [.. request.Targets];
                if (request.TargetGrids.Count > 0 || targets.Count == 0 || targets.Any(t => !selectable.Contains(t)) ||
                    targets.Count > action.RealCanSelectTargetCount(selectableEnemies, selectableTeammates)) return "目标失效、数量不合法或超出射程。";
                if (request.Skill is Skill directional && directional.CanSelectTargetRange > 0)
                    targets = directional.SelectTargetsByCanSelectTargetRange(actor, enemies, teammates, targets);
            }
            return "";
        }

        /// <summary>
        /// 在协调方回合内重验并消耗双方预算；接管回合的宿主负责确认硬直与回合收尾。
        /// </summary>
        /// <param name="turnActor">当前回合的协调方</param>
        /// <param name="requestId">待确认的申请标识</param>
        /// <returns>执行结果及协作方的动作记录</returns>
        public ChainActionResult ConfirmChainAction(Character turnActor, Guid requestId) => ConfirmChainAction(turnActor, requestId, inActionLoop: false);

        private ChainActionResult ConfirmChainAction(Character turnActor, Guid requestId, bool inActionLoop)
        {
            if (_executingChainAction) return new(false, "协同行动正在执行。");
            if (!_chainRequests.TryGetValue(requestId, out ChainActionRequest? request) || request.TurnActor != turnActor) return new(false, "申请不存在或不属于当前角色。");
            string failure = ValidateChainAction(request, out List<Character> targets, out List<Grid> grids);
            if (failure != "")
            {
                _chainRequests.Remove(requestId);
                return new(false, failure);
            }
            _chainRequests.Remove(requestId);
            _executingChainAction = true;
            ActionRecord? previous = _currentAction;
            ActionRecord confirmation = inActionLoop ? previous! : new(TotalRound)
            {
                Actor = turnActor, ActionType = CharacterActionType.ChainAction, ActionIndex = LastRound.Actions.Count + 1
            };
            if (!inActionLoop) LastRound.Actions.Add(confirmation);
            confirmation.TurnActor = turnActor;
            confirmation.ChainActor = request.Actor;
            confirmation.ChainActionRequestId = requestId;
            DecisionPoints ownerDP = _decisionPoints[turnActor], dp = _decisionPoints[request.Actor];
            int ownerCost = ownerDP.GetActionPointCost(CharacterActionType.ChainAction);
            int actorCost = dp.GetActionPointCost(request.ActionType, request.Skill, chainAction: true);
            ownerDP.CurrentDecisionPoints -= ownerCost;
            ownerDP.AddActionType(CharacterActionType.ChainAction);
            dp.ConsumeChainAction(request.ActionType, request.Skill);
            AddChainDecisionStatistics(turnActor, ownerCost);
            AddChainDecisionStatistics(request.Actor, actorCost);
            confirmation.DecisionPointsCost = ownerCost;
            LastRound.ActionTypes.Add(CharacterActionType.ChainAction);
            try
            {
                Character actor = request.Actor;
                double original = _hardnessTimes[actor], hardness = 0;
                TriggerOnCharacterActionStart(actor, new ActionContext(this, actor, dp) { ActionType = request.ActionType });
                ActionRecord execution = new(TotalRound)
                {
                    Actor = actor, TurnActor = turnActor, ChainActionRequestId = requestId,
                    ActionType = request.ActionType, ActionIndex = LastRound.Actions.Count + 1,
                    Skill = request.Skill, Item = request.Item, DecisionPointsCost = actorCost
                };
                execution.Targets.AddRange(targets);
                LastRound.Actions.Add(execution);
                _currentAction = execution;
                bool checkProtected = true;
                if (request.ActionType == CharacterActionType.NormalAttack)
                {
                    ExecuteNormalAttackEffects(actor, dp, targets, ref hardness, ref checkProtected);
                }
                else
                {
                    Skill skill = request.Skill!;
                    CheckSkilledImmune(actor, targets, skill, request.Item);
                    SkillTarget skillTarget = new(skill, targets, grids);
                    LastRound.AddEffectTriggered(actor, skill);
                    if (request.ActionType == CharacterActionType.PreCastSkill)
                    {
                        _chainCasts[actor] = new(request, original);
                        _castingSkills[actor] = skillTarget;
                        actor.CharacterState = CharacterState.Casting;
                        OnCharacterPreCastSkillEvent(new SkillCastContext(this, actor)
                        { DP = dp, SkillTarget = skillTarget, Skill = skill, Targets = targets, Grids = grids });
                        skill.OnSkillCasting(this, actor, targets, grids);
                        TriggerAfterCharacterStartCasting(actor, skill, targets);
                        // 钩子可以在发动时打断吟唱，此时收尾入口已恢复原硬直
                        if (_chainCasts.ContainsKey(actor))
                        {
                            AddCharacter(actor, Calculation.Round2Digits(skill.RealCastTime), false);
                            execution.CastTime = _hardnessTimes[actor];
                        }
                    }
                    else if (request.Item is Item item)
                    {
                        CheckCanCast(actor, item, out double costMP, out double costEP);
                        ExecuteItemEffects(item, actor, dp, skill, targets, grids, costMP, costEP);
                        hardness = skill.RealHardnessTime > 0 ? skill.RealHardnessTime : 5;
                        HardnessContext ctx = new(this, actor) { Skill = skill, BaseHardnessTime = hardness, IsCheckProtected = true };
                        TriggerAlterHardnessTimeAfterCastSkill(actor, ctx);
                        hardness = ctx.BaseHardnessTime;
                        checkProtected = ctx.IsCheckProtected;
                    }
                    else
                    {
                        CheckCanCast(actor, skill, out double cost);
                        OnCharacterPreCastSkillEvent(new SkillCastContext(this, actor)
                        { DP = dp, SkillTarget = skillTarget, Skill = skill, Targets = targets, Grids = grids });
                        skill.OnSkillCasting(this, actor, targets, grids);
                        TriggerAfterCharacterStartCasting(actor, skill, targets);
                        ExecuteSkillEffects(actor, dp, skillTarget, cost, ref hardness, ref checkProtected);
                    }
                }
                if (request.ActionType != CharacterActionType.PreCastSkill)
                {
                    execution.HardnessTime = Calculation.Round2Digits(hardness);
                    if (actor.HP > 0 && _queue.Contains(actor)) ChangeCharacterHardnessTime(actor, hardness, false, checkProtected);
                    RecordChainAssist(request, execution, confirmation, targets);
                }
                OnCharacterActionTakenEvent(new ActionContext(this, actor, dp) { ActionType = request.ActionType, Record = LastRound.Snapshot() });
                TriggerOnCharacterActionTaken(actor, new ActionContext(this, actor, dp) { ActionType = request.ActionType });
                AfterCharacterAction(actor, request.ActionType);
                RoundRecordSink?.SendAction(execution.Snapshot());
                if (!inActionLoop) RoundRecordSink?.SendAction(confirmation.Snapshot());
                SendChainQueueUpdate(actor, dp);
                GetPendingChainActions(turnActor);
                return new(true, IsCasting: _chainCasts.ContainsKey(actor), HardnessTime: hardness, Record: execution.Snapshot());
            }
            finally
            {
                _currentAction = previous;
                _executingChainAction = false;
            }
        }

        private void AddChainDecisionStatistics(Character actor, int cost)
        {
            if (_stats.TryGetValue(actor.Master ?? actor, out CharacterStatistics? stats))
            {
                stats.UseDecisionPoints += cost;
                stats.TurnDecisions++;
            }
        }

        private void RecordChainAssist(ChainActionRequest request, ActionRecord execution, ActionRecord? confirmation = null, List<Character>? actualTargets = null)
        {
            if ((actualTargets ?? execution.Targets).Count == 0) return;
            Character owner = request.TurnActor.Master ?? request.TurnActor;
            Character actor = request.Actor.Master ?? request.Actor;
            if (owner != actor && _assistDetail.TryGetValue(owner, out AssistDetail? detail))
            {
                // 现有击杀贡献规则按“辅助过实际击杀者”的时间认定非伤害助攻。
                detail.NotDamageAssistLastTime[actor] = TotalTime;
                ActionRecord record = confirmation ?? execution;
                if (!record.NonDamageAssistTargets.Contains(actor)) record.NonDamageAssistTargets.Add(actor);
            }
        }

        private void RecordChainAssistBeforeKill(Character killer)
        {
            if (_currentAction is not { ChainActionRequestId: not null, TurnActor: Character coordinator } action ||
                action.Actor != killer) return;
            Character owner = coordinator.Master ?? coordinator, actor = killer.Master ?? killer;
            if (owner != actor && _assistDetail.TryGetValue(owner, out AssistDetail? detail))
                detail.NotDamageAssistLastTime[actor] = TotalTime;
        }

        private void SendChainQueueUpdate(Character actor, DecisionPoints dp)
        {
            OnQueueUpdatedEvent(new QueueUpdatedContext(this, actor, dp)
            {
                Characters = [.. _queue],
                HardnessTime = _hardnessTimes.GetValueOrDefault(actor),
                Reason = QueueUpdatedReason.Action,
                Message = "协同行动更新行动顺序表。"
            });
            RoundRecordSink?.SendRound(LastRound.Snapshot());
            RoundRecordSink?.SendQueueData(_hardnessTimes.ToDictionary(kv => kv.Key.Guid, kv => kv.Value));
        }

    }
}
