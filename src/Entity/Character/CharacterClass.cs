using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Entity
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
        /// 流派选择顺序（IdName），用于次要定位展开时的同等级排序
        /// </summary>
        public List<string> SubClassOrder { get; set; } = [];

        /// <summary>
        /// 同时掌握的战斗天赋数量上限
        /// <para/>已学天赋是战斗内【转换战斗天赋】的策略池，上限只约束「同时掌握」的数量
        /// <para/>遗忘（<see cref="ForgetCombatTalent"/>）后名额立即释放，可再学新天赋完成替换
        /// </summary>
        public const int MaxLearnedTalentCount = 3;

        /// <summary>
        /// 已学习的战斗天赋，按定位分组（同一定位可掌握多个；同时掌握上限见 <see cref="MaxLearnedTalentCount"/>）
        /// <para>天赋绑定于职业：由已选流派反查其所属职业，再从该职业按定位索引的天赋池中选取</para>
        /// <para>学习不等于生效：只有 <see cref="CombatTalent"/> 会被物化挂载到角色身上</para>
        /// </summary>
        public Dictionary<RoleType, List<Skill>> LearnedCombatTalents { get; set; } = [];

        /// <summary>
        /// 当前生效的战斗天赋，始终至多 1 个
        /// </summary>
        public Skill? CombatTalent { get; set; } = null;

        /// <summary>
        /// 【转换战斗天赋】战技实例
        /// <para/>仅当 <see cref="HasCombatTalentSwitch"/> 为 true（已学天赋 ≥ 2）时有效，使用前需判断是否为空
        /// </summary>
        public Skill? CombatTalentSwitchSkill { get; set; } = null;

        /// <summary>
        /// 职业升级路线图奖励账本，key = 职业 IdName，与 <see cref="Classes"/> 中的职业记录一一对应
        /// <para/>由规划器在选职业 / 升级 / 学习技能 / 洗点时维护（见 <see cref="Model.ClassPlanner"/>）
        /// </summary>
        public Dictionary<string, ClassRewardLedger> RewardLedgers { get; set; } = [];

        /// <summary>
        /// 职业技能是否按路线图「已习得」挂载
        /// <para/>默认 true：严格按路线图走，职业池不再全量授予，须消耗选择权习得后才会挂载
        /// <para/>置 false 则职业池全量授予、选择权仅记账
        /// </summary>
        public bool SkillSelectionEnabled { get; set; } = true;

        /// <summary>
        /// 读取或创建某职业记录的奖励账本
        /// </summary>
        public ClassRewardLedger GetOrCreateLedger(Class classRecord)
        {
            string key = classRecord.GetIdName();
            if (!RewardLedgers.TryGetValue(key, out ClassRewardLedger? ledger))
            {
                ledger = new ClassRewardLedger();
                RewardLedgers[key] = ledger;
            }
            return ledger;
        }

        /// <summary>
        /// 1 级时选择的默认职业（洗点恢复用；满 20 级前不可修改，见平衡常数 <see cref="EquilibriumConstant.MinLevelCanModifyDefaultClass"></see>）
        /// </summary>
        public HashSet<Class> DefaultClasses { get; set; } = [];

        /// <summary>
        /// 1 级时选择的默认流派（洗点恢复用）
        /// </summary>
        public HashSet<SubClass> DefaultSubClasses { get; set; } = [];

        /// <summary>
        /// 卸载计划授予角色的全部技能与特效（供洗点 / 重新规划 / 外部装配使用），并撤销核心天赋加成
        /// </summary>
        /// <param name="character">目标角色，null 时作用于 <see cref="Character"/></param>
        public void UnapplyFromCharacter(Character? character = null)
        {
            character ??= Character;
            if (IsCoreTalentLevelBonusApplied)
            {
                SetCoreTalentLevelBonus(false);
            }
            RemovePlannedSkillsFromCharacter(character);
        }

        /// <summary>
        /// 已学战斗天赋总数
        /// </summary>
        public int LearnedTalentCount => LearnedCombatTalents.Values.Sum(list => list.Count);

        /// <summary>
        /// 全部已学战斗天赋（按定位分组顺序平坦展开）
        /// </summary>
        public IEnumerable<Skill> AllLearnedTalents => LearnedCombatTalents.SelectMany(kv => kv.Value);

        /// <summary>
        /// 是否具备【转换战斗天赋】的使用前提：已学天赋 ≥ 2
        /// <para>同一定位掌握多个天赋时同样可用，转换后定位可能不变</para>
        /// </summary>
        public bool HasCombatTalentSwitch => LearnedTalentCount >= 2;

        /// <summary>
        /// 反查某天赋所属的定位（按引用或 IdName 匹配）；未学习返回 <see cref="RoleType.None"/>
        /// </summary>
        public RoleType RoleOf(Skill talent)
        {
            string idName = talent.GetIdName();
            foreach (KeyValuePair<RoleType, List<Skill>> kv in LearnedCombatTalents)
            {
                if (kv.Value.Any(t => ReferenceEquals(t, talent) || t.GetIdName() == idName))
                {
                    return kv.Key;
                }
            }
            return RoleType.None;
        }

        /// <summary>
        /// 是否已学习某天赋（按引用或 IdName 匹配）
        /// </summary>
        public bool HasLearnedTalent(Skill talent) => RoleOf(talent) != RoleType.None;

        /// <summary>
        /// 取该定位下已学天赋中的第一个未激活项；都已激活过则返回该定位的第一个（实现同定位循环切换）
        /// </summary>
        /// <param name="roleType">目标定位</param>
        public Skill? TalentOf(RoleType roleType)
        {
            if (!LearnedCombatTalents.TryGetValue(roleType, out List<Skill>? list) || list.Count == 0)
            {
                return null;
            }
            Skill? inactive = list.FirstOrDefault(t => !ReferenceEquals(t, CombatTalent));
            return inactive ?? list[0];
        }

        /// <summary>
        /// 取已学天赋序列中 <paramref name="current"/> 的下一个（循环），用于未指定目标的自动转换
        /// </summary>
        public Skill? NextTalentAfter(Skill? current)
        {
            List<Skill> all = [.. AllLearnedTalents];
            if (all.Count == 0)
            {
                return null;
            }
            if (current is null)
            {
                return all[0];
            }
            int index = all.FindIndex(t => ReferenceEquals(t, current) || t.GetIdName() == current.GetIdName());
            return all[(index + 1 + all.Count) % all.Count];
        }

        /// <summary>
        /// 战斗内激活 / 转换已学战斗天赋（【转换战斗天赋】战技与上层共用）
        /// <para>始终至多 1 个生效：撤销旧天赋的特效与核心加成 → 挂载新天赋 → 按需配对等级加成。
        /// 非战斗时的自由转换由上层直接调用本方法即可，不经过决策点结算。</para>
        /// </summary>
        /// <param name="roleType">目标定位（必须已学习对应天赋）</param>
        /// <param name="error">失败原因</param>
        /// <returns>是否成功</returns>
        public bool SwitchCombatTalent(RoleType roleType, out string? error)
        {
            Skill? talent = TalentOf(roleType);
            if (talent is null)
            {
                error = $"{GetRoleTypeName(roleType)}定位的战斗天赋尚未学习，无法转换。";
                return false;
            }
            return ActivateTalent(talent, out error);
        }

        /// <summary>
        /// 按天赋实例激活 / 转换（同一定位掌握多个天赋时使用；【转换战斗天赋】的 TargetTalentId 走此路径）
        /// </summary>
        /// <param name="talent">目标天赋（必须已学习）</param>
        /// <param name="error">失败原因</param>
        public bool SwitchCombatTalent(Skill talent, out string? error)
        {
            if (talent is null || !HasLearnedTalent(talent))
            {
                error = $"天赋【{talent?.Name}】尚未学习，无法转换。";
                return false;
            }
            return ActivateTalent(talent, out error);
        }

        /// <summary>
        /// 撤销当前生效的战斗天赋（卸载其特效并配对撤销核心等级加成），不改变已学列表
        /// </summary>
        public void DeactivateCombatTalent()
        {
            if (CombatTalent is null)
            {
                return;
            }
            if (IsCoreTalentLevelBonusApplied)
            {
                SetCoreTalentLevelBonus(false);
            }
            CombatTalent.RemoveSkillFromCharacter(Character);
            CombatTalent = null;
        }

        /// <summary>
        /// 遗忘一个已学战斗天赋：从已学列表移除并释放名额，可再学新天赋完成替换
        /// <para>遗忘的是当前生效天赋时，自动接续到剩余已学天赋的第一个；无剩余则进入未激活状态</para>
        /// <para>已学不足 2 个时【转换战斗天赋】战技会被收回；定位随生效天赋变化重新推导</para>
        /// </summary>
        /// <param name="talent">要遗忘的已学天赋</param>
        /// <param name="error">失败原因</param>
        /// <returns>是否成功</returns>
        public bool ForgetCombatTalent(Skill talent, out string? error)
        {
            error = null;
            if (talent is null)
            {
                error = "天赋不能为空。";
                return false;
            }
            RoleType roleType = RoleOf(talent);
            if (roleType == RoleType.None)
            {
                error = $"天赋【{talent.Name}】尚未学习，无法遗忘。";
                return false;
            }
            List<Skill> learned = LearnedCombatTalents[roleType];
            Skill? target = learned.FirstOrDefault(t => ReferenceEquals(t, talent) || t.GetIdName() == talent.GetIdName());
            if (target is null)
            {
                error = $"天赋【{talent.Name}】尚未学习，无法遗忘。";
                return false;
            }
            bool wasActive = ReferenceEquals(CombatTalent, target);
            if (wasActive)
            {
                DeactivateCombatTalent();
            }
            else
            {
                target.RemoveSkillFromCharacter(Character);
            }
            learned.Remove(target);
            if (learned.Count == 0)
            {
                LearnedCombatTalents.Remove(roleType);
            }
            // 遗忘的正是生效天赋时自动接续，避免「仍有已学天赋却无天赋生效」的空档
            if (wasActive && AllLearnedTalents.FirstOrDefault() is Skill next)
            {
                ActivateTalent(next, out _);
            }
            RefreshCombatTalentSwitchSkill(Character);
            SyncRoleTypes();
            return true;
        }

        /// <summary>
        /// 激活指定天赋：撤销旧天赋的特效与核心加成 → 新天赋设为 1 级并挂载 → 按需配对等级加成 → 同步定位
        /// </summary>
        private bool ActivateTalent(Skill talent, out string? error)
        {
            error = null;
            if (ReferenceEquals(CombatTalent, talent))
            {
                return true; // 目标天赋已处于激活状态
            }
            if (CombatTalent != null)
            {
                if (IsCoreTalentLevelBonusApplied)
                {
                    SetCoreTalentLevelBonus(false);
                }
                CombatTalent.RemoveSkillFromCharacter(Character);
            }
            // 设定：激活的战斗天赋从 1 级开始并挂载（0 级会被 AddSkillToCharacter 静默跳过）
            EnsureSkillLearned(talent);
            CombatTalent = talent;
            CombatTalent.Source = SkillSource.CombatTalent;
            CombatTalent.AddSkillToCharacter(Character);
            if (IsCombatTalentCore)
            {
                SetCoreTalentLevelBonus(true);
            }
            // 主要定位跟随生效天赋：转换天赋后 MOV 等按定位取值的属性随之变化
            SyncRoleTypes();
            return true;
        }

        /// <summary>
        /// 按「生效战斗天赋 + 已选流派」重新推导并写回角色定位（主要 / 次要）
        /// <para>激活或转换天赋、选职业、职业升级、洗点、读档后调用；不再由玩家手动选择</para>
        /// </summary>
        public void SyncRoleTypes()
        {
            ClassPlanRoleResolver.ApplyTo(this, Character);
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
        /// 复制职业规划到新角色
        /// </summary>
        /// <param name="owner"></param>
        /// <returns>属于新角色的副本</returns>
        public CharacterClass Copy(Character owner)
        {
            CharacterClass copy = new(owner)
            {
                ClassPoints = ClassPoints,
                SkillSelectionEnabled = SkillSelectionEnabled,
                SubClassOrder = [.. SubClassOrder]
            };
            foreach (KeyValuePair<string, ClassRewardLedger> kv in RewardLedgers)
            {
                copy.RewardLedgers[kv.Key] = kv.Value.Copy();
            }
            Dictionary<Class, Class> classMap = [];
            foreach (Class c in Classes)
            {
                Class cc = c.Copy();
                classMap[c] = cc;
                copy.Classes.Add(cc);
            }
            foreach (SubClass sc in SubClasses)
            {
                // 流派副本必须绑到所属职业的副本（Level 委托），孤儿流派则自建职业副本
                Class ownerClass = classMap.TryGetValue(sc.Class, out Class? classMapped) ? classMapped : sc.Class.Copy();
                copy.SubClasses.Add(sc.Copy(ownerClass));
            }
            Dictionary<Skill, Skill> talentMap = [];
            foreach (KeyValuePair<RoleType, List<Skill>> kv in LearnedCombatTalents)
            {
                List<Skill> copies = [];
                foreach (Skill talent in kv.Value)
                {
                    Skill talentCopy = Class.CopySkillState(talent);
                    talentMap[talent] = talentCopy;
                    copies.Add(talentCopy);
                }
                copy.LearnedCombatTalents[kv.Key] = copies;
            }
            copy.CombatTalent = CombatTalent != null && talentMap.TryGetValue(CombatTalent, out Skill? talentMapped) ? talentMapped : CombatTalent != null ? Class.CopySkillState(CombatTalent) : null;
            copy.CombatTalentSwitchSkill = CombatTalentSwitchSkill != null ? Class.CopySkillState(CombatTalentSwitchSkill) : null;
            // 默认计划随副本携带（定义引用，洗点恢复时再实例化为记录）
            copy.DefaultClasses = [.. DefaultClasses];
            copy.DefaultSubClasses = [.. DefaultSubClasses];
            return copy;
        }

        /// <summary>
        /// 把职业规划物化到角色身上：卸载计划旧技能 → 按职业等级门槛装载职业技能、流派固有被动与战斗天赋
        /// </summary>
        /// <param name="character">目标角色，null 时作用于 <see cref="Character"/></param>
        public void ApplyTo(Character? character = null)
        {
            if (character != null && Character != character)
            {
                Character = character;
            }
            // 抵消上次物化施加的加成（幂等重建的前提），随后全量重挂
            if (IsCoreTalentLevelBonusApplied)
            {
                SetCoreTalentLevelBonus(false);
            }
            RemovePlannedSkillsFromCharacter(Character);
            AddPlannedSkillsToCharacter(Character);
            if (IsCombatTalentCore)
            {
                SetCoreTalentLevelBonus(true);
            }
        }

        /// <summary>
        /// 当前激活的战斗天赋是否为核心定位（其被动含「自身与职业技能全等级 +1」）
        /// </summary>
        public bool IsCombatTalentCore => CombatTalent != null && RoleOf(CombatTalent) == RoleType.Core;

        /// <summary>
        /// 核心定位天赋的等级加成是否已作用到角色技能实例
        /// </summary>
        public bool IsCoreTalentLevelBonusApplied { get; private set; } = false;

        /// <summary>
        /// 已被核心天赋加成过的技能（撤销时按此名单精确回退，避免「挂载/卸载顺序变化」导致 ExLevel 错位）
        /// </summary>
        private readonly HashSet<Skill> _coreTalentBonusSkills = [];

        /// <summary>
        /// 应用 / 撤销核心定位天赋的等级加成（激活时 +1，失活时 −1）
        /// <para>作用于普通攻击与所有「自身/职业」主动技能；撤销按加成时的技能名单配对回退</para>
        /// </summary>
        /// <param name="activate">true 为激活（+1），false 为撤销（−1）</param>
        public void SetCoreTalentLevelBonus(bool activate)
        {
            if (activate == IsCoreTalentLevelBonusApplied)
            {
                return;
            }
            int delta = activate ? 1 : -1;
            Character.NormalAttack.ExLevel += delta;
            if (activate)
            {
                _coreTalentBonusSkills.Clear();
                foreach (Skill s in Character.Skills)
                {
                    if (!s.IsActive) continue;
                    if (s.Source is SkillSource.Item or SkillSource.MagicCardPack or SkillSource.Reward) continue;
                    s.ExLevel += delta;
                    _coreTalentBonusSkills.Add(s);
                }
            }
            else
            {
                // 只回退当初加成过的技能：此后新挂载的技能不受影响
                foreach (Skill s in _coreTalentBonusSkills)
                {
                    s.ExLevel += delta;
                }
                _coreTalentBonusSkills.Clear();
            }
            IsCoreTalentLevelBonusApplied = activate;
        }

        /// <summary>
        /// 卸载计划授予角色的全部技能与特效（职业池 / 流派固有被动 / 已学与激活天赋）
        /// </summary>
        /// <param name="character"></param>
        private void RemovePlannedSkillsFromCharacter(Character character)
        {
            CombatTalent?.RemoveSkillFromCharacter(character);
            CombatTalentSwitchSkill?.RemoveSkillFromCharacter(character);
            foreach (Skill talent in AllLearnedTalents)
            {
                talent.RemoveSkillFromCharacter(character);
            }
            foreach (SubClass sc in SubClasses)
            {
                foreach (Skill skill in sc.InherentPassives.Values.SelectMany(s => s))
                {
                    skill.RemoveSkillFromCharacter(character);
                }
            }
            foreach (Class c in Classes)
            {
                foreach (Skill skill in c.PassiveSkills)
                {
                    skill.RemoveSkillFromCharacter(character);
                }
                foreach (Skill skill in c.Skills)
                {
                    skill.RemoveSkillFromCharacter(character);
                }
                foreach (Skill skill in c.Magics)
                {
                    skill.RemoveSkillFromCharacter(character);
                }
                foreach (Skill skill in c.SuperSkills)
                {
                    skill.RemoveSkillFromCharacter(character);
                }
            }
        }

        /// <summary>
        /// 按当前计划装载技能到角色（来源统一盖戳；固有被动按职业等级门槛 1 / 6 授予）
        /// </summary>
        /// <param name="character"></param>
        private void AddPlannedSkillsToCharacter(Character character)
        {
            foreach (Class c in Classes)
            {
                foreach (Skill skill in SelectClassSkills(c))
                {
                    skill.Source = SkillSource.Class;
                    skill.AddSkillToCharacter(character);
                }
            }
            foreach (SubClass sc in SubClasses)
            {
                foreach (KeyValuePair<int, HashSet<Skill>> gate in sc.InherentPassives)
                {
                    // 固有被动按职业等级门槛授予（设定：1 级与 6 级各 1 个）
                    if (sc.Class.Level < gate.Key)
                    {
                        continue;
                    }
                    foreach (Skill skill in gate.Value)
                    {
                        skill.Source = SkillSource.SubClass;
                        skill.AddSkillToCharacter(character);
                    }
                }
            }
            // 已学 ≠ 生效：只有当前激活的战斗天赋会被挂载（其余仅保留在计划里备选）
            if (CombatTalent != null)
            {
                EnsureSkillLearned(CombatTalent);
                CombatTalent.Source = SkillSource.CombatTalent;
                CombatTalent.AddSkillToCharacter(character);
            }
            // 【转换战斗天赋】战技：仅在具备次要定位（已学天赋 ≥ 2）时授予
            if (HasCombatTalentSwitch && CombatTalentSwitchSkill != null)
            {
                EnsureSkillLearned(CombatTalentSwitchSkill);
                CombatTalentSwitchSkill.Source = SkillSource.Class;
                CombatTalentSwitchSkill.AddSkillToCharacter(character);
            }
        }

        /// <summary>
        /// 按当前是否具备次要定位，即时授予 / 收回【转换战斗天赋】战技
        /// </summary>
        /// <param name="character">目标角色，null 时作用于 <see cref="Character"/></param>
        public void RefreshCombatTalentSwitchSkill(Character? character = null)
        {
            character ??= Character;
            if (CombatTalentSwitchSkill is null)
            {
                return;
            }
            if (HasCombatTalentSwitch)
            {
                EnsureSkillLearned(CombatTalentSwitchSkill);
                CombatTalentSwitchSkill.Source = SkillSource.Class;
                CombatTalentSwitchSkill.AddSkillToCharacter(character);
            }
            else
            {
                CombatTalentSwitchSkill.RemoveSkillFromCharacter(character);
            }
        }

        /// <summary>
        /// 保证技能至少为 1 级（0 级技能不会被挂载；赋值前剥离 ExLevel，避免把突破加成折进基础等级）
        /// </summary>
        private static void EnsureSkillLearned(Skill skill)
        {
            if (skill.Level <= 0)
            {
                skill.Level = Math.Max(1, skill.Level - skill.ExLevel);
            }
        }

        /// <summary>
        /// 取某职业本次要挂载到角色身上的职业技能
        /// <para/><see cref="SkillSelectionEnabled"/> 为 true 时只挂载账本中已习得的技能，否则保持全量授予
        /// </summary>
        public IEnumerable<Skill> SelectClassSkills(Class classRecord)
        {
            IEnumerable<Skill> pool = [.. classRecord.PassiveSkills, .. classRecord.Skills, .. classRecord.Magics, .. classRecord.SuperSkills];
            if (!SkillSelectionEnabled)
            {
                return pool;
            }
            if (!RewardLedgers.TryGetValue(classRecord.GetIdName(), out ClassRewardLedger? ledger))
            {
                return [];
            }
            return pool.Where(s => ledger.LearnedSkillIds.Contains(s.GetIdName()));
        }

        /// <summary>
        /// 重新构建角色职业，设置定位和技能等
        /// </summary>
        /// <param name="obj"></param>
        public void ReBuildCharacterClass(ClassObject obj)
        {
            // 无论新计划是否带天赋，先清引用，防止旧对象悬垂
            CombatTalent?.RemoveSkillFromCharacter(Character);
            CombatTalent = null;
            foreach (SubClass sc in SubClasses)
            {
                foreach (Skill skill in sc.InherentPassives.Values.SelectMany(s => s))
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
                foreach (Skill skill in sc.InherentPassives.Values.SelectMany(s => s))
                {
                    skill.AddSkillToCharacter(Character);
                }
            }
            if (obj.CurrentCombatTalent != null)
            {
                CombatTalent = obj.CurrentCombatTalent;
                CombatTalent.AddSkillToCharacter(Character);
            }
            // 定位由「生效天赋 + 已选流派」自动推导
            SyncRoleTypes();
        }
    }

    /// <summary>
    /// 决定如何构建角色的职业。这个类没有 JSON 转换器支持
    /// <para>定位不再由此对象指定：由「生效战斗天赋 + 已选流派」自动推导</para>
    /// </summary>
    public class ClassObject(Class[] c, SubClass[] s, Skill? currentCombatTalent = null)
    {
        public Class[] Classes { get; set; } = c;
        public SubClass[] SubClasses { get; set; } = s;
        public Skill? CurrentCombatTalent { get; set; } = currentCombatTalent;
    }
}
