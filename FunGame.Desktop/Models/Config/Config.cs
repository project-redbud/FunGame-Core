using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using FunGame.Core.Api.Model.Enum;
using FunGame.Core.Api.Util;

namespace FunGame.Desktop.Models.Config
{
    public static class Config
    {
        /**
         * Game Configs
         */
        public static FunGameEnums.FunGame FunGameType = FunGameEnums.FunGame.FunGame_Desktop;

        public static INIHelper INIHelper = new();
        public static AssemblyHelper AssemblyHelper = new();

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

        /**
         * WebHelper Configs
         */
        public const string WebHelper_SetGreen = "-WebHelper .set green";
        public const string WebHelper_SetGreenAndPing = "-WebHelper .set greenandping";
        public const string WebHelper_SetRed = "-WebHelper .set red";
        public const string WebHelper_SetYellow = "-WebHelper .set yellow";
        public const string WebHelper_WaitConnectAndSetYellow = "-WebHelper .waitconnect .set yellow";
        public const string WebHelper_WaitLoginAndSetYellow = "-WebHelper .waitlogin .set yellow";
        public const string WebHelper_Disconnect = "-WebHelper .disconnect";
        public const string WebHelper_Disconnected = "-WebHelper .disconnected";
        public const string WebHelper_LogOut = "-WebHelper .logout";
        public const string WebHelper_GetUser = "-WebHelper .get user";
        public const string WebHelper_SetUser = "-WebHelper .set user";
        public const string WebHelper_SetNotice = "-WebHelper .set notice";
        public static int WebHelper_HeartBeatFaileds = 0;

        /**
         * Socket Configs
         */
        public static string SERVER_IPADRESS = ""; // 服务器IP地址
        public static int SERVER_PORT = 0; // 服务器端口号
        public static Encoding DEFAULT_ENCODING = Encoding.UTF8;

        /**
         * FunGame Configs
         */
        public const string GameMode_Mix = "混战模式";
        public const string GameMode_Team = "团队模式";
        public const string GameMode_MixHasPass = "带密码的混战模式";
        public const string GameMode_TeamHasPass = "带密码的团队模式";

        public const string FunGame_PresetMessage = "- 快捷消息 -";
        public const string FunGame_SignIn = "签到";
        public const string FunGame_ShowCredits = "积分";
        public const string FunGame_ShowStock = "查看库存";
        public const string FunGame_ShowStore = "游戏商店";
        public const string FunGame_CreateMix = "创建游戏 混战";
        public const string FunGame_CreateTeam = "创建游戏 团队";
        public const string FunGame_StartGame = "开始游戏";
        public const string FunGame_Connect = "连接服务器";
        public const string FunGame_ConnectTo = "连接指定服务器";
        public const string FunGame_Disconnect = "登出并断开连接";
        public const string FunGame_DisconnectWhenNotLogin = "断开连接";
        public const string FunGame_Retry = "重新连接";
        public const string FunGame_AutoRetryOn = "开启自动重连";
        public const string FunGame_AutoRetryOff = "关闭自动重连";
        public static readonly object[] PresetOnineItems =
        {
            FunGame_PresetMessage,
            FunGame_SignIn,
            FunGame_ShowCredits,
            FunGame_ShowStock,
            FunGame_ShowStore,
            FunGame_CreateMix,
            FunGame_CreateTeam,
            FunGame_StartGame,
            FunGame_Disconnect,
            FunGame_AutoRetryOn,
            FunGame_AutoRetryOff
        };
        public static readonly object[] PresetNoLoginItems =
        {
            FunGame_PresetMessage,
            FunGame_DisconnectWhenNotLogin,
            FunGame_AutoRetryOn,
            FunGame_AutoRetryOff
        };
        public static readonly object[] PresetNoConnectItems =
        {
            FunGame_PresetMessage,
            FunGame_Connect,
            FunGame_ConnectTo,
            FunGame_Retry,
            FunGame_AutoRetryOn,
            FunGame_AutoRetryOff
        };
    }
}
