using System.Collections;
using System.Net;
using System.Net.Mail;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.Server;

namespace Milimoe.FunGame.Core.Service
{
    internal class MailManager
    {
        internal static Hashtable HashClient { get; } = new Hashtable();

        internal static MailSendResult Send(MailSender Sender, MailObject Mail, out string ErrorMsg)
        {
            ErrorMsg = "";
            try
            {
                SmtpClientInfo Info = Sender.SmtpClientInfo;
                SmtpClient Smtp;
                Guid MailSenderID = Sender.MailSenderID;
                if (!HashClient.ContainsKey(MailSenderID))
                {
                    Smtp = new()
                    {
                        Host = Info.Host,
                        Port = Info.Port,
                        EnableSsl = Info.OpenSSL,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(Info.SenderMailAddress, Info.SenderPassword)
                    };
                    HashClient.Add(MailSenderID, Smtp);
                }
                else Smtp = (SmtpClient)HashClient[MailSenderID]!;
                MailMessage Msg = new()
                {
                    Subject = Mail.Subject,
                    SubjectEncoding = System.Text.Encoding.UTF8,
                    Body = Mail.Body,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    From = new MailAddress(Mail.Sender, Mail.SenderName, System.Text.Encoding.UTF8),
                    IsBodyHtml = Mail.HTML,
                    Priority = Mail.Priority
                };
                foreach (string To in Mail.ToList)
                {
                    if (To.Trim() != "") Msg.To.Add(To);
                }
                if (Mail.CCList != null)
                {
                    foreach (string CC in Mail.CCList)
                    {
                        if (CC.Trim() != "") Msg.CC.Add(CC);
                    }
                }
                if (Mail.BCCList != null)
                {
                    foreach (string BCC in Mail.BCCList)
                    {
                        if (BCC.Trim() != "") Msg.Bcc.Add(BCC);
                    }
                }
                Smtp.SendMailAsync(Msg);
                return MailSendResult.Success;
            }
            catch (Exception e)
            {
                ErrorMsg = e.GetErrorInfo();
                return MailSendResult.Fail;
            }
        }

        internal static bool Dispose(MailSender Sender)
        {
            try
            {
                Guid MailSenderID = Sender.MailSenderID;
                if (HashClient.ContainsKey(MailSenderID))
                {
                    ((SmtpClient)HashClient[MailSenderID]!).Dispose();
                    HashClient.Remove(MailSenderID);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
