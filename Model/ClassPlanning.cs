using System.Runtime.Intrinsics.X86;
using System.Text;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 职业规划管理类
    /// 负责处理角色的职业点数分配、职业升级、流派选择、技能选择等逻辑
    /// </summary>
    public class ClassPlanning(Character character)
    {
        /// <summary>
        /// 所属的角色
        /// </summary>
        public Character Character => character;

        /// <summary>
        /// 角色当前的职业点数
        /// </summary>
        public int ClassPoints { get; private set; } = 0;

        /// <summary>
        /// 所有可用职业列表（全局静态池）
        /// </summary>
        public List<Class> AllClasses { get; private set; } = [];

        /// <summary>
        /// 所有可用流派列表（全局静态池）
        /// </summary>
        public List<SubClass> AllSubClasses { get; private set; } = [];

        /// <summary>
        /// 已选择的职业列表（运行时）
        /// </summary>
        public List<SelectedClass> SelectedClasses { get; private set; } = [];

        /// <summary>
        /// 已选择的流派列表（运行时）
        /// </summary>
        public List<SelectedSubClass> SelectedSubClasses { get; private set; } = [];

        /// <summary>
        /// 可选的战斗天赋池（按定位分类）
        /// </summary>
        public Dictionary<RoleType, List<Skill>> CombatTalentPool { get; private set; } = [];

        /// <summary>
        /// 当前激活的战斗天赋
        /// </summary>
        public Skill? CurrentCombatTalent { get; private set; }

        /// <summary>
        /// 角色定位（最多3个）
        /// </summary>
        public List<RoleType> RoleTypes { get; private set; } = [];

        /// <summary>
        /// 默认职业状态（角色1级时选择的职业和流派）
        /// </summary>
        public DefaultClassState DefaultState { get; private set; } = new();

        /// <summary>
        /// 是否可以修改默认职业和流派（角色≥20级时可修改）
        /// </summary>
        public bool CanModifyDefault => Character.Level >= 20;

        /// <summary>
        /// 已使用的总职业点数
        /// </summary>
        public int UsedClassPoints => SelectedClasses.Sum(c => c.Level);

        /// <summary>
        /// 剩余可用的职业点数
        /// </summary>
        public int AvailableClassPoints => ClassPoints - UsedClassPoints;

        /// <summary>
        /// 初始化职业规划器
        /// </summary>
        /// <param name="allClasses">所有职业池</param>
        /// <param name="allSubClasses">所有流派池</param>
        /// <param name="combatTalentPool">战斗天赋池</param>
        public void Initialize(List<Class> allClasses, List<SubClass> allSubClasses, Dictionary<RoleType, List<Skill>> combatTalentPool)
        {
            AllClasses = allClasses;
            AllSubClasses = allSubClasses;
            CombatTalentPool = combatTalentPool;

            // 计算当前职业点数
            CalculateClassPoints();

            // 加载角色当前的职业配置
            LoadCurrentConfiguration();
        }

        /// <summary>
        /// 计算角色当前可用的职业点数
        /// </summary>
        private void CalculateClassPoints()
        {
            ClassPoints = 0;
            foreach (int level in Character.GameplayEquilibriumConstant.ClassPointsGetterList)
            {
                if (Character.Level >= level)
                {
                    ClassPoints++;
                }
            }

            // 至少1点
            if (ClassPoints == 0) ClassPoints = 1;
        }

        /// <summary>
        /// 加载角色当前的职业配置
        /// </summary>
        private void LoadCurrentConfiguration()
        {
            CharacterClass charClass = Character.Class;

            // 加载已选择的职业
            foreach (Class c in charClass.Classes)
            {
                SelectedClasses.Add(new SelectedClass
                {
                    Class = c,
                    Level = c.Level,
                    SelectedSkills = LoadSelectedSkillsForClass(c)
                });
            }

            // 加载已选择的流派
            foreach (SubClass sc in charClass.SubClasses)
            {
                SelectedSubClasses.Add(new SelectedSubClass
                {
                    SubClass = sc,
                    Level = sc.Level
                });
            }

            // 加载当前战斗天赋
            CurrentCombatTalent = charClass.CombatTalent;

            // 加载角色定位
            if (Character.FirstRoleType != RoleType.None)
                RoleTypes.Add(Character.FirstRoleType);
            if (Character.SecondRoleType != RoleType.None)
                RoleTypes.Add(Character.SecondRoleType);
            if (Character.ThirdRoleType != RoleType.None)
                RoleTypes.Add(Character.ThirdRoleType);

            // 加载默认状态（从数据库或配置文件）
            LoadDefaultState();
        }

        /// <summary>
        /// 加载已选择的职业技能
        /// </summary>
        private List<SelectedSkill> LoadSelectedSkillsForClass(Class @class)
        {
            // 这里需要从角色当前技能中筛选出属于该职业的技能
            // 简化实现：返回空列表
            return [];
        }

        /// <summary>
        /// 加载默认的职业状态
        /// </summary>
        private void LoadDefaultState()
        {
            // 从角色数据中加载默认状态
            // 如果没有默认状态，则使用当前第一个职业和流派
            if (SelectedClasses.Count > 0 && SelectedSubClasses.Count > 0)
            {
                DefaultState = new DefaultClassState
                {
                    DefaultClass = SelectedClasses[0].Class,
                    DefaultSubClass = SelectedSubClasses[0].SubClass
                };
            }
        }

        /// <summary>
        /// 获取指定职业的可用流派
        /// </summary>
        public List<SubClass> GetAvailableSubClasses(Class @class)
        {
            return AllSubClasses.Where(sc => sc.Class.Id == @class.Id).ToList();
        }

        /// <summary>
        /// 选择或升级职业
        /// </summary>
        /// <param name="classToSelect">要选择或升级的职业</param>
        /// <param name="isUpgrade">true: 升级现有职业, false: 添加新职业（兼职）</param>
        /// <returns>操作结果</returns>
        public ClassOperationResult SelectOrUpgradeClass(Class classToSelect, bool isUpgrade = false)
        {
            // 检查职业点数
            if (AvailableClassPoints < 1)
                return new ClassOperationResult(false, "职业点数不足");

            // 检查是否已选择相同职业
            SelectedClass? existingClass = SelectedClasses.FirstOrDefault(c => c.Class.Id == classToSelect.Id);

            if (existingClass != null)
            {
                if (isUpgrade)
                {
                    // 升级现有职业
                    if (existingClass.Level >= 10)
                        return new ClassOperationResult(false, "职业已满级（最高10级）");

                    existingClass.Level++;
                    UpdateClassSkills(existingClass);
                    return new ClassOperationResult(true, $"职业 {classToSelect.Name} 升级到 {existingClass.Level} 级");
                }
                else
                {
                    return new ClassOperationResult(false, "不能选择重复的职业");
                }
            }
            else
            {
                // 添加新职业（兼职）
                if (isUpgrade)
                    return new ClassOperationResult(false, "未找到该职业，无法升级");

                if (AvailableClassPoints < 1)
                    return new ClassOperationResult(false, "职业点数不足");

                SelectedClass newClass = new()
                {
                    Class = classToSelect,
                    Level = 1,
                    SelectedSkills = []
                };

                SelectedClasses.Add(newClass);
                InitializeClassSkills(newClass);

                return new ClassOperationResult(true, $"添加新职业: {classToSelect.Name} 1级");
            }
        }

        /// <summary>
        /// 选择流派
        /// </summary>
        public ClassOperationResult SelectSubClass(SubClass subClass)
        {
            SelectedSubClass newSubClass = new()
            {
                SubClass = subClass,
                Level = 1
            };

            SelectedSubClasses.Add(newSubClass);

            // 如果是主要流派且角色<20级，保存为默认状态
            if (DefaultState.DefaultSubClass is null && !CanModifyDefault)
            {
                DefaultState.DefaultClass = subClass.Class;
                DefaultState.DefaultSubClass = subClass;
            }

            return new ClassOperationResult(true, $"选择流派: {subClass.Name}");
        }

        /// <summary>
        /// 初始化职业技能（根据职业等级）
        /// </summary>
        private void InitializeClassSkills(SelectedClass selectedClass)
        {
            // 根据职业等级和升级路线图初始化技能
            // 1级: 获得流派固有被动*1 (需要先选择流派)
            // 2级: 职业技能选择权*2
            // 3级: 职业技能等级+1
            // 4级: 职业技能选择权*1(被动)或数值提升
            // 5级: 职业技能选择权*2，职业技能等级+1
            // 6级: 获得流派固有被动*1
            // 7级: 职业技能等级+1
            // 8级: 职业技能选择权*2，职业技能等级+1
            // 9级: 职业技能选择权*2(被动)或数值提升
            // 10级: 职业技能选择权*2，职业技能等级+1

            // 这里需要根据具体的职业技能池来实现
            // 简化实现
        }

        /// <summary>
        /// 更新职业技能（升级时）
        /// </summary>
        private void UpdateClassSkills(SelectedClass selectedClass)
        {
            int newLevel = selectedClass.Level;

            // 根据新的等级解锁或升级技能
            // 实现逻辑依赖于具体的职业技能池
        }

        /// <summary>
        /// 选择职业技能
        /// </summary>
        public ClassOperationResult SelectClassSkill(Class @class, Skill skill, SkillSelectionType selectionType)
        {
            SelectedClass? selectedClass = SelectedClasses.FirstOrDefault(c => c.Class.Id == @class.Id);
            if (selectedClass == null)
                return new ClassOperationResult(false, "未选择该职业");

            // 检查技能是否属于该职业的技能池
            if (!IsSkillInClassPool(@class, skill))
                return new ClassOperationResult(false, "该技能不属于此职业的技能池");

            // 检查流派和属性限制
            if (!CheckSkillRequirements(skill, selectedClass))
                return new ClassOperationResult(false, "不满足技能选择条件");

            // 根据选择类型处理
            switch (selectionType)
            {
                case SkillSelectionType.ActiveSkill:
                    // 检查是否已达到技能选择上限
                    int activeSkillCount = selectedClass.SelectedSkills.Count(s =>
                        s.Skill.SkillType == SkillType.Skill ||
                        s.Skill.SkillType == SkillType.Magic ||
                        s.Skill.SkillType == SkillType.SuperSkill);

                    if (activeSkillCount >= 8) // 最多8个自选技能
                        return new ClassOperationResult(false, "已达到主动技能选择上限");

                    break;

                case SkillSelectionType.PassiveSkill:
                    // 检查被动技能选择
                    int passiveCount = selectedClass.SelectedSkills.Count(s =>
                        s.Skill.SkillType == SkillType.Passive);

                    if (passiveCount >= 3) // 最多3个自选被动
                        return new ClassOperationResult(false, "已达到被动技能选择上限");

                    break;

                case SkillSelectionType.NumericBoost:
                    // 数值提升选择
                    if (selectedClass.NumericBoostCount >= 2) // 最多2次数值提升
                        return new ClassOperationResult(false, "已达到数值提升选择上限");

                    // 应用数值提升
                    ApplyNumericBoost(selectedClass);
                    selectedClass.NumericBoostCount++;
                    return new ClassOperationResult(true, "选择数值提升");
            }

            // 添加技能
            SelectedSkill selectedSkill = new()
            {
                Skill = skill,
                SelectionType = selectionType,
                Level = CalculateSkillLevel(selectedClass.Level, skill.SkillType)
            };

            selectedClass.SelectedSkills.Add(selectedSkill);

            return new ClassOperationResult(true, $"选择技能: {skill.Name}");
        }

        /// <summary>
        /// 检查技能是否在职业池中
        /// </summary>
        private bool IsSkillInClassPool(Class @class, Skill skill)
        {
            // 检查技能是否属于该职业的技能集合
            return @class.Skills.Contains(skill) ||
                   @class.Magics.Contains(skill) ||
                   @class.SuperSkills.Contains(skill) ||
                   @class.PassiveSkills.Contains(skill);
        }

        /// <summary>
        /// 检查技能选择条件
        /// </summary>
        private bool CheckSkillRequirements(Skill skill, SelectedClass selectedClass)
        {
            // 检查流派限制
            SelectedSubClass? subClass = SelectedSubClasses.FirstOrDefault(sc => sc.SubClass.Class.Id == selectedClass.Class.Id);
            if (subClass != null)
            {
                // 检查技能是否有流派限制
                // 这里需要技能对象有流派限制属性，暂时简化
            }

            // 检查属性要求
            if (!CheckAttributeRequirements(skill))
                return false;

            return true;
        }

        /// <summary>
        /// 检查属性要求
        /// </summary>
        private bool CheckAttributeRequirements(Skill skill)
        {
            // 检查技能是否有属性要求（如力量≥15）
            // 这里需要技能对象有属性要求属性，暂时简化
            return true;
        }

        /// <summary>
        /// 计算技能等级
        /// </summary>
        private int CalculateSkillLevel(int classLevel, SkillType skillType)
        {
            // 根据职业等级计算技能等级
            int baseLevel = classLevel switch
            {
                1 => 1,
                2 => 1,
                3 => 2,
                4 => 2,
                5 => 3,
                6 => 3,
                7 => 4,
                8 => 5,
                9 => 5,
                10 => 6,
                _ => 1
            };

            // 魔法技能额外+1
            if (skillType == SkillType.Magic)
                baseLevel++;

            return baseLevel;
        }

        /// <summary>
        /// 应用数值提升
        /// </summary>
        private void ApplyNumericBoost(SelectedClass selectedClass)
        {
            // 根据职业等级应用数值提升
            if (selectedClass.Level >= 4 && selectedClass.Level < 7)
            {
                // 4级数值提升: 6基础属性点 + 1成长总值
                ApplyAttributePoints(6);
                ApplyGrowthPoints(1);
            }
            else if (selectedClass.Level >= 9)
            {
                // 9级数值提升: 12基础属性点 + 2成长总值
                ApplyAttributePoints(12);
                ApplyGrowthPoints(2);
            }
        }

        /// <summary>
        /// 应用基础属性点
        /// </summary>
        private void ApplyAttributePoints(int points)
        {
            // 这里需要修改角色的基础属性
            // 简化实现
        }

        /// <summary>
        /// 应用成长总值
        /// </summary>
        private void ApplyGrowthPoints(int points)
        {
            // 这里需要修改角色的成长值
            // 简化实现
        }

        /// <summary>
        /// 选择角色定位
        /// </summary>
        public ClassOperationResult SelectRoleTypes(List<RoleType> roleTypes)
        {
            if (roleTypes.Count > 3)
                return new ClassOperationResult(false, "最多只能选择3个定位");

            // 检查定位是否来自已选择的流派
            List<RoleType> availableRoleTypes = [.. SelectedSubClasses.SelectMany(sc => sc.SubClass.RoleTypes).Distinct()];

            foreach (RoleType roleType in roleTypes)
            {
                if (!availableRoleTypes.Contains(roleType))
                    return new ClassOperationResult(false, $"定位 {roleType} 不可用");
            }

            RoleTypes = roleTypes;

            // 更新角色对象的定位
            UpdateCharacterRoleTypes();

            return new ClassOperationResult(true, "定位选择成功");
        }

        /// <summary>
        /// 选择战斗天赋
        /// </summary>
        public ClassOperationResult SelectCombatTalent(Skill combatTalent)
        {
            // 检查战斗天赋是否可用
            if (!IsCombatTalentAvailable(combatTalent))
                return new ClassOperationResult(false, "战斗天赋不可用");

            CurrentCombatTalent = combatTalent;

            // 如果是核心定位的战斗天赋，应用技能等级+1效果
            if (RoleTypes.Contains(RoleType.Core))
            {
                ApplyCoreTalentBonus();
            }

            return new ClassOperationResult(true, $"选择战斗天赋: {combatTalent.Name}");
        }

        /// <summary>
        /// 检查战斗天赋是否可用
        /// </summary>
        private bool IsCombatTalentAvailable(Skill combatTalent)
        {
            // 检查战斗天赋是否在可用池中
            foreach (RoleType roleType in RoleTypes)
            {
                if (CombatTalentPool.TryGetValue(roleType, out List<Skill>? talents) && talents.Contains(combatTalent))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 应用核心天赋加成（所有自身主动技能等级+1）
        /// </summary>
        private void ApplyCoreTalentBonus()
        {
            // 这里需要为角色所有自身主动技能等级+1
            // 简化实现
        }

        /// <summary>
        /// 获取转换战斗天赋的战技
        /// </summary>
        public Skill? GetSwitchCombatTalentSkill()
        {
            if (RoleTypes.Count <= 1 || CurrentCombatTalent == null)
                return null;

            // 如果有次要定位，提供转换战斗天赋战技
            // 这里需要创建一个Skill对象
            return null; // 简化实现
        }

        /// <summary>
        /// 洗点（重置职业规划）
        /// </summary>
        public ClassOperationResult Reset(bool resetToDefault = false)
        {
            if (resetToDefault && !CanModifyDefault)
            {
                // 20级前只能重置到默认状态
                ResetToDefaultState();
                return new ClassOperationResult(true, "已重置到默认职业状态");
            }
            else
            {
                // 完全重置
                SelectedClasses.Clear();
                SelectedSubClasses.Clear();
                CurrentCombatTalent = null;
                RoleTypes.Clear();

                return new ClassOperationResult(true, "职业规划已重置");
            }
        }

        /// <summary>
        /// 重置到默认状态
        /// </summary>
        private void ResetToDefaultState()
        {
            SelectedClasses.Clear();
            SelectedSubClasses.Clear();

            if (DefaultState.DefaultClass != null)
            {
                SelectedClasses.Add(new SelectedClass
                {
                    Class = DefaultState.DefaultClass,
                    Level = 1,
                    SelectedSkills = []
                });

                InitializeClassSkills(SelectedClasses[0]);
            }

            if (DefaultState.DefaultSubClass != null)
            {
                SelectedSubClasses.Add(new SelectedSubClass
                {
                    SubClass = DefaultState.DefaultSubClass,
                    Level = 1
                });
            }

            // 重置定位和天赋
            RoleTypes.Clear();
            CurrentCombatTalent = null;

            UpdateCharacterRoleTypes();
        }

        /// <summary>
        /// 更新角色对象的定位
        /// </summary>
        private void UpdateCharacterRoleTypes()
        {
            Character.FirstRoleType = RoleTypes.Count > 0 ? RoleTypes[0] : RoleType.None;
            Character.SecondRoleType = RoleTypes.Count > 1 ? RoleTypes[1] : RoleType.None;
            Character.ThirdRoleType = RoleTypes.Count > 2 ? RoleTypes[2] : RoleType.None;
        }

        /// <summary>
        /// 应用职业规划到角色
        /// </summary>
        public ClassOperationResult ApplyToCharacter()
        {
            try
            {
                // 创建ClassObject
                ClassObject classObject = new([.. SelectedClasses.Select(sc => sc.Class)], [.. SelectedSubClasses.Select(ssc => ssc.SubClass)])
                {
                    FirstRoleType = RoleTypes.Count > 0 ? RoleTypes[0] : RoleType.None,
                    SecondRoleType = RoleTypes.Count > 1 ? RoleTypes[1] : RoleType.None,
                    ThirdRoleType = RoleTypes.Count > 2 ? RoleTypes[2] : RoleType.None,
                    CurrentCombatTalent = CurrentCombatTalent
                };

                // 应用职业技能
                foreach (SelectedClass selectedClass in SelectedClasses)
                {
                    ApplyClassSkillsToCharacter(selectedClass);
                }

                // 应用流派固有被动
                foreach (SelectedSubClass selectedSubClass in SelectedSubClasses)
                {
                    ApplySubClassPassivesToCharacter(selectedSubClass);
                }

                // 通过CharacterClass重建角色职业
                Character.Class.ReBuildCharacterClass(classObject);

                // 如果是核心定位，应用技能等级加成
                if (RoleTypes.Contains(RoleType.Core) && CurrentCombatTalent != null)
                {
                    ApplyCoreSkillLevelBonus();
                }

                return new ClassOperationResult(true, "职业规划应用成功");
            }
            catch (Exception ex)
            {
                return new ClassOperationResult(false, $"应用失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 应用职业技能到角色
        /// </summary>
        private void ApplyClassSkillsToCharacter(SelectedClass selectedClass)
        {
            foreach (SelectedSkill selectedSkill in selectedClass.SelectedSkills)
            {
                // 设置技能等级
                selectedSkill.Skill.Level = selectedSkill.Level;

                // 添加到角色
                selectedSkill.Skill.AddSkillToCharacter(Character);
            }
        }

        /// <summary>
        /// 应用流派固有被动到角色
        /// </summary>
        private void ApplySubClassPassivesToCharacter(SelectedSubClass selectedSubClass)
        {
            // 根据流派等级应用对应的固有被动
            // 1级和6级获得不同的固有被动
            List<Skill> inherentPassives = GetInherentPassivesForLevel(selectedSubClass.SubClass, selectedSubClass.Level);

            foreach (Skill passive in inherentPassives)
            {
                passive.AddSkillToCharacter(Character);
            }
        }

        /// <summary>
        /// 根据流派等级获取固有被动
        /// </summary>
        private static List<Skill> GetInherentPassivesForLevel(SubClass subClass, int level)
        {
            List<Skill> passives = [];

            // 1级固有被动
            if (level >= 1)
            {
                // 获取1级固有被动（第一个）
                if (subClass.InherentPassives.Count > 0)
                    passives.Add(subClass.InherentPassives.First());
            }

            // 6级固有被动
            if (level >= 6)
            {
                // 获取6级固有被动（第二个）
                if (subClass.InherentPassives.Count > 1)
                    passives.Add(subClass.InherentPassives.Skip(1).First());
            }

            return passives;
        }

        /// <summary>
        /// 应用核心技能等级加成
        /// </summary>
        private void ApplyCoreSkillLevelBonus()
        {
            // 为所有自身主动技能等级+1
            foreach (Skill skill in Character.Skills)
            {
                if (skill.SkillType != SkillType.Passive)
                {
                    skill.ExLevel += 1;
                }
            }

            // 为职业技能等级+1
            foreach (SelectedClass selectedClass in SelectedClasses)
            {
                foreach (SelectedSkill selectedSkill in selectedClass.SelectedSkills)
                {
                    selectedSkill.Level += 1;
                    selectedSkill.Skill.ExLevel += 1;
                }
            }
        }

        /// <summary>
        /// 获取职业规划详细信息
        /// </summary>
        public string GetPlanningInfo()
        {
            StringBuilder sb = new();

            sb.AppendLine("=== 职业规划信息 ===");
            sb.AppendLine($"角色: {Character.Name}");
            sb.AppendLine($"等级: {Character.Level}");
            sb.AppendLine($"职业点数: {ClassPoints} (已用: {UsedClassPoints}, 剩余: {AvailableClassPoints})");
            sb.AppendLine();

            sb.AppendLine("=== 已选择职业 ===");
            foreach (SelectedClass sc in SelectedClasses)
            {
                sb.AppendLine($"- {sc.Class.Name} Lv.{sc.Level}");
                foreach (SelectedSkill skill in sc.SelectedSkills)
                {
                    sb.AppendLine($"  {skill.SelectionType}: {skill.Skill.Name} Lv.{skill.Level}");
                }
                if (sc.NumericBoostCount > 0)
                    sb.AppendLine($"  数值提升次数: {sc.NumericBoostCount}");
            }

            sb.AppendLine();
            sb.AppendLine("=== 已选择流派 ===");
            foreach (SelectedSubClass ssc in SelectedSubClasses)
            {
                sb.AppendLine($"- {ssc.SubClass.Name} Lv.{ssc.Level}");
            }

            sb.AppendLine();
            sb.AppendLine("=== 角色定位 ===");
            sb.AppendLine(string.Join(", ", RoleTypes.Select(rt => rt.ToString())));

            sb.AppendLine();
            sb.AppendLine("=== 战斗天赋 ===");
            sb.AppendLine(CurrentCombatTalent?.Name ?? "未选择");

            if (RoleTypes.Contains(RoleType.Core) && CurrentCombatTalent != null)
            {
                sb.AppendLine("(核心天赋激活: 所有自身主动技能等级+1)");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// 已选择的职业（包含等级和选择的技能）
    /// </summary>
    public class SelectedClass
    {
        public Class Class { get; set; } = null!;
        public int Level { get; set; } = 1;
        public List<SelectedSkill> SelectedSkills { get; set; } = [];
        public int NumericBoostCount { get; set; } = 0;
    }

    /// <summary>
    /// 已选择的流派
    /// </summary>
    public class SelectedSubClass
    {
        public SubClass SubClass { get; set; } = null!;
        public int Level { get; set; } = 1;
    }

    /// <summary>
    /// 已选择的技能
    /// </summary>
    public class SelectedSkill
    {
        public Skill Skill { get; set; } = null!;
        public SkillSelectionType SelectionType { get; set; }
        public int Level { get; set; } = 1;
    }

    /// <summary>
    /// 技能选择类型
    /// </summary>
    public enum SkillSelectionType
    {
        ActiveSkill,
        PassiveSkill,
        NumericBoost
    }

    /// <summary>
    /// 默认职业状态（用于洗点）
    /// </summary>
    public class DefaultClassState
    {
        public Class? DefaultClass { get; set; }
        public SubClass? DefaultSubClass { get; set; }
    }

    /// <summary>
    /// 职业操作结果
    /// </summary>
    public class ClassOperationResult(bool success, string message)
    {
        public bool Success { get; } = success;
        public string Message { get; } = message;
    }
}