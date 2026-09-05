using FunGame.Core.Library.Constant;

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
        /// 已学习的战斗天赋，与已选定位一一对应（至多 3 个）
        /// <para>天赋绑定于职业：由已选流派反查其所属职业，再从该职业按定位索引的天赋池中选取。</para>
        /// </summary>
        public Dictionary<RoleType, Skill> LearnedCombatTalents { get; set; } = [];

        /// <summary>
        /// 当前生效的战斗天赋，始终至多 1 个
        /// </summary>
        public Skill? CombatTalent { get; set; } = null;

        /// <summary>
        /// 1 级时选择的默认职业（洗点恢复用；满 20 级前不可修改，见平衡常数 MinLevelCanModifyDefaultClass）
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
            if (character == null)
            {
                character = Character;
            }
            if (IsCoreTalentLevelBonusApplied)
            {
                SetCoreTalentLevelBonus(false);
            }
            RemovePlannedSkillsFromCharacter(character);
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
        /// 复制职业规划到新主人
        /// <para>用于把已有角色身上的计划搬到新实例（装配/复制路径换主）。职业与流派记录深拷贝
        /// （<see cref="Class.Copy"/> / <see cref="SubClass.Copy(Class)"/>），已学与激活天赋做技能
        /// 实例副本——新老角色不共享职业等级与技能实例。</para>
        /// </summary>
        /// <param name="owner">新计划的所属角色</param>
        /// <returns>换主后的计划副本</returns>
        public CharacterClass Copy(Character owner)
        {
            CharacterClass copy = new(owner)
            {
                ClassPoints = ClassPoints
            };
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
            foreach (KeyValuePair<RoleType, Skill> kv in LearnedCombatTalents)
            {
                Skill sc = Class.CopySkillState(kv.Value);
                talentMap[kv.Value] = sc;
                copy.LearnedCombatTalents[kv.Key] = sc;
            }
            copy.CombatTalent = CombatTalent != null && talentMap.TryGetValue(CombatTalent, out Skill? talentMapped) ? talentMapped : CombatTalent != null ? Class.CopySkillState(CombatTalent) : null;
            // 默认计划随副本携带（定义引用，洗点恢复时再实例化为记录）
            copy.DefaultClasses = [.. DefaultClasses];
            copy.DefaultSubClasses = [.. DefaultSubClasses];
            return copy;
        }

        /// <summary>
        /// 把职业规划物化到角色身上：卸载计划旧技能 → 按职业等级门槛装载职业技能、流派固有被动与战斗天赋
        /// <para>「规划 → 物化 → 返回可用角色」链路的核心装配动作。目标角色应为干净实例（无本计划
        /// 以外的技能/加成）。职业技能来源盖 <see cref="SkillSource.Class"/>、固有被动盖
        /// <see cref="SkillSource.SubClass"/>、天赋盖 <see cref="SkillSource.CombatTalent"/>。</para>
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
        public bool IsCombatTalentCore =>
            CombatTalent != null && LearnedCombatTalents.TryGetValue(RoleType.Core, out Skill? talent) && ReferenceEquals(talent, CombatTalent);

        /// <summary>
        /// 核心定位天赋的等级加成是否已作用到角色技能实例
        /// </summary>
        public bool IsCoreTalentLevelBonusApplied { get; private set; } = false;

        /// <summary>
        /// 应用 / 撤销核心定位天赋的等级加成（激活时 +1，失活时 −1）
        /// <para>作用于普通攻击与所有「自身/职业」主动技能（来源非 装备/魔法卡包/回合奖励 者），
        /// 由 <see cref="ApplyTo"/>、转换天赋等调用方保证加减配对。</para>
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
            foreach (Skill s in Character.Skills)
            {
                if (!s.IsActive) continue;
                if (s.Source is SkillSource.Item or SkillSource.MagicCardPack or SkillSource.Reward) continue;
                s.ExLevel += delta;
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
            foreach (Skill talent in LearnedCombatTalents.Values)
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
                foreach (Skill skill in c.PassiveSkills)
                {
                    skill.Source = SkillSource.Class;
                    skill.AddSkillToCharacter(character);
                }
                foreach (Skill skill in c.Skills)
                {
                    skill.Source = SkillSource.Class;
                    skill.AddSkillToCharacter(character);
                }
                foreach (Skill skill in c.Magics)
                {
                    skill.Source = SkillSource.Class;
                    skill.AddSkillToCharacter(character);
                }
                foreach (Skill skill in c.SuperSkills)
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
            foreach (Skill talent in LearnedCombatTalents.Values)
            {
                talent.Source = SkillSource.CombatTalent;
                talent.AddSkillToCharacter(character);
            }
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
            // 写回定位：obj 是重建后的完整状态，三个定位随重建一并生效
            Character.FirstRoleType = obj.FirstRoleType;
            Character.SecondRoleType = obj.SecondRoleType;
            Character.ThirdRoleType = obj.ThirdRoleType;
        }
    }

    /// <summary>
    /// 决定如何构建角色的职业。这个类没有 JSON 转换器支持
    /// </summary>
    public class ClassObject(Class[] c, SubClass[] s, RoleType firstRoleType = RoleType.None, RoleType secondRoleType = RoleType.None, RoleType thirdRoleType = RoleType.None, Skill? currentCombatTalent = null)
    {
        public Class[] Classes { get; set; } = c;
        public SubClass[] SubClasses { get; set; } = s;
        public RoleType FirstRoleType { get; set; } = firstRoleType;
        public RoleType SecondRoleType { get; set; } = secondRoleType;
        public RoleType ThirdRoleType { get; set; } = thirdRoleType;
        public Skill? CurrentCombatTalent { get; set; } = currentCombatTalent;
    }
}
