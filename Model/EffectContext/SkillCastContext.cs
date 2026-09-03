using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 技能施放域上下文：吟唱开始/打断、技能释放前/后、物品技能、施法相关事件
    /// </summary>
    public class SkillCastContext(IGamingQueue? queue, Character? caster = null) : HookContext(queue, caster)
    {
        /// <summary>
        /// 局外触发者（局外对目标触发技能效果时使用，此时 <see cref="HookContext.Trigger"/> 为 null）
        /// </summary>
        public User? User { get; init; } = null;

        /// <summary>
        /// 正在施放的技能
        /// </summary>
        public Skill? Skill { get; init; } = null;

        /// <summary>
        /// 技能来源的物品（物品技能事件时使用）
        /// </summary>
        public Item? Item { get; init; } = null;

        /// <summary>
        /// 施法者当前的决策点
        /// </summary>
        public DecisionPoints? DP { get; init; } = null;

        /// <summary>
        /// 技能目标包（吟唱/释放事件时使用）
        /// </summary>
        public SkillTarget SkillTarget { get; init; }

        /// <summary>
        /// 指向性目标列表
        /// </summary>
        public List<Character> Targets { get; init; } = [];

        /// <summary>
        /// 非指向性目标格子
        /// </summary>
        public List<Grid> Grids { get; init; } = [];

        /// <summary>
        /// 随技能传递的动态参数
        /// </summary>
        public Dictionary<string, object> Others { get; init; } = [];

        /// <summary>
        /// 消耗的 MP
        /// </summary>
        public double MPCost { get; init; } = 0;

        /// <summary>
        /// 消耗的 EP
        /// </summary>
        public double EPCost { get; init; } = 0;

        /// <summary>
        /// 消耗值（释放技能事件时为 EP 消耗）
        /// </summary>
        public double Cost { get; init; } = 0;

        /// <summary>
        /// 打断施法者
        /// </summary>
        public Character? Interrupter { get; init; } = null;
    }
}
