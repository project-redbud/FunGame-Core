namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IItem : IActiveEnable, IRelateCharacter
    {
        public string Describe { get; set; }
        public decimal Price { get; set; }
        public char Key { get; set; }
    }
}
