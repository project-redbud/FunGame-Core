namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 职业相关的核心属性分配：初始核心属性（力量 / 敏捷 / 智力）与初始核心属性成长（Growth）
    /// <para/>同一结构同时服务两处：1 级「职业初始分配」（挂在 <see cref="Entity.Class.InitialAllocation"/>）
    /// 与 4 / 9 级「数值提升」（挂在 <see cref="ClassLevelUpReward.NumericBoost"/>，缺省回落到
    /// <see cref="EquilibriumConstant.DefaultNumericBoostAllocation"/>）。
    /// <para/>不直接写死到角色字段：由 <see cref="IClassAttributeApplier"/> 决定如何落地，便于模组替换加成口径。
    /// </summary>
    /// <param name="str">初始力量</param>
    /// <param name="agi">初始敏捷</param>
    /// <param name="int_">初始智力</param>
    /// <param name="strGrowth">力量成长</param>
    /// <param name="agiGrowth">敏捷成长</param>
    /// <param name="intGrowth">智力成长</param>
    public class ClassAttributeAllocation(double str = 0, double agi = 0, double int_ = 0, double strGrowth = 0, double agiGrowth = 0, double intGrowth = 0)
    {
        /// <summary>
        /// 初始力量
        /// </summary>
        public double STR { get; set; } = str;

        /// <summary>
        /// 初始敏捷
        /// </summary>
        public double AGI { get; set; } = agi;

        /// <summary>
        /// 初始智力
        /// </summary>
        public double INT { get; set; } = int_;

        /// <summary>
        /// 力量成长（+BaseSTR/Lv）
        /// </summary>
        public double STRGrowth { get; set; } = strGrowth;

        /// <summary>
        /// 敏捷成长（+BaseAGI/Lv）
        /// </summary>
        public double AGIGrowth { get; set; } = agiGrowth;

        /// <summary>
        /// 智力成长（+BaseINT/Lv）
        /// </summary>
        public double INTGrowth { get; set; } = intGrowth;

        /// <summary>
        /// 是否为空分配
        /// </summary>
        public bool IsEmpty => STR == 0 && AGI == 0 && INT == 0 && STRGrowth == 0 && AGIGrowth == 0 && INTGrowth == 0;

        /// <summary>
        /// 复制一份
        /// </summary>
        public ClassAttributeAllocation Copy() => new(STR, AGI, INT, STRGrowth, AGIGrowth, INTGrowth);

        /// <summary>
        /// 累加另一份分配
        /// </summary>
        public void Add(ClassAttributeAllocation? other)
        {
            if (other is null)
            {
                return;
            }
            STR += other.STR;
            AGI += other.AGI;
            INT += other.INT;
            STRGrowth += other.STRGrowth;
            AGIGrowth += other.AGIGrowth;
            INTGrowth += other.INTGrowth;
        }

        /// <summary>
        /// 扣减另一份分配（撤销用）
        /// </summary>
        public void Subtract(ClassAttributeAllocation? other)
        {
            if (other is null)
            {
                return;
            }
            STR -= other.STR;
            AGI -= other.AGI;
            INT -= other.INT;
            STRGrowth -= other.STRGrowth;
            AGIGrowth -= other.AGIGrowth;
            INTGrowth -= other.INTGrowth;
        }

        /// <summary>
        /// 按次数缩放（同一档位发放多次时使用）
        /// </summary>
        public ClassAttributeAllocation Scale(int times) => new(STR * times, AGI * times, INT * times, STRGrowth * times, AGIGrowth * times, INTGrowth * times);

        /// <summary>
        /// 人类可读描述
        /// </summary>
        public string Describe()
        {
            List<string> parts = [];
            Append(parts, "力量", STR);
            Append(parts, "敏捷", AGI);
            Append(parts, "智力", INT);
            Append(parts, "力量成长", STRGrowth);
            Append(parts, "敏捷成长", AGIGrowth);
            Append(parts, "智力成长", INTGrowth);
            return parts.Count == 0 ? "无属性分配" : string.Join(" ", parts);
        }

        public override string ToString() => Describe();

        private static void Append(List<string> parts, string name, double value)
        {
            if (value == 0)
            {
                return;
            }
            parts.Add($"{name}{(value >= 0 ? "+" : "-")}{Math.Abs(value):0.##}");
        }
    }
}
