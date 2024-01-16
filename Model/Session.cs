using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class Session
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Server_Address { get; set; } = "";

        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int Server_Port { get; set; } = 0;

        /// <summary>
        /// SocketToken
        /// </summary>
        public Guid SocketToken { get; set; } = Guid.Empty;

        /// <summary>
        /// LoginKey
        /// </summary>
        public Guid LoginKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 已登录的用户
        /// </summary>
        public User LoginUser { get; set; } = General.UnknownUserInstance;

        /// <summary>
        /// 已登录用户名
        /// </summary>
        public string LoginUserName { get; set; } = "";

        /// <summary>
        /// 所处的房间
        /// </summary>
        public Room InRoom { get; set; } = General.HallInstance;
    }
}
