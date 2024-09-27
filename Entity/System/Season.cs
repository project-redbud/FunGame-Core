namespace Milimoe.FunGame.Core.Entity.System
{
    public class Season(long id, string name, string description)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public string Description { get; } = description;
    }
}
