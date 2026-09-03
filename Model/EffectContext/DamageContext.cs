using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectResult;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 伤害域上下文：伤害计算前后的修改、伤害应用、免疫/闪避/暴击检定、能量获取修改
    /// <para/>框架在单次伤害结算内维护该上下文的中间值（Damage/ActualDamage/DamageResult 等随阶段推进更新）；
    /// 模组只能读取当前值，禁止直接写入——写入请通过各钩子的回读对象（<see cref="EffectResult"/> 下同名类型）表达。
    /// </summary>
    public class DamageContext(IGamingQueue? queue, Character actor, Character? enemy = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 受击方（框架填充，模组只读）
        /// </summary>
        public Character? Enemy { get; internal set; } = enemy;

        /// <summary>
        /// 伤害值（期望/最终伤害视钩子时机而定；框架维护，模组只读）
        /// </summary>
        public double Damage { get; internal set; } = 0;

        /// <summary>
        /// 实际造成的伤害（框架维护，模组只读）
        /// </summary>
        public double ActualDamage { get; internal set; } = 0;

        /// <summary>
        /// 是否是普通攻击（框架维护；模组禁止写入，修改请通过 <see cref="AlterDamageTypeResult.IsNormalAttack"/> 返回）
        /// </summary>
        public bool IsNormalAttack { get; internal set; } = false;

        /// <summary>
        /// 伤害类型（框架维护；模组禁止写入，修改请通过 <see cref="AlterDamageTypeResult.DamageType"/> 返回）
        /// </summary>
        public DamageType DamageType { get; internal set; } = DamageType.Physical;

        /// <summary>
        /// 魔法类型（框架维护；模组禁止写入，修改请通过 <see cref="AlterDamageTypeResult.MagicType"/> 返回）
        /// </summary>
        public MagicType MagicType { get; internal set; } = MagicType.None;

        /// <summary>
        /// 伤害结算结果（框架维护，模组只读）
        /// </summary>
        public DamageResult DamageResult { get; internal set; } = DamageResult.Normal;

        /// <summary>
        /// 是否已闪避（框架维护；模组禁止写入，修改请通过 <see cref="AlterActualDamageResult.IsEvaded"/> 返回）
        /// </summary>
        public bool IsEvaded { get; internal set; } = false;

        /// <summary>
        /// 护盾消息（框架填充，模组只读）
        /// </summary>
        public string ShieldMessage { get; internal set; } = "";

        /// <summary>
        /// 原始输出消息（框架维护；模组禁止写入，修改请通过 <see cref="OnApplyDamageResult.OriginalMessage"/> 返回）
        /// </summary>
        public string OriginalMessage { get; internal set; } = "";

        /// <summary>
        /// 各特效的伤害增减贡献记录（框架填充；模组只读）
        /// </summary>
        public Dictionary<Effect, double> TotalDamageBonus { get; internal set; } = [];

        /// <summary>
        /// 基础能量获取值（框架维护；模组禁止写入，修改请通过 <see cref="AlterEPResult.BaseEP"/> 返回）
        /// </summary>
        public double BaseEP { get; internal set; } = 0;

        /// <summary>
        /// 检定骰子数值（框架在检定前写入；模组只读）
        /// </summary>
        public double Dice { get; internal set; } = 0;

        /// <summary>
        /// 检定加值（框架内部暂存；模组禁止写入——检定加值通过钩子回读对象的 <c>*Delta</c> 属性施加）
        /// </summary>
        public double ThrowingBonus { get; internal set; } = 0;
    }
}
