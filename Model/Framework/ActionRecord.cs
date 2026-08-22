using System.Text;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 操作记录：回合内的一次行动<para/>
    /// 由于决策点系统的存在，一个回合允许执行多次操作，每次操作对应一条 <see cref="ActionRecord"/>；
    /// 与回合级聚合的 <see cref="RoundRecord"/> 不同，本类按单次操作细致记录其行动类型、技能/物品、目标及每个目标的结果明细。
    /// </summary>
    /// <param name="round">归属的回合号</param>
    public class ActionRecord(int round)
    {
        /// <summary>
        /// 归属的回合号
        /// </summary>
        public int Round { get; set; } = round;

        /// <summary>
        /// 执行操作的角色
        /// </summary>
        public Character Actor { get; set; } = new();

        /// <summary>
        /// 本回合内操作的次序（从 1 开始）
        /// </summary>
        public int ActionIndex { get; set; } = 0;

        /// <summary>
        /// 操作类型（普通攻击/技能/爆发技/使用物品/移动/放弃行动/结束回合等）
        /// </summary>
        public CharacterActionType ActionType { get; set; } = CharacterActionType.None;

        /// <summary>
        /// 操作使用的技能（释放技能/爆发技时为非 null）
        /// </summary>
        public Skill? Skill { get; set; } = null;

        /// <summary>
        /// 操作使用的物品（使用物品时为非 null）
        /// </summary>
        public Item? Item { get; set; } = null;

        /// <summary>
        /// 消耗文本（如 "-30 MP" / "-20 EP" / "-1 使用次数"）
        /// </summary>
        public string Cost { get; set; } = "";

        /// <summary>
        /// 本次操作消耗的魔法值（结构化数值，供状态推算）
        /// </summary>
        public double MPCost { get; set; } = 0;

        /// <summary>
        /// 本次操作消耗的能量值（结构化数值，供状态推算）
        /// </summary>
        public double EPCost { get; set; } = 0;

        /// <summary>
        /// 本次操作释放技能的冷却时长（释放时的 <see cref="Skill.RealCD"/>，供状态推算）
        /// </summary>
        public double SkillCD { get; set; } = 0;

        /// <summary>
        /// 本次操作消耗的决策点
        /// </summary>
        public double DecisionPointsCost { get; set; } = 0;

        /// <summary>
        /// 本次操作的目标列表
        /// </summary>
        public List<Character> Targets { get; } = [];

        /// <summary>
        /// 每个目标受到的伤害值
        /// </summary>
        public Dictionary<Character, double> Damages { get; } = [];

        /// <summary>
        /// 每个目标按伤害类型分桶的伤害值（物理/魔法/真实；各桶之和与 <see cref="Damages"/> 一致）
        /// </summary>
        public Dictionary<Character, Dictionary<DamageType, double>> DamageDetails { get; } = [];

        /// <summary>
        /// 每个目标是否暴击
        /// </summary>
        public Dictionary<Character, bool> IsCritical { get; } = [];

        /// <summary>
        /// 每个目标是否闪避（或技能豁免）
        /// </summary>
        public Dictionary<Character, bool> IsEvaded { get; } = [];

        /// <summary>
        /// 每个目标是否免疫
        /// </summary>
        public Dictionary<Character, bool> IsImmune { get; } = [];

        /// <summary>
        /// 每个目标受到的治疗量
        /// </summary>
        public Dictionary<Character, double> Heals { get; } = [];

        /// <summary>
        /// 对每个目标施加的特效类型
        /// </summary>
        public Dictionary<Character, List<EffectType>> ApplyEffects { get; } = [];

        /// <summary>
        /// 本次操作附带的文本消息（含失败原因等）
        /// </summary>
        public List<string> Messages { get; } = [];

        /// <summary>
        /// 操作是否成功执行（决策点不足/配额超限等失败时为 false）
        /// </summary>
        public bool IsSuccess { get; set; } = true;

        /// <summary>
        /// 失败原因（<see cref="IsSuccess"/> 为 false 时填写）
        /// </summary>
        public string FailReason { get; set; } = "";

        /// <summary>
        /// 吟唱持续时间（吟唱操作为非 0）
        /// </summary>
        public double CastTime { get; set; } = 0;

        /// <summary>
        /// 本操作产生的硬直时间
        /// </summary>
        public double HardnessTime { get; set; } = 0;

        /// <summary>
        /// 记录对 <paramref name="character"/> 施加的特效类型
        /// </summary>
        /// <param name="character">被施加特效的角色</param>
        /// <param name="types">特效类型列表</param>
        public void AddApplyEffects(Character character, params IEnumerable<EffectType> types)
        {
            if (ApplyEffects.TryGetValue(character, out List<EffectType>? list) && list != null)
            {
                list.AddRange(types);
            }
            else
            {
                ApplyEffects.TryAdd(character, [.. types]);
            }
        }

        /// <summary>
        /// 渲染本操作的文本描述（与 <see cref="RoundRecord.ToString()"/> 中的行动段落格式一致）
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!IsSuccess)
            {
                return FailReason != "" ? FailReason : $"[ {Actor} ] 操作失败！";
            }

            StringBuilder builder = new();
            if (ActionType == CharacterActionType.NormalAttack)
            {
                builder.Append($"[ {Actor} ] {Actor.NormalAttack.Name} -> ");
            }
            else if (ActionType is CharacterActionType.CastSkill or CharacterActionType.CastSuperSkill or CharacterActionType.PreCastSkill)
            {
                string skillName = Skill?.Name ?? "技能";
                string skillCost = Cost != "" ? $"（{Cost}）" : "";
                builder.Append($"[ {Actor} ] {skillName}{skillCost} -> ");
            }
            else if (ActionType == CharacterActionType.UseItem)
            {
                string itemName = Item?.Name ?? "物品";
                string itemCost = Cost != "" ? $"（{Cost}）" : "";
                builder.Append($"[ {Actor} ] {itemName}{itemCost} -> ");
            }
            else if (ActionType == CharacterActionType.Move)
            {
                builder.Append($"[ {Actor} ] 移动 -> ");
            }
            else if (ActionType == CharacterActionType.EndTurn)
            {
                builder.Append($"[ {Actor} ] 结束回合 -> ");
            }
            else
            {
                builder.Append($"[ {Actor} ] 行动 -> ");
            }

            if (Targets.Count > 0)
            {
                builder.Append(string.Join(" / ", GetTargetsState()));
            }

            if (Messages.Count > 0)
            {
                builder.AppendLine();
                builder.Append(string.Join("\r\n", Messages));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 生成当前操作的结构快照<para/>
        /// 所有集合与字典均为独立副本，但其中的实体引用（<see cref="Character"/> / <see cref="Skill"/> / <see cref="Item"/>）与当前记录共享
        /// </summary>
        /// <returns></returns>
        public ActionRecord Snapshot()
        {
            ActionRecord snapshot = new(Round)
            {
                Actor = Actor,
                ActionIndex = ActionIndex,
                ActionType = ActionType,
                Skill = Skill,
                Item = Item,
                Cost = Cost,
                MPCost = MPCost,
                EPCost = EPCost,
                SkillCD = SkillCD,
                DecisionPointsCost = DecisionPointsCost,
                IsSuccess = IsSuccess,
                FailReason = FailReason,
                CastTime = CastTime,
                HardnessTime = HardnessTime
            };
            snapshot.Targets.AddRange(Targets);
            foreach (KeyValuePair<Character, double> kv in Damages)
            {
                snapshot.Damages[kv.Key] = kv.Value;
            }
            foreach (KeyValuePair<Character, Dictionary<DamageType, double>> kv in DamageDetails)
            {
                snapshot.DamageDetails[kv.Key] = new(kv.Value);
            }
            foreach (KeyValuePair<Character, bool> kv in IsCritical)
            {
                snapshot.IsCritical[kv.Key] = kv.Value;
            }
            foreach (KeyValuePair<Character, bool> kv in IsEvaded)
            {
                snapshot.IsEvaded[kv.Key] = kv.Value;
            }
            foreach (KeyValuePair<Character, bool> kv in IsImmune)
            {
                snapshot.IsImmune[kv.Key] = kv.Value;
            }
            foreach (KeyValuePair<Character, double> kv in Heals)
            {
                snapshot.Heals[kv.Key] = kv.Value;
            }
            foreach (KeyValuePair<Character, List<EffectType>> kv in ApplyEffects)
            {
                snapshot.ApplyEffects[kv.Key] = [.. kv.Value];
            }
            snapshot.Messages.AddRange(Messages);
            return snapshot;
        }

        /// <summary>
        /// 渲染每个目标的状态明细
        /// </summary>
        /// <returns></returns>
        private List<string> GetTargetsState()
        {
            List<string> strings = [];
            foreach (Character target in Targets.Distinct())
            {
                string hasDamage = "";
                string hasHeal = "";
                string hasEffect = "";
                string hasEvaded = "";
                if (Damages.TryGetValue(target, out double damage))
                {
                    hasDamage = $"伤害：{damage:0.##}";
                    if (IsCritical.TryGetValue(target, out bool isCritical) && isCritical)
                    {
                        hasDamage = "暴击，" + hasDamage;
                    }
                }
                if (Heals.TryGetValue(target, out double heals))
                {
                    hasHeal = $"治疗：{heals:0.##}";
                }
                if (ApplyEffects.TryGetValue(target, out List<EffectType>? effectTypes) && effectTypes != null)
                {
                    hasEffect = $"施加：{string.Join(" + ", effectTypes.Select(SkillSet.GetEffectTypeName))}";
                }
                if (IsEvaded.TryGetValue(target, out bool isEvaded) && isEvaded)
                {
                    if (ActionType == CharacterActionType.NormalAttack)
                    {
                        hasEvaded = hasDamage == "" ? "完美闪避" : "闪避";
                    }
                    else if (ActionType is CharacterActionType.PreCastSkill or CharacterActionType.CastSkill or CharacterActionType.CastSuperSkill)
                    {
                        hasEvaded = "技能免疫";
                    }
                }
                if (IsImmune.TryGetValue(target, out bool isImmune) && isImmune && target.Guid != Actor.Guid)
                {
                    hasDamage = "免疫";
                }
                string[] strs = [hasDamage, hasHeal, hasEffect, hasEvaded];
                strs = [.. strs.Where(s => s != "")];
                if (strs.Length == 0) strings.Add($"[ {target} ]");
                else strings.Add($"[ {target}（{string.Join(" / ", strs)}）]");
            }
            return strings;
        }
    }
}
