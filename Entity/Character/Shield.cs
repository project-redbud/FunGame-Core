using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色的护盾，对不同的魔法类型有不同值
    /// </summary>
    public class Shield
    {
        /// <summary>
        /// 绑定到特效的护盾对象。键为特效，值为对应的护盾对象。
        /// </summary>
        public Dictionary<Effect, ShieldOfEffect> ShieldOfEffects { get; } = [];

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
        /// 混合护盾
        /// </summary>
        public double Mix { get; set; } = 0;
        
        /// <summary>
        /// 总计混合护盾
        /// </summary>
        public double TotalMix => Mix + ShieldOfEffects.Values.Where(soe => soe.ShieldType == ShieldType.Mix && soe.Shield > 0).Sum(soe => soe.Shield);

        /// <summary>
        /// 总计物理护盾
        /// </summary>
        public double TotalPhysical => Physical + ShieldOfEffects.Values.Where(soe => soe.ShieldType == ShieldType.Physical && soe.Shield > 0).Sum(soe => soe.Shield);

        /// <summary>
        /// 总计魔法护盾
        /// </summary>
        public double TotalMagicial => None + Starmark + PurityNatural + PurityContemporary + Bright + Shadow + Element + Aster + SpatioTemporal + ShieldOfEffects.Values.Where(soe => soe.ShieldType == ShieldType.Magical && soe.Shield > 0).Sum(soe => soe.Shield);

        /// <summary>
        /// 获取或设置护盾值
        /// </summary>
        /// <param name="isMagic"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public double this[bool isMagic, MagicType type = MagicType.None]
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
        /// 添加一个绑定到特效的护盾，注意：如果特效已经存在护盾，则会覆盖原有护盾。
        /// </summary>
        /// <param name="soe"></param>
        public void AddShieldOfEffect(ShieldOfEffect soe)
        {
            ShieldOfEffects[soe.Effect] = soe;
        }

        /// <summary>
        /// 移除某个特效的护盾
        /// </summary>
        /// <param name="effect"></param>
        public void RemoveShieldOfEffect(Effect effect)
        {
            ShieldOfEffects.Remove(effect);
        }

        /// <summary>
        /// 计算并更新特效的护盾值，如果护盾值小于等于 0，则移除该特效的护盾。
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public double CalculateShieldOfEffect(Effect effect, double damage)
        {
            if (ShieldOfEffects.TryGetValue(effect, out ShieldOfEffect? soe))
            {
                soe.Calculate(damage);
                if (soe.Shield <= 0)
                {
                    soe.Shield = 0;
                    ShieldOfEffects.Remove(effect);
                }
            }
            return soe?.Shield ?? 0;
        }

        /// <summary>
        /// 复制一个护盾对象。注意：不会复制绑定到特效的护盾对象。
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
                SpatioTemporal = SpatioTemporal,
                Mix = Mix
            };
        }
    }

    /// <summary>
    /// 绑定到特效的护盾对象。这个类没有 JSON 转换器支持。
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="shield"></param>
    /// <param name="shieldType"></param>
    /// <param name="magicType"></param>
    public class ShieldOfEffect(Effect effect, double shield, ShieldType shieldType, MagicType magicType = MagicType.None)
    {
        public Effect Effect { get; } = effect;
        public ShieldType ShieldType { get; set; } = shieldType;
        public MagicType MagicType { get; set; } = magicType;
        public double Shield { get; set; } = shield;

        public double Calculate(double damage)
        {
            Shield -= damage;
            return Shield;
        }
    }
}
