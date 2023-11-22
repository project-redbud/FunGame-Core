namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface IMailSender
    {
        public string SenderMailAddress { get; }
        public string SenderName { get; }
        public string SenderPassword { get; }
    }
}
