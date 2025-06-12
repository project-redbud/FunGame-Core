using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色的魔法抗性，对不同的魔法类型有不同抗性
    /// </summary>
    public class MagicResistance
    {
        /// <summary>
        /// 无属性魔法抗性
        /// </summary>
        public double None { get; set; } = 0;

        /// <summary>
        /// 星痕魔法抗性
        /// </summary>
        public double Starmark { get; set; } = 0;

        /// <summary>
        /// 纯粹结晶魔法抗性
        /// </summary>
        public double PurityNatural { get; set; } = 0;

        /// <summary>
        /// 纯现代结晶魔法抗性
        /// </summary>
        public double PurityContemporary { get; set; } = 0;

        /// <summary>
        /// 光魔法抗性
        /// </summary>
        public double Bright { get; set; } = 0;

        /// <summary>
        /// 影魔法抗性
        /// </summary>
        public double Shadow { get; set; } = 0;

        /// <summary>
        /// 元素魔法抗性
        /// </summary>
        public double Element { get; set; } = 0;

        /// <summary>
        /// 紫宛魔法抗性
        /// </summary>
        public double Aster { get; set; } = 0;

        /// <summary>
        /// 时空魔法抗性
        /// </summary>
        public double SpatioTemporal { get; set; } = 0;

        /// <summary>
        /// 平均魔法抗性
        /// </summary>
        public double Avg
        {
            get
            {
                double mdf = Calculation.Round4Digits((None + Starmark + PurityNatural + PurityContemporary + Bright + Shadow + Element + Aster + SpatioTemporal) / 9) * 100;
                if (Calculation.IsApproximatelyZero(mdf)) mdf = 0;
                return mdf;
            }
        }

        /// <summary>
        /// 获取或设置抗性值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public double this[MagicType type]
        {
            get
            {
                return type switch
                {
                    MagicType.Starmark => Starmark,
                    MagicType.PurityNatural => PurityNatural,
                    MagicType.PurityContemporary => PurityContemporary,
                    MagicType.Bright => Bright,
                    MagicType.Shadow => Shadow,
                    MagicType.Element => Element,
                    MagicType.Aster => Aster,
                    MagicType.SpatioTemporal => SpatioTemporal,
                    _ => None
                };
            }
            set
            {
                switch (type)
                {
                    case MagicType.Starmark:
                        Starmark = value;
                        break;
                    case MagicType.PurityNatural:
                        PurityNatural = value;
                        break;
                    case MagicType.PurityContemporary:
                        PurityContemporary = value;
                        break;
                    case MagicType.Bright:
                        Bright = value;
                        break;
                    case MagicType.Shadow:
                        Shadow = value;
                        break;
                    case MagicType.Element:
                        Element = value;
                        break;
                    case MagicType.Aster:
                        Aster = value;
                        break;
                    case MagicType.SpatioTemporal:
                        SpatioTemporal = value;
                        break;
                    default:
                        None = value;
                        break;
                }
            }
        }

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
                SpatioTemporal = value;
                Aster = value;
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
                SpatioTemporal += value;
                Aster += value;
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
            SpatioTemporal += value;
            Aster += value;
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
                Aster = Aster,
                SpatioTemporal = SpatioTemporal
            };
        }
    }
}
