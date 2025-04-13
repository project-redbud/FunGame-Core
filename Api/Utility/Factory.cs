using System.Data;
using Milimoe.FunGame.Core.Api.EntityFactory;
using Milimoe.FunGame.Core.Api.OpenEntityAdapter;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class Factory
    {
        /// <summary>
        /// 支持动态扩展的工厂实例
        /// </summary>
        public static Factory OpenFactory { get; } = new();

        private Factory()
        {

        }

        internal HashSet<EntityFactoryDelegate<Character>> CharacterFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Inventory>> InventoryFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Skill>> SkillFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Effect>> EffectFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Item>> ItemFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Room>> RoomFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<User>> UserFactories { get; } = [];

        public delegate T? EntityFactoryDelegate<T>(long id, string name, Dictionary<string, object> args);

        /// <summary>
        /// 注册工厂方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        public void RegisterFactory<T>(EntityFactoryDelegate<T> d)
        {
            if (typeof(T) == typeof(Character) && d is EntityFactoryDelegate<Character> character)
            {
                CharacterFactories.Add(character);
            }
            if (typeof(T) == typeof(Inventory) && d is EntityFactoryDelegate<Inventory> inventory)
            {
                InventoryFactories.Add(inventory);
            }
            if (typeof(T) == typeof(Skill) && d is EntityFactoryDelegate<Skill> skill)
            {
                SkillFactories.Add(skill);
            }
            if (typeof(T) == typeof(Effect) && d is EntityFactoryDelegate<Effect> effect)
            {
                EffectFactories.Add(effect);
            }
            if (typeof(T) == typeof(Item) && d is EntityFactoryDelegate<Item> item)
            {
                ItemFactories.Add(item);
            }
            if (typeof(T) == typeof(Room) && d is EntityFactoryDelegate<Room> room)
            {
                RoomFactories.Add(room);
            }
            if (typeof(T) == typeof(User) && d is EntityFactoryDelegate<User> user)
            {
                UserFactories.Add(user);
            }
        }

        /// <summary>
        /// 构造一个实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedInstanceClassException"></exception>
        public T GetInstance<T>(long id, string name, Dictionary<string, object> args)
        {
            if (typeof(T) == typeof(Character))
            {
                foreach (EntityFactoryDelegate<Character> d in CharacterFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T character)
                        {
                            return character;
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }
                return (T)(object)GetCharacter();
            }
            if (typeof(T) == typeof(Inventory))
            {
                foreach (EntityFactoryDelegate<Inventory> d in InventoryFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T inventory)
                        {
                            return inventory;
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }
                return (T)(object)GetInventory();
            }
            if (typeof(T) == typeof(Skill))
            {
                foreach (EntityFactoryDelegate<Skill> d in SkillFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T skill)
                        {
                            return skill;
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }

                Skill openSkill = new OpenSkill(id, name, args);
                if (args.TryGetValue("values", out object? value) && value is Dictionary<string, object> dict)
                {
                    foreach (string key in dict.Keys)
                    {
                        openSkill.Values[key] = dict[key];
                    }
                }

                return (T)(object)openSkill;
            }
            if (typeof(T) == typeof(Effect))
            {
                foreach (EntityFactoryDelegate<Effect> d in EffectFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T effect)
                        {
                            return effect;
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }
                return (T)(object)GetEffect();
            }
            if (typeof(T) == typeof(Item))
            {
                foreach (EntityFactoryDelegate<Item> d in ItemFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T item)
                        {
                            return item;
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }
                return (T)(object)new OpenItem(id, name, args);
            }
            if (typeof(T) == typeof(Room))
            {
                foreach (EntityFactoryDelegate<Room> d in RoomFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T room)
                        {
                            return room;
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }
                return (T)(object)GetRoom();
            }
            if (typeof(T) == typeof(User))
            {
                foreach (EntityFactoryDelegate<User> d in UserFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T user)
                        {
                            return user;
                        }
                    }
                    catch (Exception e)
                    {
                        TXTHelper.AppendErrorLog(e.GetErrorInfo());
                    }
                }
                return (T)(object)GetUser();
            }
            throw new NotSupportedInstanceClassException();
        }

        /// <summary>
        /// 此方法使用 <see cref="EntityModuleConfig{T}"/> 取得一个实体字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module_name"></param>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public static Dictionary<string, T> GetGameModuleInstances<T>(string module_name, string file_name) where T : BaseEntity
        {
            EntityModuleConfig<T> config = new(module_name, file_name);
            config.LoadConfig();
            if (typeof(T) == typeof(Skill))
            {
                OpenSkillAdapter.Adaptation(config);
            }
            if (typeof(T) == typeof(Item))
            {
                OpenItemAdapter.Adaptation(config);
            }
            return config;
        }

        /// <summary>
        /// 使用 <see cref="EntityModuleConfig{T}"/> 构造一个实体字典并保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module_name"></param>
        /// <param name="file_name"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static void CreateGameModuleEntityConfig<T>(string module_name, string file_name, Dictionary<string, T> dict) where T : BaseEntity
        {
            EntityModuleConfig<T> config = new(module_name, file_name);
            foreach (string key in dict.Keys)
            {
                config[key] = dict[key];
            }
            config.SaveConfig();
        }

        internal HashSet<SQLHelperFactoryDelegate> SQLHelperFactories { get; } = [];

        public delegate SQLHelper? SQLHelperFactoryDelegate();

        /// <summary>
        /// 注册工厂方法 [SQLHelper]
        /// </summary>
        /// <param name="d"></param>
        public void RegisterFactory(SQLHelperFactoryDelegate d)
        {
            SQLHelperFactories.Add(d);
        }

        /// <summary>
        /// 构造一个 SQLHelper 实例
        /// </summary>
        /// <returns></returns>
        public SQLHelper? GetSQLHelper()
        {
            foreach (SQLHelperFactoryDelegate d in SQLHelperFactories)
            {
                try
                {
                    if (d.Invoke() is SQLHelper helper)
                    {
                        return helper;
                    }
                }
                catch (Exception e)
                {
                    TXTHelper.AppendErrorLog(e.GetErrorInfo());
                }
            }
            return null;
        }

        internal HashSet<MailSenderFactoryDelegate> MailSenderFactories { get; } = [];

        public delegate MailSender? MailSenderFactoryDelegate();

        /// <summary>
        /// 注册工厂方法 [MailSender]
        /// </summary>
        /// <param name="d"></param>
        public void RegisterFactory(MailSenderFactoryDelegate d)
        {
            MailSenderFactories.Add(d);
        }

        /// <summary>
        /// 构造一个 MailSender 实例
        /// </summary>
        /// <returns></returns>
        public MailSender? GetMailSender()
        {
            foreach (MailSenderFactoryDelegate d in MailSenderFactories)
            {
                try
                {
                    if (d.Invoke() is MailSender sender)
                    {
                        return sender;
                    }
                }
                catch (Exception e)
                {
                    TXTHelper.AppendErrorLog(e.GetErrorInfo());
                }
            }
            return null;
        }

        private readonly static CharacterFactory CharacterFactory = new();
        private readonly static InventoryFactory InventoryFactory = new();
        private readonly static SkillFactory SkillFactory = new();
        private readonly static EffectFactory EffectFactory = new();
        private readonly static ItemFactory ItemFactory = new();
        private readonly static RoomFactory RoomFactory = new();
        private readonly static UserFactory UserFactory = new();

        /// <summary>
        /// 获取角色实例
        /// </summary>
        /// <returns></returns>
        public static Character GetCharacter()
        {
            return CharacterFactory.Create();
        }

        /// <summary>
        /// 获取库存实例
        /// </summary>
        /// <returns></returns>
        public static Inventory GetInventory()
        {
            return InventoryFactory.Create();
        }

        /// <summary>
        /// 获取技能实例
        /// </summary>
        /// <returns></returns>
        public static Skill GetSkill()
        {
            return SkillFactory.Create();
        }

        /// <summary>
        /// 获取技能特效实例
        /// </summary>
        /// <returns></returns>
        public static Effect GetEffect()
        {
            return EffectFactory.Create();
        }

        /// <summary>
        /// 获取物品实例
        /// </summary>
        /// <returns></returns>
        public static Item GetItem()
        {
            return ItemFactory.Create();
        }

        /// <summary>
        /// 获取房间实例
        /// </summary>
        /// <param name="id">房间内部序列号</param>
        /// <param name="roomid">房间号</param>
        /// <param name="createTime">创建时间</param>
        /// <param name="roomMaster">房主</param>
        /// <param name="roomType">房间类型</param>
        /// <param name="gameModule">游戏模组</param>
        /// <param name="gameMap"></param>
        /// <param name="roomState">房间状态</param>
        /// <param name="isRank"></param>
        /// <param name="password">房间密码</param>
        /// <param name="maxUsers">人数上限</param>
        /// <returns></returns>
        public static Room GetRoom(long id = 0, string roomid = "-1", DateTime? createTime = null, User? roomMaster = null, RoomType roomType = RoomType.All, string gameModule = "", string gameMap = "", RoomState roomState = RoomState.Created, bool isRank = false, string password = "", int maxUsers = 4)
        {
            return RoomFactory.Create(id, roomid, createTime, roomMaster, roomType, gameModule, gameMap, roomState, isRank, password, maxUsers);
        }

        /// <summary>
        /// 通过DataSet获取房间实例
        /// </summary>
        /// <param name="drRoom"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Room GetRoom(DataRow drRoom, User user)
        {
            Room room = General.HallInstance;
            if (drRoom != null)
            {
                long id = (long)drRoom[RoomQuery.Column_ID];
                string roomid = (string)drRoom[RoomQuery.Column_Roomid];
                if (!DateTime.TryParse(drRoom[RoomQuery.Column_CreateTime].ToString(), out DateTime createTime))
                {
                    createTime = General.DefaultTime;
                }
                User roomMaster = user;
                RoomType roomType = (RoomType)Convert.ToInt32(drRoom[RoomQuery.Column_RoomType]);
                string gameModule = (string)drRoom[RoomQuery.Column_GameModule];
                string gameMap = (string)drRoom[RoomQuery.Column_GameMap];
                RoomState roomState = (RoomState)Convert.ToInt32(drRoom[RoomQuery.Column_RoomState]);
                bool isRank = Convert.ToInt32(drRoom[RoomQuery.Column_IsRank]) == 1;
                string password = (string)drRoom[RoomQuery.Column_Password];
                int maxUsers = Convert.ToInt32(drRoom[RoomQuery.Column_MaxUsers]);
                room = GetRoom(id, roomid, createTime, roomMaster, roomType, gameModule, gameMap, roomState, isRank, password, maxUsers);
            }
            return room;
        }

        /// <summary>
        /// 通过DataSet获取房间列表
        /// </summary>
        /// <param name="dsRoom"></param>
        /// <param name="dsUser"></param>
        /// <returns></returns>
        public static List<Room> GetRooms(DataSet dsRoom, DataSet dsUser)
        {
            List<Room> list =
            [
                General.HallInstance
            ];
            if (dsRoom != null && dsRoom.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drRoom in dsRoom.Tables[0].Rows)
                {
                    long Id = (long)drRoom[RoomQuery.Column_ID];
                    string Roomid = (string)drRoom[RoomQuery.Column_Roomid];
                    if (!DateTime.TryParse(drRoom[RoomQuery.Column_CreateTime].ToString(), out DateTime createTime))
                    {
                        createTime = General.DefaultTime;
                    }
                    User roomMaster = General.UnknownUserInstance;
                    if (dsUser != null && dsUser.Tables.Count > 0)
                    {
                        DataRow[] rows = dsUser.Tables[0].Select($"{UserQuery.Column_Id} = {(long)drRoom[RoomQuery.Column_RoomMaster]}");
                        if (rows.Length > 0)
                        {
                            roomMaster = GetUser(rows[0]);
                        }
                    }
                    RoomType roomType = (RoomType)Convert.ToInt32(drRoom[RoomQuery.Column_RoomType]);
                    string gameModule = (string)drRoom[RoomQuery.Column_GameModule];
                    string gameMap = (string)drRoom[RoomQuery.Column_GameMap];
                    RoomState roomState = (RoomState)Convert.ToInt32(drRoom[RoomQuery.Column_RoomState]);
                    bool isRank = Convert.ToInt32(drRoom[RoomQuery.Column_IsRank]) == 1;
                    string password = (string)drRoom[RoomQuery.Column_Password];
                    list.Add(GetRoom(Id, Roomid, createTime, roomMaster, roomType, gameModule, gameMap, roomState, isRank, password));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取大厅（-1号房）
        /// </summary>
        /// <returns></returns>
        public static Room GetHall()
        {
            return RoomFactory.Create();
        }

        /// <summary>
        /// 获取用户实例
        /// </summary>
        /// <returns></returns>
        public static User GetUser()
        {
            return UserFactory.Create();
        }

        /// <summary>
        /// 获取用户实例
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Username"></param>
        /// <param name="RegTime"></param>
        /// <param name="LastTime"></param>
        /// <param name="Email"></param>
        /// <param name="NickName"></param>
        /// <param name="IsAdmin"></param>
        /// <param name="IsOperator"></param>
        /// <param name="IsEnable"></param>
        /// <param name="GameTime"></param>
        /// <param name="AutoKey"></param>
        /// <returns></returns>
        public static User GetUser(long Id = 0, string Username = "", DateTime? RegTime = null, DateTime? LastTime = null, string Email = "", string NickName = "", bool IsAdmin = false, bool IsOperator = false, bool IsEnable = true, double GameTime = 0, string AutoKey = "")
        {
            return UserFactory.Create(Id, Username, RegTime, LastTime, Email, NickName, IsAdmin, IsOperator, IsEnable, GameTime, AutoKey);
        }

        /// <summary>
        /// 获取用户实例
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static User GetUser(DataSet ds)
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return GetUser(ds.Tables[0].Rows[0]);
            }
            return UserFactory.Create();
        }

        /// <summary>
        /// 获取用户实例
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static User GetUser(DataRow dr)
        {
            if (dr != null)
            {
                long Id = (long)dr[UserQuery.Column_Id];
                string Username = (string)dr[UserQuery.Column_Username];
                if (!DateTime.TryParse(dr[UserQuery.Column_RegTime].ToString(), out DateTime RegTime))
                {
                    RegTime = General.DefaultTime;
                }
                if (!DateTime.TryParse(dr[UserQuery.Column_LastTime].ToString(), out DateTime LastTime))
                {
                    LastTime = General.DefaultTime;
                }
                string Email = (string)dr[UserQuery.Column_Email];
                string NickName = (string)dr[UserQuery.Column_Nickname];
                bool IsAdmin = Convert.ToInt32(dr[UserQuery.Column_IsAdmin]) == 1;
                bool IsOperator = Convert.ToInt32(dr[UserQuery.Column_IsOperator]) == 1;
                bool IsEnable = Convert.ToInt32(dr[UserQuery.Column_IsEnable]) == 1;
                double GameTime = Convert.ToDouble(dr[UserQuery.Column_GameTime]);
                string AutoKey = (string)dr[UserQuery.Column_AutoKey];
                return UserFactory.Create(Id, Username, RegTime, LastTime, Email, NickName, IsAdmin, IsOperator, IsEnable, GameTime, AutoKey);
            }
            return UserFactory.Create();
        }
    }
}
