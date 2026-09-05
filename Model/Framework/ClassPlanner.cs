using FunGame.Core.Entity;
using FunGame.Core.Library.Common.Event;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 职业规划操作结果
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    public class ClassPlanResult(bool success, string message, Dictionary<string, object>? data = null)
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; } = success;

        /// <summary>
        /// 结果消息（失败原因 / 描述）
        /// </summary>
        public string Message { get; } = message;

        /// <summary>
        /// 附加数据（供上层 / 事件消费）
        /// </summary>
        public Dictionary<string, object>? Data { get; } = data;

        public static ClassPlanResult Ok(string message = "") => new(true, message);

        public static ClassPlanResult Fail(string message) => new(false, message);

        public static implicit operator bool(ClassPlanResult result) => result.Success;
    }

    /// <summary>
    /// 职业规划系统：把「规划操作 → 校验 → 写入 <see cref="Character.Class"/>」收敛为带事件推送的入口
    /// <para>核心库只做合法性校验与状态写入；点数消耗策略、奖励内容、UI 流程由上层 / 默认表提供。
    /// 校验与写法均显式，不依赖反射。</para>
    /// </summary>
    public class ClassPlanner
    {
        private readonly Character _character;

        /// <summary>
        /// 被规划的角色
        /// </summary>
        public Character Character { get; }

        /// <summary>
        /// 目标职业计划（即 <see cref="Character.Class"/>）
        /// </summary>
        public CharacterClass Plan { get; }

        /// <summary>
        /// 游戏平衡常数
        /// </summary>
        public EquilibriumConstant Eq => _character.GameplayEquilibriumConstant;

        /// <summary>
        /// 每次成功动作后触发（供模组 / 上层监听；把 Interface/Event/ClassPlanEvents.cs 中
        /// 各事件接口实例的监听方法挂载到此处即可）
        /// </summary>
        public event Action<ClassPlanner, ClassPlanEventArgs>? Planned;

        /// <summary>
        /// 以 <paramref name="character"/> 为目标创建规划器
        /// </summary>
        /// <param name="character"></param>
        public ClassPlanner(Character character)
        {
            _character = character;
            Character = character;
            Plan = Character.Class;
        }

        // ==================== 动作 ====================

        /// <summary>
        /// 选择职业与流派（新职业条目，含首职业与兼职），消耗 1 点职业点数
        /// <para>1 级且尚无默认计划时，本次选择自动记录为默认职业 / 流派（洗点恢复用）。</para>
        /// </summary>
        /// <param name="classDef">职业定义（模组注册侧）</param>
        /// <param name="subClassDef">流派定义，必须属于 <paramref name="classDef"/></param>
        /// <returns>结果</returns>
        public ClassPlanResult SelectClass(Class classDef, SubClass subClassDef)
        {
            if (classDef == null || subClassDef == null)
            {
                return ClassPlanResult.Fail("职业与流派不能为空。");
            }
            if (subClassDef.Class.GetIdName() != classDef.GetIdName())
            {
                return ClassPlanResult.Fail($"流派【{subClassDef.Name}】不属于职业【{classDef.Name}】，请重新选择。");
            }
            if (Plan.Classes.Any(c => c.GetIdName() == classDef.GetIdName()))
            {
                return ClassPlanResult.Fail($"已选择职业【{classDef.Name}】，不允许重复职业（含同职业的其他流派）。");
            }
            if (Plan.ClassPoints < 1)
            {
                return ClassPlanResult.Fail($"职业点数不足，选择新职业需消耗 1 点（当前 {Plan.ClassPoints} 点）。");
            }
            Plan.ClassPoints--;
            // 新职业记录从 1 级开始（首职业与兼职同规则），等级由此后的 UpgradeClass 提升
            Class record = classDef.Copy();
            record.Level = 1;
            Plan.Classes.Add(record);
            Plan.SubClasses.Add(subClassDef.Copy(record));
            // 1 级首职业自动记为默认（洗点恢复用），此后角色满 20 级才允许修改默认
            if (Plan.DefaultClasses.Count == 0 && Character.Level <= 1)
            {
                Plan.DefaultClasses.Add(classDef);
                Plan.DefaultSubClasses.Add(subClassDef);
            }
            Raise(ClassPlanPhase.SelectClass, true, $"已选择职业【{classDef.Name}】流派【{subClassDef.Name}】。");
            return ClassPlanResult.Ok();
        }

        /// <summary>
        /// 职业升级（+1 级，不超过上限），消耗 1 点职业点数
        /// </summary>
        /// <param name="record"><see cref="CharacterClass.Classes"/> 中的职业记录</param>
        public ClassPlanResult UpgradeClass(Class record)
        {
            if (record == null || !Plan.Classes.Contains(record))
            {
                return ClassPlanResult.Fail("职业记录不存在于当前计划中。");
            }
            if (record.Level >= Eq.MaxClassLevel)
            {
                return ClassPlanResult.Fail($"职业【{record.Name}】已达等级上限 {Eq.MaxClassLevel} 级。");
            }
            if (Plan.ClassPoints < 1)
            {
                return ClassPlanResult.Fail($"职业点数不足，职业升级需消耗 1 点（当前 {Plan.ClassPoints} 点）。");
            }
            Plan.ClassPoints--;
            record.Level++;
            Raise(ClassPlanPhase.UpgradeClass, true, $"职业【{record.Name}】升至 {record.Level} 级。");
            return ClassPlanResult.Ok();
        }

        /// <summary>
        /// 选择角色定位（覆盖式写回三个定位；至多 3 个且必须来自已选流派的候选并集）
        /// <para>定位变化会使已学天赋与已激活天赋失效，本操作会一并清除，需重新选择天赋。</para>
        /// </summary>
        /// <param name="roleTypes">新定位（去重后按序写入 First/Second/Third）</param>
        public ClassPlanResult SelectRoleTypes(IEnumerable<RoleType> roleTypes)
        {
            if (roleTypes == null)
            {
                return ClassPlanResult.Fail("定位列表不能为空。");
            }
            RoleType[] selected = [.. roleTypes.Where(r => r != RoleType.None).Distinct()];
            if (selected.Length == 0)
            {
                return ClassPlanResult.Fail("请至少选择一个定位。");
            }
            if (selected.Length > 3)
            {
                return ClassPlanResult.Fail("角色至多拥有 3 个定位。");
            }
            if (Plan.SubClasses.Count == 0)
            {
                return ClassPlanResult.Fail("尚未选择任何流派，定位候选为空。请先选择职业与流派。");
            }
            HashSet<RoleType> candidates = [.. Plan.SubClasses.SelectMany(sc => sc.RoleTypes)];
            if (selected.Any(r => !candidates.Contains(r)))
            {
                return ClassPlanResult.Fail("所选定位必须来自已选流派提供的候选定位。");
            }
            // 定位变动 → 旧天赋作废（已物化的先卸载再清引用）
            ClearTalents();
            Character.FirstRoleType = selected.Length > 0 ? selected[0] : RoleType.None;
            Character.SecondRoleType = selected.Length > 1 ? selected[1] : RoleType.None;
            Character.ThirdRoleType = selected.Length > 2 ? selected[2] : RoleType.None;
            Raise(ClassPlanPhase.SelectRoleTypes, true, $"已选择定位：{string.Join(" / ", selected.Select(GetRoleTypeName))}。");
            return ClassPlanResult.Ok();
        }

        /// <summary>
        /// 学习战斗天赋：数量必须与已选定位一致，且天赋须属于对应定位在已选职业中的天赋池
        /// </summary>
        /// <param name="roleType">天赋对应的定位（必须在已选定位中）</param>
        /// <param name="talent">天赋技能（来自职业天赋池的实例）</param>
        public ClassPlanResult LearnCombatTalent(RoleType roleType, Skill talent)
        {
            if (talent == null)
            {
                return ClassPlanResult.Fail("天赋不能为空。");
            }
            if (roleType != Character.FirstRoleType && roleType != Character.SecondRoleType && roleType != Character.ThirdRoleType)
            {
                return ClassPlanResult.Fail("天赋对应的定位不在角色已选定位中。");
            }
            bool inPool = Plan.Classes.Any(c => c.CombatTalents.TryGetValue(roleType, out HashSet<Skill>? pool) && pool.Any(t => t.GetIdName() == talent.GetIdName()));
            if (!inPool)
            {
                return ClassPlanResult.Fail($"天赋【{talent.Name}】不属于已选职业的 {GetRoleTypeName(roleType)} 天赋池。");
            }
            int roleCount = new[] { Character.FirstRoleType, Character.SecondRoleType, Character.ThirdRoleType }.Where(r => r != RoleType.None).Distinct().Count();
            if (!Plan.LearnedCombatTalents.ContainsKey(roleType) && Plan.LearnedCombatTalents.Count >= roleCount)
            {
                return ClassPlanResult.Fail("已学天赋数量与定位数量一致，无法继续学习（先修改定位或替换同定位天赋）。");
            }
            // 覆盖同定位旧天赋：若旧天赋正在生效，先失活
            if (Plan.CombatTalent != null && Plan.LearnedCombatTalents.TryGetValue(roleType, out Skill? old) && ReferenceEquals(Plan.CombatTalent, old))
            {
                DeactivateTalent();
            }
            if (Plan.LearnedCombatTalents.TryGetValue(roleType, out Skill? existing))
            {
                existing.RemoveSkillFromCharacter(Character);
            }
            Plan.LearnedCombatTalents[roleType] = talent;
            Raise(ClassPlanPhase.LearnTalent, true, $"已学习 {GetRoleTypeName(roleType)} 天赋【{talent.Name}】。");
            return ClassPlanResult.Ok();
        }

        /// <summary>
        /// 激活 / 转换战斗天赋（始终至多 1 个生效；核心定位天赋的等级加成自动加减配对）
        /// <para>委托 <see cref="CharacterClass.SwitchCombatTalent"/>，与【转换战斗天赋】战技共用同一路径。</para>
        /// </summary>
        /// <param name="roleType">要激活的已学天赋对应定位</param>
        public ClassPlanResult ActivateCombatTalent(RoleType roleType)
        {
            if (!Plan.SwitchCombatTalent(roleType, out string? error))
            {
                return ClassPlanResult.Fail(error ?? "天赋转换失败。");
            }
            Skill? talent = Plan.CombatTalent;
            Raise(ClassPlanPhase.ActivateTalent, true, $"已激活 {GetRoleTypeName(roleType)} 天赋【{talent?.Name}】。");
            return ClassPlanResult.Ok();
        }

        /// <summary>
        /// 洗点：清空当前职业规划（含已物化的技能与特效）；20 级前只能恢复到 1 级默认职业与流派，
        /// 20 级起可完全重选。清空后点数按等级重算，由上层重新规划。
        /// </summary>
        public ClassPlanResult ResetPlan()
        {
            // 整卸已物化的技能/特效并撤销加成，再清空计划状态
            Plan.UnapplyFromCharacter(Character);
            Plan.CombatTalent = null;
            Plan.LearnedCombatTalents.Clear();
            Plan.Classes.Clear();
            Plan.SubClasses.Clear();
            Character.FirstRoleType = RoleType.None;
            Character.SecondRoleType = RoleType.None;
            Character.ThirdRoleType = RoleType.None;
            if (Character.Level < Eq.MinLevelCanModifyDefaultClass)
            {
                if (Plan.DefaultClasses.Count == 0)
                {
                    return ClassPlanResult.Fail("无默认职业可恢复（洗点前请先完成 1 级职业选择）。");
                }
                foreach (Class def in Plan.DefaultClasses)
                {
                    Class record = def.Copy();
                    record.Level = 1; // 恢复的是 1 级默认状态
                    Plan.Classes.Add(record);
                    foreach (SubClass sub in Plan.DefaultSubClasses.Where(s => s.Class.GetIdName() == def.GetIdName()))
                    {
                        Plan.SubClasses.Add(sub.Copy(record));
                    }
                }
                Plan.ClassPoints = 0;
                Plan.OnLevelUp();
                Raise(ClassPlanPhase.ResetPlan, true, $"已恢复 1 级默认职业（角色未满 {Eq.MinLevelCanModifyDefaultClass} 级）。");
            }
            else
            {
                Plan.ClassPoints = 0;
                Plan.OnLevelUp();
                Raise(ClassPlanPhase.ResetPlan, true, $"已清空职业规划（角色已满 {Eq.MinLevelCanModifyDefaultClass} 级，可重新选择并更新默认）。");
            }
            return ClassPlanResult.Ok();
        }

        /// <summary>
        /// 修改默认职业与流派（仅角色满 <see cref="EquilibriumConstant.MinLevelCanModifyDefaultClass"/> 级时允许）
        /// </summary>
        public ClassPlanResult ChangeDefaultPlan(Class classDef, SubClass subClassDef)
        {
            if (Character.Level < Eq.MinLevelCanModifyDefaultClass)
            {
                return ClassPlanResult.Fail($"角色需满 {Eq.MinLevelCanModifyDefaultClass} 级才能修改默认职业与流派。");
            }
            if (subClassDef.Class.GetIdName() != classDef.GetIdName())
            {
                return ClassPlanResult.Fail($"流派【{subClassDef.Name}】不属于职业【{classDef.Name}】。");
            }
            Plan.DefaultClasses.Clear();
            Plan.DefaultSubClasses.Clear();
            Plan.DefaultClasses.Add(classDef);
            Plan.DefaultSubClasses.Add(subClassDef);
            Raise(ClassPlanPhase.ChangeDefault, true, $"默认职业已更新为【{classDef.Name}】/【{subClassDef.Name}】。");
            return ClassPlanResult.Ok();
        }

        /// <summary>
        /// 校验当前计划整体一致性（供上层 / 测试在规划结束时断言）
        /// </summary>
        /// <param name="error">不一致时的原因</param>
        /// <returns>是否一致</returns>
        public bool ValidateState(out string? error)
        {
            error = null;
            if (Plan.Classes.GroupBy(c => c.GetIdName()).Any(g => g.Count() > 1))
            {
                error = "计划中存在重复职业。";
                return false;
            }
            if (Plan.Classes.Any(c => c.Level > Eq.MaxClassLevel))
            {
                error = "职业等级超过上限。";
                return false;
            }
            foreach (SubClass sc in Plan.SubClasses)
            {
                if (!Plan.Classes.Any(c => ReferenceEquals(c, sc.Class)))
                {
                    error = $"流派【{sc.Name}】未绑定到计划中的职业记录。";
                    return false;
                }
            }
            if (Plan.CombatTalent != null)
            {
                if (!Plan.LearnedCombatTalents.Values.Any(t => ReferenceEquals(t, Plan.CombatTalent)))
                {
                    error = "激活的天赋不在已学列表中。";
                    return false;
                }
                if (Plan.IsCombatTalentCore != Plan.IsCoreTalentLevelBonusApplied)
                {
                    error = "核心定位天赋的等级加成与应用状态不一致。";
                    return false;
                }
            }
            else if (Plan.IsCoreTalentLevelBonusApplied)
            {
                error = "无激活天赋但存在遗留的核心天赋等级加成。";
                return false;
            }
            return true;
        }

        // ==================== 私有 ====================

        /// <summary>
        /// 撤销当前激活天赋（卸载特效并配对撤销核心天赋加成），不改计划引用
        /// </summary>
        private void DeactivateTalent()
        {
            if (Plan.CombatTalent == null)
            {
                return;
            }
            if (Plan.IsCoreTalentLevelBonusApplied)
            {
                Plan.SetCoreTalentLevelBonus(false);
            }
            Plan.CombatTalent.RemoveSkillFromCharacter(Character);
            Plan.CombatTalent = null;
        }

        /// <summary>
        /// 清空已学与已激活天赋（已物化的先卸载）
        /// </summary>
        private void ClearTalents()
        {
            DeactivateTalent();
            foreach (Skill talent in Plan.LearnedCombatTalents.Values)
            {
                talent.RemoveSkillFromCharacter(Character);
            }
            Plan.LearnedCombatTalents.Clear();
        }

        private static string GetRoleTypeName(RoleType roleType)
        {
            return roleType switch
            {
                RoleType.Core => "核心",
                RoleType.Vanguard => "先锋",
                RoleType.Guardian => "近卫",
                RoleType.Support => "支援",
                RoleType.Medic => "治疗",
                _ => roleType.ToString()
            };
        }

        private void Raise(ClassPlanPhase phase, bool success, string message)
        {
            if (Planned == null)
            {
                return;
            }
            Planned?.Invoke(this, new ClassPlanEventArgs(phase, Plan, success, message));
        }
    }
}
