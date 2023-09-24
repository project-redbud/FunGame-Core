namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class SendTalkEventArgs : GeneralEventArgs
    {
        public string Message { get; set; } = "";

        public SendTalkEventArgs(string message = "")
        {
            this.Message = message;
        }
    }
}
