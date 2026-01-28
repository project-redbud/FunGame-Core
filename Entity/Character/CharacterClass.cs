using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色职业管理类
    /// </summary>
    /// <param name="character"></param>
    public class CharacterClass(Character character)
    {
        /// <summary>
        /// 所属的角色
        /// </summary>
        public Character Character { get; set; } = character;

        /// <summary>
        /// 职业点数
        /// </summary>
        public int ClassPoints { get; set; } = 1;

        /// <summary>
        /// 已选择职业
        /// </summary>
        public HashSet<Class> Classes { get; set; } = [];

        /// <summary>
        /// 已选择流派
        /// </summary>
        public HashSet<SubClass> SubClasses { get; set; } = [];

        /// <summary>
        /// 已激活战斗天赋
        /// </summary>
        public Skill? CombatTalent { get; set; } = null;

        /// <summary>
        /// 通过升级重新计算职业点数
        /// </summary>
        public void OnLevelUp()
        {
            ClassPoints = 0;
            foreach (int level in Character.GameplayEquilibriumConstant.ClassPointsGetterList)
            {
                if (Character.Level >= level)
                {
                    ClassPoints++;
                }
            }
            if (ClassPoints == 0)
            {
                ClassPoints = 1;
            }
        }

        /// <summary>
        /// 重新构建角色职业，设置定位和技能等
        /// </summary>
        /// <param name="obj"></param>
        public void ReBuildCharacterClass(ClassObject obj)
        {
            Character.FirstRoleType = RoleType.None;
            Character.SecondRoleType = RoleType.None;
            Character.ThirdRoleType = RoleType.None;
            CombatTalent?.RemoveSkillFromCharacter(Character);
            foreach (SubClass sc in SubClasses)
            {
                foreach (Skill skill in sc.InherentPassives)
                {
                    skill.RemoveSkillFromCharacter(Character);
                }
            }
            foreach (Class c in Classes)
            {
                foreach (Skill skill in c.PassiveSkills)
                {
                    skill.RemoveSkillFromCharacter(Character);
                }
                foreach (Skill skill in c.Skills)
                {
                    skill.RemoveSkillFromCharacter(Character);
                }
                foreach (Skill skill in c.Magics)
                {
                    skill.RemoveSkillFromCharacter(Character);
                }
                foreach (Skill skill in c.SuperSkills)
                {
                    skill.RemoveSkillFromCharacter(Character);
                }
            }
            Classes.Clear();
            SubClasses.Clear();
            foreach (Class c in obj.Classes)
            {
                Classes.Add(c);
                foreach (Skill skill in c.PassiveSkills)
                {
                    skill.AddSkillToCharacter(Character);
                }
                foreach (Skill skill in c.Skills)
                {
                    skill.AddSkillToCharacter(Character);
                }
                foreach (Skill skill in c.Magics)
                {
                    skill.AddSkillToCharacter(Character);
                }
                foreach (Skill skill in c.SuperSkills)
                {
                    skill.AddSkillToCharacter(Character);
                }
            }
            foreach (SubClass sc in obj.SubClasses)
            {
                SubClasses.Add(sc);
                foreach (Skill skill in sc.InherentPassives)
                {
                    skill.AddSkillToCharacter(Character);
                }
            }
            if (obj.CurrentCombatTalent != null)
            {
                CombatTalent = obj.CurrentCombatTalent;
                CombatTalent.AddSkillToCharacter(Character);
            }
        }
    }

    /// <summary>
    /// 决定如何构建角色的职业。这个类没有 JSON 转换器支持
    /// </summary>
    public class ClassObject(Class[] c, SubClass[] s)
    {
        public Class[] Classes { get; set; } = c;
        public SubClass[] SubClasses { get; set; } = s;
        public RoleType FirstRoleType { get; set; } = RoleType.None;
        public RoleType SecondRoleType { get; set; } = RoleType.None;
        public RoleType ThirdRoleType { get; set; } = RoleType.None;
        public Skill? CurrentCombatTalent { get; set; } = null;
    }
}
