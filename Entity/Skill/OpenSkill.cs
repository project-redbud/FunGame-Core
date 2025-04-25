using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 用于动态扩展技能，<see cref="Description"/> 返回所有特效的描述
    /// </summary>
    public class OpenSkill : Skill
    {
        public override long Id { get; set; }
        public override string Name { get; set; }
        public override string Description => string.Join("\r\n", Effects.Select(e => e.Description));

        public OpenSkill(long id, string name, Dictionary<string, object> args, Character? character = null) : base(SkillType.Passive, character)
        {
            Id = id;
            Name = name;
            foreach (string str in args.Keys)
            {
                Values[str] = args[str];
                switch (str.ToLower())
                {
                    case "active":
                        if (bool.TryParse(args[str].ToString(), out bool isActive) && isActive)
                        {
                            SkillType = SkillType.Item;
                        }
                        break;
                    case "self":
                        if (bool.TryParse(args[str].ToString(), out bool self))
                        {
                            CanSelectSelf = self;
                        }
                        break;
                    case "enemy":
                        if (bool.TryParse(args[str].ToString(), out bool enemy))
                        {
                            CanSelectEnemy = enemy;
                        }
                        break;
                    case "teammate":
                        if (bool.TryParse(args[str].ToString(), out bool teammate))
                        {
                            CanSelectTeammate = teammate;
                        }
                        break;
                    case "count":
                        if (int.TryParse(args[str].ToString(), out int count) && count > 0)
                        {
                            CanSelectTargetCount = count;
                        }
                        break;
                    case "range":
                        if (int.TryParse(args[str].ToString(), out int range) && range > 0)
                        {
                            CanSelectTargetRange = range;
                        }
                        break;
                    case "mpcost":
                        if (double.TryParse(args[str].ToString(), out double mpcost) && mpcost > 0)
                        {
                            MPCost = mpcost;
                        }
                        break;
                    case "epcost":
                        if (double.TryParse(args[str].ToString(), out double epcost) && epcost > 0)
                        {
                            EPCost = epcost;
                        }
                        break;
                    case "costall":
                        if (bool.TryParse(args[str].ToString(), out bool costall) && costall)
                        {
                            CostAllEP = costall;
                        }
                        break;
                    case "mincost":
                        if (double.TryParse(args[str].ToString(), out double mincost) && mincost > 0)
                        {
                            MinCostEP = mincost;
                        }
                        break;
                    case "cd":
                        if (double.TryParse(args[str].ToString(), out double cd) && cd > 0)
                        {
                            CD = cd;
                        }
                        break;
                    case "cast":
                        if (double.TryParse(args[str].ToString(), out double cast) && cast > 0)
                        {
                            CastTime = cast;
                        }
                        break;
                    case "ht":
                        if (double.TryParse(args[str].ToString(), out double ht) && ht > 0)
                        {
                            HardnessTime = ht;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }
}
