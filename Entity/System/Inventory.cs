namespace Milimoe.FunGame.Core.Entity
{
    public class Inventory
    {
        public long Id => User.Id;
        public string Name { get; set; } = "";
        public User User { get; }
        public Dictionary<string, Character> Characters { get; set; } = new();
        public Dictionary<string, Item> Items { get; set; } = new();

        internal Inventory(User user)
        {
            User = user;
        }
    }
}
