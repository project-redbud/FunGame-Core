using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IServerModel
    {
        /// <summary>
        /// 服务器实例是否在运行
        /// </summary>
        public bool Running { get; }

        /// <summary>
        /// 客户端的套接字实例
        /// </summary>
        public ISocketMessageProcessor? Socket { get; }

        /// <summary>
        /// 客户端的数据库连接实例
        /// </summary>
        public SQLHelper? SQLHelper { get; }

        /// <summary>
        /// 客户端的邮件服务实例
        /// </summary>
        public MailSender? MailSender { get; }

        /// <summary>
        /// 客户端的用户实例，在用户登录后有效
        /// </summary>
        public User User { get; }

        /// <summary>
        /// 客户端的名称，默认是客户端的IP地址
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// 客户端是否启动了开发者模式
        /// </summary>
        public bool IsDebugMode { get; }

        /// <summary>
        /// 客户端所在的房间
        /// </summary>
        public Room InRoom { get; set; }

        /// <summary>
        /// 客户端的游戏模组服务器
        /// </summary>
        public GameModuleServer? NowGamingServer { get; set; }

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="type"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public Task<bool> Send(ISocketMessageProcessor socket, SocketMessageType type, params object[] objs);

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
    }
}
