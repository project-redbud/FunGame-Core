using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Model
{
    public class SmtpClientInfo : IMailSender
    {
        public string Host => _Host;
        public int Port => _Port;
        public bool OpenSSL => _OpenSSL;
        public string SenderMailAddress => _SenderMailAddress;
        public string SenderName => _SenderName;
        public string SenderPassword => _SenderPassword;

        private string _Host = "";
        private int _Port = 587;
        private bool _OpenSSL = true;
        private string _SenderMailAddress = "";
        private string _SenderName = "";
        private string _SenderPassword = "";

        internal SmtpClientInfo(string SenderMailAddress, string SenderName, string SenderPassword, string Host, int Port, bool OpenSSL)
        {
            _Host = Host;
            _Port = Port;
            _OpenSSL = OpenSSL;
            _SenderMailAddress = SenderMailAddress;
            _SenderName = SenderName;
            _SenderPassword = SenderPassword;
        }
    }
}
