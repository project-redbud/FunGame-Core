using System.Data;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class RoomFactory
    {
        internal static Room GetInstance(DataSet? DsRoom, DataSet? DsUser)
        {
            return new Room(DsRoom, DsUser);
        }
    }
}
