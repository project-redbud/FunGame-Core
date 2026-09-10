using FunGame.Core.Entity;
using FunGame.Core.Library.Common.Event;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model
{
    /// <summary>
    /// 职业规划系统：把「规划操作 → 校验 → 写入 <see cref="Character.Class"/>」收敛为带事件推送的入口
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
        /// 职业升级路线图的结算器（扩展点：替换实现即可整体改写选择权 / 数值提升 / 技能提级口径）
        /// </summary>
        public IClassRewardSettler Settler { get; set; } = DefaultClassRewardSettler.Instance;

        /// <summary>
        /// 是否启用「已习得才挂载」的职业技能选择制
        /// <para/>false（默认）时职业池全量授予，选择权仅记账；置 true 后只挂载账本中已习得的职业技能
        /// </summary>
        public bool SkillSelectionEnabled
        {
            get => Plan.SkillSelectionEnabled;
            set => Plan.SkillSelectionEnabled = value;
        }

        /// <summary>
        /// 每次成功动作后触发（把 Interface/Event/ClassPlanEvents.cs 中各事件接口实例的监听方法挂载到此处）
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
        /// <para>1 级且尚无默认计划时，本次选择自动记录为默认职业 / 流派（洗点恢复用）</para>
        /// </summary>
        /// <param name="classDef">职业定义</param>
        /// <param name="subClassDef">流派定义，必须属于 <paramref name="classDef"/></param>
        /// <returns>结果</returns>
        public ClassPlanResult SelectClass(Class classDef, SubClass subClassDef)
        {
            if (classDef is null || subClassDef is null)
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
            // 兼职规则：职业数上限与角色等级门槛（默认不限制，可在平衡常数配置）
            if (Eq.MaxClassCount > 0 && Plan.Classes.Count >= Eq.MaxClassCount)
            {
                return ClassPlanResult.Fail($"至多可拥有 {Eq.MaxClassCount} 个职业（兼职上限），无法再选择职业【{classDef.Name}】。");
            }
            if (Plan.Classes.Count >= 1 && Character.Level < Eq.MinCharacterLevelForMulticlass)
            {
                return ClassPlanResult.Fail($"角色需满 {Eq.MinCharacterLevelForMulticlass} 级才能选择兼职职业（当前 {Character.Level} 级）。");
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
            SubClass subRecord = subClassDef.Copy(record);
            Plan.SubClasses.Add(subRecord);
            // 记录流派选择顺序（次要定位的同级排序依据）
            if (!Plan.SubClassOrder.Contains(subRecord.GetIdName()))
            {
                Plan.SubClassOrder.Add(subRecord.GetIdName());
            }
            // 1 级首职业自动记为默认（洗点恢复用），此后角色满 20 级才允许修改默认
            if (Plan.DefaultClasses.Count == 0 && Character.Level <= 1)
            {
                Plan.DefaultClasses.Add(classDef);
                Plan.DefaultSubClasses.Add(subClassDef);
            }
            // 新职业从 1 级起步：立即结算 1 级奖励（职业初始属性分配 + 流派固有被动）
            ClassRewardSettlementResult settled = SettleRewards(record, 0, record.Level);
            // 新流派带来的候选定位立即可用于次要定位
            Plan.SyncRoleTypes();
            string message = $"已选择职业【{classDef.Name}】流派【{subClassDef.Name}】。" + (settled.Success ? $"1 级奖励：{settled.Message}" : "")
                + $"当前定位：{DescribeRoles()}。";
            Raise(ClassPlanPhase.SelectClass, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 职业升级（+1 级，不超过上限），消耗 1 点职业点数
        /// </summary>
        /// <param name="record"><see cref="CharacterClass.Classes"/> 中的职业记录</param>
        public ClassPlanResult UpgradeClass(Class record)
        {
            if (record is null || !Plan.Classes.Contains(record))
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
            int fromLevel = record.Level;
            record.Level++;
            ClassRewardSettlementResult settled = SettleRewards(record, fromLevel, record.Level);
            // 职业等级变化会影响次要定位的展开顺序
            Plan.SyncRoleTypes();
            string message = $"职业【{record.Name}】升至 {record.Level} 级。" + (settled.Success ? $"奖励：{settled.Message}" : $"奖励结算失败：{settled.Message}");
            Raise(ClassPlanPhase.UpgradeClass, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 重新推导角色定位（主要 = 当前生效战斗天赋所属定位；次要 = 流派按职业等级降序展开）
        /// <para>定位已不再由玩家手动选择：本方法只做刷新，供上层在外部改动流派 / 天赋后调用</para>
        /// </summary>
        public ClassPlanResult RefreshRoleTypes()
        {
            Plan.SyncRoleTypes();
            string message = $"已刷新角色定位：{DescribeRoles()}。";
            Raise(ClassPlanPhase.SelectRoleTypes, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 学习战斗天赋：数量必须与已选定位一致，且天赋须属于对应定位在已选职业中的天赋池
        /// </summary>
        /// <param name="roleType">天赋对应的定位（必须在已选定位中）</param>
        /// <param name="talent">天赋技能（来自职业天赋池的实例）</param>
        public ClassPlanResult LearnCombatTalent(RoleType roleType, Skill talent)
        {
            if (talent is null)
            {
                return ClassPlanResult.Fail("天赋不能为空。");
            }
            HashSet<RoleType> candidates = [.. Plan.SubClasses.SelectMany(sc => sc.RoleTypes)];
            if (candidates.Count == 0)
            {
                return ClassPlanResult.Fail("尚未选择任何流派，定位候选为空。请先选择职业与流派。");
            }
            if (!candidates.Contains(roleType))
            {
                return ClassPlanResult.Fail($"定位【{CharacterSet.GetRoleTypeName(roleType)}】不在已选流派提供的候选定位中。");
            }
            bool inPool = Plan.Classes.Any(c => c.CombatTalents.TryGetValue(roleType, out HashSet<Skill>? pool) && pool.Any(t => t.GetIdName() == talent.GetIdName()));
            if (!inPool)
            {
                return ClassPlanResult.Fail($"天赋【{talent.Name}】不属于已选职业的 {CharacterSet.GetRoleTypeName(roleType)} 天赋池。");
            }
            if (Plan.HasLearnedTalent(talent))
            {
                return ClassPlanResult.Fail($"天赋【{talent.Name}】已学习，无需重复学习。");
            }
            if (Plan.LearnedTalentCount >= CharacterClass.MaxLearnedTalentCount)
            {
                return ClassPlanResult.Fail($"同时掌握的战斗天赋已达上限 {CharacterClass.MaxLearnedTalentCount} 个，请先遗忘一个再学习。");
            }
            // 追加进该定位的天赋列表（同一定位可掌握多个，学习本身不挂载；激活才挂载）
            if (!Plan.LearnedCombatTalents.TryGetValue(roleType, out List<Skill>? learned))
            {
                learned = [];
                Plan.LearnedCombatTalents[roleType] = learned;
            }
            learned.Add(talent);
            // 学满 2 个天赋即具备【转换战斗天赋】的使用前提，此时按需授予战技
            Plan.RefreshCombatTalentSwitchSkill(Character);
            Plan.SyncRoleTypes();
            string message = $"已学习 {CharacterSet.GetRoleTypeName(roleType)} 天赋【{talent.Name}】（已学 {Plan.LearnedTalentCount} / {CharacterClass.MaxLearnedTalentCount}，"
                + $"{(Plan.CombatTalent is null ? "尚未激活任何天赋" : $"当前生效【{Plan.CombatTalent.Name}】")}）。";
            Raise(ClassPlanPhase.LearnTalent, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 激活 / 转换战斗天赋（始终至多 1 个生效；核心定位天赋的等级加成自动加减配对）
        /// <para>委托 <see cref="CharacterClass.SwitchCombatTalent(RoleType, out string?)"/>，与【转换战斗天赋】战技共用同一路径</para>
        /// </summary>
        /// <param name="roleType">要激活的已学天赋对应定位</param>
        public ClassPlanResult ActivateCombatTalent(RoleType roleType)
        {
            if (!Plan.SwitchCombatTalent(roleType, out string? error))
            {
                return ClassPlanResult.Fail(error ?? "天赋转换失败。");
            }
            Skill? talent = Plan.CombatTalent;
            string message = $"已激活 {CharacterSet.GetRoleTypeName(roleType)} 天赋【{talent?.Name}】。";
            Raise(ClassPlanPhase.ActivateTalent, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 按天赋实例激活 / 转换
        /// </summary>
        /// <param name="talent">要激活的已学天赋</param>
        public ClassPlanResult ActivateCombatTalent(Skill talent)
        {
            if (!Plan.SwitchCombatTalent(talent, out string? error))
            {
                return ClassPlanResult.Fail(error ?? "天赋转换失败。");
            }
            string message = $"已激活 {CharacterSet.GetRoleTypeName(Plan.RoleOf(talent))} 天赋【{talent.Name}】。";
            Raise(ClassPlanPhase.ActivateTalent, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 遗忘一个已学战斗天赋：释放名额，可再学新天赋完成替换
        /// <para/>已学天赋是战斗内【转换战斗天赋】的策略池，上限 <see cref="CharacterClass.MaxLearnedTalentCount"/>
        /// <para/>只约束「同时掌握」的数量，遗忘不消耗也不退还资源
        /// <para/>遗忘的是当前生效天赋时自动接续到剩余已学天赋的第一个；已学不足 2 个会收回【转换战斗天赋】战技
        /// </summary>
        /// <param name="talent">要遗忘的已学天赋</param>
        public ClassPlanResult ForgetCombatTalent(Skill talent)
        {
            if (talent is null)
            {
                return ClassPlanResult.Fail("天赋不能为空。");
            }
            string name = talent.Name;
            if (!Plan.ForgetCombatTalent(talent, out string? error))
            {
                return ClassPlanResult.Fail(error ?? "遗忘天赋失败。");
            }
            string active = Plan.CombatTalent is null ? "当前未激活任何天赋" : $"当前生效【{Plan.CombatTalent.Name}】";
            string message = $"已遗忘天赋【{name}】（已学 {Plan.LearnedTalentCount} / {CharacterClass.MaxLearnedTalentCount}，{active}）。";
            Raise(ClassPlanPhase.ForgetTalent, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 注入【转换战斗天赋】战技实例（由模组提供模板），并按当前是否具备次要定位即时授予 / 收回
        /// <para>仅挂载 / 卸载实例，不改变该战技自身的类型与决策点行为</para>
        /// </summary>
        /// <param name="switchSkill">模组的战技实例（须为独立的职业战技实例）</param>
        public ClassPlanResult SetCombatTalentSwitchSkill(Skill switchSkill)
        {
            if (switchSkill is null)
            {
                return ClassPlanResult.Fail("【转换战斗天赋】战技不能为空。");
            }
            Plan.CombatTalentSwitchSkill = switchSkill;
            Plan.RefreshCombatTalentSwitchSkill(Character);
            bool granted = Plan.HasCombatTalentSwitch;
            string message = $"已配置【转换战斗天赋】战技【{switchSkill.Name}】{(granted ? "并已授予角色" : "（待学会第 2 个天赋后授予）")}。";
            Raise(ClassPlanPhase.LearnTalent, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 洗点：清空当前职业规划（含已物化的技能与特效）；20 级前只能恢复到 1 级默认职业与流派，20 级起可完全重选。清空后点数按等级重算，由上层重新规划
        /// </summary>
        public ClassPlanResult ResetPlan()
        {
            // 整卸已物化的技能/特效并撤销加成，再清空计划状态
            Plan.UnapplyFromCharacter(Character);
            // 路线图奖励一并撤销：属性扣回、账本清空
            foreach (Class record in Plan.Classes)
            {
                Settler.Revoke(CreateRewardContext(record), Plan.GetOrCreateLedger(record));
            }
            Plan.RewardLedgers.Clear();
            Plan.CombatTalent = null;
            Plan.LearnedCombatTalents.Clear();
            Plan.Classes.Clear();
            Plan.SubClasses.Clear();
            Plan.SubClassOrder.Clear();
            Character.PrimaryRoleType = RoleType.None;
            Character.SecondaryRoleTypes.Clear();
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
                        SubClass subRecord = sub.Copy(record);
                        Plan.SubClasses.Add(subRecord);
                        if (!Plan.SubClassOrder.Contains(subRecord.GetIdName()))
                        {
                            Plan.SubClassOrder.Add(subRecord.GetIdName());
                        }
                    }
                    // 恢复的 1 级默认职业同样要拿到 1 级奖励（职业初始属性分配 + 固有被动）
                    SettleRewards(record, 0, record.Level);
                }
                Plan.ClassPoints = 0;
                Plan.OnLevelUp();
                Plan.SyncRoleTypes();
                string restoreMessage = $"已恢复 1 级默认职业（角色未满 {Eq.MinLevelCanModifyDefaultClass} 级）。";
                Raise(ClassPlanPhase.ResetPlan, true, restoreMessage);
                return ClassPlanResult.Ok(restoreMessage);
            }
            else
            {
                Plan.ClassPoints = 0;
                Plan.OnLevelUp();
                Plan.SyncRoleTypes();
                string clearMessage = $"已清空职业规划（角色已满 {Eq.MinLevelCanModifyDefaultClass} 级，可重新选择并更新默认）。";
                Raise(ClassPlanPhase.ResetPlan, true, clearMessage);
                return ClassPlanResult.Ok(clearMessage);
            }
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
            string message = $"默认职业已更新为【{classDef.Name}】/【{subClassDef.Name}】。";
            Raise(ClassPlanPhase.ChangeDefault, true, message);
            return ClassPlanResult.Ok(message);
        }

        // ==================== 路线图奖励 ====================

        /// <summary>
        /// 消耗选择权习得职业技能 / 被动（按技能类型自动判断使用哪种选择权）
        /// <para>技能须来自该职业池副本并满足流派、属性前置；习得后立即物化到角色</para>
        /// </summary>
        /// <param name="record">职业记录</param>
        /// <param name="skill">要习得的技能（需传入独立实例）</param>
        public ClassPlanResult LearnClassSkill(Class record, Skill skill)
        {
            if (record is null || !Plan.Classes.Contains(record))
            {
                return ClassPlanResult.Fail("职业记录不存在于当前计划中。");
            }
            ClassRewardLedger ledger = Plan.GetOrCreateLedger(record);
            ClassRewardSettlementResult result = Settler.SpendSkillChoice(CreateRewardContext(record), ledger, skill);
            if (!result.Success)
            {
                return ClassPlanResult.Fail(result.Message);
            }
            Raise(ClassPlanPhase.LearnClassSkill, true, $"职业【{record.Name}】{result.Message}");
            return ClassPlanResult.Ok(result.Message);
        }

        /// <summary>
        /// 领取 1 级初始分配权：按 <see cref="EquilibriumConstant.InitialAttributeBudget"/> 分配核心属性
        /// <para>受限分配：受职业模板与角色模板限值的交集约束；每个职业仅可领取一次</para>
        /// </summary>
        /// <param name="record">职业记录</param>
        /// <param name="allocation">本次分配到的初始核心属性与成长</param>
        public ClassPlanResult TakeInitialAllocation(Class record, ClassAttributeAllocation allocation)
        {
            if (record is null || !Plan.Classes.Contains(record))
            {
                return ClassPlanResult.Fail("职业记录不存在于当前计划中。");
            }
            ClassRewardLedger ledger = Plan.GetOrCreateLedger(record);
            ClassRewardSettlementResult result = Settler.SpendInitialAllocation(CreateRewardContext(record), ledger, allocation);
            if (!result.Success)
            {
                return ClassPlanResult.Fail(result.Message);
            }
            Raise(ClassPlanPhase.AllocateAttribute, true, $"职业【{record.Name}】{result.Message}");
            return ClassPlanResult.Ok(result.Message);
        }

        /// <summary>
        /// 兑换一次数值提升（4 / 9 级提供，替代被动选择），按指定的核心属性分配落地
        /// <para>不受模板上下限约束，仅校验 <see cref="EquilibriumConstant.NumericBoostBudget"/> 的总额度</para>
        /// </summary>
        /// <param name="record">职业记录</param>
        /// <param name="allocation">本次分配到的初始核心属性与成长</param>
        public ClassPlanResult TakeNumericBoost(Class record, ClassAttributeAllocation allocation)
        {
            if (record is null || !Plan.Classes.Contains(record))
            {
                return ClassPlanResult.Fail("职业记录不存在于当前计划中。");
            }
            ClassRewardLedger ledger = Plan.GetOrCreateLedger(record);
            ClassRewardSettlementResult result = Settler.SpendNumericBoost(CreateRewardContext(record), ledger, allocation);
            if (!result.Success)
            {
                return ClassPlanResult.Fail(result.Message);
            }
            Raise(ClassPlanPhase.AllocateAttribute, true, $"职业【{record.Name}】{result.Message}");
            return ClassPlanResult.Ok(result.Message);
        }

        /// <summary>
        /// 兑换一次数值提升并把额度全部分配到角色核心属性（<see cref="Character.PrimaryAttribute"/>）上
        /// </summary>
        /// <param name="record">职业记录</param>
        public ClassPlanResult TakeNumericBoost(Class record)
        {
            if (record is null || !Plan.Classes.Contains(record))
            {
                return ClassPlanResult.Fail("职业记录不存在于当前计划中。");
            }
            ClassRewardContext ctx = CreateRewardContext(record);
            ClassRewardLedger ledger = Plan.GetOrCreateLedger(record);
            ClassAttributeBudget? budget = Settler.NumericBoostBudget(ctx, ledger);
            if (budget is null)
            {
                return ClassPlanResult.Fail("未配置数值提升额度（EquilibriumConstant.NumericBoostBudget），请显式指定分配。");
            }
            ClassAttributeAllocation allocation = Character.PrimaryAttribute switch
            {
                PrimaryAttribute.AGI => new(0, budget.AttributePoints, 0, 0, budget.GrowthPoints, 0),
                PrimaryAttribute.INT => new(0, 0, budget.AttributePoints, 0, 0, budget.GrowthPoints),
                _ => new(budget.AttributePoints, 0, 0, budget.GrowthPoints, 0, 0)
            };
            return TakeNumericBoost(record, allocation);
        }

        /// <summary>
        /// 按当前职业等级重放路线图奖励（存档恢复 / 外部直接改动职业等级后对齐账本）
        /// <para>水位低于当前等级时补发；水位高于当前等级（等级被下调）时先撤销再重放</para>
        /// </summary>
        public ClassPlanResult SyncRewards()
        {
            foreach (Class record in Plan.Classes)
            {
                ClassRewardLedger ledger = Plan.GetOrCreateLedger(record);
                if (ledger.SettledToLevel == record.Level)
                {
                    continue;
                }
                if (ledger.SettledToLevel > record.Level)
                {
                    Settler.Revoke(CreateRewardContext(record), ledger);
                }
                SettleRewards(record, ledger.SettledToLevel, record.Level);
            }
            const string message = "已按当前职业等级重放路线图奖励。";
            Raise(ClassPlanPhase.SettleReward, true, message);
            return ClassPlanResult.Ok(message);
        }

        /// <summary>
        /// 取某职业可选的职业技能池（供上层列候选；已习得的会一并给出）
        /// </summary>
        public IEnumerable<Skill> GetClassSkillPool(Class record)
        {
            return DefaultClassRewardSettler.AllPoolSkills(record);
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
            foreach (Class c in Plan.Classes)
            {
                if (Plan.RewardLedgers.TryGetValue(c.GetIdName(), out ClassRewardLedger? ledger) && ledger.SettledToLevel > c.Level)
                {
                    error = $"职业【{c.Name}】的奖励结算水位（{ledger.SettledToLevel}）高于当前职业等级（{c.Level}）。";
                    return false;
                }
            }
            foreach (SubClass sc in Plan.SubClasses)
            {
                if (!Plan.Classes.Any(c => ReferenceEquals(c, sc.Class)))
                {
                    error = $"流派【{sc.Name}】未绑定到计划中的职业记录。";
                    return false;
                }
            }
            if (Plan.LearnedTalentCount > CharacterClass.MaxLearnedTalentCount)
            {
                error = $"已学天赋数量 {Plan.LearnedTalentCount} 超过上限 {CharacterClass.MaxLearnedTalentCount}。";
                return false;
            }
            if (Plan.CombatTalent != null)
            {
                if (!Plan.HasLearnedTalent(Plan.CombatTalent))
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
        /// 描述当前定位（主要 / 次要）用于提示文案
        /// </summary>
        private string DescribeRoles()
        {
            string primary = Character.PrimaryRoleType == RoleType.None
                ? "无（未激活战斗天赋）"
                : CharacterSet.GetRoleTypeName(Character.PrimaryRoleType);
            string secondary = Character.SecondaryRoleTypes.Count == 0
                ? "无"
                : string.Join(" / ", Character.SecondaryRoleTypes.Select(CharacterSet.GetRoleTypeName));
            return $"主要 {primary}；次要 {secondary}";
        }

        /// <summary>
        /// 构造结算上下文（自动绑定该职业记录对应的流派，并判定其是否为首个职业）
        /// </summary>
        private ClassRewardContext CreateRewardContext(Class record)
        {
            SubClass? subClass = Plan.SubClasses.FirstOrDefault(sc => ReferenceEquals(sc.Class, record));
            bool isFirstClass = Plan.Classes.Count <= 1;
            return new ClassRewardContext(Character, record, subClass, Plan, isFirstClass);
        }

        /// <summary>
        /// 结算 (fromLevel, toLevel] 区间的路线图奖励并推送事件
        /// </summary>
        private ClassRewardSettlementResult SettleRewards(Class record, int fromLevel, int toLevel)
        {
            ClassRewardLedger ledger = Plan.GetOrCreateLedger(record);
            ClassRewardSettlementResult result = Settler.Settle(CreateRewardContext(record), ledger, fromLevel, toLevel);
            Raise(ClassPlanPhase.SettleReward, result.Success, $"职业【{record.Name}】{result.Message}");
            return result;
        }

        /// <summary>
        /// 撤销已学与已激活天赋（已物化的先卸载）
        /// </summary>
        public void ClearTalents()
        {
            Plan.DeactivateCombatTalent();
            foreach (Skill talent in Plan.AllLearnedTalents)
            {
                talent.RemoveSkillFromCharacter(Character);
            }
            Plan.LearnedCombatTalents.Clear();
        }

        /// <summary>
        /// 推送规划事件
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="success"></param>
        /// <param name="message"></param>
        private void Raise(ClassPlanPhase phase, bool success, string message)
        {
            if (Planned is null)
            {
                return;
            }
            Planned?.Invoke(this, new ClassPlanEventArgs(phase, Plan, success, message));
        }
    }
}
