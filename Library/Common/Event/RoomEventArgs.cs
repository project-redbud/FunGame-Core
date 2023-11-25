using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class RoomEventArgs : GeneralEventArgs
    {
        public Room Room { get; set; } = General.HallInstance;
        public string RoomID { get; set; } = "";
        public long RoomMaster { get; set; } = 0;
        public string RoomTypeString { get; set; } = RoomSet.All;
        public RoomType RoomType { get; set; } = RoomType.All;
        public GameMode GameMode { get; set; }
        public GameMap GameMap => GameMode.Map;
        public RoomState RoomState { get; set; } = RoomState.Created;
        public bool HasPassword => Password.Trim() != "";
        public string Password { get; set; } = "";

        public RoomEventArgs(string type, string password)
        {
            RoomTypeString = type;
            RoomType = type switch
            {
                RoomSet.Mix => RoomType.Mix,
                RoomSet.Team => RoomType.Team,
                RoomSet.FastAuto => RoomType.FastAuto,
                RoomSet.Custom => RoomType.Custom,
                _ => RoomType.All
            };
            GameMode = RoomSet.GetGameMode(RoomType);
            Password = password;
            Room = Factory.GetRoom(RoomType: RoomType, Password: Password);
        }

        public RoomEventArgs(Room room)
        {
            Room = room;
            RoomID = room.Roomid;
            RoomMaster = room.RoomMaster != null ? room.RoomMaster.Id : 0;
            RoomType = room.RoomType;
            GameMode = room.GameMode;
            RoomTypeString = room.RoomType switch
            {
                RoomType.Mix => RoomSet.Mix,
                RoomType.Team => RoomSet.Team,
                RoomType.FastAuto => RoomSet.FastAuto,
                RoomType.Custom => RoomSet.Custom,
                _ => RoomSet.All
            };
            RoomState = room.RoomState;
            Password = room.Password;
        }
    }
}
