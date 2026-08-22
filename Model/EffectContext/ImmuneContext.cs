using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Interface.Entity;

namespace FunGame.Core.Model.EffectContext
{
    /// <summary>
    /// 免疫/豁免检定域上下文：技能免疫检定、特效豁免检定、免疫/豁免事件
    /// </summary>
    public class ImmuneContext(IGamingQueue queue, Character character) : HookContext(queue, character)
    {
        /// <summary>
        /// 检定目标（技能免疫检定时使用）
        /// </summary>
        public Character? Target { get; set; } = null;

        /// <summary>
        /// 检定来源角色
        /// </summary>
        public Character? Source { get; set; } = null;

        /// <summary>
        /// 检定关联的技能
        /// </summary>
        public ISkill? Skill { get; set; } = null;

        /// <summary>
        /// 检定关联的物品
        /// </summary>
        public Item? Item { get; set; } = null;

        /// <summary>
        /// 豁免检定的特效
        /// </summary>
        public Effect? Effect { get; set; } = null;

        /// <summary>
        /// 是否是闪避豁免
        /// </summary>
        public bool IsEvade { get; set; } = false;

        /// <summary>
        /// 检定加值（可修改）
        /// </summary>
        public double ThrowingBonus { get; set; } = 0;
    }
}
