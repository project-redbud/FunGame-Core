using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public static class SkillExtension
    {
        public static string SkillOwner(this Skill skill, Character? character = null)
        {
            if (character is null && skill.Character is not null)
            {
                character = skill.Character;
            }
            if (character is null)
            {
                return "你";
            }
            return character.NickName != "" ? character.NickName : character.GetName();
        }

        public static string TargetDescription(this Skill skill)
        {
            if (skill.IsNonDirectional && skill.GamingQueue?.Map != null)
            {
                return skill.RangeTargetDescription();
            }

            string str;

            if (skill.SelectAllTeammates)
            {
                str = "友方全体角色";
            }
            else if (skill.SelectAllEnemies)
            {
                str = "敌方全体角色";
            }
            else if (skill.CanSelectTeammate && !skill.CanSelectEnemy)
            {
                str = $"目标{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}友方角色{(!skill.CanSelectSelf ? "（不可选择自身）" : "")}";
            }
            else if (!skill.CanSelectTeammate && skill.CanSelectEnemy)
            {
                str = $"目标{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}敌方角色";
            }
            else if (!skill.CanSelectTeammate && !skill.CanSelectEnemy && skill.CanSelectSelf)
            {
                str = $"自身";
            }
            else
            {
                str = $"{(skill.CanSelectTargetCount > 1 ? $"至多 {skill.CanSelectTargetCount} 个" : "")}目标";
            }

            if (skill.CanSelectTargetRange > 0 && skill.GamingQueue?.Map != null)
            {
                str += $"以及以{(skill.CanSelectTargetCount > 1 ? "这些" : "该")}目标为中心，半径为 {skill.CanSelectTargetRange} 格的菱形区域中的等同阵营角色";
            }

            return str;
        }

        public static string RangeTargetDescription(this Skill skill)
        {
            string str = "";

            int range = skill.CanSelectTargetRange;
            if (range <= 0)
            {
                str = "目标地点";
            }
            else
            {
                switch (skill.SkillRangeType)
                {
                    case SkillRangeType.Diamond:
                        str = $"目标半径为 {skill.CanSelectTargetRange} 格的菱形区域";
                        break;
                    case SkillRangeType.Circle:
                        str = $"目标半径为 {skill.CanSelectTargetRange} 格的圆形区域";
                        break;
                    case SkillRangeType.Square:
                        str = $"目标边长为 {skill.CanSelectTargetRange * 2 + 1} 格的正方形区域";
                        break;
                    case SkillRangeType.Line:
                        str = $"自身与目标地点之间的、宽度为 {skill.CanSelectTargetRange} 格的直线区域";
                        break;
                    case SkillRangeType.LinePass:
                        str = $"自身与目标地点之间的、宽度为 {skill.CanSelectTargetRange} 格的直线区域以及贯穿该目标地点直至地图边缘的等宽直线区域";
                        break;
                    case SkillRangeType.Sector:
                        str = $"目标最大半径为 {skill.CanSelectTargetRange} 格的扇形区域";
                        break;
                    default:
                        break;
                }
            }

            if (skill.SelectIncludeCharacterGrid)
            {
                if (skill.CanSelectTeammate && !skill.CanSelectEnemy)
                {
                    str = $"{str}中的所有友方角色{(!skill.CanSelectSelf ? "（包括自身）" : "")}";
                }
                else if (!skill.CanSelectTeammate && skill.CanSelectEnemy)
                {
                    str = $"{str}中的所有敌方角色";
                }
                else
                {
                    str = $"{str}中的所有角色";
                }
            }
            else
            {
                str = "一个未被角色占据的";
            }

            return str;
        }
    }
}
