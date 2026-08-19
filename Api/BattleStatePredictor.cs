using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Api
{
    /// <summary>
    /// 状态预测器<para/>
    /// 基于"定期检查点 + 事件流推算"：从目标回合之前的最近状态检查点出发，沿事件流与 <see cref="RoundRecord.TotalTime"/> 时间轴推算角色状态；<para/>
    /// 供回放/观战展示 HP/MP/EP、装备、技能冷却、状态栏特效。允许由特效钩子（如回复量修改）导致的小误差，在下一个检查点自动校正。
    /// </summary>
    public static class BattleStatePredictor
    {
        /// <summary>
        /// 推算指定回合结束时所有角色的状态（key 为角色 Guid）
        /// </summary>
        /// <param name="rounds">全部回合记录</param>
        /// <param name="targetRound">目标回合号</param>
        /// <returns>角色 Guid -> 状态快照；目标回合前无检查点（或角色不在检查点中）时为空/缺失</returns>
        public static Dictionary<Guid, CharacterStateSnapshot> PredictAll(IEnumerable<RoundRecord> rounds, int targetRound)
        {
            Dictionary<Guid, CharacterStateSnapshot> states = [];
            List<RoundRecord> ordered = [.. rounds.OrderBy(r => r.Round).DistinctBy(r => r.Round)];
            // 基准 = 目标回合之前（不含）的最近检查点，保证目标回合自身是检查点时仍以上一检查点为推算基准
            RoundRecord? baseRound = ordered.LastOrDefault(r => r.Round < targetRound && r.Checkpoint != null);
            if (baseRound?.Checkpoint == null)
            {
                return states;
            }

            foreach (CharacterStateSnapshot cp in baseRound.Checkpoint)
            {
                if (cp.Character == null || cp.Character.Guid == Guid.Empty) continue;
                states[cp.Character.Guid] = Copy(cp);
            }

            double prevTime = baseRound.TotalTime;
            foreach (RoundRecord round in ordered.Where(r => r.Round > baseRound.Round && r.Round <= targetRound))
            {
                double currentTime = round.TotalTime;
                double elapsed = Math.Max(0, currentTime - prevTime);

                // 时间流逝：自动回复（存活角色按 HR/MR × 时间），技能冷却与特效剩余时间沿时间轴递减
                foreach (CharacterStateSnapshot state in states.Values)
                {
                    if (state.HP > 0)
                    {
                        state.HP = Math.Min(state.MaxHP, state.HP + state.HR * elapsed);
                        state.MP = Math.Min(state.MaxMP, state.MP + state.MR * elapsed);
                    }
                    foreach (SkillStateSnapshot skill in state.Skills)
                    {
                        skill.CurrentCD = Math.Max(0, skill.CurrentCD - elapsed);
                    }
                    foreach (EffectStateSnapshot effect in state.Effects)
                    {
                        effect.RemainDuration = Math.Max(0, effect.RemainDuration - elapsed);
                        // 按回合递减的特效每经过一个回合减少 1
                        if (effect.RemainDurationTurn > 0) effect.RemainDurationTurn--;
                    }
                }

                // 应用本回合操作事件
                foreach (ActionRecord action in round.Actions)
                {
                    ApplyAction(action, states);
                }

                // 复活
                foreach (Character character in round.Respawns)
                {
                    if (states.TryGetValue(character.Guid, out CharacterStateSnapshot? respawnState))
                    {
                        respawnState.HP = respawnState.MaxHP;
                    }
                }

                prevTime = currentTime;
            }

            return states;
        }

        /// <summary>
        /// 推算指定回合结束时单个角色的状态
        /// </summary>
        /// <param name="rounds">全部回合记录</param>
        /// <param name="targetRound">目标回合号</param>
        /// <param name="characterGuid">角色 Guid</param>
        /// <returns>状态快照；无检查点基准或角色不在基准中时为 null</returns>
        public static CharacterStateSnapshot? Predict(IEnumerable<RoundRecord> rounds, int targetRound, Guid characterGuid)
        {
            return PredictAll(rounds, targetRound).TryGetValue(characterGuid, out CharacterStateSnapshot? state) ? state : null;
        }

        /// <summary>
        /// 将单次操作事件应用到状态集合（伤害/治疗/消耗/技能冷却/新施加特效）
        /// </summary>
        private static void ApplyAction(ActionRecord action, Dictionary<Guid, CharacterStateSnapshot> states)
        {
            if (action.Actor == null || !states.TryGetValue(action.Actor.Guid, out CharacterStateSnapshot? actorState))
            {
                return;
            }

            // MP/EP 消耗
            actorState.MP = Math.Max(0, actorState.MP - action.MPCost);
            actorState.EP = Math.Max(0, actorState.EP - action.EPCost);

            // 技能冷却重置（同一技能多次释放取最近一次）
            if (action.Skill != null && action.SkillCD > 0)
            {
                SkillStateSnapshot? skillState = actorState.Skills.FirstOrDefault(s => s.SkillId == action.Skill.Id);
                skillState?.CurrentCD = action.SkillCD;
            }

            // 伤害
            foreach (KeyValuePair<Character, double> kv in action.Damages)
            {
                if (states.TryGetValue(kv.Key.Guid, out CharacterStateSnapshot? targetState))
                {
                    targetState.HP = Math.Max(0, targetState.HP - kv.Value);
                }
            }

            // 治疗
            foreach (KeyValuePair<Character, double> kv in action.Heals)
            {
                if (states.TryGetValue(kv.Key.Guid, out CharacterStateSnapshot? targetState))
                {
                    targetState.HP = Math.Min(targetState.MaxHP, targetState.HP + kv.Value);
                }
            }

            // 窗口内新施加的特效（展示级：仅记录类型，剩余时间依赖下一个检查点校正）
            foreach (KeyValuePair<Character, List<EffectType>> kv in action.ApplyEffects)
            {
                if (states.TryGetValue(kv.Key.Guid, out CharacterStateSnapshot? targetState))
                {
                    foreach (EffectType type in kv.Value)
                    {
                        if (targetState.Effects.All(e => e.EffectType != type))
                        {
                            targetState.Effects.Add(new EffectStateSnapshot { EffectType = type });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 深拷贝状态快照（预测时基于副本推算，不修改检查点数据）
        /// </summary>
        private static CharacterStateSnapshot Copy(CharacterStateSnapshot source)
        {
            CharacterStateSnapshot copy = new()
            {
                Character = source.Character,
                HP = source.HP,
                MaxHP = source.MaxHP,
                MP = source.MP,
                MaxMP = source.MaxMP,
                EP = source.EP,
                HR = source.HR,
                MR = source.MR
            };
            foreach (KeyValuePair<string, string> kv in source.Attributes)
            {
                copy.Attributes[kv.Key] = kv.Value;
            }
            foreach (KeyValuePair<EquipSlotType, long> kv in source.Equipments)
            {
                copy.Equipments[kv.Key] = kv.Value;
            }
            foreach (SkillStateSnapshot skill in source.Skills)
            {
                copy.Skills.Add(new SkillStateSnapshot { SkillId = skill.SkillId, SkillName = skill.SkillName, Level = skill.Level, CurrentCD = skill.CurrentCD });
            }
            foreach (EffectStateSnapshot effect in source.Effects)
            {
                copy.Effects.Add(new EffectStateSnapshot { EffectId = effect.EffectId, EffectName = effect.EffectName, EffectType = effect.EffectType, RemainDuration = effect.RemainDuration, RemainDurationTurn = effect.RemainDurationTurn, SourceGuid = effect.SourceGuid });
            }
            return copy;
        }
    }
}
