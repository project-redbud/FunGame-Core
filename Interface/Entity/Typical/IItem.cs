namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IItem : IActiveEnable, IRelateCharacter
    {
        public string Description { get; }
        public double Price { get; set; }
    }
}
