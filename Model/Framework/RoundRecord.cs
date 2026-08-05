using System.Text;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    public class RoundRecord(int round)
    {
        public int Round { get; set; } = round;
        public Character Actor { get; set; } = new();
        /// <summary>
        /// 本回合内的全部操作记录（决策点系统允许一回合多次操作，按执行顺序排列，<see cref="ActionRecord.ActionIndex"/> 从 1 开始）
        /// </summary>
        public List<ActionRecord> Actions { get; } = [];
        public HashSet<CharacterActionType> ActionTypes { get; } = [];
        public Dictionary<CharacterActionType, List<Character>> Targets { get; } = [];
        public Dictionary<CharacterActionType, Skill> Skills { get; } = [];
        public Dictionary<Skill, string> SkillsCost { get; set; } = [];
        public Dictionary<CharacterActionType, Item> Items { get; set; } = [];
        public Dictionary<Item, string> ItemsCost { get; set; } = [];
        public bool HasKill { get; set; } = false;
        public List<Character> Assists { get; set; } = [];
        public Dictionary<Character, double> Damages { get; set; } = [];
        public Dictionary<Character, bool> IsCritical { get; set; } = [];
        public Dictionary<Character, bool> IsEvaded { get; set; } = [];
        public Dictionary<Character, bool> IsImmune { get; set; } = [];
        public Dictionary<Character, double> Heals { get; set; } = [];
        public Dictionary<Character, Skill> Effects { get; set; } = [];
        public Dictionary<Character, List<EffectType>> ApplyEffects { get; set; } = [];
        public List<string> ActorContinuousKilling { get; set; } = [];
        public List<string> DeathContinuousKilling { get; set; } = [];
        public double CastTime { get; set; } = 0;
        public double HardnessTime { get; set; } = 0;
        public Dictionary<Character, double> RespawnCountdowns { get; set; } = [];
        public List<Character> Respawns { get; set; } = [];
        public List<Skill> RoundRewards { get; set; } = [];
        public List<string> OtherMessages { get; set; } = [];

        /// <summary>
        /// 全角色清单（开局时由队列写入所有参与角色，供回放端在开局获取完整角色列表；后续回合由序列化时动态收集出现的角色）
        /// </summary>
        public List<Character> AllCharacters { get; set; } = [];

        /// <summary>
        /// 回合结束时的游戏总时间（状态推算的时间轴基准）
        /// </summary>
        public double TotalTime { get; set; } = 0;

        /// <summary>
        /// 状态检查点（周期性生成的全角色状态快照列表，每个元素为一个角色状态，作为状态推算的精确基准；非检查点回合为 null）
        /// </summary>
        public List<CharacterStateSnapshot>? Checkpoint { get; set; } = null;

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

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"=== Round {Round} ===");
            if (RoundRewards.Count > 0)
            {
                builder.AppendLine($"[ {Actor} ] 回合奖励 -> {string.Join(" / ", RoundRewards.Select(s => s.Name)).Trim()}");
            }

            if (Effects.Count > 0)
            {
                builder.AppendLine($"[ {Actor} ] 发动了技能：{string.Join("，", Effects.Where(kv => kv.Key.Guid == Actor.Guid).Select(e => e.Value.Name))}");
            }

            foreach (CharacterActionType type in ActionTypes)
            {
                if (!Targets.TryGetValue(type, out List<Character>? targets) || targets is null)
                {
                    targets = [];
                }

                if (type == CharacterActionType.NormalAttack)
                {
                    builder.Append($"[ {Actor} ] {Actor.NormalAttack.Name} -> ");
                }
                else if (type == CharacterActionType.CastSkill || type == CharacterActionType.CastSuperSkill || type == CharacterActionType.PreCastSkill)
                {
                    if (Skills.TryGetValue(type, out Skill? skill) && skill != null)
                    {
                        string skillCost = SkillsCost.TryGetValue(skill, out string? cost) ? $"（{cost}）" : "";
                        builder.Append($"[ {Actor} ] {skill.Name}{skillCost} -> ");
                    }
                    else
                    {
                        builder.Append($"技能 -> ");
                    }
                }
                else if (type == CharacterActionType.UseItem)
                {
                    if (Items.TryGetValue(type, out Item? item) && item != null)
                    {
                        string itemCost = ItemsCost.TryGetValue(item, out string? cost) ? $"（{cost}）" : "";
                        builder.Append($"[ {Actor} ] {item.Name}{itemCost} -> ");
                    }
                    else
                    {
                        builder.Append($"技能 -> ");
                    }
                }
                builder.AppendLine(string.Join(" / ", GetTargetsState(type, targets)));
            }

            if (DeathContinuousKilling.Count > 0) builder.AppendLine($"{string.Join("\r\n", DeathContinuousKilling)}");
            if (ActorContinuousKilling.Count > 0) builder.AppendLine($"{string.Join("\r\n", ActorContinuousKilling)}");
            if (Assists.Count > 0) builder.AppendLine($"本回合助攻：[ {string.Join(" ] / [ ", Assists)} ]");
            if (OtherMessages.Count > 0) builder.AppendLine(string.Join("\r\n", OtherMessages));

            if (CastTime > 0)
            {
                builder.AppendLine($"[ {Actor} ] 吟唱持续时间：{CastTime:0.##}");
            }
            else
            {
                builder.AppendLine($"[ {Actor} ] 回合结束，硬直时间：{HardnessTime:0.##}");
            }

            foreach (Character character in RespawnCountdowns.Keys)
            {
                builder.AppendLine($"[ {character} ] 进入复活倒计时：{RespawnCountdowns[character]:0.##}");
            }

            foreach (Character character in Respawns)
            {
                builder.AppendLine($"[ {character} ] 复活了");
            }

            return builder.ToString();
        }

        private List<string> GetTargetsState(CharacterActionType type, List<Character> targets)
        {
            List<string> strings = [];
            foreach (Character target in targets.Distinct())
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
                    if (type == CharacterActionType.NormalAttack)
                    {
                        hasEvaded = hasDamage == "" ? "完美闪避" : "闪避";
                    }
                    else if ((type == CharacterActionType.PreCastSkill || type == CharacterActionType.CastSkill || type == CharacterActionType.CastSuperSkill))
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

        /// <summary>
        /// 生成当前记录的结构快照<para/>
        /// 所有集合与字典均为独立副本，但其中的实体引用（<see cref="Character"/> / <see cref="Skill"/> / <see cref="Item"/>）与当前记录共享；<para/>
        /// 用于事件分发与回合归档，保证订阅端看到的是快照时刻的回合状态，不受后续对集合结构的修改（新增、覆盖、删除条目）影响
        /// </summary>
        /// <returns></returns>
        public RoundRecord Snapshot()
        {
            RoundRecord snapshot = new(Round)
            {
                Actor = Actor,
                HasKill = HasKill,
                Assists = [.. Assists],
                Damages = new(Damages),
                IsCritical = new(IsCritical),
                IsEvaded = new(IsEvaded),
                IsImmune = new(IsImmune),
                Heals = new(Heals),
                Effects = new(Effects),
                ApplyEffects = ApplyEffects.ToDictionary(kv => kv.Key, kv => (List<EffectType>)[.. kv.Value]),
                ActorContinuousKilling = [.. ActorContinuousKilling],
                DeathContinuousKilling = [.. DeathContinuousKilling],
                CastTime = CastTime,
                HardnessTime = HardnessTime,
                RespawnCountdowns = new(RespawnCountdowns),
                Respawns = [.. Respawns],
                RoundRewards = [.. RoundRewards],
                OtherMessages = [.. OtherMessages],
                SkillsCost = new(SkillsCost),
                Items = new(Items),
                ItemsCost = new(ItemsCost)
            };
            foreach (CharacterActionType type in ActionTypes)
            {
                snapshot.ActionTypes.Add(type);
            }
            foreach (KeyValuePair<CharacterActionType, List<Character>> kv in Targets)
            {
                snapshot.Targets[kv.Key] = [.. kv.Value];
            }
            foreach (KeyValuePair<CharacterActionType, Skill> kv in Skills)
            {
                snapshot.Skills[kv.Key] = kv.Value;
            }
            foreach (ActionRecord action in Actions)
            {
                snapshot.Actions.Add(action.Snapshot());
            }
            // 检查点列表拷贝（元素共享，防快照后对列表结构的修改）
            if (Checkpoint != null)
            {
                snapshot.Checkpoint = [.. Checkpoint];
            }
            snapshot.AllCharacters = [.. AllCharacters];
            snapshot.TotalTime = TotalTime;
            return snapshot;
        }
    }
}
