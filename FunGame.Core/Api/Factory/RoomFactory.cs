using System.Data;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class RoomFactory
    {
        internal static Room GetInstance(DataRow? DrRoom, DataRow? DrUser)
        {
            return new Room(DrRoom, DrUser);
        }
    }
}
