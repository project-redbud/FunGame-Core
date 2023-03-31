using System.Data;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class RoomFactory
    {
        internal static Room GetInstance(DataSet? DsRoom, DataSet? DsUser, int Index = 0)
        {
            return new Room(DsRoom, DsUser, Index);
        }
    }
}
