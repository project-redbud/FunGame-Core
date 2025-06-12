using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色的护盾，对不同的魔法类型有不同值
    /// </summary>
    public class Shield
    {
        /// <summary>
        /// 物理护盾
        /// </summary>
        public double Physical { get; set; } = 0;

        /// <summary>
        /// 无属性魔法护盾
        /// </summary>
        public double None { get; set; } = 0;

        /// <summary>
        /// 星痕魔法护盾
        /// </summary>
        public double Starmark { get; set; } = 0;

        /// <summary>
        /// 纯粹结晶护盾
        /// </summary>
        public double PurityNatural { get; set; } = 0;

        /// <summary>
        /// 纯现代结晶护盾
        /// </summary>
        public double PurityContemporary { get; set; } = 0;

        /// <summary>
        /// 光护盾
        /// </summary>
        public double Bright { get; set; } = 0;

        /// <summary>
        /// 影护盾
        /// </summary>
        public double Shadow { get; set; } = 0;

        /// <summary>
        /// 元素护盾
        /// </summary>
        public double Element { get; set; } = 0;

        /// <summary>
        /// 紫宛护盾
        /// </summary>
        public double Aster { get; set; } = 0;

        /// <summary>
        /// 时空护盾
        /// </summary>
        public double SpatioTemporal { get; set; } = 0;

        /// <summary>
        /// 总计物理护盾
        /// </summary>
        public double TotalPhysical => Physical;

        /// <summary>
        /// 总计魔法护盾
        /// </summary>
        public double TotalMagicial => None + Starmark + PurityNatural + PurityContemporary + Bright + Shadow + Element + Aster + SpatioTemporal;

        /// <summary>
        /// 获取或设置护盾值
        /// </summary>
        /// <param name="isMagic"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public double this[bool isMagic = false, MagicType type = MagicType.None]
        {
            get
            {
                if (isMagic)
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
                return Physical;
            }
            set
            {
                if (isMagic)
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
                else
                {
                    Physical = value;
                }
            }
        }

        /// <summary>
        /// 复制一个护盾对象
        /// </summary>
        /// <returns></returns>
        public Shield Copy()
        {
            return new()
            {
                Physical = Physical,
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
