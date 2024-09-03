namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IItem : IActiveEnable, IRelateCharacter
    {
        public string Describe { get; set; }
        public double Price { get; set; }
        public char Key { get; set; }
    }
}
