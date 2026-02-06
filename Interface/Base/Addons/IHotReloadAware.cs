namespace Milimoe.FunGame.Core.Interface.Base.Addons
{
    /// <summary>
    /// 实现此接口的插件/模组才能被热更新模式加载
    /// </summary>
    public interface IHotReloadAware
    {
        /// <summary>
        /// 在卸载前调用，自行做一些清理，否则卸载不安全
        /// </summary>
        public void OnBeforeUnload();
    }
}
