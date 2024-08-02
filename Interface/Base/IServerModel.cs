using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IServerModel
    {
        /// <summary>
        /// 服务器实例是否在运行
        /// </summary>
        public abstract bool Running { get; }

        /// <summary>
        /// 客户端的套接字实例
        /// </summary>
        public abstract ClientSocket? Socket { get; }

        /// <summary>
        /// 客户端的用户实例，在用户登录后有效
        /// </summary>
        public abstract User User { get; }

        /// <summary>
        /// 客户端的名称，默认是客户端的IP地址
        /// </summary>
        public abstract string ClientName { get; }

        /// <summary>
        /// 客户端是否启动了开发者模式
        /// </summary>
        public bool IsDebugMode { get; }

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="type"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public bool Send(ClientSocket socket, SocketMessageType type, params object[] objs);

        /// <summary>
        /// 向客户端发送系统消息
        /// </summary>
        /// <param name="showtype"></param>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <param name="usernames"></param>
        public void SendSystemMessage(ShowMessageType showtype, string msg, string title, int autoclose, params string[] usernames);

        /// <summary>
        /// 获取客户端的名称，通常未登录时显示为客户端的IP地址，登录后显示为账号名
        /// </summary>
        /// <returns></returns>
        public string GetClientName();

        /// <summary>
        /// 开始接收客户端消息
        /// <para>请勿在 <see cref="Library.Common.Addon.GameModuleServer"/> 中调用此方法</para>
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool Read(ClientSocket socket);

        /// <summary>
        /// 启动对客户端的监听
        /// <para>请勿在 <see cref="Library.Common.Addon.GameModuleServer"/> 中调用此方法</para>
        /// </summary>
        public void Start();
    }
}
