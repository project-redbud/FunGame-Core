using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using FunGame.Core.Api.Util;

namespace FunGame.Desktop.Models.Config
{
    public static class Config
    {
        /**
         * Game Configs
         */
        public static string VERSION = "";
        public static string VERSION_TYPE = "";
        public static string VERSION_PATCH = "";
        public static string VERSION_DATE = "";

        public static INIHelper DefaultINIHelper = new();
        public static AssemblyHelper DefaultAssemblyHelper = new();

        /**
         * WebHelper Configs
         */
        public const string WebHelper_SetGreen = "-WebHelper .set green";
        public const string WebHelper_SetGreenAndPing = "-WebHelper .set greenandping";
        public const string WebHelper_SetRed = "-WebHelper .set red";
        public const string WebHelper_Disconnected = "-WebHelper .disconnected";
        public const string WebHelper_GetUser = "-WebHelper .get user";
        public static int WebHelper_HeartBeatFaileds = 0;

        /**
         * Socket Configs
         */
        public static string SERVER_IPADRESS = ""; // 默认IP地址
        public static int SERVER_PORT; // 默认端口
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
        public const string FunGame_Retry = "重新连接";
        public const string FunGame_AutoRetryOn = "开启自动重连";
        public const string FunGame_AutoRetryOff = "关闭自动重连";
        public static readonly object[] PresetItems =
        {
            FunGame_PresetMessage,
            FunGame_SignIn,
            FunGame_ShowCredits,
            FunGame_ShowStock,
            FunGame_ShowStore,
            FunGame_CreateMix,
            FunGame_CreateTeam,
            FunGame_StartGame,
            FunGame_Retry,
            FunGame_AutoRetryOn,
            FunGame_AutoRetryOff
        };
        public const string FunGame_ConnectByDebug = "连接到Debug";
    }
}
