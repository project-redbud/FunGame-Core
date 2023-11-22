using System.Net;
using System.Net.Mail;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Service
{
    internal class MailManager
    {
        /// <summary>
        /// 用于保存Smtp客户端
        /// 一个邮件服务对应一个Smtp客户端
        /// </summary>
        internal static Dictionary<Guid, SmtpClient> SmtpClients { get; } = new();

        /// <summary>
        /// 用于保存邮件服务
        /// 允许服务器同时存在多个服务
        /// </summary>
        internal static Dictionary<Guid, MailSender> MailSenders { get; } = new();

        /// <summary>
        /// 获取某个已经保存过的邮件服务
        /// </summary>
        /// <param name="MailSenderID"></param>
        /// <returns></returns>
        internal static MailSender? GetSender(Guid MailSenderID)
        {
            if (MailSenders.TryGetValue(MailSenderID, out MailSender? value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 统一调用此方法发送邮件
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Mail"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        internal static MailSendResult Send(MailSender Sender, MailObject Mail, out string ErrorMsg)
        {
            ErrorMsg = "";
            try
            {
                SmtpClientInfo Info = Sender.SmtpClientInfo;
                SmtpClient Smtp;
                Guid MailSenderID = Sender.MailSenderID;
                if (!SmtpClients.ContainsKey(MailSenderID))
                {
                    Smtp = new()
                    {
                        Host = Info.Host,
                        Port = Info.Port,
                        EnableSsl = Info.OpenSSL,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(Info.SenderMailAddress, Info.SenderPassword)
                    };
                    SmtpClients.Add(MailSenderID, Smtp);
                }
                else Smtp = SmtpClients[MailSenderID];
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
                foreach (string CC in Mail.CCList)
                {
                    if (CC.Trim() != "") Msg.CC.Add(CC);
                }
                foreach (string BCC in Mail.BCCList)
                {
                    if (BCC.Trim() != "") Msg.Bcc.Add(BCC);
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

        /// <summary>
        /// 关闭邮件服务
        /// </summary>
        /// <param name="Sender"></param>
        /// <returns></returns>
        internal static bool Dispose(MailSender Sender)
        {
            try
            {
                Guid MailSenderID = Sender.MailSenderID;
                if (SmtpClients.TryGetValue(MailSenderID, out SmtpClient? value))
                {
                    value.Dispose();
                    SmtpClients.Remove(MailSenderID);
                    MailSenders.Remove(MailSenderID);
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
