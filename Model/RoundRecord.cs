using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class RoundRecord(int round)
    {
        public int Round { get; set; } = round;
        public Character Actor { get; set; } = Factory.GetCharacter();
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
        public Dictionary<Character, Effect> Effects { get; set; } = [];
        public Dictionary<Character, List<EffectType>> ApplyEffects { get; set; } = [];
        public List<string> ActorContinuousKilling { get; set; } = [];
        public List<string> DeathContinuousKilling { get; set; } = [];
        public double CastTime { get; set; } = 0;
        public double HardnessTime { get; set; } = 0;
        public Dictionary<Character, double> RespawnCountdowns { get; set; } = [];
        public List<Character> Respawns { get; set; } = [];
        public List<Skill> RoundRewards { get; set; } = [];
        public List<string> OtherMessages { get; set; } = [];

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
                builder.AppendLine($"[ {Actor} ] 回合奖励 -> {string.Join(" / ", RoundRewards.Select(s => s.Description)).Trim()}");
            }

            if (Effects.Count > 0)
            {
                builder.AppendLine($"[ {Actor} ] 发动了技能：{string.Join("，", Effects.Where(kv => kv.Key == Actor).Select(e => e.Value.Name))}");
            }

            foreach (CharacterActionType type in ActionTypes)
            {
                if (type == CharacterActionType.PreCastSkill)
                {
                    continue;
                }

                if (!Targets.TryGetValue(type, out List<Character>? targets) || targets is null)
                {
                    targets = [];
                }

                if (type == CharacterActionType.NormalAttack)
                {
                    builder.Append($"[ {Actor} ] {Actor.NormalAttack.Name} -> ");
                }
                else if (type == CharacterActionType.CastSkill || type == CharacterActionType.CastSuperSkill)
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

            if (ActionTypes.Any(type => type == CharacterActionType.PreCastSkill) && Skills.TryGetValue(CharacterActionType.PreCastSkill, out Skill? magic) && magic != null)
            {
                builder.AppendLine($"[ {Actor} ] 吟唱 [ {magic.Name} ]，持续时间：{CastTime:0.##}");
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
                if (IsEvaded.ContainsKey(target))
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
                if (IsImmune.ContainsKey(target) && hasDamage != "" && target != Actor)
                {
                    hasDamage = "免疫";
                }
                string[] strs = [hasDamage, hasHeal, hasEffect, hasEvaded];
                strs = [.. strs.Where(s => s != "")];
                if (strs.Length == 0) strings.Add($"[ {target} ]）");
                else strings.Add($"[ {target}（{string.Join(" / ", strs)}）]）");
            }
            return strings;
        }
    }
}
