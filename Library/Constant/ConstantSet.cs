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
        public const string RunTime_Connect = "RunTime_Connect";
        public const string RunTime_Reg = "RunTime_Reg";
        public const string RunTime_CheckReg = "RunTime_CheckReg";
        public const string RunTime_Login = "RunTime_Login";
        public const string RunTime_CheckLogin = "RunTime_CheckLogin";
        public const string RunTime_Logout = "RunTime_Logout";
        public const string RunTime_Disconnect = "RunTime_Disconnect";
        public const string RunTime_HeartBeat = "RunTime_HeartBeat";
        public const string Main_GetNotice = "Main_GetNotice";
        public const string Main_IntoRoom = "Main_IntoRoom";
        public const string Main_QuitRoom = "Main_QuitRoom";
        public const string Main_Chat = "Main_Chat";
        public const string Main_CreateRoom = "Main_CreateRoom";
        public const string Main_UpdateRoom = "Main_UpdateRoom";
        public const string Main_MatchRoom = "Main_MatchRoom";
        public const string Room_ChangeRoomSetting = "Room_ChangeRoomSetting";
        public const string Room_UpdateRoomMaster = "Room_UpdateRoomMaster";
        public const string Room_GetRoomPlayerCount = "Room_GetRoomPlayerCount";
    }

    /// <summary>
    /// 需要同步更新
    /// Milimoe.FunGame.Core.Library.Constant.DataRequestType,
    /// Milimoe.FunGame.Core.Api.Transmittal.DataRequest.GetTypeString(DataRequestType type)
    /// </summary>
    public class DataRequestSet
    {
        public const string UnKnown = "UnKnown";
        public const string Main_GetNotice = "Main_GetNotice";
        public const string Main_CreateRoom = "Main_CreateRoom";
        public const string Main_UpdateRoom = "Main_UpdateRoom";
        public const string Reg_GetRegVerifyCode = "Reg_GetRegVerifyCode";
        public const string Login_GetFindPasswordVerifyCode = "Login_GetFindPasswordVerifyCode";
        public const string Login_UpdatePassword = "Login_UpdatePassword";
        public const string Room_GetRoomSettings = "Room_GetRoomSettings";
        public const string Room_GetRoomPlayerCount = "Room_GetRoomPlayerCount";
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
