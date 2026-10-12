using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 角色定位推导：不再由玩家手动选择，而是从「生效战斗天赋 + 已选流派」自动得出
    /// <para/>· 主要定位 = 当前生效战斗天赋所属定位（未激活任何天赋时为 <see cref="RoleType.None"/>）
    /// <para/>· 次要定位 = 流派按「所属职业等级降序、同级按选择顺序」展开其候选定位，去重并跳过主要定位，至多 2 个
    /// <para/>主要定位决定 <see cref="Character.MOV"/> 等按定位取值的属性，转换天赋时会随之变化
    /// </summary>
    public static class ClassPlanRoleResolver
    {
        /// <summary>次要定位数量上限</summary>
        public const int MaxSecondaryCount = 2;

        /// <summary>
        /// 解析主要定位：当前生效战斗天赋所属定位；未激活或已学列表中没有该天赋时返回 <see cref="RoleType.None"/>
        /// </summary>
        public static RoleType ResolvePrimary(CharacterClass plan)
        {
            return plan.CombatTalent is null ? RoleType.None : plan.RoleOf(plan.CombatTalent);
        }

        /// <summary>
        /// 解析次要定位：流派按「所属职业等级降序、同级按玩家选择顺序」展开候选定位，去重并跳过主要定位
        /// </summary>
        /// <param name="plan">职业计划（提供已选流派与流派选择顺序）</param>
        /// <param name="primary">主要定位（会被跳过）</param>
        public static List<RoleType> ResolveSecondary(CharacterClass plan, RoleType primary)
        {
            List<SubClass> ordered = [.. plan.SubClasses
                .OrderByDescending(s => s.Class.Level)
                .ThenBy(s => OrderIndex(plan, s))];
            List<RoleType> result = [];
            foreach (SubClass subClass in ordered)
            {
                foreach (RoleType role in subClass.RoleTypes)
                {
                    if (role == RoleType.None || role == primary || result.Contains(role))
                    {
                        continue;
                    }
                    result.Add(role);
                    if (result.Count >= MaxSecondaryCount)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 全部定位（主要在前，其次次要），供 UI / 校验使用
        /// </summary>
        public static List<RoleType> ResolveAll(CharacterClass plan)
        {
            RoleType primary = ResolvePrimary(plan);
            List<RoleType> all = [];
            if (primary != RoleType.None)
            {
                all.Add(primary);
            }
            foreach (RoleType role in ResolveSecondary(plan, primary))
            {
                if (!all.Contains(role))
                {
                    all.Add(role);
                }
            }
            return all;
        }

        /// <summary>
        /// 计算并把定位写回角色（主要 + 次要）
        /// </summary>
        /// <param name="plan">职业计划</param>
        /// <param name="character">目标角色</param>
        public static void ApplyTo(CharacterClass plan, Character character)
        {
            RoleType primary = ResolvePrimary(plan);
            character.PrimaryRoleType = primary;
            character.SecondaryRoleTypes = ResolveSecondary(plan, primary);
        }

        private static int OrderIndex(CharacterClass plan, SubClass subClass)
        {
            int index = plan.SubClassOrder.IndexOf(subClass.GetIdName());
            return index >= 0 ? index : int.MaxValue;
        }
    }
}
