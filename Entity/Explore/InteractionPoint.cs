using Milimoe.FunGame.Core.Interface.Entity;

namespace Milimoe.FunGame.Core.Entity
{
    public class InteractionPoint : BaseEntity
    {
        public int CustomValue { get; set; } = 0;
        public string Description { get; set; } = "";

        public override bool Equals(IBaseEntity? other)
        {
            return other is InteractionPoint && other.GetIdName() == GetIdName();
        }
    }
}
