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
        GamingRequest,
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
        Gaming,
        AnonymousGameServer
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
        Reg_Reg,
        Login_Login,
        Login_GetFindPasswordVerifyCode,
        Login_UpdatePassword,
        Room_UpdateRoomSettings,
        Room_GetRoomPlayerCount,
        Room_UpdateRoomMaster,
        UserCenter_UpdateUser,
        UserCenter_UpdatePassword,
        UserCenter_DailySignIn,
        Inventory_GetStore,
        Inventory_GetMarket,
        Inventory_StoreBuy,
        Inventory_MarketBuy,
        Inventory_UpdateInventory,
        Inventory_Use,
        Inventory_StoreSell,
        Inventory_MarketSell,
        Inventory_MarketDelist,
        Inventory_UpdateMarketPrice,
        Inventory_GetOffer,
        Inventory_MakeOffer,
        Inventory_ReviseOffer,
        Inventory_RespondOffer,
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
        /// <summary>
        /// 魔法，编号 1xxx
        /// </summary>
        Magic,

        /// <summary>
        /// 战技，编号 2xxx
        /// </summary>
        Skill,

        /// <summary>
        /// 爆发技，编号 3xxx
        /// </summary>
        SuperSkill,

        /// <summary>
        /// 被动，编号 4xxx（物品被动从5xxx开始）
        /// </summary>
        Passive,

        /// <summary>
        /// 物品的主动技能，编号 6xxx
        /// </summary>
        Item
    }

    /// <summary>
    /// 注意：具有控制效果的特效，应该和技能本身的特效(一般此项为None)区分开来。此效果被赋值会改变一些判断的结果。
    /// </summary>
    public enum EffectType
    {
        /// <summary>
        /// 无特殊效果
        /// </summary>
        None,

        /// <summary>
        /// 这是来自装备的特效
        /// </summary>
        Item,

        /// <summary>
        /// 标记，目标受到某些技能的标记
        /// </summary>
        Mark,

        /// <summary>
        /// 眩晕，目标无法行动
        /// </summary>
        Stun,

        /// <summary>
        /// 冰冻，目标无法行动
        /// </summary>
        Freeze,

        /// <summary>
        /// 沉默，目标无法使用技能
        /// </summary>
        Silence,

        /// <summary>
        /// 定身
        /// </summary>
        Root,

        /// <summary>
        /// 恐惧
        /// </summary>
        Fear,

        /// <summary>
        /// 睡眠，目标暂时无法行动，受到伤害后会苏醒
        /// </summary>
        Sleep,

        /// <summary>
        /// 击退
        /// </summary>
        Knockback,

        /// <summary>
        /// 击倒，目标被击倒在地，暂时无法行动
        /// </summary>
        Knockdown,

        /// <summary>
        /// 嘲讽，目标被迫攻击施法者
        /// </summary>
        Taunt,

        /// <summary>
        /// 减速，目标行动速度和攻击频率降低
        /// </summary>
        Slow,

        /// <summary>
        /// 衰弱，目标的攻击和防御降低
        /// </summary>
        Weaken,

        /// <summary>
        /// 中毒，目标在一段时间内持续受到伤害
        /// </summary>
        Poison,

        /// <summary>
        /// 燃烧，目标受到火焰伤害，持续一段时间
        /// </summary>
        Burn,

        /// <summary>
        /// 流血，目标持续受到物理伤害
        /// </summary>
        Bleed,

        /// <summary>
        /// 致盲，目标无法准确攻击，命中率降低
        /// </summary>
        Blind,

        /// <summary>
        /// 致残，减少目标的行动或攻击能力
        /// </summary>
        Cripple,

        /// <summary>
        /// 护盾，减少受到的伤害或抵消部分伤害
        /// </summary>
        Shield,

        /// <summary>
        /// 持续治疗，逐步恢复生命值
        /// </summary>
        HealOverTime,

        /// <summary>
        /// 加速，提升行动速度
        /// </summary>
        Haste,

        /// <summary>
        /// 无敌，暂时不会受到任何伤害
        /// </summary>
        Invulnerable,

        /// <summary>
        /// 不可选中，无法成为普攻和技能的目标
        /// </summary>
        Unselectable,

        /// <summary>
        /// 伤害提升，增加攻击输出
        /// </summary>
        DamageBoost,

        /// <summary>
        /// 物理护甲/魔法抗性提升，减少所受伤害
        /// </summary>
        DefenseBoost,

        /// <summary>
        /// 暴击提升，增加暴击率或暴击伤害
        /// </summary>
        CritBoost,

        /// <summary>
        /// 魔法恢复，增加魔法值回复速度
        /// </summary>
        MPRegen,

        /// <summary>
        /// 破甲，提高物理/魔法穿透
        /// </summary>
        PenetrationBoost,

        /// <summary>
        /// 降低魔法抗性，目标更容易受到魔法伤害
        /// </summary>
        MagicResistBreak,

        /// <summary>
        /// 诅咒，降低目标的属性或给予负面效果
        /// </summary>
        Curse,

        /// <summary>
        /// 疲劳，减少目标的攻击或技能效果
        /// </summary>
        Exhaustion,

        /// <summary>
        /// 魔力燃烧，消耗目标的魔法值
        /// </summary>
        ManaBurn,

        /// <summary>
        /// 魅惑，控制目标替施法者作战
        /// </summary>
        Charm,

        /// <summary>
        /// 缴械，目标无法进行普通攻击
        /// </summary>
        Disarm,

        /// <summary>
        /// 混乱，目标的行动变得随机化
        /// </summary>
        Confusion,

        /// <summary>
        /// 石化，目标无法行动，并大幅增加受到的伤害
        /// </summary>
        Petrify,

        /// <summary>
        /// 法术沉默，目标无法施放魔法技能
        /// </summary>
        SilenceMagic,

        /// <summary>
        /// 放逐，目标暂时无法被攻击，也无法行动
        /// </summary>
        Banish,

        /// <summary>
        /// 毁灭，目标在倒计时结束后受到大量伤害或死亡
        /// </summary>
        Doom,

        /// <summary>
        /// 物理免疫
        /// </summary>
        PhysicalImmune,

        /// <summary>
        /// 魔法免疫
        /// </summary>
        MagicalImmune,

        /// <summary>
        /// 技能免疫
        /// </summary>
        SkilledImmune,

        /// <summary>
        /// 完全免疫：物理免疫 + 技能免疫
        /// </summary>
        AllImmune,

        /// <summary>
        /// 闪避提升
        /// </summary>
        EvadeBoost,

        /// <summary>
        /// 生命偷取
        /// </summary>
        Lifesteal,

        /// <summary>
        /// 重伤，目标受到的治疗效果降低
        /// </summary>
        GrievousWound
    }

    public enum ItemType
    {
        /// <summary>
        /// 魔法卡包 编号 10xxx
        /// </summary>
        MagicCardPack,

        /// <summary>
        /// 武器 编号 11xxx
        /// </summary>
        Weapon,

        /// <summary>
        /// 防具 编号 12xxx
        /// </summary>
        Armor,

        /// <summary>
        /// 鞋子 编号 13xxx
        /// </summary>
        Shoes,

        /// <summary>
        /// 饰品 编号 14xxx
        /// </summary>
        Accessory,

        /// <summary>
        /// 消耗品 编号 15xxx
        /// </summary>
        Consumable,

        /// <summary>
        /// 魔法卡 编号 16xxx
        /// </summary>
        MagicCard,

        /// <summary>
        /// 收藏品 编号 17xxx
        /// </summary>
        Collectible,

        /// <summary>
        /// 特殊物品 编号 18xxx
        /// </summary>
        SpecialItem,

        /// <summary>
        /// 任务物品 编号 19xxx
        /// </summary>
        QuestItem,

        /// <summary>
        /// 礼包 编号 20xxx
        /// </summary>
        GiftBox,

        /// <summary>
        /// 其他 编号 21xxx
        /// </summary>
        Others
    }

    /// <summary>
    /// 指示物品在哪个栏位上
    /// </summary>
    public enum EquipSlotType
    {
        None,
        MagicCardPack,
        Weapon,
        Armor,
        Shoes,
        Accessory1,
        Accessory2
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
        CharacterStatistics,
        Club,
        Quest
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

    public enum PrimaryAttribute
    {
        None,
        STR,
        AGI,
        INT
    }

    public enum WeaponType
    {
        /// <summary>
        /// 不是武器
        /// </summary>
        None,

        /// <summary>
        /// 单手剑
        /// </summary>
        OneHandedSword,

        /// <summary>
        /// 双手重剑
        /// </summary>
        TwoHandedSword,

        /// <summary>
        /// 弓
        /// </summary>
        Bow,

        /// <summary>
        /// 手枪
        /// </summary>
        Pistol,

        /// <summary>
        /// 步枪
        /// </summary>
        Rifle,

        /// <summary>
        /// 双持短刀
        /// </summary>
        DualDaggers,

        /// <summary>
        /// 法器
        /// </summary>
        Talisman,

        /// <summary>
        /// 法杖
        /// </summary>
        Staff,

        /// <summary>
        /// 长柄
        /// </summary>
        Polearm,

        /// <summary>
        /// 拳套
        /// </summary>
        Gauntlet,

        /// <summary>
        /// 暗器
        /// </summary>
        HiddenWeapon
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
        UseItem,
        EndTurn
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

    public enum QualityType
    {
        White,
        Green,
        Blue,
        Purple,
        Orange,
        Red,
        Gold
    }

    public enum RarityType
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
        GameModule,
        Warning
    }

    public enum SQLMode
    {
        None,
        MySQL,
        SQLite
    }

    public enum QuestType
    {
        Continuous,
        Immediate,
        Progressive
    }

    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public enum OfferActionType
    {
        /// <summary>
        /// 报价发起方修改报价，
        /// 如果当前状态为 <see cref="OfferState.Created"/>（已创建） 和 <see cref="OfferState.Negotiating"/>（协商中），
        /// 则状态转为 <see cref="OfferState.PendingOfferorConfirmation"/>（等待发起方确认）
        /// </summary>
        OfferorRevise,

        /// <summary>
        /// 报价发起方确认报价，状态转为 <see cref="OfferState.OfferorConfirmed"/>（发起方已确认）
        /// </summary>
        OfferorConfirm,

        /// <summary>
        /// 报价发起方发送报价，状态转为 <see cref="OfferState.Sent"/>（已发送）
        /// </summary>
        OfferorSend,

        /// <summary>
        /// 报价接收方修改报价，状态转为 <see cref="OfferState.PendingOffereeConfirmation"/>（等待接收方确认）
        /// </summary>
        OffereeRevise,

        /// <summary>
        /// 报价接收方确认报价，状态转为 <see cref="OfferState.OffereeConfirmed"/>（接收方已确认）
        /// </summary>
        OffereeConfirm,

        /// <summary>
        /// 报价接收方发送报价，状态转为 <see cref="OfferState.Negotiating"/>（协商中）
        /// </summary>
        OffereeSend,

        /// <summary>
        /// 报价发起方同意接收方的协商，状态转为 <see cref="OfferState.NegotiationAccepted"/>（协商已接受）
        /// </summary>
        OfferorAccept,

        /// <summary>
        /// 报价发起方手动取消了报价，状态转为 <see cref="OfferState.Cancelled"/>（已取消）
        /// </summary>
        OfferorCancel,

        /// <summary>
        /// 报价接收方同意报价，状态转为 <see cref="OfferState.Completed"/>（已完成）
        /// </summary>
        OffereeAccept,

        /// <summary>
        /// 报价接收方拒绝报价，状态转为 <see cref="OfferState.Rejected"/>（已拒绝）
        /// </summary>
        OffereeReject
    }

    public enum QueueUpdatedReason
    {
        None,
        Action,
        Special,
        PreCastSuperSkill,
        Respawn
    }

    public enum DispelType
    {
        /// <summary>
        /// 无驱散 [ 默认，不能驱散其他特效 ]
        /// </summary>
        None,

        /// <summary>
        /// 弱驱散
        /// </summary>
        Weak,

        /// <summary>
        /// 持续性弱驱散
        /// </summary>
        DurativeWeak,

        /// <summary>
        /// 临时弱驱散
        /// </summary>
        TemporaryWeak,

        /// <summary>
        /// 强驱散
        /// </summary>
        Strong,

        /// <summary>
        /// 持续性强驱散
        /// </summary>
        DurativeStrong,

        /// <summary>
        /// 临时强驱散
        /// </summary>
        TemporaryStrong,

        /// <summary>
        /// 特殊驱散
        /// </summary>
        Special
    }

    public enum DispelledType
    {
        /// <summary>
        /// 可弱驱散 [ 默认 ]
        /// </summary>
        Weak,

        /// <summary>
        /// 需强驱散
        /// </summary>
        Strong,

        /// <summary>
        /// 需特殊驱散
        /// </summary>
        Special,

        /// <summary>
        /// 不可驱散 [ 最高优先级 ]
        /// </summary>
        CannotBeDispelled
    }

    /// <summary>
    /// 标准实现的免疫类型
    /// </summary>
    public enum ImmuneType
    {
        None,
        Physical,
        Magical,
        Skilled,
        All,
        Special
    }
}
