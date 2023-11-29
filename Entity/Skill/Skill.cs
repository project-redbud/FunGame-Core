using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public abstract class Skill : BaseEntity
    {
        public string Describe { get; set; } = "";
        public char Key { get; set; }
        public bool Active { get; set; }
        public bool Enable { get; set; }
        public MagicType MagicType { get; set; }
    }
}
