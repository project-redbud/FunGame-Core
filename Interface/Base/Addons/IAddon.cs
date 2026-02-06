namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IAddon
    {
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        public string Author { get; }

        public bool Load(params object[] objs);
        public void UnLoad(params object[] objs);
    }
}
