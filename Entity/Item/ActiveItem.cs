using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class ActiveItem : Item
    {
        public ActiveSkill? Skill { get; set; } = null;

        protected ActiveItem()
        {
            Active = true;
        }

        protected ActiveItem(int id, string name)
        {
            Active = true;
            Id = id;
            Name = name;
        }

        internal static ActiveItem GetInstance()
        {
            return new();
        }
        
        internal static ActiveItem GetInstance(int id, string name)
        {
            return new(id, name);
        }

        public override bool Equals(IBaseEntity? other)
        {
            if (other != null && other.Guid == this.Guid)
                return true;
            else return false;
        }
    }
}
