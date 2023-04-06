using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class RoomEventArgs
    {
        public string RoomID { get; set; } = "";
        public long RoomMaster { get; set; } = 0;
        public RoomType RoomType { get; set; } = RoomType.None;
        public RoomState RoomState { get; set; } = RoomState.Created;
        public bool HasPassword => Password.Trim() != "";
        public string Password { get; set; } = "";

        public RoomEventArgs(string RoomType, string Password)
        {
            this.RoomType = RoomType switch
            {
                GameMode.GameMode_Mix => Constant.RoomType.Mix,
                GameMode.GameMode_Team => Constant.RoomType.Team,
                GameMode.GameMode_MixHasPass => Constant.RoomType.MixHasPass,
                GameMode.GameMode_TeamHasPass => Constant.RoomType.TeamHasPass,
                _ => Constant.RoomType.None
            };
            this.Password = Password;
        }

        public RoomEventArgs(Room Room)
        {
            RoomID = Room.Roomid;
            RoomMaster = Room.RoomMaster != null ? Room.RoomMaster.Id : 0;
            RoomType = Room.RoomType;
            RoomState = Room.RoomState;
            Password = Room.Password;
        }
    }
}
