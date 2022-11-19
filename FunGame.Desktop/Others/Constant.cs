using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.Others
{
    public class MainControllerSet
    {
        public const string SetGreen = ".set green";
        public const string SetGreenAndPing = ".set greenandping";
        public const string SetRed = ".set red";
        public const string SetYellow = ".set yellow";
        public const string WaitConnectAndSetYellow = ".waitconnect .set yellow";
        public const string WaitLoginAndSetYellow = ".waitlogin .set yellow";
        public const string Disconnect = ".disconnect";
        public const string Disconnected = ".disconnected";
        public const string LogOut = ".logout";
        public const string LogIn = ".login";
        public const string SetUser = ".set user";
        public const string Connected = ".connected";
        public const string Connect = ".connect";
        public const string GetServerConnection = ".getserverconnection";
        public const string Close = ".close";
    }

    public class Constant
    {
        /**
         * Game Configs
         */
        public static int FunGameType { get; } = (int)FunGameEnum.FunGame.FunGame_Desktop;

        /**
         * Socket Configs
         */
        public static string SERVER_IPADRESS { get; set; } = ""; // 服务器IP地址
        public static int SERVER_PORT { get; set; } = 0; // 服务器端口号
        public static Encoding DEFAULT_ENCODING { get; set; } = Encoding.UTF8;

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
