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
        /// 作用于自身
        /// </summary>
        public virtual bool TargetSelf { get; set; } = false;

        /// <summary>
        /// 作用目标数量
        /// </summary>
        public virtual int TargetCount { get; set; } = 0;

        /// <summary>
        /// 作用范围
        /// </summary>
        public virtual double TargetRange { get; set; } = 0;

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
        /// 魔法类型
        /// </summary>
        public virtual MagicType MagicType { get; set; } = MagicType.None;

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

        protected Effect(Skill skill)
        {
            Skill = skill;
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
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        public virtual void AlterDamageTypeBeforeCalculation(Character character, Character enemy, ref bool isNormalAttack, ref bool isMagicDamage, ref MagicType magicType)
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
        public virtual void AlterExpectedDamageBeforeCalculation(Character character, Character enemy, ref double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType)
        {

        }

        /// <summary>
        /// 在伤害计算完成后修改实际伤害 [ 允许取消伤害 ]
        /// </summary>
        /// <param name="character"></param>
        /// <param name="enemy"></param>
        /// <param name="damage"></param>
        /// <param name="isNormalAttack"></param>
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        /// <returns>返回 true 表示取消此伤害，等同于闪避</returns>
        public virtual bool AlterActualDamageAfterCalculation(Character character, Character enemy, ref double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
        {
            return false;
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
        public virtual void OnSkillCasting(Character caster)
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
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <param name="others"></param>
        public virtual void OnSkillCasted(Character caster, List<Character> enemys, List<Character> teammates, Dictionary<string, object> others)
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
        /// <param name="isNormalAttack"></param>
        /// <param name="isMagicDamage"></param>
        /// <param name="magicType"></param>
        /// <param name="damageResult"></param>
        public virtual void AfterDamageCalculation(Character character, Character enemy, double damage, bool isNormalAttack, bool isMagicDamage, MagicType magicType, DamageResult damageResult)
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
        /// <returns>返回 true 表示更改生效</returns>
        public virtual bool AlterEnemyListBeforeAction(Character character, List<Character> enemys, List<Character> teammates, List<Skill> skills, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney)
        {
            return false;
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
        /// 对敌人造成技能伤害 [ 强烈建议使用此方法造成伤害而不是自行调用 <see cref="IGamingQueue.DamageToEnemy"/> ]
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="enemy"></param>
        /// <param name="isMagic"></param>
        /// <param name="magicType"></param>
        /// <param name="expectedDamage"></param>
        /// <returns></returns>
        public DamageResult DamageToEnemy(Character actor, Character enemy, bool isMagic, MagicType magicType, double expectedDamage)
        {
            if (GamingQueue is null) return DamageResult.Evaded;
            DamageResult result = !isMagic ? GamingQueue.CalculatePhysicalDamage(actor, enemy, false, expectedDamage, out double damage) : GamingQueue.CalculateMagicalDamage(actor, enemy, false, MagicType, expectedDamage, out damage);
            GamingQueue.DamageToEnemy(actor, enemy, damage, false, isMagic, magicType, result);
            return result;
        }

        /// <summary>
        /// 打断施法 [ 尽可能的调用此方法而不是直接调用 <see cref="IGamingQueue.InterruptCasting"/>，以防止中断性变更 ]
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="interrupter"></param>
        public void InterruptCasting(Character caster, Character interrupter)
        {
            GamingQueue?.InterruptCasting(caster, interrupter);
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
                isDurative = $"（剩余：{RemainDuration:0.##} 时间）";
            }
            else if (DurationTurn > 0)
            {
                isDurative = "（剩余：" + RemainDurationTurn + " 回合）";
            }
            builder.AppendLine("【" + Name + " - 等级 " + Level + "】" + Description + isDurative);

            return builder.ToString();
        }

        /// <summary>
        /// 复制一个特效
        /// </summary>
        /// <returns></returns>
        public Effect Copy(Skill skill)
        {
            Effect copy = new(skill)
            {
                Id = Id,
                Name = Name,
                Description = Description,
                EffectType = EffectType,
                TargetSelf = TargetSelf,
                TargetCount = TargetCount,
                TargetRange = TargetRange,
                Durative = Durative,
                Duration = Duration,
                DurationTurn = DurationTurn,
                MagicType = MagicType,
                GamingQueue = GamingQueue
            };

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
    }
}
