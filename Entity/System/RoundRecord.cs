using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class RoundRecord(int round)
    {
        public int Round { get; set; } = round;
        public Character Actor { get; set; } = Factory.GetCharacter();
        public CharacterActionType ActionType { get; set; } = CharacterActionType.None;
        public List<Character> Targets { get; set; } = [];
        public Skill? Skill { get; set; } = null;
        public string SkillCost { get; set; } = "";
        public Item? Item { get; set; } = null;
        public bool HasKill { get; set; } = false;
        public Dictionary<Character, double> Damages { get; set; } = [];
        public Dictionary<Character, bool> IsCritical { get; set; } = [];
        public Dictionary<Character, double> Heals { get; set; } = [];
        public Dictionary<Character, EffectType> Effects { get; set; } = [];
        public List<string> ActorContinuousKilling { get; set; } = [];
        public List<string> DeathContinuousKilling { get; set; } = [];
        public double CastTime { get; set; } = 0;
        public double HardnessTime { get; set; } = 0;
        public Dictionary<Character, double> RespawnCountdowns { get; set; } = [];
        public List<Character> Respawns { get; set; } = [];
        public List<Skill> RoundRewards { get; set; } = [];

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"=== Round {Round} ===");
            if (RoundRewards.Count > 0)
            {
                builder.AppendLine($"[ {Actor} ] 回合奖励 -> {string.Join(" / ", RoundRewards.Select(s => s.Description)).Trim()}");
            }
            if (ActionType == CharacterActionType.NormalAttack || ActionType == CharacterActionType.CastSkill || ActionType == CharacterActionType.CastSuperSkill)
            {
                if (ActionType == CharacterActionType.NormalAttack)
                {
                    builder.Append($"[ {Actor} ] {Actor.NormalAttack.Name} -> ");
                }
                else if (ActionType == CharacterActionType.CastSkill || ActionType == CharacterActionType.CastSuperSkill)
                {
                    if (Skill != null)
                    {
                        builder.Append($"[ {Actor} ] {Skill.Name}（{SkillCost}）-> ");
                    }
                    else
                    {
                        builder.Append($"释放魔法 -> ");
                    }
                }
                builder.AppendLine(string.Join(" / ", GetTargetsState()));
                if (DeathContinuousKilling.Count > 0) builder.AppendLine($"{string.Join("\r\n", DeathContinuousKilling)}");
                if (ActorContinuousKilling.Count > 0) builder.AppendLine($"{string.Join("\r\n", ActorContinuousKilling)}");
            }

            if (ActionType == CharacterActionType.PreCastSkill && Skill != null)
            {
                if (Skill.IsMagic)
                {
                    builder.AppendLine($"[ {Actor} ] 吟唱 [ {Skill.Name} ]，持续时间：{CastTime:0.##}");
                }
                else
                {
                    builder.AppendLine($"[ {Actor} ]：{Skill.Name}（{SkillCost}）-> ");
                    builder.AppendLine(string.Join(" / ", GetTargetsState()));
                    builder.AppendLine($"[ {Actor} ] 回合结束，硬直时间：{HardnessTime:0.##}");
                }
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

        private List<string> GetTargetsState()
        {
            List<string> strings = [];
            foreach (Character target in Targets.Distinct())
            {
                string hasDamage = "";
                string hasHeal = "";
                string hasEffect = "";
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
                if (Effects.TryGetValue(target, out EffectType effectType))
                {
                    hasEffect = $"施加：{SkillSet.GetEffectTypeName(effectType)}";
                }
                if (ActionType == CharacterActionType.NormalAttack && hasDamage == "")
                {
                    hasDamage = "完美闪避";
                }
                if ((ActionType == CharacterActionType.PreCastSkill || ActionType == CharacterActionType.CastSkill || ActionType == CharacterActionType.CastSuperSkill) && hasDamage == "" && target != Actor)
                {
                    hasDamage = "免疫";
                }
                string[] strs = [hasDamage, hasHeal, hasEffect];
                strings.Add($"[ {target}（{string.Join(" / ", strs.Where(s => s != ""))}）]）");
            }
            return strings;
        }
    }
}
