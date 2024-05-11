namespace Milimoe.FunGame.Core.Interface.Sockets
{
    public interface ISocketHeartBeat
    {
        public int HeartBeatFaileds { get; }
        public bool SendingHeartBeat { get; }
    }
}
