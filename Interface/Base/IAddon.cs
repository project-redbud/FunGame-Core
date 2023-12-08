using Milimoe.FunGame.Core.Controller;

namespace Milimoe.FunGame.Core.Interface
{
    public interface IAddon
    {
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        public string Author { get; }

        public bool Load(params object[] objs);
    }
}
