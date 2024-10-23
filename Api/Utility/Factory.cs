using System.Data;
using Milimoe.FunGame.Core.Api.Factory;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
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

        public delegate T? EntityFactoryDelegate<T>(Dictionary<string, object> args);

        /// <summary>
        /// 注册工厂方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factoryDelegate"></param>
        public void RegisterFactory<T>(EntityFactoryDelegate<T> factoryDelegate)
        {
            if (typeof(T) == typeof(Character) && factoryDelegate is EntityFactoryDelegate<Character> character)
            {
                CharacterFactories.Add(character);
            }
            if (typeof(T) == typeof(Inventory) && factoryDelegate is EntityFactoryDelegate<Inventory> inventory)
            {
                InventoryFactories.Add(inventory);
            }
            if (typeof(T) == typeof(Skill) && factoryDelegate is EntityFactoryDelegate<Skill> skill)
            {
                SkillFactories.Add(skill);
            }
            if (typeof(T) == typeof(Effect) && factoryDelegate is EntityFactoryDelegate<Effect> effect)
            {
                EffectFactories.Add(effect);
            }
            if (typeof(T) == typeof(Item) && factoryDelegate is EntityFactoryDelegate<Item> item)
            {
                ItemFactories.Add(item);
            }
            if (typeof(T) == typeof(Room) && factoryDelegate is EntityFactoryDelegate<Room> room)
            {
                RoomFactories.Add(room);
            }
            if (typeof(T) == typeof(User) && factoryDelegate is EntityFactoryDelegate<User> user)
            {
                UserFactories.Add(user);
            }
        }

        /// <summary>
        /// 构造一个实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedInstanceClassException"></exception>
        public T GetInstance<T>(Dictionary<string, object> args)
        {
            if (typeof(T) == typeof(Character))
            {
                foreach (EntityFactoryDelegate<Character> d in CharacterFactories)
                {
                    if (d.Invoke(args) is T character)
                    {
                        return character;
                    }
                }
            }
            if (typeof(T) == typeof(Inventory))
            {
                foreach (EntityFactoryDelegate<Inventory> d in InventoryFactories)
                {
                    if (d.Invoke(args) is T inventory)
                    {
                        return inventory;
                    }
                }
            }
            if (typeof(T) == typeof(Skill))
            {
                foreach (EntityFactoryDelegate<Skill> d in SkillFactories)
                {
                    if (d.Invoke(args) is T skill)
                    {
                        return skill;
                    }
                }
            }
            if (typeof(T) == typeof(Effect))
            {
                foreach (EntityFactoryDelegate<Effect> d in EffectFactories)
                {
                    if (d.Invoke(args) is T effect)
                    {
                        return effect;
                    }
                }
            }
            if (typeof(T) == typeof(Item))
            {
                foreach (EntityFactoryDelegate<Item> d in ItemFactories)
                {
                    if (d.Invoke(args) is T item)
                    {
                        return item;
                    }
                }
            }
            if (typeof(T) == typeof(Room))
            {
                foreach (EntityFactoryDelegate<Room> d in RoomFactories)
                {
                    if (d.Invoke(args) is T room)
                    {
                        return room;
                    }
                }
            }
            if (typeof(T) == typeof(User))
            {
                foreach (EntityFactoryDelegate<User> d in UserFactories)
                {
                    if (d.Invoke(args) is T user)
                    {
                        return user;
                    }
                }
            }
            throw new NotSupportedInstanceClassException();
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
                string roomid = (string)drRoom[RoomQuery.Column_RoomID];
                DateTime createTime = (DateTime)drRoom[RoomQuery.Column_CreateTime];
                User roomMaster = user;
                RoomType roomType = (RoomType)Convert.ToInt32(drRoom[RoomQuery.Column_RoomType]);
                string gameModule = (string)drRoom[RoomQuery.Column_GameModule];
                string gameMap = (string)drRoom[RoomQuery.Column_GameMap];
                RoomState roomState = (RoomState)Convert.ToInt32(drRoom[RoomQuery.Column_RoomState]);
                bool isRank = Convert.ToInt32(drRoom[RoomQuery.Column_IsRank]) == 1;
                string password = (string)drRoom[RoomQuery.Column_Password];
                int maxUsers = (int)drRoom[RoomQuery.Column_MaxUsers];
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
                    string Roomid = (string)drRoom[RoomQuery.Column_RoomID];
                    DateTime createTime = (DateTime)drRoom[RoomQuery.Column_CreateTime];
                    User roomMaster = General.UnknownUserInstance;
                    if (dsUser != null && dsUser.Tables.Count > 0)
                    {
                        DataRow[] rows = dsUser.Tables[0].Select($"{UserQuery.Column_UID} = {(long)drRoom[RoomQuery.Column_RoomMaster]}");
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
        internal static Room GetHall()
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
        /// <param name="Credits"></param>
        /// <param name="Materials"></param>
        /// <param name="GameTime"></param>
        /// <param name="AutoKey"></param>
        /// <returns></returns>
        public static User GetUser(long Id = 0, string Username = "", DateTime? RegTime = null, DateTime? LastTime = null, string Email = "", string NickName = "", bool IsAdmin = false, bool IsOperator = false, bool IsEnable = true, double Credits = 0, double Materials = 0, double GameTime = 0, string AutoKey = "")
        {
            return UserFactory.Create(Id, Username, RegTime, LastTime, Email, NickName, IsAdmin, IsOperator, IsEnable, Credits, Materials, GameTime, AutoKey);
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
                long Id = (long)dr[UserQuery.Column_UID];
                string Username = (string)dr[UserQuery.Column_Username];
                DateTime RegTime = (DateTime)dr[UserQuery.Column_RegTime];
                DateTime LastTime = (DateTime)dr[UserQuery.Column_LastTime];
                string Email = (string)dr[UserQuery.Column_Email];
                string NickName = (string)dr[UserQuery.Column_Nickname];
                bool IsAdmin = Convert.ToInt32(dr[UserQuery.Column_IsAdmin]) == 1;
                bool IsOperator = Convert.ToInt32(dr[UserQuery.Column_IsOperator]) == 1;
                bool IsEnable = Convert.ToInt32(dr[UserQuery.Column_IsEnable]) == 1;
                double Credits = Convert.ToDouble(dr[UserQuery.Column_Credits]);
                double Materials = Convert.ToDouble(dr[UserQuery.Column_Materials]);
                double GameTime = Convert.ToDouble(dr[UserQuery.Column_GameTime]);
                string AutoKey = (string)dr[UserQuery.Column_AutoKey];
                return UserFactory.Create(Id, Username, RegTime, LastTime, Email, NickName, IsAdmin, IsOperator, IsEnable, Credits, Materials, GameTime, AutoKey);
            }
            return UserFactory.Create();
        }
    }
}
