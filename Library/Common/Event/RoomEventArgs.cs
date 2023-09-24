using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class RoomEventArgs : GeneralEventArgs
    {
        public Room Room { get; set; } = General.HallInstance;
        public string RoomID { get; set; } = "";
        public long RoomMaster { get; set; } = 0;
        public string RoomTypeString { get; set; } = GameMode.All;
        public RoomType RoomType { get; set; } = RoomType.All;
        public RoomState RoomState { get; set; } = RoomState.Created;
        public bool HasPassword => Password.Trim() != "";
        public string Password { get; set; } = "";

        public RoomEventArgs(string RoomType, string Password)
        {
            RoomTypeString = RoomType;
            this.RoomType = RoomType switch
            {
                GameMode.Mix => Constant.RoomType.Mix,
                GameMode.Team => Constant.RoomType.Team,
                GameMode.MixHasPass => Constant.RoomType.MixHasPass,
                GameMode.TeamHasPass => Constant.RoomType.TeamHasPass,
                GameMode.AllHasPass => Constant.RoomType.AllHasPass,
                _ => Constant.RoomType.All
            };
            this.Password = Password;
            Room = Factory.GetRoom(RoomType: this.RoomType, Password: this.Password);
        }

        public RoomEventArgs(Room Room)
        {
            this.Room = Room;
            RoomID = Room.Roomid;
            RoomMaster = Room.RoomMaster != null ? Room.RoomMaster.Id : 0;
            RoomType = Room.RoomType;
            RoomTypeString = Room.RoomType switch
            {
                RoomType.Mix => GameMode.Mix,
                RoomType.Team => GameMode.Team,
                RoomType.MixHasPass => GameMode.MixHasPass,
                RoomType.TeamHasPass => GameMode.TeamHasPass,
                RoomType.AllHasPass => GameMode.AllHasPass,
                _ => GameMode.All
            };
            RoomState = Room.RoomState;
            Password = Room.Password;
        }
    }
}
