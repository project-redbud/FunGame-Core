using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class RoomFactory
    {
        internal static Milimoe.FunGame.Core.Entity.Room GetInstanceByRandomID(RoomType type, Milimoe.FunGame.Core.Entity.User? user)
        {
            Milimoe.FunGame.Core.Entity.Room room = new(user)
            {
                RoomType = type,
                RoomState = RoomState.Created
            };
            return room;
        }

        internal static Milimoe.FunGame.Core.Entity.Room GetInstanceByRoomID(RoomType type, string roomid, Milimoe.FunGame.Core.Entity.User? user)
        {
            Milimoe.FunGame.Core.Entity.Room room = new(roomid, user)
            {
                RoomType = type,
                RoomState = RoomState.Created
            };
            return room;
        }
    }
}
