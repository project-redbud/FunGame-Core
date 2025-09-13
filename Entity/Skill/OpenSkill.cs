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
                    case "allenemy":
                    case "allenemys":
                    case "allenemies":
                        if (bool.TryParse(args[str].ToString(), out bool allenemy))
                        {
                            SelectAllEnemies = allenemy;
                        }
                        break;
                    case "allteammate":
                    case "allteammates":
                        if (bool.TryParse(args[str].ToString(), out bool allteammate))
                        {
                            SelectAllTeammates = allteammate;
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
                    case "nd":
                    case "nondirectional":
                        if (bool.TryParse(args[str].ToString(), out bool nondirectional))
                        {
                            IsNonDirectional = nondirectional;
                        }
                        break;
                    case "rangetype":
                        if (int.TryParse(args[str].ToString(), out int rangetype) && rangetype > 0)
                        {
                            SkillRangeType = (SkillRangeType)rangetype;
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
                    case "costallep":
                        if (bool.TryParse(args[str].ToString(), out bool costall) && costall)
                        {
                            CostAllEP = costall;
                        }
                        break;
                    case "mincost":
                    case "mincostep":
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
                    case "casttime":
                        if (double.TryParse(args[str].ToString(), out double cast) && cast > 0)
                        {
                            CastTime = cast;
                        }
                        break;
                    case "cr":
                    case "castrange":
                        if (int.TryParse(args[str].ToString(), out int castrange) && castrange > 0)
                        {
                            CastRange = castrange;
                        }
                        break;
                    case "caw":
                    case "castanywhere":
                        if (bool.TryParse(args[str].ToString(), out bool castanywhere))
                        {
                            CastAnywhere = castanywhere;
                        }
                        break;
                    case "ht":
                    case "hardnesstime":
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
