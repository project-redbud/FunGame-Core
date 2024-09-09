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

    /// <summary>
    /// 配合 <see cref="InterfaceMethod"/> <see cref="InterfaceSet"/> 使用，也别忘了修改 <see cref="Api.Utility.Implement"/>
    /// </summary>
    public enum InterfaceType
    {
        IClient,
        IServer
    }

    /// <summary>
    /// 配合 <see cref="RoomSet"/> 使用
    /// </summary>
    public enum RoomType
    {
        All,
        Mix,
        Team,
        Solo,
        FastAuto,
        Custom
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
    /// 需要同步更新 <see cref="SocketSet"/>
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
        MatchRoom,
        StartGame,
        EndGame,
        Gaming
    }

    /// <summary>
    /// 需要同步更新 <see cref="DataRequestSet"/>
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
        Main_StartGame,
        Reg_GetRegVerifyCode,
        Login_Login,
        Login_GetFindPasswordVerifyCode,
        Login_UpdatePassword,
        Room_GetRoomSettings,
        Room_GetRoomPlayerCount,
        Room_UpdateRoomMaster,
        Gaming
    }

    /// <summary>
    /// 需要同步更新 <see cref="GamingSet"/>
    /// </summary>
    public enum GamingType
    {
        None,
        Connect,
        Disconnect,
        Reconnect,
        BanCharacter,
        PickCharacter,
        Random,
        Round,
        LevelUp,
        Move,
        Attack,
        Skill,
        Item,
        Magic,
        Buy,
        SuperSkill,
        Pause,
        Unpause,
        Surrender,
        UpdateInfo,
        Punish
    }

    public enum TransmittalType
    {
        Socket,
        WebSocket
    }

    public enum SocketRuntimeType
    {
        Client,
        Server,
        Addon
    }

    public enum ErrorIPAddressType
    {
        None,
        IsNotAddress,
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

    public enum UserType
    {
        General,
        Empty,
        Guest,
        LocalUser
    }

    public enum ShowMessageType
    {
        None,
        General,
        Tip,
        Warning,
        Error,
        YesNo,
        OKCancel,
        RetryCancel,
        Input,
        InputCancel
    }

    public enum TimeType
    {
        None,
        General,
        LongDateOnly,
        ShortDateOnly,
        LongTimeOnly,
        ShortTimeOnly,
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
        None,
        Starmark,
        PurityNatural,
        PurityContemporary,
        Bright,
        Shadow,
        Element,
        Fleabane,
        Particle
    }

    /// <summary>
    /// 角色目前所处的状态
    /// </summary>
    public enum CharacterState
    {
        /// <summary>
        /// 可以行动 [ 战斗相关 ]
        /// </summary>
        Actionable,

        /// <summary>
        /// 完全行动不能 [ 战斗相关 ]
        /// </summary>
        NotActionable,

        /// <summary>
        /// 行动受限 [ 战斗相关 ]
        /// </summary>
        ActionRestricted,

        /// <summary>
        /// 战斗不能 [ 战斗相关 ]
        /// </summary>
        BattleRestricted,

        /// <summary>
        /// 技能受限 [ 战斗相关 ]
        /// </summary>
        SkillRestricted,

        /// <summary>
        /// 处于吟唱中 [ 战斗相关 ] [ 技能相关 ]
        /// </summary>
        Casting,

        /// <summary>
        /// 预释放爆发技(插队) [ 战斗相关 ] [ 技能相关 ]
        /// </summary>
        PreCastSuperSkill
    }

    public enum PrimaryAttribute
    {
        None,
        STR,
        AGI,
        INT
    }

    public enum ActionType
    {
        None,
        Ban,
        Pick,
        Random,
        Move,
        Attack,
        Skill,
        Item,
        Magic,
        Buy,
        SuperSkill,
        Pause,
        Unpause,
        Surrender,
        UpdateUserInfo,
        Punish
    }

    public enum CharacterActionType
    {
        None,
        NormalAttack,
        PreCastSkill,
        CastSkill,
        CastSuperSkill,
        UseItem
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
        None,
        Core,
        Vanguard,
        Guardian,
        Support,
        Medic
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
        GetRoomPlayerCount,
        StartGame,
        EndGame
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

    public enum InvokeMessageType
    {
        None,
        Core,
        Error,
        System,
        Api,
        Interface,
        DataRequest,
        Plugin,
        GameModule
    }
}
