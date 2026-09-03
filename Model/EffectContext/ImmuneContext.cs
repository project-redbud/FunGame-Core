using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Interface.Entity;
using FunGame.Core.Model.EffectResult;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 免疫/豁免检定域上下文：技能免疫检定、特效豁免检定、免疫/豁免事件
    /// </summary>
    public class ImmuneContext(IGamingQueue queue, Character character) : HookContext(queue, character)
    {
        /// <summary>
        /// 检定目标（技能免疫检定时使用；框架填充，模组只读）
        /// </summary>
        public Character? Target { get; internal set; } = null;

        /// <summary>
        /// 检定来源角色（框架填充，模组只读）
        /// </summary>
        public Character? Source { get; internal set; } = null;

        /// <summary>
        /// 检定关联的技能（框架填充，模组只读）
        /// </summary>
        public ISkill? Skill { get; internal set; } = null;

        /// <summary>
        /// 检定关联的物品（框架填充，模组只读）
        /// </summary>
        public Item? Item { get; internal set; } = null;

        /// <summary>
        /// 豁免检定的特效（框架填充，模组只读）
        /// </summary>
        public Effect? Effect { get; internal set; } = null;

        /// <summary>
        /// 是否是闪避豁免（框架填充，模组只读）
        /// </summary>
        public bool IsEvade { get; internal set; } = false;

        /// <summary>
        /// 检定加值（框架维护当前值；模组禁止写入，修改请通过 <see cref="OnExemptionCheckResult.ThrowingBonusDelta"/> 返回）
        /// </summary>
        public double ThrowingBonus { get; internal set; } = 0;
    }
}
