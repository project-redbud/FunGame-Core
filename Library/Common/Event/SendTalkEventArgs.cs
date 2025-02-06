namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class SendTalkEventArgs(string message = "") : GeneralEventArgs
    {
        public string Message { get; set; } = message;
    }
}
