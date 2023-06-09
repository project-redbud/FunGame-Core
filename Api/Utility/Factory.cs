using Milimoe.FunGame.Core.Api.Factory;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

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
        /// <param name="RoomState">房间状态</param>
        /// <param name="Password">房间密码</param>
        /// <returns></returns>
        public static Room GetRoom(long Id = 0, string Roomid = "-1", DateTime? CreateTime = null, User? RoomMaster = null, RoomType RoomType = RoomType.None, RoomState RoomState = RoomState.Created, string Password = "")
        {
            return RoomFactory.Create(Id, Roomid, CreateTime, RoomMaster, RoomType, RoomState, Password);
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
    }
}
