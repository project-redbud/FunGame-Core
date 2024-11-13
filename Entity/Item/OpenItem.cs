namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 用于动态扩展物品
    /// </summary>
    public class OpenItem : Item
    {
        public override long Id { get; set; }
        public override string Name { get; set; }

        public OpenItem(long id, string name, Dictionary<string, object> args)
        {
            Id = id;
            Name = name;
            foreach (string key in args.Keys)
            {
                Others[key] = args[key];
            }
        }
    }
}
