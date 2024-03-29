﻿/**
 * 此文件用于保存字符串常量（String Set）
 */
namespace Milimoe.FunGame.Core.Library.Constant
{
    /// <summary>
    /// 配合 <see cref="InterfaceMethod"/> <see cref="InterfaceType"/> 使用，也别忘了修改 <see cref="Api.Utility.Implement"/>
    /// </summary>
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
            public const string GameModeList = "GameModeList";
            public const string GameMapList = "GameMapList";
        }
    }

    /// <summary>
    /// 需要同步更新 <see cref="SocketMessageType"/>
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
        public const string Connect = "Connect";
        public const string Disconnect = "Disconnect";
        public const string System = "System";
        public const string HeartBeat = "HeartBeat";
        public const string ForceLogout = "ForceLogout";
        public const string Chat = "Chat";
        public const string UpdateRoomMaster = "UpdateRoomMaster";
        public const string MatchRoom = "MatchRoom";
        public const string StartGame = "StartGame";
        public const string EndGame = "EndGame";
        public const string Gaming = "Gaming";

        /// <summary>
        /// 将通信类型的枚举转换为字符串
        /// </summary>
        /// <param name="type">通信类型</param>
        /// <returns>等效字符串</returns>
        public static string GetTypeString(SocketMessageType type)
        {
            return type switch
            {
                SocketMessageType.DataRequest => DataRequest,
                SocketMessageType.Connect => Connect,
                SocketMessageType.Disconnect => Disconnect,
                SocketMessageType.System => System,
                SocketMessageType.HeartBeat => HeartBeat,
                SocketMessageType.ForceLogout => ForceLogout,
                SocketMessageType.Chat => Chat,
                SocketMessageType.UpdateRoomMaster => UpdateRoomMaster,
                SocketMessageType.MatchRoom => MatchRoom,
                SocketMessageType.StartGame => StartGame,
                SocketMessageType.EndGame => EndGame,
                SocketMessageType.Gaming => Gaming,
                _ => Unknown
            };
        }
    }

    /// <summary>
    /// 需要同步更新 <see cref="DataRequestType"/>
    /// </summary>
    public class DataRequestSet
    {
        public const string UnKnown = "UnKnown";
        /**
         * RunTime
         */
        public const string RunTime_Logout = "RunTime::Logout";
        /**
         * Main
         */
        public const string Main_GetNotice = "Main::GetNotice";
        public const string Main_IntoRoom = "Main::IntoRoom";
        public const string Main_QuitRoom = "Main::QuitRoom";
        public const string Main_CreateRoom = "Main::CreateRoom";
        public const string Main_UpdateRoom = "Main::UpdateRoom";
        public const string Main_MatchRoom = "Main::MatchRoom";
        public const string Main_Chat = "Main::Chat";
        public const string Main_Ready = "Main::Ready";
        public const string Main_CancelReady = "Main::CancelReady";
        public const string Main_StartGame = "Main::StartGame";
        /**
         * Register
         */
        public const string Reg_GetRegVerifyCode = "Reg::GetRegVerifyCode";
        /**
         * Login
         */
        public const string Login_Login = "Login::Login";
        public const string Login_GetFindPasswordVerifyCode = "Login::GetFindPasswordVerifyCode";
        public const string Login_UpdatePassword = "Login::UpdatePassword";
        /**
         * Room
         */
        public const string Room_GetRoomSettings = "Room::GetRoomSettings";
        public const string Room_GetRoomPlayerCount = "Room::GetRoomPlayerCount";
        public const string Room_UpdateRoomMaster = "Room::UpdateRoomMaster";
        /**
         * Gaming
         */
        public const string Gaming = "Gaming";

        /// <summary>
        /// 获取Type的等效字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeString(DataRequestType type)
        {
            return type switch
            {
                DataRequestType.RunTime_Logout => RunTime_Logout,
                DataRequestType.Main_GetNotice => Main_GetNotice,
                DataRequestType.Main_CreateRoom => Main_CreateRoom,
                DataRequestType.Main_UpdateRoom => Main_UpdateRoom,
                DataRequestType.Main_IntoRoom => Main_IntoRoom,
                DataRequestType.Main_QuitRoom => Main_QuitRoom,
                DataRequestType.Main_MatchRoom => Main_MatchRoom,
                DataRequestType.Main_Chat => Main_Chat,
                DataRequestType.Main_Ready => Main_Ready,
                DataRequestType.Main_CancelReady => Main_CancelReady,
                DataRequestType.Main_StartGame => Main_StartGame,
                DataRequestType.Reg_GetRegVerifyCode => Reg_GetRegVerifyCode,
                DataRequestType.Login_Login => Login_Login,
                DataRequestType.Login_GetFindPasswordVerifyCode => Login_GetFindPasswordVerifyCode,
                DataRequestType.Login_UpdatePassword => Login_UpdatePassword,
                DataRequestType.Room_GetRoomSettings => Room_GetRoomSettings,
                DataRequestType.Room_GetRoomPlayerCount => Room_GetRoomPlayerCount,
                DataRequestType.Room_UpdateRoomMaster => Room_UpdateRoomMaster,
                DataRequestType.Gaming => Gaming,
                _ => UnKnown
            };
        }
    }

    public class GamingSet
    {
        public const string None = "Gaming::None";
        public const string Connect = "Gaming::Connect";
        public const string Disconnect = "Gaming::Disconnect";
        public const string Reconnect = "Gaming::Reconnect";
        public const string BanCharacter = "Gaming::BanCharacter";
        public const string PickCharacter = "Gaming::PickCharacter";
        public const string Random = "Gaming::Random";
        public const string Round = "Gaming::Round";
        public const string LevelUp = "Gaming::LevelUp";
        public const string Move = "Gaming::Move";
        public const string Attack = "Gaming::Attack";
        public const string Skill = "Gaming::Skill";
        public const string Item = "Gaming::Item";
        public const string Magic = "Gaming::Magic";
        public const string Buy = "Gaming::Buy";
        public const string SuperSkill = "Gaming::SuperSkill";
        public const string Pause = "Gaming::Pause";
        public const string Unpause = "Gaming::Unpause";
        public const string Surrender = "Gaming::Surrender";
        public const string UpdateInfo = "Gaming::UpdateInfo";
        public const string Punish = "Gaming::Punish";

        /// <summary>
        /// 获取Type的等效字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeString(GamingType type)
        {
            return type switch
            {
                GamingType.Connect => Connect,
                GamingType.Disconnect => Disconnect,
                GamingType.Reconnect => Reconnect,
                GamingType.BanCharacter => BanCharacter,
                GamingType.PickCharacter => PickCharacter,
                GamingType.Random => Random,
                GamingType.Round => Round,
                GamingType.LevelUp => LevelUp,
                GamingType.Move => Move,
                GamingType.Attack => Attack,
                GamingType.Skill => Skill,
                GamingType.Item => Item,
                GamingType.Magic => Magic,
                GamingType.Buy => Buy,
                GamingType.SuperSkill => SuperSkill,
                GamingType.Pause => Pause,
                GamingType.Unpause => Unpause,
                GamingType.Surrender => Surrender,
                GamingType.UpdateInfo => UpdateInfo,
                GamingType.Punish => Punish,
                _ => None
            };
        }
    }

    public class ReflectionSet
    {
        public const string FUNGAME_IMPL = "FunGame.Implement";
        public static string EXEFolderPath { get; } = AppDomain.CurrentDomain.BaseDirectory; // 程序目录
        public static string PluginFolderPath { get; } = AppDomain.CurrentDomain.BaseDirectory + @"plugins\"; // 插件目录
        public static string GameModeFolderPath { get; } = AppDomain.CurrentDomain.BaseDirectory + @"gamemods\"; // 游戏模组目录
        public static string GameMapFolderPath { get; } = AppDomain.CurrentDomain.BaseDirectory + @"maps\"; // 游戏地图目录
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

    public class RoomSet
    {
        public const string All = "全部";
        public const string Mix = "混战模式";
        public const string Team = "团队模式";
        public const string Solo = "对弈模式";
        public const string FastAuto = "快速自走模式";
        public const string Custom = "自定义模式";

        /// <summary>
        /// 获取Type的等效字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeString(RoomType type)
        {
            return type switch
            {
                RoomType.Mix => Mix,
                RoomType.Team => Team,
                RoomType.Solo => Solo,
                RoomType.FastAuto => FastAuto,
                RoomType.Custom => Custom,
                _ => All
            };
        }

        /// <summary>
        /// 获取字符串对应的枚举
        /// </summary>
        /// <param name="typestring"></param>
        /// <returns></returns>
        public static RoomType GetRoomType(string typestring)
        {
            return typestring switch
            {
                Mix => RoomType.Mix,
                Team => RoomType.Team,
                FastAuto => RoomType.FastAuto,
                Custom => RoomType.Custom,
                _ => RoomType.All
            };
        }
    }

    public class UserSet
    {
        public const long UnknownUserId = 0;
        public const long GuestUserId = -5;
        public const long LocalUserId = -6;

        public const string UnknownUserName = "未知用户";
        public const string GuestUserName = "游客用户";
        public const string LocalUserName = "本地用户";
    }
}
