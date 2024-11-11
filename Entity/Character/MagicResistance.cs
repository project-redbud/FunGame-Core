namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色的魔法抗性，对不同的魔法类型有不同抗性
    /// </summary>
    public class MagicResistance()
    {
        public double None { get; set; } = 0;
        public double Starmark { get; set; } = 0;
        public double PurityNatural { get; set; } = 0;
        public double PurityContemporary { get; set; } = 0;
        public double Bright { get; set; } = 0;
        public double Shadow { get; set; } = 0;
        public double Element { get; set; } = 0;
        public double Fleabane { get; set; } = 0;
        public double Particle { get; set; } = 0;

        /// <summary>
        /// 对所有抗性赋值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assignment"></param>
        public void SetAllValue(double value, bool assignment = true)
        {
            if (assignment)
            {
                None = value;
                Particle = value;
                Fleabane = value;
                Element = value;
                Shadow = value;
                Bright = value;
                PurityContemporary = value;
                PurityNatural = value;
                Starmark = value;
            }
            else
            {
                None += value;
                Particle += value;
                Fleabane += value;
                Element += value;
                Shadow += value;
                Bright += value;
                PurityContemporary += value;
                PurityNatural += value;
                Starmark += value;
            }
        }
        
        /// <summary>
        /// 增加所有抗性，传入负数来减少
        /// </summary>
        /// <param name="value"></param>
        public void AddAllValue(double value)
        {
            None += value;
            Particle += value;
            Fleabane += value;
            Element += value;
            Shadow += value;
            Bright += value;
            PurityContemporary += value;
            PurityNatural += value;
            Starmark += value;
        }

        /// <summary>
        /// 复制一个魔法抗性对象
        /// </summary>
        /// <returns></returns>
        public MagicResistance Copy()
        {
            return new()
            {
                None = None,
                Starmark = Starmark,
                PurityNatural = PurityNatural,
                PurityContemporary = PurityContemporary,
                Bright = Bright,
                Shadow = Shadow,
                Element = Element,
                Fleabane = Fleabane,
                Particle = Particle
            };
        }
    }
}
