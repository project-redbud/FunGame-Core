using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Desktop.Others
{
    public class Config
    {
        /**
         * FunGame Desktop Configs
         */
        public static bool FunGame_isAutoConnect = true; // 是否自动连接服务器
        public static bool FunGame_isAutoLogin = false; // 是否自动登录
        public static bool FunGame_isMatching = false; // 是否在匹配中
        public static bool FunGame_isConnected = false; // 是否连接上服务器
        public static bool FunGame_isRetrying = false; // 是否正在重连
        public static bool FunGame_isAutoRetry = true; // 是否自动重连
        public static bool Match_Mix = false; // 混战模式选项
        public static bool Match_Team = false; // 团队模式选项
        public static bool Match_HasPass = false; // 密码房间选项
        public static string FunGame_Roomid = "-1"; // 房间号
        public static string FunGame_Notice = ""; // 公告
    }
}
