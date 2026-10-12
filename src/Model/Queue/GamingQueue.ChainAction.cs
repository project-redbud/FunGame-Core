using FunGame.Core.Api;
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
            IEnumerable<Grid>? targetGrids = null)
        {
            ChainActionRequest request = new(_chainTurnId, TotalRound, turnActor, actor, type,
                skill ?? item?.Skills.Active, item, targets, targetGrids ?? []);
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
            List<Character> teammates = [.. _allCharacters.Union(_queue).Where(c => c != actor && AreChainTeammates(actor, c)).Distinct()];
            List<Character> enemies = [.. _allCharacters.Union(_queue).Where(c => c != actor && !teammates.Contains(c)).Distinct()];
            var (_, _, availableSkills, availableItems) = GetTurnStartNeedyList(actor, teammates, enemies);
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
                if (grids.Count == 0 || grids.Count > area.RealCanSelectTargetCount(selectableEnemies, selectableTeammates) ||
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
        /// 确认时重验并消耗双方预算。开始执行前移除申请，重复确认不会重复扣费
        /// </summary>
        public ChainActionResult ConfirmChainAction(Character turnActor, Guid requestId)
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
            bool inActionLoop = previous?.ActionType == CharacterActionType.ChainAction && previous.Actor == turnActor;
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
            ownerDP.ActionsHardnessTime.Add(GameplayEquilibriumConstant.ChainActionHardnessTime);
            dp.ConsumeChainAction(request.ActionType, request.Skill);
            AddChainDecisionStatistics(turnActor, ownerCost);
            AddChainDecisionStatistics(request.Actor, actorCost);
            confirmation.DecisionPointsCost = ownerCost;
            confirmation.HardnessTime = GameplayEquilibriumConstant.ChainActionHardnessTime;
            LastRound.ActionTypes.Add(CharacterActionType.ChainAction);
            ActionRecord execution = new(TotalRound)
            {
                Actor = request.Actor, TurnActor = turnActor, ChainActionRequestId = requestId,
                ActionType = request.ActionType, ActionIndex = LastRound.Actions.Count + 1,
                Skill = request.Skill, Item = request.Item, DecisionPointsCost = actorCost
            };
            execution.Targets.AddRange(targets);
            LastRound.Actions.Add(execution);
            _currentAction = execution;
            try
            {
                double original = _hardnessTimes[request.Actor], hardness = 0;
                bool checkProtected = true;
                Character actor = request.Actor;
                TriggerOnCharacterActionStart(actor, new ActionContext(this, actor, dp) { ActionType = request.ActionType });
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

        private bool FinishInterruptedChainCast(Character actor)
        {
            if (!_chainCasts.Remove(actor, out ChainCastContext? context)) return false;
            _castingSkills.Remove(actor);
            actor.CharacterState = CharacterState.Actionable;
            actor.UpdateCharacterState();
            if (actor.HP > 0 && _queue.Contains(actor)) AddCharacter(actor, context.OriginalHardnessTime + 3, false);
            SendChainQueueUpdate(actor, _decisionPoints[actor]);
            return true;
        }

        private bool ProcessChainCastTurn(Character actor, ChainCastContext context)
        {
            // 吟唱结算是独立收尾回合：不恢复预算、不发奖励、不开放正常决策。
            DecisionPoints dp = _decisionPoints[actor];
            ChainActionRequest request = context.Request;
            Skill skill = request.Skill!;
            ActionRecord action = new(TotalRound)
            {
                Actor = actor, TurnActor = request.TurnActor, ChainActionRequestId = request.Id,
                ActionType = CharacterActionType.CastSkill, Skill = skill, ActionIndex = 1
            };
            LastRound.Actions.Add(action);
            _currentAction = action;
            double hardness = 3;
            bool checkProtected = true;
            try
            {
                actor.CharacterState = CharacterState.Actionable;
                actor.UpdateCharacterState();
                List<Character> targets = [];
                List<Grid> grids = [];
                if (_castingSkills.TryGetValue(actor, out SkillTarget cast))
                {
                    grids = cast.TargetGrids;
                    targets = [.. cast.Targets.Where(t => t == actor || !t.IsUnselectable)];
                    if (skill.IsNonDirectional && _map != null)
                        targets = skill.SelectTargetsByRange(actor, GetEnemies(actor), GetTeammates(actor), [], grids);
                    else if (skill.CanSelectTargetRange > 0)
                        targets = skill.SelectTargetsByCanSelectTargetRange(actor, GetEnemies(actor), GetTeammates(actor), targets);
                }
                _castingSkills.Remove(actor);
                _chainCasts.Remove(actor);
                bool hasTarget = targets.Count > 0 || (skill.IsNonDirectional && grids.Count > 0 && skill.AllowSelectNoCharacterGrid);
                if (actor.HP > 0 && actor.CharacterState == CharacterState.Actionable && hasTarget && skill.Enable &&
                    skill.CurrentCD == 0 && !skill.IsInEffect && CheckCanCast(actor, skill, out double cost))
                {
                    action.Targets.AddRange(targets);
                    LastRound.Targets[CharacterActionType.CastSkill] = [.. targets];
                    LastRound.Skills[CharacterActionType.CastSkill] = skill;
                    LastRound.ActionTypes.Add(CharacterActionType.CastSkill);
                    CheckSkilledImmune(actor, targets, skill);
                    hardness = 0;
                    ExecuteSkillEffects(actor, dp, new(skill, targets, grids), cost, ref hardness, ref checkProtected);
                    RecordChainAssist(request, action, actualTargets: targets);
                }
                else
                {
                    action.IsSuccess = false;
                    action.FailReason = "协同吟唱结算失败：目标、状态或资源失效。";
                    LastRound.OtherMessages.Add(action.FailReason);
                }
                hardness = Calculation.Round2Digits(context.OriginalHardnessTime + hardness);
                action.HardnessTime = hardness;
                LastRound.HardnessTime = hardness;
                OnCharacterActionTakenEvent(new ActionContext(this, actor, dp) { ActionType = CharacterActionType.CastSkill, Record = LastRound.Snapshot() });
                TriggerOnCharacterActionTaken(actor, new ActionContext(this, actor, dp) { ActionType = CharacterActionType.CastSkill });
                AfterCharacterAction(actor, CharacterActionType.CastSkill);
                OnCharacterDecisionCompletedEvent(new ActionContext(this, actor, dp) { Record = LastRound.Snapshot() });
                TriggerOnCharacterDecisionCompleted(actor, dp);
                ProcessCharacterDeath();
                if (actor.HP > 0 && !_eliminated.Contains(actor))
                {
                    AddCharacter(actor, hardness, checkProtected);
                    OnQueueUpdatedEvent(new QueueUpdatedContext(this, actor, dp)
                    { Characters = [.. _queue], HardnessTime = _hardnessTimes[actor], Reason = QueueUpdatedReason.Action, Message = "协同吟唱结算恢复行动硬直。" });
                }
                LastRound.TotalTime = TotalTime;
                if (CheckpointInterval > 0 && TotalRound % CheckpointInterval == 0) LastRound.Checkpoint = CreateStateCheckpoint();
                RoundRecordSink?.SendAction(action.Snapshot());
                OnTurnEndEvent(new TurnContext(this, actor, dp));
                if (_isGameEnd)
                {
                    LastRound.CharacterStatistics = new(_stats);
                    LastRound.Checkpoint = CreateStateCheckpoint();
                    Rounds.Add(LastRound.Snapshot());
                }
                SendRoundEndData();
                if (_isGameEnd) RoundRecordSink?.End();
                return _isGameEnd;
            }
            finally
            {
                _chainCasts.Remove(actor);
                _castingSkills.Remove(actor);
                _currentAction = null;
                _isInRound = false;
            }
        }
    }
}
