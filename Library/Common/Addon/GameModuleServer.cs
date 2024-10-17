using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Entity;
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
        /// 包含了一些常用方法的控制器
        /// </summary>
        public BaseAddonController<IGameModuleServer> Controller
        {
            get => _Controller ?? throw new NotImplementedException();
            set => _Controller = value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private BaseAddonController<IGameModuleServer>? _Controller;

        /// <summary>
        /// 全局数据库连接器
        /// </summary>
        public SQLHelper? SQLHelper => Singleton.Get<SQLHelper>();

        /// <summary>
        /// 全局邮件发送器
        /// </summary>
        public MailSender? MailSender => Singleton.Get<MailSender>();

        /// <summary>
        /// 启动服务器监听 请在此处实现服务器逻辑
        /// </summary>
        /// <param name="GameModule"></param>
        /// <param name="Room"></param>
        /// <param name="Users"></param>
        /// <param name="RoomMasterServerModel"></param>
        /// <param name="ServerModels"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public abstract bool StartServer(string GameModule, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> ServerModels, params object[] Args);

        /// <summary>
        /// 接收并处理GamingMessage
        /// </summary>
        /// <param name="username">发送此消息的账号</param>
        /// <param name="type">消息类型</param>
        /// <param name="data">消息参数</param>
        /// <returns>底层会将字典中的数据发送给客户端</returns>
        public abstract Task<Dictionary<string, object>> GamingMessageHandler(string username, GamingType type, Dictionary<string, object> data);

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
        /// 给客户端发送局内消息
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        protected virtual async Task SendGamingMessage(IEnumerable<IServerModel> clients, GamingType type, Dictionary<string, object> data)
        {
            // 发送局内消息
            foreach (IServerModel s in clients)
            {
                await s.Send(SocketMessageType.Gaming, type, data);
            }
        }

        /// <summary>
        /// 给客户端发送消息
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="type"></param>
        /// <param name="args"></param>
        protected virtual async Task Send(IEnumerable<IServerModel> clients, SocketMessageType type, params object[] args)
        {
            // 发送消息
            foreach (IServerModel s in clients)
            {
                await s.Send(type, args);
            }
        }
    }
}
