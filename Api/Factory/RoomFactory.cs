using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class RoomFactory : IFactory<Room>
    {
        public Type EntityType => typeof(Room);

        public Room Create()
        {
            return RoomFactory.Create();
        }

        public static Room Create(long Id = 0, string Roomid = "-1", DateTime? CreateTime = null, User? RoomMaster = null, RoomType RoomType = RoomType.None, RoomState RoomState = RoomState.Created, string Password = "")
        {
            return new Room(Id, Roomid, CreateTime, RoomMaster, RoomType, RoomState, Password);
        }
    }
}
