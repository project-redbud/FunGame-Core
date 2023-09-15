namespace Milimoe.FunGame.Core.Interface
{
    public interface IPlugin
    {
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        public string Author { get; }

        public void Load();
    }
}
