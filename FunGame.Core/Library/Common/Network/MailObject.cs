using System.Net.Mail;

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

        public MailObject(string Sender, string SenderName, string Subject, string Body, MailPriority Priority, bool HTML, string[] ToList, string[] CcList)
        {
            this.Sender = Sender;
            this.SenderName = SenderName;
            this.Subject = Subject;
            this.Body = Body;
            this.Priority = Priority;
            this.HTML = HTML;
            this.ToList = ToList;
            this.CCList = CcList;
        }
    }
}
