using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 伤害域上下文：伤害计算前后的修改、伤害应用、免疫/闪避/暴击检定、能量获取修改
    /// </summary>
    public class DamageContext(IGamingQueue? queue, Character actor, Character? enemy = null) : HookContext(queue, actor)
    {
        /// <summary>
        /// 受击方
        /// </summary>
        public Character? Enemy { get; set; } = enemy;

        /// <summary>
        /// 伤害值（期望/最终伤害视钩子时机而定）
        /// </summary>
        public double Damage { get; set; } = 0;

        /// <summary>
        /// 实际造成的伤害
        /// </summary>
        public double ActualDamage { get; set; } = 0;

        /// <summary>
        /// 是否是普通攻击
        /// </summary>
        public bool IsNormalAttack { get; set; } = false;

        /// <summary>
        /// 伤害类型
        /// </summary>
        public DamageType DamageType { get; set; } = DamageType.Physical;

        /// <summary>
        /// 魔法类型
        /// </summary>
        public MagicType MagicType { get; set; } = MagicType.None;

        /// <summary>
        /// 伤害结算结果
        /// </summary>
        public DamageResult DamageResult { get; set; } = DamageResult.Normal;

        /// <summary>
        /// 是否已闪避
        /// </summary>
        public bool IsEvaded { get; set; } = false;

        /// <summary>
        /// 护盾消息
        /// </summary>
        public string ShieldMessage { get; set; } = "";

        /// <summary>
        /// 原始输出消息（可修改）
        /// </summary>
        public string OriginalMessage { get; set; } = "";

        /// <summary>
        /// 各特效的伤害增减贡献记录
        /// </summary>
        public Dictionary<Effect, double> TotalDamageBonus { get; set; } = [];

        /// <summary>
        /// 基础能量获取值（可修改）
        /// </summary>
        public double BaseEP { get; set; } = 0;

        /// <summary>
        /// 检定骰子数值
        /// </summary>
        public double Dice { get; set; } = 0;

        /// <summary>
        /// 检定加值（可修改）
        /// </summary>
        public double ThrowingBonus { get; set; } = 0;
    }
}
