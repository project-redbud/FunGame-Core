using System.Collections.Concurrent;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class GameModuleServer : IGameModuleServer
    {
        /// <summary>
        /// 服务器模组的名称<para/>
        /// 如果服务器模组配合一个相关联的模组使用，那么它们的 <see cref="GameModule.Name"/> 名称必须相同。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 模组描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 模组版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 模组作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 默认地图
        /// </summary>
        public abstract string DefaultMap { get; }

        /// <summary>
        /// 模组的依赖集合
        /// </summary>
        public abstract GameModuleDepend GameModuleDepend { get; }

        /// <summary>
        /// 是否是匿名服务器
        /// </summary>
        public virtual bool IsAnonymous { get; set; } = false;

        /// <summary>
        /// 包含了一些常用方法的控制器
        /// </summary>
        public ServerAddonController<IGameModuleServer> Controller
        {
            get => _Controller ?? throw new NotImplementedException();
            internal set => _Controller = value;
        }

        /// <summary>
        /// base控制器
        /// </summary>
        BaseAddonController<IGameModuleServer> IAddonController<IGameModuleServer>.Controller
        {
            get => Controller;
            set => _Controller = (ServerAddonController<IGameModuleServer>?)value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private ServerAddonController<IGameModuleServer>? _Controller;

        /// <summary>
        /// 此模组所有正在运行的游戏对象
        /// </summary>
        public ConcurrentDictionary<string, GamingObject> GamingObjects { get; } = [];

        /// <summary>
        /// 启动服务器监听 请在此处实现服务器逻辑。注意，此方法必须立即返回
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract bool StartServer(GamingObject obj, params object[] args);

        /// <summary>
        /// 接收并处理GamingMessage
        /// </summary>
        /// <param name="model">发送此消息的客户端</param>
        /// <param name="type">消息类型</param>
        /// <param name="data">消息参数</param>
        /// <returns>底层会将字典中的数据发送给客户端</returns>
        public abstract Task<Dictionary<string, object>> GamingMessageHandler(IServerModel model, GamingType type, Dictionary<string, object> data);

        /// <summary>
        /// 启动匿名服务器监听
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool StartAnonymousServer(IServerModel model, Dictionary<string, object> data)
        {
            return true;
        }
        
        /// <summary>
        /// 结束匿名服务器监听
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void CloseAnonymousServer(IServerModel model)
        {

        }

        /// <summary>
        /// 接收并处理匿名服务器监听消息<para/>
        /// 此方法为可选实现，可以帮助 RESTful API 处理不需要验证的 WebSocket 请求
        /// </summary>
        /// <param name="model">发送此消息的客户端</param>
        /// <param name="data">消息参数</param>
        /// <returns>底层会将字典中的数据发送给客户端</returns>
        public virtual async Task<Dictionary<string, object>> AnonymousGameServerHandler(IServerModel model, Dictionary<string, object> data)
        {
            await Task.Delay(1);
            return [];
        }

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool IsLoaded = false;

        /// <summary>
        /// 加载模组
        /// </summary>
        public bool Load(params object[] objs)
        {
            if (IsLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此模组
            if (BeforeLoad())
            {
                // 模组加载后，不允许再次加载此模组
                IsLoaded = true;
            }
            return IsLoaded;
        }

        /// <summary>
        /// 模组完全加载后需要做的事
        /// </summary>
        public virtual void AfterLoad(params object[] args)
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此模组
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad()
        {
            return true;
        }

        /// <summary>
        /// 给所有客户端发送游戏结束通知
        /// </summary>
        /// <param name="obj"></param>
        public virtual async void SendEndGame(GamingObject obj)
        {
            GamingObjects.TryRemove(obj.Room.Roomid, out _);
            await Send(obj.All.Values, SocketMessageType.EndGame, obj.Room, obj.Users);
            foreach (IServerModel model in obj.All.Values)
            {
                model.NowGamingServer = null;
            }
        }

        /// <summary>
        /// 给客户端发送局内消息
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns>发送失败的客户端</returns>
        protected virtual async Task<List<IServerModel>> SendGamingMessage(IEnumerable<IServerModel> clients, GamingType type, Dictionary<string, object> data)
        {
            // 发送局内消息
            List<IServerModel> failedModels = [];
            foreach (IServerModel s in clients)
            {
                try
                {
                    await s.Send(SocketMessageType.Gaming, type, data);
                }
                catch (System.Exception e)
                {
                    Controller.Error(e);
                    failedModels.Add(s);
                }
            }
            return failedModels;
        }

        /// <summary>
        /// 给客户端发送消息
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns>发送失败的客户端</returns>
        protected virtual async Task<List<IServerModel>> Send(IEnumerable<IServerModel> clients, SocketMessageType type, params object[] args)
        {
            // 发送消息
            List<IServerModel> failedModels = [];
            foreach (IServerModel s in clients)
            {
                try
                {
                    await s.Send(type, args);
                }
                catch (System.Exception e)
                {
                    Controller.Error(e);
                    failedModels.Add(s);
                }
            }
            return failedModels;
        }

        /// <summary>
        /// 给客户端发送匿名服务器消息
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="data"></param>
        /// <returns>发送失败的客户端</returns>
        protected virtual async Task<List<IServerModel>> SendAnonymousGameServerMessage(IEnumerable<IServerModel> clients, Dictionary<string, object> data)
        {
            List<IServerModel> failedModels = [];
            foreach (IServerModel s in clients)
            {
                try
                {
                    await s.Send(SocketMessageType.AnonymousGameServer, data);
                }
                catch (System.Exception e)
                {
                    Controller.Error(e);
                    failedModels.Add(s);
                }
            }
            return failedModels;
        }
    }
}
