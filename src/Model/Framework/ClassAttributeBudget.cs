namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 核心属性分配的额度：属性点总额 + 成长总额，以及是否受模板上下限约束
    /// <para/>· 1 级初始分配：30 点 + 3.0 成长，受限（校验 <see cref="ClassAttributeLimit"/> 交集）
    /// <para/>· 4 / 9 级数值提升：9 点 + 0.9 成长，不受限（仅校验总额，可任意分配到三项属性与成长）
    /// </summary>
    /// <param name="attributePoints">可分配的初始核心属性点总额</param>
    /// <param name="growthPoints">可分配的成长总额</param>
    /// <param name="limited">是否受模板上下限约束</param>
    public class ClassAttributeBudget(double attributePoints, double growthPoints, bool limited = true)
    {
        /// <summary>
        /// 无参构造（供 JSON 反序列化）
        /// </summary>
        public ClassAttributeBudget() : this(0, 0, false)
        {
        }

        /// <summary>
        /// 可分配的初始核心属性点总额（力量 / 敏捷 / 智力之和的上限）
        /// </summary>
        public double AttributePoints { get; set; } = attributePoints;

        /// <summary>
        /// 可分配的成长总额（三项成长之和的上限）
        /// </summary>
        public double GrowthPoints { get; set; } = growthPoints;

        /// <summary>
        /// 是否受模板上下限约束（true 时校验 <see cref="ClassAttributeLimit"/>）
        /// </summary>
        public bool Limited { get; set; } = limited;

        /// <summary>
        /// 容差：避免浮点累加误差误判越界
        /// </summary>
        private const double Tolerance = 1e-9;

        /// <summary>
        /// 复制一份
        /// </summary>
        public ClassAttributeBudget Copy() => new(AttributePoints, GrowthPoints, Limited);

        /// <summary>
        /// 校验一份分配是否可用
        /// <para/>始终校验：各项非负、属性点合计与成长合计不超过总额；受限时再校验模板上下限
        /// </summary>
        /// <param name="allocation">待校验的分配</param>
        /// <param name="limit">模板上下限交集，受限额度时参与校验；null 表示无额外限值</param>
        /// <param name="error">越界原因</param>
        public bool Check(ClassAttributeAllocation allocation, ClassAttributeLimit? limit, out string? error)
        {
            error = null;
            if (allocation is null)
            {
                error = "分配不能为空。";
                return false;
            }
            if (allocation.HasNegative)
            {
                error = "属性与成长分配不能为负数。";
                return false;
            }
            if (allocation.AttributePoints > AttributePoints + Tolerance)
            {
                error = $"属性点超出额度（已分配 {allocation.AttributePoints:0.##} / 上限 {AttributePoints:0.##}）。";
                return false;
            }
            if (allocation.GrowthPoints > GrowthPoints + Tolerance)
            {
                error = $"成长超出额度（已分配 {allocation.GrowthPoints:0.##} / 上限 {GrowthPoints:0.##}）。";
                return false;
            }
            if (Limited && limit is not null && !limit.IsSatisfiedBy(allocation, out error))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 人类可读描述
        /// </summary>
        public string Describe()
        {
            return $"{AttributePoints:0.##} 点属性 + {GrowthPoints:0.##} 成长{(Limited ? "（受模板上下限约束）" : "（可任意分配）")}";
        }

        public override string ToString() => Describe();
    }
}
