using Milimoe.FunGame.Core.Controller;

namespace Milimoe.FunGame.Core.Interface.Addons
{
    public interface IAddonController<T> where T : IAddon
    {
        public BaseAddonController<T> Controller { get; set; }
    }
}
