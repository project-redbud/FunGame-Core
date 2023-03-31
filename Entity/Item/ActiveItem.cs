using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class ActiveItem : Item
    {
        public ActiveSkill? Skill { get; set; } = null;

        internal ActiveItem()
        {
            Active = true;
        }

        internal ActiveItem(int id, string name)
        {
            Active = true;
            Id = id;
            Name = name;
        }

        public override bool Equals(IBaseEntity? other)
        {
            if (other != null && other.Guid == this.Guid)
                return true;
            else return false;
        }
    }
}
