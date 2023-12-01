using System.Data;
using Milimoe.FunGame.Core.Api.Factory;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class Factory
    {
        private readonly static CharacterFactory CharacterFactory = new();
        private readonly static InventoryFactory InventoryFactory = new();
        private readonly static ItemFactory ItemFactory = new();
        private readonly static RoomFactory RoomFactory = new();
        private readonly static SkillFactory SkillFactory = new();
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
        /// 获取物品实例，默认返回Passiveitem 被动物品 需要强制转换
        /// </summary>
        /// <param name="type">Item类型 主动 或 被动</param>
        /// <returns></returns>
        public static Item GetItem(ItemType type = ItemType.Passive)
        {
            return ItemFactory.Create(type);
        }

        /// <summary>
        /// 获取主动物品实例
        /// </summary>
        /// <returns></returns>
        public static ActiveItem GetActiveItem()
        {
            return (ActiveItem)ItemFactory.Create(ItemType.Active);
        }

        /// <summary>
        /// 获取被动物品实例
        /// </summary>
        /// <returns></returns>
        public static PassiveItem GetPassiveItem()
        {
            return (PassiveItem)ItemFactory.Create(ItemType.Passive);
        }

        /// <summary>
        /// 获取房间实例
        /// </summary>
        /// <param name="Id">房间内部序列号</param>
        /// <param name="Roomid">房间号</param>
        /// <param name="CreateTime">创建时间</param>
        /// <param name="RoomMaster">房主</param>
        /// <param name="RoomType">房间类型</param>
        /// <param name="GameMode">游戏模组</param>
        /// <param name="GameMap"></param>
        /// <param name="RoomState">房间状态</param>
        /// <param name="IsRank"></param>
        /// <param name="Password">房间密码</param>
        /// <returns></returns>
        public static Room GetRoom(long Id = 0, string Roomid = "-1", DateTime? CreateTime = null, User? RoomMaster = null, RoomType RoomType = RoomType.All, string GameMode = "", string GameMap = "", RoomState RoomState = RoomState.Created, bool IsRank = false, string Password = "")
        {
            return RoomFactory.Create(Id, Roomid, CreateTime, RoomMaster, RoomType, GameMode, GameMap, RoomState, IsRank, Password);
        }

        /// <summary>
        /// 通过DataSet获取房间实例
        /// </summary>
        /// <param name="DrRoom"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static Room GetRoom(DataRow DrRoom, User User)
        {
            Room room = General.HallInstance;
            if (DrRoom != null)
            {
                long Id = (long)DrRoom[RoomQuery.Column_ID];
                string Roomid = (string)DrRoom[RoomQuery.Column_RoomID];
                DateTime CreateTime = (DateTime)DrRoom[RoomQuery.Column_CreateTime];
                User RoomMaster = User;
                RoomType RoomType = (RoomType)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomType]);
                string GameMode = (string)DrRoom[RoomQuery.Column_GameMode];
                string GameMap = (string)DrRoom[RoomQuery.Column_GameMap];
                RoomState RoomState = (RoomState)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomState]);
                bool IsRank = Convert.ToInt32(DrRoom[RoomQuery.Column_IsRank]) == 1;
                string Password = (string)DrRoom[RoomQuery.Column_Password];
                room = GetRoom(Id, Roomid, CreateTime, RoomMaster, RoomType, GameMode, GameMap, RoomState, IsRank, Password);
            }
            return room;
        }

        /// <summary>
        /// 通过DataSet获取房间列表
        /// </summary>
        /// <param name="DsRoom"></param>
        /// <param name="DsUser"></param>
        /// <returns></returns>
        public static List<Room> GetRooms(DataSet DsRoom, DataSet DsUser)
        {
            List<Room> list =
            [
                General.HallInstance
            ];
            if (DsRoom != null && DsRoom.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow DrRoom in DsRoom.Tables[0].Rows)
                {
                    long Id = (long)DrRoom[RoomQuery.Column_ID];
                    string Roomid = (string)DrRoom[RoomQuery.Column_RoomID];
                    DateTime CreateTime = (DateTime)DrRoom[RoomQuery.Column_CreateTime];
                    User RoomMaster = General.UnknownUserInstance;
                    if (DsUser != null && DsUser.Tables.Count > 0)
                    {
                        DataRow[] rows = DsUser.Tables[0].Select($"{UserQuery.Column_UID} = {(long)DrRoom[RoomQuery.Column_RoomMaster]}");
                        if (rows.Length > 0)
                        {
                            RoomMaster = GetUser(rows[0]);
                        }
                    }
                    RoomType RoomType = (RoomType)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomType]);
                    string GameMode = (string)DrRoom[RoomQuery.Column_GameMode];
                    string GameMap = (string)DrRoom[RoomQuery.Column_GameMap];
                    RoomState RoomState = (RoomState)Convert.ToInt32(DrRoom[RoomQuery.Column_RoomState]);
                    bool IsRank = Convert.ToInt32(DrRoom[RoomQuery.Column_IsRank]) == 1;
                    string Password = (string)DrRoom[RoomQuery.Column_Password];
                    list.Add(GetRoom(Id, Roomid, CreateTime, RoomMaster, RoomType, GameMode, GameMap, RoomState, IsRank, Password));
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
        /// 获取技能实例，默认返回PassiveSkill 被动技能 需要强制转换
        /// </summary>
        /// <param name="type">Skill类型 主动 或 被动</param>
        /// <returns></returns>
        public static Skill GetSkill(SkillType type = SkillType.Passive)
        {
            return SkillFactory.Create(type);
        }

        /// <summary>
        /// 获取主动技能实例
        /// </summary>
        /// <returns></returns>
        public static ActiveSkill GetActiveSkill()
        {
            return (ActiveSkill)SkillFactory.Create(SkillType.Active);
        }

        /// <summary>
        /// 获取被动技能实例
        /// </summary>
        /// <returns></returns>
        public static PassiveSkill GetPassiveSkill()
        {
            return (PassiveSkill)SkillFactory.Create(SkillType.Passive);
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
        public static User GetUser(long Id = 0, string Username = "", DateTime? RegTime = null, DateTime? LastTime = null, string Email = "", string NickName = "", bool IsAdmin = false, bool IsOperator = false, bool IsEnable = true, decimal Credits = 0, decimal Materials = 0, decimal GameTime = 0, string AutoKey = "")
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
                decimal Credits = Convert.ToDecimal(dr[UserQuery.Column_Credits]);
                decimal Materials = Convert.ToDecimal(dr[UserQuery.Column_Materials]);
                decimal GameTime = Convert.ToDecimal(dr[UserQuery.Column_GameTime]);
                string AutoKey = (string)dr[UserQuery.Column_AutoKey];
                return UserFactory.Create(Id, Username, RegTime, LastTime, Email, NickName, IsAdmin, IsOperator, IsEnable, Credits, Materials, GameTime, AutoKey);
            }
            return UserFactory.Create();
        }
    }
}
