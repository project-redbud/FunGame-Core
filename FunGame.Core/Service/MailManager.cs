using System.Net;
using System.Net.Mail;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Server;

namespace Milimoe.FunGame.Core.Service
{
    internal class MailManager
    {
        internal SmtpClient? SmtpClient { get; }

        internal static MailSendResult Send(MailSender Sender, MailObject Mail)
        {
            SmtpClientInfo Info = Sender.SmtpClientInfo;
            SmtpClient Smtp = new()
            {
                Host = Info.Host,
                Port = Info.Port,
                EnableSsl = Info.OpenSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(Info.SenderMailAddress, Info.SenderPassword)
            };
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
            try
            {
                Smtp.Send(Msg);
                return MailSendResult.Success;
            }
            catch
            {
                return MailSendResult.Fail;
            }
        }
    }
}
