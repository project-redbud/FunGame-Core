using System.Net.Mail;
using Milimoe.FunGame.Core.Api.Transmittal;

namespace Milimoe.FunGame.Core.Library.Common.Network
{
    public class MailObject
    {
        public string Sender { get; } = "";
        public string SenderName { get; } = "";
        public string Subject { get; } = "";
        public string Body { get; } = "";
        public MailPriority Priority { get; } = MailPriority.Normal;
        public bool HTML { get; } = false;
        public string[] ToList { get; } = Array.Empty<string>();
        public string[] CCList { get; } = Array.Empty<string>();
        public string[] BCCList { get; } = Array.Empty<string>();

        public MailObject(MailSender Sender, string Subject, string Body, MailPriority Priority, bool HTML, string[] ToList, string[] CCList, string[] BCCList)
        {
            this.Sender = Sender.SmtpClientInfo.SenderMailAddress;
            this.SenderName = Sender.SmtpClientInfo.SenderName;
            this.Subject = Subject;
            this.Body = Body;
            this.Priority = Priority;
            this.HTML = HTML;
            this.ToList = ToList;
            this.CCList = CCList;
            this.BCCList = BCCList;
        }
    }
}
