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

    public class SocketSet
    {
        public const int MaxRetryTimes = 20;
        public const int MaxConnection_1C2G = 10;
        public const int MaxConnection_2C2G = 20;
        public const int MaxConnection_4C4G = 40;

        public const string Socket = "Socket";
        public const string Unknown = "Unknown";
        public const string Connect = "Connect";
        public const string GetNotice = "GetNotice";
        public const string Login = "Login";
        public const string CheckLogin = "CheckLogin";
        public const string Logout = "Logout";
        public const string Disconnect = "Disconnect";
        public const string HeartBeat = "HeartBeat";
        public const string IntoRoom = "IntoRoom";
        public const string QuitRoom = "QuitRoom";
        public const string Chat = "Chat";
        public const string Reg = "Reg";
        public const string CheckReg = "CheckReg";
        public const string CreateRoom = "CreateRoom";
        public const string UpdateRoom = "UpdateRoom";
        public const string ChangeRoomSetting = "ChangeRoomSetting";
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
        public const string GameMode_Mix = "混战模式";
        public const string GameMode_Team = "团队模式";
        public const string GameMode_MixHasPass = "带密码的混战模式";
        public const string GameMode_TeamHasPass = "带密码的团队模式";
    }
}
