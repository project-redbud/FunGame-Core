using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 职业计划存档快照：只保存可序列化的 IdName 与状态，读档时经 <see cref="ClassDefinitionRegistry"/> 重建
    /// <para/>· 职业记录（等级 + 技能池各技能等级 / 突破等级）
    /// <para/>· 流派记录、奖励账本、已学与激活天赋、转换战斗天赋战技、三个定位、默认职业与流派
    /// </summary>
    public class ClassPlanSnapshot
    {
        /// <summary>是否启用「已习得才挂载」选择制</summary>
        public bool SkillSelectionEnabled { get; set; } = true;

        /// <summary>剩余职业点数</summary>
        public int ClassPoints { get; set; }

        /// <summary>主要定位（= 生效战斗天赋所属定位）</summary>
        public string PrimaryRoleType { get; set; } = nameof(RoleType.None);

        /// <summary>次要定位（由流派自动推导）</summary>
        public List<string> SecondaryRoleTypes { get; set; } = [];

        /// <summary>流派选择顺序（IdName），用于次要定位的同级排序</summary>
        public List<string> SubClassOrder { get; set; } = [];

        /// <summary>职业记录</summary>
        public List<ClassRecordSnapshot> Classes { get; set; } = [];

        /// <summary>流派记录</summary>
        public List<SubClassRecordSnapshot> SubClasses { get; set; } = [];

        /// <summary>路线图奖励账本（key = 职业 IdName）</summary>
        public Dictionary<string, ClassRewardLedger> RewardLedgers { get; set; } = [];

        /// <summary>已学战斗天赋（key = 定位名，value = 该定位下已学的天赋 IdName 列表，同定位可掌握多个）</summary>
        public Dictionary<string, List<string>> LearnedTalents { get; set; } = [];

        /// <summary>当前激活战斗天赋的 IdName，null 表示未激活</summary>
        public string? ActiveTalentId { get; set; }

        /// <summary>【转换战斗天赋】战技 IdName</summary>
        public string? CombatTalentSwitchSkillId { get; set; }

        /// <summary>默认职业 IdName（洗点恢复用）</summary>
        public List<string> DefaultClassIds { get; set; } = [];

        /// <summary>默认流派 IdName（洗点恢复用）</summary>
        public List<string> DefaultSubClassIds { get; set; } = [];

        /// <summary>
        /// 从职业计划导出快照
        /// </summary>
        public static ClassPlanSnapshot Capture(CharacterClass plan)
        {
            ClassPlanSnapshot snapshot = new()
            {
                SkillSelectionEnabled = plan.SkillSelectionEnabled,
                ClassPoints = plan.ClassPoints,
                PrimaryRoleType = plan.Character.PrimaryRoleType.ToString(),
                SecondaryRoleTypes = [.. plan.Character.SecondaryRoleTypes.Select(r => r.ToString())],
                SubClassOrder = [.. plan.SubClassOrder],
                CombatTalentSwitchSkillId = plan.CombatTalentSwitchSkill?.GetIdName(),
                DefaultClassIds = [.. plan.DefaultClasses.Select(c => c.GetIdName())],
                DefaultSubClassIds = [.. plan.DefaultSubClasses.Select(s => s.GetIdName())]
            };
            foreach (Class record in plan.Classes)
            {
                ClassRecordSnapshot recordSnapshot = new()
                {
                    IdName = record.GetIdName(),
                    Id = record.Id,
                    Name = record.Name,
                    Level = record.Level
                };
                foreach (Skill skill in AllPoolSkills(record))
                {
                    recordSnapshot.Skills.Add(new ClassSkillStateSnapshot
                    {
                        Id = skill.Id,
                        Name = skill.Name,
                        SkillType = skill.SkillType.ToString(),
                        Level = skill.Level,
                        ExLevel = skill.ExLevel
                    });
                }
                snapshot.Classes.Add(recordSnapshot);
            }
            foreach (SubClass subClass in plan.SubClasses)
            {
                snapshot.SubClasses.Add(new SubClassRecordSnapshot
                {
                    IdName = subClass.GetIdName(),
                    Id = subClass.Id,
                    Name = subClass.Name,
                    OwnerClassIdName = subClass.Class.GetIdName()
                });
            }
            foreach (KeyValuePair<string, ClassRewardLedger> kv in plan.RewardLedgers)
            {
                snapshot.RewardLedgers[kv.Key] = kv.Value.Copy();
            }
            foreach (KeyValuePair<RoleType, List<Skill>> kv in plan.LearnedCombatTalents)
            {
                snapshot.LearnedTalents[kv.Key.ToString()] = [.. kv.Value.Select(t => t.GetIdName())];
            }
            snapshot.ActiveTalentId = plan.CombatTalent?.GetIdName();
            return snapshot;
        }

        /// <summary>
        /// 把快照写回职业计划（覆盖式重建）；返回重建过程中的错误列表（空表示完全成功）
        /// </summary>
        /// <param name="plan">目标职业计划</param>
        /// <param name="character">所属角色（用于写回三个定位）</param>
        public List<string> ApplyTo(CharacterClass plan, Character character)
        {
            List<string> errors = [];
            plan.UnapplyFromCharacter(character);
            plan.Classes.Clear();
            plan.SubClasses.Clear();
            plan.RewardLedgers.Clear();
            plan.LearnedCombatTalents.Clear();
            plan.CombatTalent = null;
            plan.CombatTalentSwitchSkill = null;
            plan.DefaultClasses.Clear();
            plan.DefaultSubClasses.Clear();
            plan.SkillSelectionEnabled = SkillSelectionEnabled;
            plan.ClassPoints = ClassPoints;

            Dictionary<string, Class> recordMap = [];
            foreach (ClassRecordSnapshot recordSnapshot in Classes)
            {
                Class? definition = ClassDefinitionRegistry.CreateClass(recordSnapshot.IdName);
                if (definition is null)
                {
                    errors.Add($"未注册职业定义：{recordSnapshot.IdName}（{recordSnapshot.Name}）");
                    continue;
                }
                Class record = definition.Copy();
                record.Level = recordSnapshot.Level;
                foreach (Skill skill in AllPoolSkills(record))
                {
                    ClassSkillStateSnapshot? state = recordSnapshot.Skills.FirstOrDefault(s => s.Id == skill.Id);
                    if (state is null)
                    {
                        continue;
                    }
                    skill.ExLevel = state.ExLevel;
                    skill.Level = Math.Max(0, state.Level - state.ExLevel);
                }
                plan.Classes.Add(record);
                recordMap[record.GetIdName()] = record;
            }

            foreach (SubClassRecordSnapshot subSnapshot in SubClasses)
            {
                if (!recordMap.TryGetValue(subSnapshot.OwnerClassIdName, out Class? owner))
                {
                    errors.Add($"流派【{subSnapshot.Name}】的所属职业未重建成功：{subSnapshot.OwnerClassIdName}");
                    continue;
                }
                SubClass? definition = ClassDefinitionRegistry.CreateSubClass(subSnapshot.IdName, owner);
                if (definition is null)
                {
                    errors.Add($"未注册流派定义：{subSnapshot.IdName}（{subSnapshot.Name}）");
                    continue;
                }
                plan.SubClasses.Add(definition);
            }

            foreach (KeyValuePair<string, ClassRewardLedger> kv in RewardLedgers)
            {
                plan.RewardLedgers[kv.Key] = kv.Value.Copy();
            }

            foreach (KeyValuePair<string, List<string>> kv in LearnedTalents)
            {
                if (!Enum.TryParse(kv.Key, out RoleType roleType))
                {
                    errors.Add($"无法识别的定位：{kv.Key}");
                    continue;
                }
                List<Skill> pool = [.. plan.Classes.SelectMany(c => c.CombatTalents.TryGetValue(roleType, out HashSet<Skill>? talentPool) ? talentPool : [])];
                List<Skill> learned = [];
                foreach (string talentId in kv.Value)
                {
                    Skill? talent = pool.FirstOrDefault(s => s.GetIdName() == talentId);
                    if (talent is null)
                    {
                        errors.Add($"已学天赋不在职业天赋池中：{talentId}");
                        continue;
                    }
                    learned.Add(talent);
                }
                if (learned.Count > 0)
                {
                    plan.LearnedCombatTalents[roleType] = learned;
                }
            }

            if (CombatTalentSwitchSkillId != null)
            {
                Skill? switchSkill = ClassDefinitionRegistry.CreateSwitchSkill(CombatTalentSwitchSkillId);
                if (switchSkill is null)
                {
                    errors.Add($"未注册【转换战斗天赋】战技：{CombatTalentSwitchSkillId}");
                }
                else
                {
                    plan.CombatTalentSwitchSkill = switchSkill;
                }
            }

            // 激活天赋：按天赋 IdName 精确恢复
            if (!string.IsNullOrEmpty(ActiveTalentId))
            {
                Skill? activeTalent = plan.AllLearnedTalents.FirstOrDefault(t => t.GetIdName() == ActiveTalentId);
                if (activeTalent is null)
                {
                    errors.Add($"激活天赋不在已学列表中：{ActiveTalentId}");
                }
                else
                {
                    plan.SwitchCombatTalent(activeTalent, out string? switchError);
                    if (switchError != null)
                    {
                        errors.Add(switchError);
                    }
                }
            }

            plan.SubClassOrder = [.. SubClassOrder];
            // 定位不再手写：由「生效战斗天赋 + 已选流派」重新推导（激活天赋时 SwitchCombatTalent 已同步过，此处兜底）
            plan.SyncRoleTypes();

            foreach (string classId in DefaultClassIds)
            {
                if (recordMap.TryGetValue(classId, out Class? defaultRecord))
                {
                    plan.DefaultClasses.Add(defaultRecord);
                    plan.DefaultSubClasses.UnionWith(plan.SubClasses.Where(s => s.Class.GetIdName() == classId));
                }
            }
            return errors;
        }

        private static RoleType ParseRole(string name)
        {
            return Enum.TryParse(name, out RoleType roleType) ? roleType : RoleType.None;
        }

        private static IEnumerable<Skill> AllPoolSkills(Class classRecord)
        {
            return classRecord.PassiveSkills.Concat(classRecord.Skills).Concat(classRecord.Magics).Concat(classRecord.SuperSkills);
        }
    }

    /// <summary>
    /// 职业记录快照
    /// </summary>
    public class ClassRecordSnapshot
    {
        /// <summary>职业 IdName</summary>
        public string IdName { get; set; } = "";

        /// <summary>职业编号</summary>
        public long Id { get; set; }

        /// <summary>职业名称</summary>
        public string Name { get; set; } = "";

        /// <summary>职业等级</summary>
        public int Level { get; set; } = 1;

        /// <summary>技能池各技能状态</summary>
        public List<ClassSkillStateSnapshot> Skills { get; set; } = [];
    }

    /// <summary>
    /// 流派记录快照
    /// </summary>
    public class SubClassRecordSnapshot
    {
        /// <summary>流派 IdName</summary>
        public string IdName { get; set; } = "";

        /// <summary>流派编号</summary>
        public long Id { get; set; }

        /// <summary>流派名称</summary>
        public string Name { get; set; } = "";

        /// <summary>所属职业 IdName</summary>
        public string OwnerClassIdName { get; set; } = "";
    }

    /// <summary>
    /// 职业技能状态快照（等级含突破加成，恢复时按基础等级 + ExLevel 拆开写回）
    /// </summary>
    public class ClassSkillStateSnapshot
    {
        /// <summary>技能编号</summary>
        public long Id { get; set; }

        /// <summary>技能名称</summary>
        public string Name { get; set; } = "";

        /// <summary>技能类型名</summary>
        public string SkillType { get; set; } = "";

        /// <summary>当前等级（含突破加成）</summary>
        public int Level { get; set; }

        /// <summary>突破等级</summary>
        public int ExLevel { get; set; }
    }
}
