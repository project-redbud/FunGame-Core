using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class PassiveItem : Item
    {
        public PassiveSkill? Skill { get; set; } = null;

        protected PassiveItem()
        {
            Active = false;
        }

        protected PassiveItem(int id, string name)
        {
            Active = false;
            Id = id;
            Name = name;
        }

        internal static PassiveItem GetInstance()
        {
            return new();
        }

        internal static PassiveItem GetInstance(int id, string name)
        {
            return new(id, name);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is PassiveItem i && i.Name == Name;
        }
    }
}
