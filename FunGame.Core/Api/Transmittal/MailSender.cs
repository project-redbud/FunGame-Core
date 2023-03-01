using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Server;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    public class MailSender
    {
        public SmtpClientInfo SmtpClientInfo => _SmtpClientInfo;
        public MailSendResult LastestResult => _LastestResult;

        private SmtpClientInfo _SmtpClientInfo;
        private MailSendResult _LastestResult = MailSendResult.NotSend;

        public MailSender(string SenderMailAddress, string SenderName, string SenderPassword, string Host, int Port, bool OpenSSL)
        {
            _SmtpClientInfo = new SmtpClientInfo(SenderMailAddress, SenderName, SenderPassword, Host, Port, OpenSSL);
        }

        public MailSendResult Send(MailObject Mail)
        {
            _LastestResult = MailManager.Send(this, Mail);
            return _LastestResult;
        }
    }
}
