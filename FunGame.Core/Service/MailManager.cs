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
                    SubjectEncoding = General.DefaultEncoding,
                    Body = Mail.Body,
                    BodyEncoding = General.DefaultEncoding,
                    From = new MailAddress(Mail.Sender, Mail.SenderName, General.DefaultEncoding),
                    IsBodyHtml = Mail.HTML,
                    Priority = Mail.Priority
                };
                foreach (string To in Mail.ToList)
                {
                    Msg.To.Add(To);
                }
                foreach (string CC in Mail.CCList)
                {
                    Msg.CC.Add(CC);
                }
                Smtp.Send(Msg);
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
