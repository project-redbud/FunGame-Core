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
        internal static Dictionary<Guid, SmtpClient> SmtpClients { get; } = [];

        /// <summary>
        /// 用于保存邮件服务
        /// 允许服务器同时存在多个服务
        /// </summary>
        internal static Dictionary<Guid, MailSender> MailSenders { get; } = [];

        /// <summary>
        /// 获取某个已经保存过的邮件服务
        /// </summary>
        /// <param name="mailSenderID"></param>
        /// <returns></returns>
        internal static MailSender? GetSender(Guid mailSenderID)
        {
            if (MailSenders.TryGetValue(mailSenderID, out MailSender? value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 统一调用此方法发送邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mail"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        internal static MailSendResult Send(MailSender sender, MailObject mail, out string errorMsg)
        {
            errorMsg = "";
            try
            {
                SmtpClientInfo info = sender.SmtpClientInfo;
                SmtpClient smtp;
                Guid senderID = sender.MailSenderID;
                if (!SmtpClients.TryGetValue(senderID, out SmtpClient? value))
                {
                    smtp = new()
                    {
                        Host = info.Host,
                        Port = info.Port,
                        EnableSsl = info.SSL,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(info.SenderMailAddress, info.SenderPassword)
                    };
                    SmtpClients.Add(senderID, smtp);
                }
                else smtp = value;
                MailMessage Msg = new()
                {
                    Subject = mail.Subject,
                    SubjectEncoding = General.DefaultEncoding,
                    Body = mail.Body,
                    BodyEncoding = General.DefaultEncoding,
                    From = new MailAddress(mail.Sender, mail.SenderName, General.DefaultEncoding),
                    IsBodyHtml = mail.HTML,
                    Priority = mail.Priority
                };
                foreach (string To in mail.ToList)
                {
                    if (To.Trim() != "") Msg.To.Add(To);
                }
                foreach (string CC in mail.CCList)
                {
                    if (CC.Trim() != "") Msg.CC.Add(CC);
                }
                foreach (string BCC in mail.BCCList)
                {
                    if (BCC.Trim() != "") Msg.Bcc.Add(BCC);
                }
                smtp.SendMailAsync(Msg);
                return MailSendResult.Success;
            }
            catch (Exception e)
            {
                errorMsg = e.GetErrorInfo();
                Api.Utility.TXTHelper.AppendErrorLog(errorMsg);
                return MailSendResult.Fail;
            }
        }

        /// <summary>
        /// 关闭邮件服务
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        internal static bool Dispose(MailSender sender)
        {
            try
            {
                Guid senderID = sender.MailSenderID;
                if (SmtpClients.TryGetValue(senderID, out SmtpClient? value))
                {
                    value.Dispose();
                    SmtpClients.Remove(senderID);
                    MailSenders.Remove(senderID);
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
