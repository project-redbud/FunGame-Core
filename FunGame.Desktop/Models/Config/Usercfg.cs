using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Desktop.Models.Config
{
    public static class Usercfg
    {
        /**
         * 玩家设定内容
         */
        public static bool Match_Mix = false; // 混战模式选项
        public static bool Match_Team = false; // 团队模式选项
        public static bool Match_HasPass = false; // 密码房间选项
        public static bool FunGame_isMatching = false; // 是否在匹配中
        public static bool FunGame_isConnected = false; // 是否连接上服务器
        public static bool FunGame_isRetrying = false; // 是否正在重连
        public static bool FunGame_isAutoRetry = true; // 是否自动重连
        public static string FunGame_roomid = "-1"; // 房间号
        public static string LoginUserName = ""; // 已登录用户名
    }
}
