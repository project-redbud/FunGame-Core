using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class GamingEventArgs(Room room, List<User> users, params object[] objs) : GeneralEventArgs(objs)
    {
        public Room Room { get; } = room;
        public List<User> Users { get; } = users;
    }
}
