using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class FunGameConfig
    {
        /// <summary>
        /// 是否自动连接服务器
        /// </summary>
        public bool FunGame_isAutoConnect { get; set; } = true;

        /// <summary>
        /// 是否自动登录
        /// </summary>
        public bool FunGame_isAutoLogin { get; set; } = false;

        /// <summary>
        /// 是否在匹配中
        /// </summary>
        public bool FunGame_isMatching { get; set; } = false;

        /// <summary>
        /// 是否连接上服务器
        /// </summary>
        public bool FunGame_isConnected { get; set; } = false;

        /// <summary>
        /// 是否正在重连
        /// </summary>
        public bool FunGame_isRetrying { get; set; } = false;

        /// <summary>
        /// 是否自动重连
        /// </summary>
        public bool FunGame_isAutoRetry { get; set; } = true;

        /// <summary>
        /// 当前游戏模式
        /// </summary>
        public string FunGame_GameMode { get; set; } = GameMode.Mix;

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string FunGame_ServerName { get; set; } = "";

        /// <summary>
        /// 公告
        /// </summary>
        public string FunGame_Notice { get; set; } = "";

        /// <summary>
        /// 自动登录的账号
        /// </summary>
        public string FunGame_AutoLoginUser { get; set; } = "";

        /// <summary>
        /// 自动登录的密码
        /// </summary>
        public string FunGame_AutoLoginPassword { get; set; } = "";

        /// <summary>
        /// 自动登录的秘钥
        /// </summary>
        public string FunGame_AutoLoginKey { get; set; } = "";
    }
}
