using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.EntityFactory
{
    internal class RoomFactory : IFactory<Room>
    {
        public Type EntityType => typeof(Room);

        public Room Create()
        {
            return RoomFactory.Create();
        }

        public static Room Create(long id = 0, string roomid = "-1", DateTime? createTime = null, User? roomMaster = null, RoomType roomType = RoomType.All, string gameModule = "", string gameMap = "", RoomState roomState = RoomState.Created, bool isRank = false, string password = "", int maxUsers = 4)
        {
            return new Room(id, roomid, createTime, roomMaster, roomType, gameModule, gameMap, roomState, isRank, password, maxUsers);
        }
    }
}
