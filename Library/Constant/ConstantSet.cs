/**
 * 此文件用于保存字符串常量（String Set）
 */
namespace Milimoe.FunGame.Core.Library.Constant
{
    public class InterfaceSet
    {
        public class Type
        {
            public const string IClient = "IClientImpl";
            public const string IServer = "IServerImpl";
        }

        public class Method
        {
            public const string RemoteServerIP = "RemoteServerIP";
            public const string DBConnection = "DBConnection";
            public const string GetServerSettings = "GetServerSettings";
        }
    }

    /// <summary>
    /// 需要同步更新
    /// Milimoe.FunGame.Core.Library.Constant.SocketMessageType, 
    /// Milimoe.FunGame.Core.Service.SocketManager.GetTypeString(SocketMessageType type)
    /// </summary>
    public class SocketSet
    {
        public const int MaxRetryTimes = 20;
        public const int MaxConnection_1C2G = 10;
        public const int MaxConnection_2C2G = 20;
        public const int MaxConnection_4C4G = 40;

        public const string Socket = "Socket";
        public const string Unknown = "Unknown";
        public const string DataRequest = "DataRequest";
        public const string Disconnect = "Disconnect";
        public const string HeartBeat = "HeartBeat";
        public const string ForceLogout = "ForceLogout";
    }

    /// <summary>
    /// 需要同步更新
    /// Milimoe.FunGame.Core.Library.Constant.DataRequestType,
    /// Milimoe.FunGame.Core.Api.Transmittal.DataRequest.GetTypeString(DataRequestType type)
    /// </summary>
    public class DataRequestSet
    {
        public const string UnKnown = "UnKnown";
        /**
         * RunTime
         */
        public const string RunTime_Connect = "RunTime::Connect";
        public const string RunTime_Reg = "RunTime::Reg";
        public const string RunTime_CheckReg = "RunTime::CheckReg";
        public const string RunTime_Login = "RunTime::Login";
        public const string RunTime_CheckLogin = "RunTime::CheckLogin";
        public const string RunTime_Logout = "RunTime::Logout";
        public const string RunTime_Disconnect = "RunTime::Disconnect";
        /**
         * Main
         */
        public const string Main_GetNotice = "Main::GetNotice";
        public const string Main_IntoRoom = "Main::IntoRoom";
        public const string Main_QuitRoom = "Main::QuitRoom";
        public const string Main_CreateRoom = "Main::CreateRoom";
        public const string Main_UpdateRoom = "Main::UpdateRoom";
        public const string Main_MatchRoom = "Main::MatchRoom";
        /**
         * Register
         */
        public const string Reg_GetRegVerifyCode = "Reg::GetRegVerifyCode";
        /**
         * Login
         */
        public const string Login_GetFindPasswordVerifyCode = "Login::GetFindPasswordVerifyCode";
        public const string Login_UpdatePassword = "Login::UpdatePassword";
        /**
         * Room
         */
        public const string Room_GetRoomSettings = "Room::GetRoomSettings";
        public const string Room_GetRoomPlayerCount = "Room::GetRoomPlayerCount";
        public const string Room_UpdateRoomMaster = "Room::UpdateRoomMaster";
    }

    public class ReflectionSet
    {
        public const string FUNGAME_IMPL = "FunGame.Implement";
        public static string EXEFolderPath { get; } = Environment.CurrentDirectory.ToString() + "\\"; // 程序目录
        public static string PluginFolderPath { get; } = Environment.CurrentDirectory.ToString() + "\\plugins\\"; // 插件目录
    }

    public class FormSet
    {
        public const string Main = "Main";
        public const string Register = "Register";
        public const string Login = "Login";
        public const string Inventory = "Inventory";
        public const string Store = "Store";
        public const string RoomSetting = "RoomSetting";
        public const string UserCenter = "UserCenter";
    }

    public class GameMode
    {
        public const string GameMode_All = "所有模式";
        public const string GameMode_AllHasPass = "带密码的所有模式";
        public const string GameMode_Mix = "混战模式";
        public const string GameMode_MixHasPass = "带密码的混战模式";
        public const string GameMode_Team = "团队模式";
        public const string GameMode_TeamHasPass = "带密码的团队模式";
    }
}
