using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 特殊效果类，需要继承
    /// </summary>
    public abstract class Effect(Skill skill) : BaseEntity
    {
        /// <summary>
        /// 所属的技能
        /// </summary>
        public Skill Skill { get; } = skill;

        /// <summary>
        /// 作用于自身
        /// </summary>
        public virtual bool TargetSelf { get; } = false;

        /// <summary>
        /// 作用目标数量
        /// </summary>
        public virtual int TargetCount { get; } = 0;

        /// <summary>
        /// 作用范围
        /// </summary>
        public virtual double TargetRange { get; } = 0;
        
        /// <summary>
        /// 持续性的<para/>
        /// 配合 <see cref="Duration"/> 使用，而不是 <see cref="DurationTurn"/>。
        /// </summary>
        public virtual bool Durative { get; } = false;

        /// <summary>
        /// 持续时间<para/>
        /// 配合 <see cref="Durative"/> 使用。
        /// </summary>
        public virtual double Duration { get; } = 0;
        
        /// <summary>
        /// 持续时间（回合）<para/>
        /// 使用此属性需要将 <see cref="Durative"/> 设置为 false。
        /// </summary>
        public virtual double DurationTurn { get; } = 0;
        
        /// <summary>
        /// 剩余持续时间
        /// </summary>
        public double RemainDuration { get; set; } = 0;

        /// <summary>
        /// 魔法类型
        /// </summary>
        public virtual MagicType MagicType { get; } = MagicType.None;

        /// <summary>
        /// 效果描述
        /// </summary>
        public virtual string Description { get; } = "";

        /// <summary>
        /// 等级，跟随技能的等级
        /// </summary>
        public int Level => Skill.Level;

        /// <summary>
        /// 此特效的施加者，用于溯源
        /// </summary>
        public Character? Source { get; set; } = null;

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
        /// 在伤害计算前修改预期伤害
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <param name="newDamage"></param>
        /// <returns>返回 true 表示修改了伤害</returns>
        public virtual bool AlterExpectedDamageBeforeCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, out double newDamage)
        {
            newDamage = damage;
            return false;
        }

        /// <summary>
        /// 在伤害计算完成后修改实际伤害
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <param name="isCritical"></param>
        /// <param name="newDamage"></param>
        /// <returns>返回 true 表示修改了伤害</returns>
        public virtual bool AlterActualDamageAfterCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, bool isCritical, out double newDamage)
        {
            newDamage = damage;
            return false;
        }

        /// <summary>
        /// 在完成普通攻击动作之后修改硬直时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="baseHardnessTime"></param>
        /// <param name="newHardnessTime"></param>
        /// <returns>返回 true 表示修改了硬直时间</returns>
        public virtual bool AlterHardnessTimeAfterNormalAttack(Character character, double baseHardnessTime, out double newHardnessTime)
        {
            newHardnessTime = baseHardnessTime;
            return false;
        }
        
        /// <summary>
        /// 在完成释放技能动作之后修改硬直时间
        /// </summary>
        /// <param name="character"></param>
        /// <param name="baseHardnessTime"></param>
        /// <param name="newHardnessTime"></param>
        /// <returns>返回 true 表示修改了硬直时间</returns>
        public virtual bool AlterHardnessTimeAfterCastSkill(Character character, double baseHardnessTime, out double newHardnessTime)
        {
            newHardnessTime = baseHardnessTime;
            return false;
        }

        /// <summary>
        /// 技能开始吟唱时 [ 爆发技插队可触发此项 ]
        /// </summary>
        /// <param name="caster"></param>
        public virtual void OnSkillCasting(Character caster)
        {

        }

        /// <summary>
        /// 技能吟唱被打断时
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="interrupter"></param>
        public virtual void OnSkillCastInterrupted(Character caster, Character interrupter)
        {

        }

        /// <summary>
        /// 吟唱结束后释放技能（魔法）/ 直接释放技能（战技/爆发技）
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="caster"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="others"></param>
        public virtual void OnSkillCasted(ActionQueue queue, Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
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
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        public virtual void AfterDamageCalculation(Character character, Character enemy, double damage, bool isMagicDamage, MagicType magicType)
        {

        }

        /// <summary>
        /// 在特效持有者的回合开始前
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnTurnStart(Character character)
        {

        }
        
        /// <summary>
        /// 在特效持有者的回合开始后
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
        public virtual void AfterDeathCalculation(Character death, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, double> earnedMoney)
        {

        }

        /// <summary>
        /// 在触发闪避时
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="evader"></param>
        /// <param name="dice"></param>
        /// <returns>返回 true 表示无视闪避</returns>
        public virtual bool OnEvadedTriggered(Character attacker, Character evader, double dice)
        {
            return false;
        }

        /// <summary>
        /// 在触发暴击时
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dice"></param>
        public virtual void OnCriticalDamageTriggered(Character character, double dice)
        {

        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Effect c && c.Name == Name;
        }
    }
}
