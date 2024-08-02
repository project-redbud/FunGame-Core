using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Skill : BaseEntity, IActiveEnable
    {
        public string Describe { get; set; } = "";
        public char Key { get; set; }
        public bool Active { get; set; }
        public bool Enable { get; set; }
        public MagicType MagicType { get; set; }

        internal Skill(bool active = false)
        {
            Active = active;
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Skill c && c.Name == Name;
        }
    }
}
