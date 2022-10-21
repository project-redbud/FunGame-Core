using Milimoe.FunGame.Core.Entity.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class RoomFactory
    {
        internal static Milimoe.FunGame.Core.Entity.General.Room GetInstanceByRandomID(Milimoe.FunGame.Core.Entity.Enum.RoomType type, Milimoe.FunGame.Core.Entity.General.User? user)
        {
            Milimoe.FunGame.Core.Entity.General.Room room = new(user)
            {
                RoomType = type,
                RoomState = Entity.Enum.RoomState.Created
            };
            return room;
        }

        internal static Milimoe.FunGame.Core.Entity.General.Room GetInstanceByRoomID(Milimoe.FunGame.Core.Entity.Enum.RoomType type, string roomid, Milimoe.FunGame.Core.Entity.General.User? user)
        {
            Milimoe.FunGame.Core.Entity.General.Room room = new(roomid, user)
            {
                RoomType = type,
                RoomState = Entity.Enum.RoomState.Created
            };
            return room;
        }
    }
}
