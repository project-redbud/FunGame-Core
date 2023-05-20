namespace Milimoe.FunGame.Core.Entity
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public User? User { get; set; } = null;
        public Dictionary<string, Character> Characters { get; set; } = new();
        public Dictionary<string, Item> Items { get; set; } = new();
    }
}
