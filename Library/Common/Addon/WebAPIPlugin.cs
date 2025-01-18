using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Interface.Addons;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class WebAPIPlugin : IAddon, IAddonController<IAddon>
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 插件作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 包含了一些常用方法的控制器
        /// </summary>
        public ServerAddonController<IAddon> Controller
        {
            get => _Controller ?? throw new NotImplementedException();
            internal set => _Controller = value;
        }

        /// <summary>
        /// base控制器
        /// </summary>
        BaseAddonController<IAddon> IAddonController<IAddon>.Controller
        {
            get => Controller;
            set => _Controller = (ServerAddonController<IAddon>?)value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private ServerAddonController<IAddon>? _Controller;

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool _isLoaded = false;

        /// <summary>
        /// 加载插件
        /// </summary>
        public bool Load(params object[] objs)
        {
            if (_isLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此插件
            if (BeforeLoad(objs))
            {
                // 插件加载后，不允许再次加载此插件
                _isLoaded = true;
            }
            return _isLoaded;
        }

        /// <summary>
        /// 接收服务器控制台的输入
        /// </summary>
        /// <param name="input"></param>
        public abstract void ProcessInput(string input);

        /// <summary>
        /// 插件完全加载后需要做的事
        /// </summary>
        public virtual void AfterLoad(WebAPIPluginLoader loader, params object[] objs)
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此插件
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad(params object[] objs)
        {
            return true;
        }
    }
}
