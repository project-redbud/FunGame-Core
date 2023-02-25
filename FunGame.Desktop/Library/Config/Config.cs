namespace Milimoe.FunGame.Desktop.Library
{
    public class Config
    {
        /**
         * FunGame Desktop Configs
         */
        public static bool FunGame_isAutoConnect { get; set; } = true; // 是否自动连接服务器
        public static bool FunGame_isAutoLogin { get; set; } = false; // 是否自动登录
        public static bool FunGame_isMatching { get; set; } = false; // 是否在匹配中
        public static bool FunGame_isConnected { get; set; } = false; // 是否连接上服务器
        public static bool FunGame_isRetrying { get; set; } = false; // 是否正在重连
        public static bool FunGame_isAutoRetry { get; set; } = true; // 是否自动重连
        public static bool Match_Mix { get; set; } = false; // 混战模式选项
        public static bool Match_Team { get; set; } = false; // 团队模式选项
        public static bool Match_HasPass { get; set; } = false; // 密码房间选项
        public static string FunGame_Roomid { get; set; } = "-1"; // 房间号
        public static string FunGame_ServerName { get; set; } = ""; // 服务器名称
        public static string FunGame_Notice { get; set; } = ""; // 公告
        public static string FunGame_AutoLoginUser { get; set; } = ""; // 自动登录的账号
        public static string FunGame_AutoLoginPassword { get; set; } = ""; // 自动登录的密码
        public static string FunGame_AutoLoginKey { get; set; } = ""; // 自动登录的秘钥

        /*** GUID For Socket ***/
        public static Guid Guid_Socket { get; set; } = Guid.Empty;
        public static Guid Guid_LoginKey { get; set; } = Guid.Empty;
    }
}
