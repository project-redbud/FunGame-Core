using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class GamingEventArgs(Room room, List<User> users, Dictionary<string, Character>? characters = null) : GeneralEventArgs
    {
        public Room Room { get; } = room;
        public List<User> Users { get; } = users;
        public Dictionary<string, Character> Characters { get; } = characters ?? [];
    }
}
