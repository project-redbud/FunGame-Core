using FunGame.Core.Api;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.Queue
{
    public partial class GamingQueue
    {
        private Character? _castSettlementActor;
        private ActionRecord? _pendingCastFailure;
        private readonly HashSet<Character> _interruptingCasts = [];
        private readonly Dictionary<Character, double> _deferredCastHardness = [];

        private enum CastTerminationReason
        {
            Interrupted,
            TargetLost,
            ResourceInsufficient,
            PreCastOverride,
            Death,
            Removed,
            MissingContext
        }

        private sealed record CastTermination(double OriginalHardness, ActionRecord? Failure);

        private CastTermination? TerminateCast(Character caster, CastTerminationReason reason, string message)
        {
            bool hadMagic = _castingSkills.Remove(caster, out SkillTarget magic);
            bool hadChain = _chainCasts.Remove(caster, out ChainCastContext? chain);
            bool hadSuper = _castingSuperSkills.Remove(caster, out Skill? superSkill);
            _deferredCastHardness.Remove(caster, out double deferredHardness);
            bool wasCasting = caster.CharacterState == CharacterState.Casting;
            bool hasCast = hadMagic || hadChain || hadSuper || wasCasting || reason == CastTerminationReason.MissingContext;
            bool lifecycleEnded = reason is CastTerminationReason.Death or CastTerminationReason.Removed;
            if (lifecycleEnded && _pendingCastFailure?.Actor == caster) _pendingCastFailure = null;

            if (wasCasting || (lifecycleEnded && caster.CharacterState == CharacterState.PreCastSuperSkill))
            {
                caster.CharacterState = CharacterState.Actionable;
            }
            caster.UpdateCharacterState();

            Skill? skill = hadMagic ? magic.Skill : chain?.Request.Skill ?? superSkill;
            ActionRecord? failure = null;
            if (hasCast)
            {
                LastRound.OtherMessages.Add(message);
                WriteLine(message);
                if (reason is CastTerminationReason.Interrupted or CastTerminationReason.TargetLost or
                    CastTerminationReason.ResourceInsufficient or CastTerminationReason.MissingContext)
                {
                    bool reuseCurrent = _currentAction?.Actor == caster &&
                        _currentAction.ActionType is CharacterActionType.CastSkill or CharacterActionType.PreCastSkill;
                    failure = reuseCurrent ? _currentAction! : new(TotalRound)
                    {
                        Actor = caster,
                        TurnActor = chain?.Request.TurnActor,
                        ChainActionRequestId = chain?.Request.Id,
                        ActionType = CharacterActionType.CastSkill,
                        ActionIndex = LastRound.Actions.Count + 1
                    };
                    failure.Skill = skill ?? failure.Skill;
                    failure.IsSuccess = false;
                    failure.FailReason = message;
                    failure.Messages.Add(message);
                    if (!reuseCurrent)
                    {
                        LastRound.Actions.Add(failure);
                        // 回合钩子可能早于释放迭代；失败硬直仍交给主循环入账。
                        if (_castSettlementActor == caster && _isInRound) _pendingCastFailure = failure;
                    }
                }
            }

            // 先摘施法字典，避免奖励回收钩子重入同一次终止。
            ClearDeferredRoundRewards(caster);
            return hasCast ? new((chain?.OriginalHardnessTime ?? 0) + deferredHardness, failure) : null;
        }

        private bool ConsumeCastFailure(Character caster, out double hardness)
        {
            hardness = 0;
            if (_pendingCastFailure?.Actor != caster) return false;
            _currentAction = _pendingCastFailure;
            _pendingCastFailure = null;
            hardness = GameplayEquilibriumConstant.CastFailureHardnessTime;
            return true;
        }

        private double GetRemainingCastTime(Character caster, Skill skill)
        {
            // 协同发动钩子先于队列时间替换，此时队列里仍是原位置。
            if (_executingChainAction && _currentAction?.Actor == caster && _currentAction.ActionType == CharacterActionType.PreCastSkill)
                return Calculation.Round2Digits(skill.RealCastTime);
            return _hardnessTimes.GetValueOrDefault(caster);
        }

        private void FinishExternalCastTermination(Character caster, CastTermination termination, double remaining)
        {
            if (_castSettlementActor == caster && _isInRound) return;
            bool queueChanged = false;
            if (caster.HP > 0 && _queue.Contains(caster) && _hardnessTimes.ContainsKey(caster))
            {
                double hardness = Calculation.Round2Digits(termination.OriginalHardness + remaining);
                if (_hardnessTimes[caster] != hardness)
                {
                    AddCharacter(caster, hardness, false);
                    queueChanged = true;
                }
            }
            if (termination.Failure != null && !ReferenceEquals(termination.Failure, _currentAction))
            {
                RoundRecordSink?.SendAction(termination.Failure.Snapshot());
            }
            if (queueChanged && _decisionPoints.TryGetValue(caster, out DecisionPoints? dp))
            {
                OnQueueUpdatedEvent(new QueueUpdatedContext(this, caster, dp)
                {
                    Characters = [.. _queue],
                    HardnessTime = _hardnessTimes[caster],
                    Reason = QueueUpdatedReason.Action,
                    Message = "吟唱终止后更新行动顺序表。"
                });
                RoundRecordSink?.SendQueueData(_hardnessTimes.ToDictionary(kv => kv.Key.Guid, kv => kv.Value));
            }
            RoundRecordSink?.SendRound(LastRound.Snapshot());
        }

        private void RemoveCastTarget(Character target)
        {
            foreach (Character caster in _castingSkills.Keys.ToArray())
            {
                if (!_castingSkills.TryGetValue(caster, out SkillTarget cast) || !cast.Targets.Remove(target)) continue;
                bool canContinue = cast.Targets.Count > 0 ||
                    (cast.Skill.IsNonDirectional && cast.TargetGrids.Count > 0 && cast.Skill.AllowSelectNoCharacterGrid);
                if (!canContinue)
                {
                    double remaining = GetRemainingCastTime(caster, cast.Skill);
                    CastTermination? termination = TerminateCast(caster, CastTerminationReason.TargetLost,
                        $"[ {caster} ] 终止了 [ {cast.Skill.Name} ] 的施法：全部目标丢失！");
                    if (termination != null)
                    {
                        double hardness = _isInRound ? Math.Min(remaining, GameplayEquilibriumConstant.CastFailureHardnessTime) : remaining;
                        FinishExternalCastTermination(caster, termination, hardness);
                    }
                }
                else
                {
                    string message = $"[ {caster} ] 丢失了 [ {target} ] 这个施法目标，继续吟唱！";
                    LastRound.OtherMessages.Add(message);
                    WriteLine(message);
                }
                SkillCastContext ctx = new(this, caster) { Skill = cast.Skill, Interrupter = target };
                TriggerOnSkillCastInterrupted(caster, target, ctx);
                OnInterruptCastingEvent(ctx);
            }
        }
    }
}
