namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 核心属性分配的上下限（按项，可空表示不限）
    /// <para/>用于「1 级初始分配」这类受限分配：职业模板与角色模板各持一份，校验时取交集（更紧的一侧生效）
    /// </summary>
    /// <param name="strMin">初始力量下限</param>
    /// <param name="strMax">初始力量上限</param>
    /// <param name="agiMin">初始敏捷下限</param>
    /// <param name="agiMax">初始敏捷上限</param>
    /// <param name="intMin">初始智力下限</param>
    /// <param name="intMax">初始智力上限</param>
    /// <param name="strGrowthMin">力量成长下限</param>
    /// <param name="strGrowthMax">力量成长上限</param>
    /// <param name="agiGrowthMin">敏捷成长下限</param>
    /// <param name="agiGrowthMax">敏捷成长上限</param>
    /// <param name="intGrowthMin">智力成长下限</param>
    /// <param name="intGrowthMax">智力成长上限</param>
    public class ClassAttributeLimit(
        double? strMin = null, double? strMax = null,
        double? agiMin = null, double? agiMax = null,
        double? intMin = null, double? intMax = null,
        double? strGrowthMin = null, double? strGrowthMax = null,
        double? agiGrowthMin = null, double? agiGrowthMax = null,
        double? intGrowthMin = null, double? intGrowthMax = null)
    {
        /// <summary>初始力量下限</summary>
        public double? STRMin { get; } = strMin;

        /// <summary>初始力量上限</summary>
        public double? STRMax { get; } = strMax;

        /// <summary>初始敏捷下限</summary>
        public double? AGIMin { get; } = agiMin;

        /// <summary>初始敏捷上限</summary>
        public double? AGIMax { get; } = agiMax;

        /// <summary>初始智力下限</summary>
        public double? INTMin { get; } = intMin;

        /// <summary>初始智力上限</summary>
        public double? INTMax { get; } = intMax;

        /// <summary>力量成长下限</summary>
        public double? STRGrowthMin { get; } = strGrowthMin;

        /// <summary>力量成长上限</summary>
        public double? STRGrowthMax { get; } = strGrowthMax;

        /// <summary>敏捷成长下限</summary>
        public double? AGIGrowthMin { get; } = agiGrowthMin;

        /// <summary>敏捷成长上限</summary>
        public double? AGIGrowthMax { get; } = agiGrowthMax;

        /// <summary>智力成长下限</summary>
        public double? INTGrowthMin { get; } = intGrowthMin;

        /// <summary>智力成长上限</summary>
        public double? INTGrowthMax { get; } = intGrowthMax;

        /// <summary>
        /// 未设置任何上下限
        /// </summary>
        public bool IsUnlimited => STRMin is null && STRMax is null && AGIMin is null && AGIMax is null && INTMin is null && INTMax is null
            && STRGrowthMin is null && STRGrowthMax is null && AGIGrowthMin is null && AGIGrowthMax is null && INTGrowthMin is null && INTGrowthMax is null;

        /// <summary>
        /// 不限
        /// </summary>
        public static ClassAttributeLimit Unlimited { get; } = new();

        /// <summary>
        /// 与另一份限值取交集：下限取较大者、上限取较小者（任一为空则取另一侧）
        /// </summary>
        public ClassAttributeLimit Intersect(ClassAttributeLimit? other)
        {
            if (other is null || other.IsUnlimited)
            {
                return this;
            }
            if (IsUnlimited)
            {
                return other;
            }
            return new ClassAttributeLimit(
                TightenMin(STRMin, other.STRMin), TightenMax(STRMax, other.STRMax),
                TightenMin(AGIMin, other.AGIMin), TightenMax(AGIMax, other.AGIMax),
                TightenMin(INTMin, other.INTMin), TightenMax(INTMax, other.INTMax),
                TightenMin(STRGrowthMin, other.STRGrowthMin), TightenMax(STRGrowthMax, other.STRGrowthMax),
                TightenMin(AGIGrowthMin, other.AGIGrowthMin), TightenMax(AGIGrowthMax, other.AGIGrowthMax),
                TightenMin(INTGrowthMin, other.INTGrowthMin), TightenMax(INTGrowthMax, other.INTGrowthMax));
        }

        /// <summary>
        /// 校验一份分配是否在限值内
        /// </summary>
        /// <param name="allocation">待校验的分配</param>
        /// <param name="error">越界原因</param>
        public bool IsSatisfiedBy(ClassAttributeAllocation allocation, out string? error)
        {
            error = null;
            if (!Check("力量", allocation.STR, STRMin, STRMax, out error)
                || !Check("敏捷", allocation.AGI, AGIMin, AGIMax, out error)
                || !Check("智力", allocation.INT, INTMin, INTMax, out error)
                || !Check("力量成长", allocation.STRGrowth, STRGrowthMin, STRGrowthMax, out error)
                || !Check("敏捷成长", allocation.AGIGrowth, AGIGrowthMin, AGIGrowthMax, out error)
                || !Check("智力成长", allocation.INTGrowth, INTGrowthMin, INTGrowthMax, out error))
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
            if (IsUnlimited)
            {
                return "无上下限";
            }
            List<string> parts = [];
            Append(parts, "力量", STRMin, STRMax);
            Append(parts, "敏捷", AGIMin, AGIMax);
            Append(parts, "智力", INTMin, INTMax);
            Append(parts, "力量成长", STRGrowthMin, STRGrowthMax);
            Append(parts, "敏捷成长", AGIGrowthMin, AGIGrowthMax);
            Append(parts, "智力成长", INTGrowthMin, INTGrowthMax);
            return string.Join(" ", parts);
        }

        public override string ToString() => Describe();

        private static double? TightenMin(double? a, double? b) => a is null ? b : b is null ? a : Math.Max(a.Value, b.Value);

        private static double? TightenMax(double? a, double? b) => a is null ? b : b is null ? a : Math.Min(a.Value, b.Value);

        private static bool Check(string name, double value, double? min, double? max, out string? error)
        {
            error = null;
            if (min is not null && value < min.Value)
            {
                error = $"{name}分配 {value:0.##} 低于下限 {min.Value:0.##}。";
                return false;
            }
            if (max is not null && value > max.Value)
            {
                error = $"{name}分配 {value:0.##} 高于上限 {max.Value:0.##}。";
                return false;
            }
            return true;
        }

        private static void Append(List<string> parts, string name, double? min, double? max)
        {
            if (min is null && max is null)
            {
                return;
            }
            string range = min is null ? $"≤{max:0.##}" : max is null ? $"≥{min:0.##}" : $"{min:0.##}~{max:0.##}";
            parts.Add($"{name}{range}");
        }
    }
}
