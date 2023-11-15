/**
 * 此文件保存Type（类型）的枚举
 */
namespace Milimoe.FunGame.Core.Library.Constant
{
    public enum FormType
    {
        Main,
        Register,
        Login,
        Inventory,
        RoomSetting,
        Store,
        UserCenter,
        ForgetPassword
    }

    public enum OpenFormType
    {
        General,
        Dialog
    }

    public enum InterfaceType
    {
        IClient,
        IServer
    }

    public enum RoomType
    {
        All,
        Mix,
        Team,
        AllHasPass,
        MixHasPass,
        TeamHasPass
    }

    public enum MessageButtonType
    {
        OK,
        OKCancel,
        YesNo,
        RetryCancel,
        Input
    }

    public enum LightType
    {
        Green,
        Yellow,
        Red
    }

    /// <summary>
    /// 需要同步更新Milimoe.FunGame.Core.Library.Constant.SocketSet
    /// </summary>
    public enum SocketMessageType
    {
        Unknown,
        DataRequest,
        Connect,
        Disconnect,
        System,
        HeartBeat,
        ForceLogout,
        Chat,
        UpdateRoomMaster,
        MatchRoom
    }

    /// <summary>
    /// 需要同步更新Milimoe.FunGame.Core.Library.Constant.DataRequestSet
    /// </summary>
    public enum DataRequestType
    {
        UnKnown,
        RunTime_Logout,
        Main_GetNotice,
        Main_CreateRoom,
        Main_UpdateRoom,
        Main_IntoRoom,
        Main_QuitRoom,
        Main_MatchRoom,
        Main_Chat,
        Main_Ready,
        Main_CancelReady,
        Reg_GetRegVerifyCode,
        Login_Login,
        Login_GetFindPasswordVerifyCode,
        Login_UpdatePassword,
        Room_GetRoomSettings,
        Room_GetRoomPlayerCount,
        Room_UpdateRoomMaster
    }

    public enum SocketRuntimeType
    {
        Client,
        Server,
        Client_Plugin,
        Server_Plugin
    }

    public enum ErrorIPAddressType
    {
        None,
        IsNotIP,
        IsNotPort,
        WrongFormat
    }

    public enum EventType
    {
        ConnectEvent,
        DisconnectEvent,
        LoginEvent,
        LogoutEvent,
        RegEvent,
        IntoRoomEvent,
        SendTalkEvent,
        CreateRoomEvent,
        QuitRoomEvent,
        ChangeRoomSettingEvent,
        StartMatchEvent,
        StartGameEvent,
        ChangeProfileEvent,
        ChangeAccountSettingEvent,
        OpenStockEvent,
        SignInEvent,
        OpenStoreEvent,
        BuyItemEvent,
        ShowRankingEvent,
        UseItemEvent,
        EndGameEvent
    }

    public enum SkillType
    {
        Active,
        Passive
    }

    public enum ItemType
    {
        Active,
        Passive
    }

    public enum EntityType
    {
        Empty,
        User,
        UserStatistics,
        Room,
        Inventory,
        Item,
        ActiveItem,
        PassiveItem,
        Skill,
        ActiveSkill,
        PassiveSkill,
        GameStatistics,
        Character,
        CharacterStatistics
    }

    public enum TimeType
    {
        None,
        General,
        DateOnly,
        TimeOnly,
        Year4,
        Year2,
        Month,
        Day,
        Hour,
        Minute,
        Second
    }

    public enum MagicType
    {
        Starmark,
        PurityNatural,
        PurityContemporary,
        Bright,
        Shadow,
        Element,
        Fleabane,
        Particle
    }

    public enum VerifyCodeType
    {
        NumberVerifyCode,
        LetterVerifyCode,
        MixVerifyCode,
        ImageVerifyCode
    }

    public enum RoleType
    {
        Core,
        Guardian,
        Vanguard,
        Logistics,
        Assistant
    }

    public enum RoleRating
    {
        X,
        S,
        APlus,
        A,
        B,
        C,
        D,
        E
    }

    public enum ItemRankType
    {
        X,
        SPlus,
        S,
        APlus,
        A,
        B,
        C,
        D
    }

    public enum ItemQualityType
    {
        White,
        Green,
        Blue,
        Purple,
        Orange,
        Red
    }

    public enum ItemRarityType
    {
        OneStar,
        TwoStar,
        ThreeStar,
        FourStar,
        FiveStar
    }

    public enum RunTimeInvokeType
    {
        None,
        GetServerConnection,
        Connect,
        Connected,
        Disconnect,
        Disconnected,
        AutoLogin,
        Close
    }

    public enum MainInvokeType
    {
        None,
        Connected,
        Disconnected,
        Disconnect,
        SetGreen,
        SetGreenAndPing,
        SetRed,
        SetYellow,
        WaitConnectAndSetYellow,
        WaitLoginAndSetYellow,
        LogOut,
        LogIn,
        SetUser,
        IntoRoom,
        QuitRoom,
        UpdateRoom,
        CreateRoom,
        Chat,
        MatchRoom,
        UpdateRoomMaster,
        GetRoomPlayerCount
    }

    public enum RegInvokeType
    {
        None,
        DuplicateUserName,
        DuplicateEmail,
        InputVerifyCode
    }

    public enum AuthenticationType
    {
        None,
        ScriptOnly,
        Column,
        Username
    }
}
