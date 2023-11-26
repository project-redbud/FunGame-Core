using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class GamingEventArgs(Room room, List<User> users, List<Character> characters) : GeneralEventArgs
    {
        public Room Room { get; } = room;
        public List<User> Users { get; } = users;
        public List<Character> Characters { get; } = characters;
    }
}
