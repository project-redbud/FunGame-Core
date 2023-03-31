namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class RoomEventArgs
    {
        public string RoomID { get; set; } = "";

        public RoomEventArgs(string RoomID = "")
        {
            this.RoomID = RoomID;
        }
    }
}
