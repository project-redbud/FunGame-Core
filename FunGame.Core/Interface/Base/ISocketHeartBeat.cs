namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocketHeartBeat
    {
        public int HeartBeatFaileds { get; }
        public bool SendingHeartBeat { get; }
    }
}
