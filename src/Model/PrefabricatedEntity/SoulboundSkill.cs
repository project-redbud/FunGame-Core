using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;

namespace FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 继承此类以表示灵魂绑定技能
    /// </summary>
    /// <remarks>
    /// 灵魂绑定：一个至少消耗 100 能量、每额外消耗 20 能量效果增强 10% 的爆发技
    /// </remarks>
    /// <param name="character"></param>
    public abstract class SoulboundSkill(Character? character = null) : Skill(SkillType.SuperSkill, character)
    {
        public override bool CostAllEP => true;
        public override double MinCostEP => 100;
    }

    /// <summary>
    /// 灵魂绑定专有特效
    /// </summary>
    public abstract class SoulboundEffect(SoulboundSkill skill) : Effect(skill)
    {
        /// <summary>
        /// 所属的灵魂绑定技能
        /// </summary>
        public SoulboundSkill SoulboundSkill => skill;

        /// <summary>
        /// 增强系数，每额外消耗 20 能量效果增强 10%
        /// </summary>
        public double Improvement
        {
            get
            {
                double improvement = 0;
                if (_improvement != null)
                {
                    improvement = _improvement.Value;
                }
                else if (Skill.Character != null)
                {
                    improvement = (Skill.Character.EP - skill.MinCostEP) / 20.0 * 0.1;
                }
                return improvement;
            }
        }

        private double? _improvement = null;

        public override void BeforeSkillCasted(SkillCastContext ctx)
        {
            _improvement = (ctx.EPCost - skill.MinCostEP) / 20.0 * 0.1;
        }

        public override void AfterSkillCasted(SkillCastContext ctx)
        {
            _improvement = null;
        }
    }
}
