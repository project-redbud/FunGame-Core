using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 特殊效果类，需要继承
    /// </summary>
    public class Effect : BaseEntity
    {
        /// <summary>
        /// 所属的技能
        /// </summary>
        public Skill Skill { get; }

        /// <summary>
        /// 特殊效果类型<para/>
        /// 注意：如果技能特效没有原生施加控制效果，请始终保持此属性为 <see cref="EffectType.None"/>。
        /// </summary>
        public virtual EffectType EffectType { get; set; } = EffectType.None;

        /// <summary>
        /// 持续性的<para/>
        /// 配合 <see cref="Duration"/> 使用，而不是 <see cref="DurationTurn"/>。
        /// </summary>
        public virtual bool Durative { get; set; } = false;

        /// <summary>
        /// 持续时间<para/>
        /// 配合 <see cref="Durative"/> 使用。
        /// </summary>
        public virtual double Duration { get; set; } = 0;

        /// <summary>
        /// 持续时间（回合）<para/>
        /// 使用此属性需要将 <see cref="Durative"/> 设置为 false。
        /// </summary>
        public virtual int DurationTurn { get; set; } = 0;

        /// <summary>
        /// 剩余持续时间
        /// </summary>
        public double RemainDuration { get; set; } = 0;

        /// <summary>
        /// 剩余持续时间（回合）
        /// </summary>
        public int RemainDurationTurn { get; set; } = 0;

        /// <summary>
        /// 是否是没有具体持续时间的持续性特效
        /// </summary>
        public virtual bool DurativeWithoutDuration { get; set; } = false;

        /// <summary>
        /// 附属于某个特效
        /// </summary>
        public Effect? ParentEffect { get; set; } = null;

        /// <summary>
        /// 是否是某个特效的附属
        /// </summary>
        public bool IsSubsidiary => ParentEffect != null;

        /// <summary>
        /// 是否强制在状态栏中隐藏
        /// </summary>
        public virtual bool ForceHideInStatusBar { get; set; } = false;

        /// <summary>
        /// 是否显示在状态栏
        /// </summary>
        public bool ShowInStatusBar => !ForceHideInStatusBar && (Skill.Item is null || (Durative && Duration > 0) || DurationTurn > 0 || DurativeWithoutDuration);

        /// <summary>
        /// 特效是否生效
        /// </summary>
        public bool IsInEffect => Level > 0 && !IsBeingTemporaryDispelled;

        /// <summary>
        /// 魔法类型
        /// </summary>
        public virtual MagicType MagicType { get; set; } = MagicType.None;

        /// <summary>
        /// 驱散性 [ 能驱散什么特效，默认无驱散 ]
        /// </summary>
        public virtual DispelType DispelType { get; set; } = DispelType.None;

        /// <summary>
        /// 被驱散性 [ 能被什么驱散类型驱散，默认弱驱散 ]
        /// </summary>
        public virtual DispelledType DispelledType { get; set; } = DispelledType.Weak;

        /// <summary>
        /// 是否是负面效果
        /// </summary>
        public virtual bool IsDebuff { get; set; } = false;

        /// <summary>
        /// 驱散性和被驱散性的具体说明
        /// </summary>
        public virtual string DispelDescription
        {
            get => GetDispelDescription("\r\n");
            set => _dispelDescription = value;
        }

        /// <summary>
        /// 是否具备弱驱散功能（强驱散包含在内）
        /// </summary>
        public bool CanWeakDispel => DispelType == DispelType.Weak || DispelType == DispelType.DurativeWeak || DispelType == DispelType.TemporaryWeak || CanStrongDispel;

        /// <summary>
        /// 是否具备强驱散功能
        /// </summary>
        public bool CanStrongDispel => DispelType == DispelType.Strong || DispelType == DispelType.DurativeStrong || DispelType == DispelType.TemporaryStrong;

        /// <summary>
        /// 是否是临时驱散 [ 需注意持续性驱散是在持续时间内将特效无效化而不是移除，适用临时驱散机制 ]
        /// </summary>
        public bool IsTemporaryDispel => DispelType == DispelType.DurativeWeak || DispelType == DispelType.TemporaryWeak || DispelType == DispelType.DurativeStrong || DispelType == DispelType.TemporaryStrong;

        /// <summary>
        /// 是否处于临时被驱散状态 [ 如果使用后不手动恢复为 false，那么行动顺序表会在时间流逝时恢复它 ]
        /// <para/>注意看标准实现，需要配合 <see cref="OnEffectLost"/> 和 <see cref="OnEffectGained"/> 使用
        /// </summary>
        public bool IsBeingTemporaryDispelled { get; set; } = false;

        /// <summary>
        /// 无视免疫类型
        /// </summary>
        public virtual ImmuneType IgnoreImmune { get; set; } = ImmuneType.None;

        /// <summary>
        /// 效果描述
        /// </summary>
        public virtual string Description { get; set; } = "";

        /// <summary>
        /// 等级，跟随技能的等级
        /// </summary>
        public int Level => Skill.Level;

        /// <summary>
        /// 此特效的施加者，用于溯源
        /// </summary>
        public virtual Character? Source { get; set; } = null;

        /// <summary>
        /// 游戏中的行动顺序表实例，在技能效果被触发时，此实例会获得赋值，使用时需要判断其是否存在
        /// </summary>
        public IGamingQueue? GamingQueue { get; set; } = null;

        /// <summary>
        /// 用于动态扩展特效的参数
        /// </summary>
        public Dictionary<string, object> Values { get; } = [];

        /// <summary>
        /// 输出文本或日志
        /// </summary>
        public Action<string> WriteLine
        {
            get
            {
                if (GamingQueue is null) return Console.WriteLine;
                else return GamingQueue.WriteLine;
            }
        }

        /// <summary>
        /// Values 构造动态特效参考这个构造函数
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="args"></param>
        protected Effect(Skill skill, Dictionary<string, object>? args = null)
        {
            Skill = skill;
            if (args != null)
            {
                foreach (string key in args.Keys)
                {
                    Values[key] = args[key];
                }
            }
        }

        internal Effect()
        {
            Skill = Factory.GetSkill();
        }

        /// <summary>
        /// 获得此特效时
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnEffectGained(Character character)
        {

        }

        /// <summary>
        /// 失去此特效时
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnEffectLost(Character character)
        {

        }

        /// <summary>
        /// 在伤害计算前修改伤害类型
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        public virtual void AlterDamageTypeBeforeCalculation(Character character, Character enemy, ref bool isNormalAttack, ref DamageType damageType, ref MagicType magicType)
        {

        }

        /// <summary>
        /// 在伤害计算前修改预期伤害
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="totalDamageBonus"></param>
        /// <returns>返回伤害增减值</returns>
        public virtual double AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, Dictionary<Effect, double> totalDamageBonus)
        {
            return 0;
        }

        /// <summary>
        /// 在伤害计算完成后修改实际伤害 [ 允许取消伤害 ]
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        /// <param name="isEvaded"></param>
        /// <param name="totalDamageBonus"></param>
        /// <returns>返回伤害增减值</returns>
        public virtual double AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult, ref bool isEvaded, Dictionary<Effect, double> totalDamageBonus)
        {
            return 0;
        }

        /// <summary>
        /// 在完成普通攻击动作之后修改硬直时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="baseHardnessTime"></param>
        /// <param name="isCheckProtected"></param>
        public virtual void AlterHardnessTimeAfterNormalAttack(Character character, ref double baseHardnessTime, ref bool isCheckProtected)
        {

        }

        /// <summary>
        /// 在完成释放技能动作之后修改硬直时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        /// <param name="baseHardnessTime"></param>
        /// <param name="isCheckProtected"></param>
        public virtual void AlterHardnessTimeAfterCastSkill(Character character, Skill skill, ref double baseHardnessTime, ref bool isCheckProtected)
        {

        }

        /// <summary>
        /// 在造成伤害时，修改获得的能量
        /// </summary>
        /// <param name="character"></param>
        /// <param name="baseEP"></param>
        public virtual void AlterEPAfterDamage(Character character, ref double baseEP)
        {

        }

        /// <summary>
        /// 在受到伤害时，修改获得的能量
        /// </summary>
        /// <param name="character"></param>
        /// <param name="baseEP"></param>
        public virtual void AlterEPAfterGetDamage(Character character, ref double baseEP)
        {

        }

        /// <summary>
        /// 技能开始吟唱时 [ 爆发技插队可触发此项 ]
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="targets"></param>
        public virtual void OnSkillCasting(Character caster, List<Character> targets)
        {

        }

        /// <summary>
        /// 技能吟唱被打断时
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <param name="interrupter"></param>
        public virtual void OnSkillCastInterrupted(Character caster, Skill skill, Character interrupter)
        {

        }

        /// <summary>
        /// 吟唱结束后释放技能（魔法）/ 直接释放技能（战技/爆发技）
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="targets"></param>
        /// <param name="others"></param>
        public virtual void OnSkillCasted(Character caster, List<Character> targets, Dictionary<string, object> others)
        {

        }

        /// <summary>
        /// 对目标触发技能效果（局外）
        /// </summary>
        /// <param name="user"></param>
        /// <param name="targets"></param>
        /// <param name="others"></param>
        public virtual void OnSkillCasted(User user, List<Character> targets, Dictionary<string, object> others)
        {

        }

        /// <summary>
        /// 时间流逝时
        /// </summary>
        /// <param name="character"></param>
        /// <param name="elapsed"></param>
        public virtual void OnTimeElapsed(Character character, double elapsed)
        {

        }

        /// <summary>
        /// 在完成伤害结算后
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="actualDamage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        public virtual void AfterDamageCalculation(Character character, Character enemy, double damage, double actualDamage, bool isNormalAttack, DamageType damageType, MagicType magicType, DamageResult damageResult)
        {

        }

        /// <summary>
        /// 在治疗结算前修改治疗值
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="canRespawn"></param>
        /// <param name="totalHealBonus"></param>
        /// <returns>返回治疗增减值</returns>
        public virtual double AlterHealValueBeforeHealToTarget(Character actor, Character target, double heal, ref bool canRespawn, Dictionary<Effect, double> totalHealBonus)
        {
            return 0;
        }

        /// <summary>
        /// 在特效持有者的回合开始前
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="skills"></param>
        /// <param name="items"></param>
        public virtual void OnTurnStart(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, List<Item> items)
        {

        }

        /// <summary>
        /// 在特效持有者的回合结束后
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnTurnEnd(Character character)
        {

        }

        /// <summary>
        /// 技能被升级时
        /// </summary>
        /// <param name="character"></param>
        /// <param name="level"></param>
        public virtual void OnSkillLevelUp(Character character, double level)
        {

        }

        /// <summary>
        /// 特效持有者升级时
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="level"></param>
        public virtual void OnOwnerLevelUp(Character owner, double level)
        {

        }

        /// <summary>
        /// 在完成死亡结算后 [ 全体广播 ]
        /// </summary>
        /// <param name="death"></param>
        /// <param name="killer"></param>
        /// <param name="continuousKilling"></param>
        /// <param name="earnedMoney"></param>
        public virtual void AfterDeathCalculation(Character death, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney)
        {

        }

        /// <summary>
        /// 闪避检定前触发
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="throwingBonus"></param>
        /// <returns>返回 false 表示不进行闪避检定</returns>
        public virtual bool BeforeEvadeCheck(Character actor, Character enemy, ref double throwingBonus)
        {
            return true;
        }

        /// <summary>
        /// 在触发闪避时
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="dice"></param>
        /// <returns>返回 true 表示无视闪避</returns>
        public virtual bool OnEvadedTriggered(Character actor, Character enemy, double dice)
        {
            return false;
        }

        /// <summary>
        /// 暴击检定前触发
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="throwingBonus"></param>
        /// <returns>返回 false 表示不进行暴击检定</returns>
        public virtual bool BeforeCriticalCheck(Character actor, Character enemy, ref double throwingBonus)
        {
            return true;
        }

        /// <summary>
        /// 在触发暴击时
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="dice"></param>
        public virtual void OnCriticalDamageTriggered(Character actor, Character enemy, double dice)
        {

        }

        /// <summary>
        /// 角色属性发生变化
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnAttributeChanged(Character character)
        {

        }

        /// <summary>
        /// 行动开始前，修改可选择的 <paramref name="enemys"/>, <paramref name="teammates"/>, <paramref name="skills"/> 列表<para/>
        /// 注意 <paramref name="continuousKilling"/> 和 <paramref name="earnedMoney"/> 是副本，修改无效
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="skills"></param>
        /// <param name="continuousKilling"></param>
        /// <param name="earnedMoney"></param>
        public virtual void AlterSelectListBeforeAction(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney)
        {

        }

        /// <summary>
        /// 开始选择目标前，修改可选择的 <paramref name="enemys"/>, <paramref name="teammates"/> 列表<para/>
        /// <see cref="ISkill"/> 有两种，使用时注意判断是 <see cref="Entity.Skill"/> 还是 <see cref="NormalAttack"/>
        /// </summary>
        /// <param name="character"></param>
        /// <param name="skill"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        public virtual void AlterSelectListBeforeSelection(Character character, ISkill skill, List<Character> enemys, List<Character> teammates)
        {

        }

        /// <summary>
        /// 行动开始前，指定角色的行动，而不是使用顺序表自带的逻辑；或者修改对应的操作触发概率
        /// </summary>
        /// <param name="character"></param>
        /// <param name="state"></param>
        /// <param name="canUseItem"></param>
        /// <param name="canCastSkill"></param>
        /// <param name="pUseItem"></param>
        /// <param name="pCastSkill"></param>
        /// <param name="pNormalAttack"></param>
        /// <returns></returns>
        public virtual CharacterActionType AlterActionTypeBeforeAction(Character character, CharacterState state, ref bool canUseItem, ref bool canCastSkill, ref double pUseItem, ref double pCastSkill, ref double pNormalAttack)
        {
            return CharacterActionType.None;
        }

        /// <summary>
        /// 可重写对某个特效的驱散实现，适用于特殊驱散类型
        /// </summary>
        /// <param name="dispeller"></param>
        /// <param name="target"></param>
        /// <param name="effect"></param>
        /// <param name="isEnemy"></param>
        public virtual void OnDispellingEffect(Character dispeller, Character target, Effect effect, bool isEnemy)
        {
            bool isDispel = false;
            // 先看特效整体是不是能被驱散的
            switch (effect.DispelledType)
            {
                case DispelledType.Weak:
                    if (CanWeakDispel)
                    {
                        isDispel = true;
                    }
                    break;
                case DispelledType.Strong:
                    if (CanStrongDispel)
                    {
                        isDispel = true;
                    }
                    break;
                default:
                    break;
            }
            if (isDispel)
            {
                bool removeEffectTypes = false;
                bool removeEffectStates = false;
                // 接下来再看看特效给角色施加的特效类型和改变状态是不是能被驱散的
                // 检查特效持续性
                if (effect.DurativeWithoutDuration || (effect.Durative && effect.Duration > 0) || effect.DurationTurn > 0)
                {
                    // 先从角色身上移除特效类型
                    if (target.CharacterEffectTypes.TryGetValue(effect, out List<EffectType>? types) && types != null)
                    {
                        RemoveEffectTypesByDispel(types, isEnemy);
                        if (types.Count == 0)
                        {
                            target.CharacterEffectTypes.Remove(effect);
                            removeEffectTypes = true;
                        }
                    }
                    else
                    {
                        removeEffectTypes = true;
                    }
                    // 友方移除控制状态
                    if (!isEnemy && effect.IsDebuff)
                    {
                        if (target.CharacterEffectStates.TryGetValue(effect, out List<CharacterState>? states) && states != null)
                        {
                            RemoveEffectStatesByDispel(states);
                            if (states.Count == 0)
                            {
                                target.CharacterEffectStates.Remove(effect);
                                removeEffectStates = true;
                            }
                        }
                        else
                        {
                            removeEffectStates = true;
                        }
                    }
                    target.UpdateCharacterState();
                }
                // 移除整个特效
                if (removeEffectTypes && removeEffectStates)
                {
                    if (IsTemporaryDispel)
                    {
                        effect.IsBeingTemporaryDispelled = true;
                    }
                    else
                    {
                        effect.RemainDuration = 0;
                        effect.RemainDurationTurn = 0;
                        target.Effects.Remove(effect);
                    }
                    effect.OnEffectLost(target);
                }
            }
        }

        /// <summary>
        /// 当特效被驱散时的
        /// </summary>
        /// <param name="dispeller"></param>
        /// <param name="target"></param>
        /// <param name="dispellerEffect"></param>
        /// <param name="isEnemy"></param>
        /// <returns>返回 false 可以阻止驱散</returns>
        public virtual bool OnEffectIsBeingDispelled(Character dispeller, Character target, Effect dispellerEffect, bool isEnemy)
        {
            return true;
        }

        /// <summary>
        /// 当角色触发生命偷取后
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="steal"></param>
        public virtual void AfterLifesteal(Character character, Character enemy, double damage, double steal)
        {

        }

        /// <summary>
        /// 在角色护盾结算前触发
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attacker"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damage"></param>
        /// <param name="damageReduce"></param>
        /// <param name="message"></param>
        /// <returns>返回 false 可以跳过护盾结算</returns>
        public virtual bool BeforeShieldCalculation(Character character, Character attacker, DamageType damageType, MagicType magicType, double damage, ref double damageReduce, ref string message)
        {
            return true;
        }

        /// <summary>
        /// 在角色护盾有效防御时 [ 破碎本身不会触发此钩子，但破碎后化解可触发 ]
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attacker"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damage"></param>
        /// <param name="shieldType"></param>
        public virtual void OnShieldNeutralizeDamage(Character character, Character attacker, DamageType damageType, MagicType magicType, double damage, ShieldType shieldType)
        {

        }

        /// <summary>
        /// 当角色护盾破碎时 [ 非绑定特效，只有同种类型的总护盾值小于等于 0 时触发 ]
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attacker"></param>
        /// <param name="type"></param>
        /// <param name="overFlowing"></param>
        /// <returns>返回 false 可以阻止后续扣除角色生命值</returns>
        public virtual bool OnShieldBroken(Character character, Character attacker, ShieldType type, double overFlowing)
        {
            return true;
        }

        /// <summary>
        /// 当角色护盾破碎时 [ 绑定特效的护盾值小于等于 0 时便会触发 ]
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attacker"></param>
        /// <param name="effect"></param>
        /// <param name="overFlowing"></param>
        /// <returns>返回 false 可以阻止后续扣除角色生命值</returns>
        public virtual bool OnShieldBroken(Character character, Character attacker, Effect effect, double overFlowing)
        {
            return true;
        }

        /// <summary>
        /// 在免疫检定时
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="skill"></param>
        /// <param name="item"></param>
        /// <returns>false：免疫检定不通过</returns>
        public virtual bool OnImmuneCheck(Character actor, Character enemy, ISkill skill, Item? item = null)
        {
            return true;
        }

        /// <summary>
        /// 在伤害免疫检定时
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="damage"></param>
        /// <returns>false：免疫检定不通过</returns>
        public virtual bool OnDamageImmuneCheck(Character actor, Character enemy, bool isNormalAttack, DamageType damageType, MagicType magicType, double damage)
        {
            return true;
        }

        /// <summary>
        /// 对敌人造成技能伤害 [ 强烈建议使用此方法造成伤害而不是自行调用 <see cref="IGamingQueue.DamageToEnemyAsync"/> ]
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="damageType"></param>
        /// <param name="magicType"></param>
        /// <param name="expectedDamage"></param>
        /// <returns></returns>
        public DamageResult DamageToEnemy(Character actor, Character enemy, DamageType damageType, MagicType magicType, double expectedDamage)
        {
            if (GamingQueue is null) return DamageResult.Evaded;
            int changeCount = 0;
            DamageResult result = DamageResult.Normal;
            double damage = expectedDamage;
            if (damageType != DamageType.True)
            {
                result = damageType == DamageType.Physical ? GamingQueue.CalculatePhysicalDamage(actor, enemy, false, expectedDamage, out damage, ref changeCount) : GamingQueue.CalculateMagicalDamage(actor, enemy, false, MagicType, expectedDamage, out damage, ref changeCount);
            }
            GamingQueue.DamageToEnemyAsync(actor, enemy, damage, false, damageType, magicType, result);
            return result;
        }

        /// <summary>
        /// 治疗一个目标 [ 强烈建议使用此方法而不是自行调用 <see cref="IGamingQueue.HealToTargetAsync"/> ]
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <param name="heal"></param>
        /// <param name="canRespawn"></param>
        public void HealToTarget(Character actor, Character target, double heal, bool canRespawn = false)
        {
            GamingQueue?.HealToTargetAsync(actor, target, heal, canRespawn);
        }

        /// <summary>
        /// 打断施法 [ 尽可能的调用此方法而不是直接调用 <see cref="IGamingQueue.InterruptCastingAsync(Character, Character)"/>，以防止中断性变更 ]
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character caster, Character interrupter)
        {
            GamingQueue?.InterruptCastingAsync(caster, interrupter);
        }

        /// <summary>
        /// 打断施法 [ 用于使敌人目标丢失 ] [ 尽可能的调用此方法而不是直接调用 <see cref="IGamingQueue.InterruptCastingAsync(Character)"/>，以防止中断性变更 ]
        /// </summary>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character interrupter)
        {
            GamingQueue?.InterruptCastingAsync(interrupter);
        }

        /// <summary>
        /// 将特效状态设置到角色上 [ 尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="character"></param>
        /// <param name="states"></param>
        public void AddEffectStatesToCharacter(Character character, List<CharacterState> states)
        {
            if (character.CharacterEffectStates.TryGetValue(this, out List<CharacterState>? value) && value != null)
            {
                states.AddRange(value);
            }
            states = [.. states.Distinct()];
            character.CharacterEffectStates[this] = states;
            character.UpdateCharacterState();
        }

        /// <summary>
        /// 将特效控制效果设置到角色上 [ 尽可能的调用此方法而不是自己实现 ]
        /// <para>施加 EffectType 的同时也会施加 CharacterState，参见 <see cref="AddEffectStatesToCharacter"/></para>
        /// </summary>
        /// <param name="character"></param>
        /// <param name="types"></param>
        public void AddEffectTypeToCharacter(Character character, List<EffectType> types)
        {
            if (character.CharacterEffectTypes.TryGetValue(this, out List<EffectType>? value) && value != null)
            {
                types.AddRange(value);
            }
            types = [.. types.Distinct()];
            character.CharacterEffectTypes[this] = types;
            List<CharacterState> states = [];
            foreach (EffectType type in types)
            {
                states.Add(SkillSet.GetCharacterStateByEffectType(type));
            }
            AddEffectStatesToCharacter(character, states);
        }

        /// <summary>
        /// 将免疫状态设置到角色上 [ 尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="character"></param>
        /// <param name="types"></param>
        public void AddImmuneTypesToCharacter(Character character, List<ImmuneType> types)
        {
            if (character.CharacterImmuneTypes.TryGetValue(this, out List<ImmuneType>? value) && value != null)
            {
                types.AddRange(value);
            }
            types = [.. types.Distinct()];
            character.CharacterImmuneTypes[this] = types;
            character.UpdateCharacterState();
        }

        /// <summary>
        /// 将特效状态从角色身上移除 [ 尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="character"></param>
        public void RemoveEffectStatesFromCharacter(Character character)
        {
            character.CharacterEffectStates.Remove(this);
            character.UpdateCharacterState();
        }

        /// <summary>
        /// 将特效控制效果从角色身上移除 [ 尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="character"></param>
        public void RemoveEffectTypesFromCharacter(Character character)
        {
            character.CharacterEffectTypes.Remove(this);
            character.UpdateCharacterState();
        }

        /// <summary>
        /// 将免疫状态从角色身上移除 [ 尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="character"></param>
        public void RemoveImmuneTypesFromCharacter(Character character)
        {
            character.CharacterImmuneTypes.Remove(this);
            character.UpdateCharacterState();
        }

        /// <summary>
        /// 从角色身上消除特效类型 [ 如果重写了 <see cref="OnDispellingEffect"/>，则尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="types"></param>
        /// <param name="isEnemy"></param>
        public void RemoveEffectTypesByDispel(List<EffectType> types, bool isEnemy)
        {
            EffectType[] loop = [.. types];
            foreach (EffectType type in loop)
            {
                bool isDebuff = SkillSet.GetIsDebuffByEffectType(type);
                if (isEnemy == isDebuff)
                {
                    // 简单判断，敌方不考虑 debuff，友方只考虑 debuff
                    continue;
                }
                DispelledType dispelledType = SkillSet.GetDispelledTypeByEffectType(type);
                bool canDispel = false;
                switch (dispelledType)
                {
                    case DispelledType.Weak:
                        if (CanWeakDispel) canDispel = true;
                        break;
                    case DispelledType.Strong:
                        if (CanStrongDispel) canDispel = true;
                        break;
                    default:
                        break;
                }
                if (canDispel)
                {
                    types.Remove(type);
                }
            }
        }

        /// <summary>
        /// 从角色身上消除状态类型 [ 如果重写了 <see cref="OnDispellingEffect"/>，则尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="states"></param>
        public void RemoveEffectStatesByDispel(List<CharacterState> states)
        {
            CharacterState[] loop = [.. states];
            foreach (CharacterState state in loop)
            {
                DispelledType dispelledType = DispelledType.Weak;
                switch (state)
                {
                    case CharacterState.NotActionable:
                    case CharacterState.ActionRestricted:
                    case CharacterState.BattleRestricted:
                        dispelledType = DispelledType.Strong;
                        break;
                    case CharacterState.SkillRestricted:
                    case CharacterState.AttackRestricted:
                        break;
                    default:
                        break;
                }
                bool canDispel = false;
                switch (dispelledType)
                {
                    case DispelledType.Weak:
                        if (CanWeakDispel) canDispel = true;
                        break;
                    case DispelledType.Strong:
                        if (CanStrongDispel) canDispel = true;
                        break;
                    default:
                        break;
                }
                if (canDispel)
                {
                    states.Remove(state);
                }
            }
        }

        /// <summary>
        /// 驱散目标 [ 尽可能的调用此方法而不是自己实现 ]
        /// <para>此方法会触发 <see cref="OnDispellingEffect"/></para>
        /// </summary>
        /// <param name="dispeller"></param>
        /// <param name="target"></param>
        /// <param name="isEnemy"></param>
        public void Dispel(Character dispeller, Character target, bool isEnemy)
        {
            if (DispelType == DispelType.None)
            {
                return;
            }
            Effect[] effects = [.. target.Effects.Where(e => e.IsInEffect && e.ShowInStatusBar)];
            foreach (Effect effect in effects)
            {
                if (effect.OnEffectIsBeingDispelled(dispeller, target, this, isEnemy))
                {
                    OnDispellingEffect(dispeller, target, effect, isEnemy);
                }
            }
        }

        /// <summary>
        /// 修改角色的硬直时间 [ 尽可能的调用此方法而不是自己实现 ]
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="addValue">加值</param>
        /// <param name="isPercentage">是否是百分比</param>
        /// <param name="isCheckProtected">是否使用插队保护机制</param>
        public void ChangeCharacterHardnessTime(Character character, double addValue, bool isPercentage, bool isCheckProtected)
        {
            GamingQueue?.ChangeCharacterHardnessTime(character, addValue, isPercentage, isCheckProtected);
        }

        /// <summary>
        /// 检查角色是否在 AI 控制状态
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsCharacterInAIControlling(Character character)
        {
            return GamingQueue?.IsCharacterInAIControlling(character) ?? false;
        }

        /// <summary>
        /// 返回特效详情
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new();

            string isDurative = "";
            if (Durative)
            {
                isDurative = $"（剩余：{RemainDuration:0.##} {GameplayEquilibriumConstant.InGameTime}）";
            }
            else if (DurationTurn > 0)
            {
                isDurative = $"（剩余：{RemainDurationTurn} 回合）";
            }

            builder.Append($"【{Name} - 等级 {Level}】{Description}{isDurative}");

            string dispels = GetDispelDescription("，");
            if (dispels != "")
            {
                builder.Append($"（{dispels}）");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 复制一个特效
        /// </summary>
        /// <returns></returns>
        public Effect Copy(Skill skill, bool copyByCode = false)
        {
            Dictionary<string, object> args = new()
            {
                { "skill", skill },
                { "values", Values }
            };
            Effect copy = Factory.OpenFactory.GetInstance<Effect>(Id, Name, args);
            if (!copyByCode)
            {
                copy.Id = Id;
                copy.Name = Name;
                copy.Description = Description;
                copy.DispelDescription = DispelDescription;
                copy.EffectType = EffectType;
                copy.DispelType = DispelType;
                copy.DispelledType = DispelledType;
                copy.IsDebuff = IsDebuff;
                copy.IgnoreImmune = IgnoreImmune;
                copy.DurativeWithoutDuration = DurativeWithoutDuration;
                copy.MagicType = MagicType;
                copy.GamingQueue = GamingQueue;
            }
            copy.Durative = Durative;
            copy.Duration = Duration;
            copy.DurationTurn = DurationTurn;
            return copy;
        }

        /// <summary>
        /// 比较两个特效
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IBaseEntity? other)
        {
            return other is Effect c && c.Id + "." + Name == Id + "." + Name;
        }

        /// <summary>
        /// 获取驱散描述
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        private string GetDispelDescription(string separator)
        {
            if (_dispelDescription.Trim() != "")
            {
                return _dispelDescription;
            }
            else if (DispelType != DispelType.None || DispelledType != DispelledType.Weak)
            {
                List<string> dispels = [];
                if (DispelType != DispelType.None)
                {
                    dispels.Add($"驱散性：{SkillSet.GetDispelType(DispelType)}");
                }
                if (DispelledType != DispelledType.Weak)
                {
                    dispels.Add($"被驱散性：{SkillSet.GetDispelledType(DispelledType)}");
                }
                return string.Join(separator, dispels);
            }
            return "";
        }

        /// <summary>
        /// 驱散描述
        /// </summary>
        private string _dispelDescription = "";
    }
}
