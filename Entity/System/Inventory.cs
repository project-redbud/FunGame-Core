namespace Milimoe.FunGame.Core.Entity
{
    public class Inventory
    {
        public long Id => User.Id;
        public string Name { get; set; } = "";
        public User User { get; }
        public Dictionary<string, Character> Characters { get; } = [];
        public Dictionary<string, Item> Items { get; } = [];

        internal Inventory(User user)
        {
            User = user;
            Name = user.Username + "的库存";
        }
    }
}
