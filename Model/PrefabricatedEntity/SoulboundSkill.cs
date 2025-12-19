using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 继承此类以表示灵魂绑定技能
    /// </summary>
    public class SoulboundSkill : Skill
    {
        public override bool CostAllEP => true;
        public override double MinCostEP => 100;

        /// <summary>
        /// 每额外消耗 20 能量效果增强 10%
        /// </summary>
        public virtual double Improvement => (LastCostEP - MinCostEP) / 20 * 0.1;
    }
}
