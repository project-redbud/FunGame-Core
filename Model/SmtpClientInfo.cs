using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Model
{
    public class SmtpClientInfo : IMailSender
    {
        public string SenderMailAddress { get; set; } = "";
        public string SenderName { get; set; } = "";
        public string SenderPassword { get; set; } = "";
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool SSL { get; set; } = false;

        internal SmtpClientInfo(string senderMailAddress, string senderName, string senderPassword, string host, int port, bool ssl)
        {
            SenderMailAddress = senderMailAddress;
            SenderName = senderName;
            SenderPassword = senderPassword;
            Host = host;
            Port = port;
            SSL = ssl;
        }
    }
}
