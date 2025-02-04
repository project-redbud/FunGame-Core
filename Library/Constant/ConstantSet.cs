﻿/**
 * 此文件用于保存字符串常量（String Set）
 */
namespace Milimoe.FunGame.Core.Library.Constant
{
    public class CommonSet
    {
        public static string GetLogLevelPrefix(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "T/",
                LogLevel.Debug => "D/",
                LogLevel.Info => "I/",
                LogLevel.Warning => "W/",
                LogLevel.Error => "E/",
                LogLevel.Critical => "C/",
                _ => "I/"
            };
        }
    }

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
            public const string SecretKey = "SecretKey";
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
        public const string Plugins_Mark = "plugins_mark";

        public const string Socket = "Socket";
        public const string Unknown = "Unknown";
        public const string DataRequest = "DataRequest";
        public const string GamingRequest = "GamingRequest";
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
        public const string AnonymousGameServer = "AnonymousGameServer";

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
        public const string Reg_Reg = "Reg::Reg";
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
                DataRequestType.Reg_Reg => Reg_Reg,
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
        public static string GameModuleFolderPath { get; } = AppDomain.CurrentDomain.BaseDirectory + @"modules\"; // 游戏模组目录
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

    public class CharacterActionSet
    {
        public const string ActionQueue = "ActionQueue";
        public const string Actor = "Actor";
        public const string CastSkill = "CastSkill";
        public const string Enemys = "Enemys";
        public const string Teammates = "Teammates";
        public const string GameMap = "GameMap";
    }

    public class CharacterSet
    {
        public static string GetPrimaryAttributeName(PrimaryAttribute type)
        {
            return type switch
            {
                PrimaryAttribute.AGI => "敏捷",
                PrimaryAttribute.INT => "智力",
                _ => "力量"
            };
        }

        public static string GetMagicDamageName(MagicType type)
        {
            return type switch
            {
                MagicType.Starmark => "星痕魔法伤害",
                MagicType.PurityNatural => "现代结晶魔法伤害",
                MagicType.PurityContemporary => "纯粹结晶魔法伤害",
                MagicType.Bright => "光魔法伤害",
                MagicType.Shadow => "影魔法伤害",
                MagicType.Element => "元素魔法伤害",
                MagicType.Fleabane => "紫宛魔法伤害",
                MagicType.Particle => "时空魔法伤害",
                _ => "魔法伤害",
            };
        }

        public static string GetMagicResistanceName(MagicType type)
        {
            return type switch
            {
                MagicType.Starmark => "星痕抗性",
                MagicType.PurityNatural => "现代结晶抗性",
                MagicType.PurityContemporary => "纯粹结晶抗性",
                MagicType.Bright => "光抗性",
                MagicType.Shadow => "影抗性",
                MagicType.Element => "元素抗性",
                MagicType.Fleabane => "紫宛抗性",
                MagicType.Particle => "时空抗性",
                _ => "魔法抗性",
            };
        }

        public static string GetContinuousKilling(int kills)
        {
            if (kills > 10) return "超越神的杀戮";
            else
            {
                return kills switch
                {
                    2 => "双杀",
                    3 => "三杀",
                    4 => "大杀特杀",
                    5 => "杀人如麻",
                    6 => "主宰比赛",
                    7 => "无人能挡",
                    8 => "变态杀戮",
                    9 => "如同神一般",
                    10 => "超越神的杀戮",
                    _ => ""
                };
            }
        }

        public static string GetCharacterState(CharacterState state)
        {
            return state switch
            {
                CharacterState.Casting => "角色正在吟唱魔法",
                CharacterState.PreCastSuperSkill => "角色预释放了爆发技",
                CharacterState.ActionRestricted => "角色现在行动受限",
                CharacterState.BattleRestricted => "角色现在战斗不能",
                CharacterState.SkillRestricted => "角色现在技能受限",
                _ => "角色现在完全行动不能"
            };
        }

        public static string GetRarityTypeName(RarityType type)
        {
            return type switch
            {
                RarityType.TwoStar => "★★",
                RarityType.ThreeStar => "★★★",
                RarityType.FourStar => "★★★★",
                RarityType.FiveStar => "★★★★★",
                _ => "★"
            };
        }
    }

    public class ItemSet
    {
        public static string GetItemTypeName(ItemType type)
        {
            return type switch
            {
                ItemType.MagicCardPack => "魔法卡包",
                ItemType.Weapon => "武器",
                ItemType.Armor => "防具",
                ItemType.Shoes => "鞋子",
                ItemType.Accessory => "饰品",
                ItemType.Consumable => "消耗品",
                ItemType.MagicCard => "魔法卡",
                ItemType.Collectible => "收藏品",
                ItemType.SpecialItem => "特殊物品",
                ItemType.QuestItem => "任务物品",
                ItemType.GiftBox => "礼包",
                ItemType.Others => "其他",
                _ => ""
            };
        }

        public static ItemType GetItemTypeFromName(string name)
        {
            return name switch
            {
                "魔法卡包" => ItemType.MagicCardPack,
                "武器" => ItemType.Weapon,
                "防具" => ItemType.Armor,
                "鞋子" => ItemType.Shoes,
                "饰品" => ItemType.Accessory,
                "消耗品" => ItemType.Consumable,
                "魔法卡" => ItemType.MagicCard,
                "收藏品" => ItemType.Collectible,
                "特殊物品" => ItemType.SpecialItem,
                "任务物品" => ItemType.QuestItem,
                "礼包" => ItemType.GiftBox,
                _ => ItemType.Others,
            };
        }

        public static string GetWeaponTypeName(WeaponType type)
        {
            return type switch
            {
                WeaponType.OneHandedSword => "单手剑",
                WeaponType.TwoHandedSword => "双手重剑",
                WeaponType.Bow => "弓",
                WeaponType.Pistol => "手枪",
                WeaponType.Rifle => "步枪",
                WeaponType.DualDaggers => "双持短刀",
                WeaponType.Talisman => "法器",
                WeaponType.Staff => "法杖",
                WeaponType.Polearm => "长柄",
                WeaponType.Gauntlet => "拳套",
                WeaponType.HiddenWeapon => "暗器",
                _ => ""
            };
        }

        public static WeaponType GetWeaponTypeFromName(string name)
        {
            return name switch
            {
                "单手剑" => WeaponType.OneHandedSword,
                "双手重剑" => WeaponType.TwoHandedSword,
                "弓" => WeaponType.Bow,
                "手枪" => WeaponType.Pistol,
                "步枪" => WeaponType.Rifle,
                "双持短刀" => WeaponType.DualDaggers,
                "法器" => WeaponType.Talisman,
                "法杖" => WeaponType.Staff,
                "长柄" => WeaponType.Polearm,
                "拳套" => WeaponType.Gauntlet,
                "暗器" => WeaponType.HiddenWeapon,
                _ => WeaponType.None
            };
        }

        public static string GetEquipSlotTypeName(EquipSlotType type)
        {
            return type switch
            {
                EquipSlotType.MagicCardPack => "魔法卡包",
                EquipSlotType.Weapon => "武器",
                EquipSlotType.Armor => "防具",
                EquipSlotType.Shoes => "鞋子",
                EquipSlotType.Accessory1 => "饰品1",
                EquipSlotType.Accessory2 => "饰品2",
                _ => ""
            };
        }

        public static EquipSlotType GetEquipSlotTypeFromName(string name)
        {
            return name switch
            {
                "魔法卡包" => EquipSlotType.MagicCardPack,
                "武器" => EquipSlotType.Weapon,
                "防具" => EquipSlotType.Armor,
                "鞋子" => EquipSlotType.Shoes,
                "饰品1" => EquipSlotType.Accessory1,
                "饰品2" => EquipSlotType.Accessory2,
                _ => EquipSlotType.None
            };
        }

        public static string GetQualityTypeName(QualityType type)
        {
            return type switch
            {
                QualityType.Green => "优秀",
                QualityType.Blue => "稀有",
                QualityType.Purple => "史诗",
                QualityType.Orange => "传说",
                QualityType.Red => "神话",
                QualityType.Gold => "不朽",
                _ => "普通"
            };
        }

        public static QualityType GetQualityTypeFromName(string name)
        {
            return name switch
            {
                "优秀" => QualityType.Green,
                "稀有" => QualityType.Blue,
                "史诗" => QualityType.Purple,
                "传说" => QualityType.Orange,
                "神话" => QualityType.Red,
                "不朽" => QualityType.Gold,
                _ => QualityType.White
            };
        }

        public static string[] QualityTypeNameArray => ["普通", "优秀", "稀有", "史诗", "传说", "神话", "不朽"];
    }

    public class SkillSet
    {
        public static string GetSkillTypeName(SkillType type)
        {
            return type switch
            {
                SkillType.Magic => "魔法",
                SkillType.Skill => "战技",
                SkillType.SuperSkill => "爆发技",
                SkillType.Item => "物品",
                _ => "被动"
            };
        }

        public static SkillType GetSkillTypeFromName(string name)
        {
            return name switch
            {
                "魔法" => SkillType.Magic,
                "战技" => SkillType.Skill,
                "爆发技" => SkillType.SuperSkill,
                "物品" => SkillType.Item,
                _ => SkillType.Passive,
            };
        }

        public static int GetSkillMaxLevel(SkillType type)
        {
            return type switch
            {
                SkillType.Magic => General.GameplayEquilibriumConstant.MaxMagicLevel,
                SkillType.Skill => General.GameplayEquilibriumConstant.MaxSkillLevel,
                SkillType.SuperSkill => General.GameplayEquilibriumConstant.MaxSuperSkillLevel,
                SkillType.Item => General.GameplayEquilibriumConstant.MaxSkillLevel,
                _ => General.GameplayEquilibriumConstant.MaxPassiveSkillLevel
            };
        }

        public static string GetEffectTypeName(EffectType type)
        {
            return type switch
            {
                EffectType.None => "无特殊效果",
                EffectType.Item => "装备特效",
                EffectType.Mark => "标记",
                EffectType.Stun => "眩晕",
                EffectType.Freeze => "冰冻",
                EffectType.Silence => "沉默",
                EffectType.Root => "定身",
                EffectType.Fear => "恐惧",
                EffectType.Sleep => "睡眠",
                EffectType.Knockback => "击退",
                EffectType.Knockdown => "击倒",
                EffectType.Taunt => "嘲讽",
                EffectType.Slow => "减速",
                EffectType.Weaken => "衰弱",
                EffectType.Poison => "中毒",
                EffectType.Burn => "燃烧",
                EffectType.Bleed => "流血",
                EffectType.Blind => "致盲",
                EffectType.Cripple => "致残",
                EffectType.Shield => "护盾",
                EffectType.HealOverTime => "持续治疗",
                EffectType.Haste => "加速",
                EffectType.Invulnerable => "无敌",
                EffectType.Unselectable => "不可选中",
                EffectType.DamageBoost => "伤害提升",
                EffectType.DefenseBoost => "防御提升",
                EffectType.CritBoost => "暴击提升",
                EffectType.ManaRegen => "魔法恢复",
                EffectType.ArmorBreak => "破甲",
                EffectType.MagicResistBreak => "降低魔抗",
                EffectType.Curse => "诅咒",
                EffectType.Exhaustion => "疲劳",
                EffectType.ManaBurn => "魔力燃烧",
                EffectType.Charm => "魅惑",
                EffectType.Disarm => "缴械",
                EffectType.Confusion => "混乱",
                EffectType.Petrify => "石化",
                EffectType.SilenceMagic => "法术沉默",
                EffectType.Banish => "放逐",
                EffectType.Doom => "毁灭",
                _ => "未知效果"
            };
        }
    }
}
