using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 护盾域上下文：护盾结算前、护盾化解伤害、护盾破碎
    /// </summary>
    public class ShieldContext(IGamingQueue? queue, Character trigger, Character? attacker = null) : HookContext(queue, trigger)
    {
        /// <summary>
        /// 攻击方
        /// </summary>
        public Character? Attacker { get; set; } = attacker;

        /// <summary>
        /// 伤害类型
        /// </summary>
        public DamageType DamageType { get; set; } = DamageType.Physical;

        /// <summary>
        /// 魔法类型
        /// </summary>
        public MagicType MagicType { get; set; } = MagicType.None;

        /// <summary>
        /// 伤害值
        /// </summary>
        public double Damage { get; set; } = 0;

        /// <summary>
        /// 护盾减伤值（可修改）
        /// </summary>
        public double DamageReduce { get; set; } = 0;

        /// <summary>
        /// 输出消息（可修改）
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 化解伤害/破碎的护盾类型（非绑定特效的护盾时使用）
        /// </summary>
        public ShieldType? ShieldType { get; set; } = null;

        /// <summary>
        /// 破碎的绑定特效（绑定特效的护盾时使用）
        /// </summary>
        public Effect? ShieldEffect { get; set; } = null;

        /// <summary>
        /// 护盾破碎后的溢出伤害
        /// </summary>
        public double OverFlowing { get; set; } = 0;
    }
}
