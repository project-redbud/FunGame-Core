using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectResult;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 护盾域上下文：护盾结算前、护盾化解伤害、护盾破碎
    /// </summary>
    public class ShieldContext(IGamingQueue? queue, Character trigger, Character? attacker = null) : HookContext(queue, trigger)
    {
        /// <summary>
        /// 攻击方（框架填充，模组只读）
        /// </summary>
        public Character? Attacker { get; internal set; } = attacker;

        /// <summary>
        /// 伤害类型（框架填充，模组只读）
        /// </summary>
        public DamageType DamageType { get; internal set; } = DamageType.Physical;

        /// <summary>
        /// 魔法类型（框架填充，模组只读）
        /// </summary>
        public MagicType MagicType { get; internal set; } = MagicType.None;

        /// <summary>
        /// 伤害值（框架填充，模组只读）
        /// </summary>
        public double Damage { get; internal set; } = 0;

        /// <summary>
        /// 护盾减伤值（框架维护当前累计；模组禁止写入，修改请通过 <see cref="BeforeShieldCalculationResult.DamageReduce"/> 返回）
        /// </summary>
        public double DamageReduce { get; internal set; } = 0;

        /// <summary>
        /// 输出消息（框架维护；模组禁止写入，修改请通过 <see cref="BeforeShieldCalculationResult.Message"/> 返回）
        /// </summary>
        public string Message { get; internal set; } = "";

        /// <summary>
        /// 化解伤害/破碎的护盾类型（非绑定特效的护盾时使用；框架填充）
        /// </summary>
        public ShieldType? ShieldType { get; internal set; } = null;

        /// <summary>
        /// 破碎的绑定特效（绑定特效的护盾时使用；框架填充）
        /// </summary>
        public Effect? ShieldEffect { get; internal set; } = null;

        /// <summary>
        /// 护盾破碎后的溢出伤害（框架在触发前同步为当前值；模组只读）
        /// </summary>
        public double OverFlowing { get; internal set; } = 0;
    }
}
