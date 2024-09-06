using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class MDF()
    {
        public MagicResistance None { get; } = new(MagicType.None);
        public MagicResistance Starmark { get; } = new(MagicType.Starmark);
        public MagicResistance PurityNatural { get; } = new(MagicType.PurityNatural);
        public MagicResistance PurityContemporary { get; } = new(MagicType.PurityContemporary);
        public MagicResistance Bright { get; } = new(MagicType.Bright);
        public MagicResistance Shadow { get; } = new(MagicType.Shadow);
        public MagicResistance Element { get; } = new(MagicType.Element);
        public MagicResistance Fleabane { get; } = new(MagicType.Fleabane);
        public MagicResistance Particle { get; } = new(MagicType.Particle);
    }

    public class MagicResistance(MagicType type = MagicType.None, double value = 0)
    {
        public MagicType MagicType { get; } = type;
        public double Value { get; set; } = value;
    }
}
