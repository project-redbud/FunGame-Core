namespace Milimoe.FunGame.Core.Library.Constant
{
    public enum StartMatchState
    {
        Matching,
        Success,
        Enable,
        Cancel
    }

    public enum CreateRoomState
    {
        Creating,
        Success
    }

    public enum RoomState
    {
        Created,
        Gaming,
        Close,
        Complete
    }

    public enum OnlineState
    {
        Offline,
        Online,
        Matching,
        InRoom,
        Gaming
    }

    public enum ClientState
    {
        Online,
        WaitConnect,
        WaitLogin
    }
}
