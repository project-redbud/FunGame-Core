using System.Net.Mail;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Core.Service;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    public class MailSender : IDisposable
    {
        /// <summary>
        /// 邮件服务内部ID
        /// </summary>
        public Guid MailSenderID { get; }

        /// <summary>
        /// Smtp客户端信息
        /// </summary>
        public SmtpClientInfo SmtpClientInfo { get; private set; }

        /// <summary>
        /// 上一个邮件发送的结果
        /// </summary>
        public MailSendResult LastestResult { get; private set; } = MailSendResult.NotSend;

        /// <summary>
        /// 上一个邮件的发送错误信息（如果发送失败）
        /// </summary>
        public string ErrorMsg { get; private set; } = "";

        /// <summary>
        /// 创建邮件服务
        /// </summary>
        /// <param name="senderMailAddress"></param>
        /// <param name="senderName"></param>
        /// <param name="senderPassword"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="ssl"></param>
        public MailSender(string senderMailAddress, string senderName, string senderPassword, string host, int port, bool ssl)
        {
            MailSenderID = Guid.NewGuid();
            SmtpClientInfo = new SmtpClientInfo(senderMailAddress, senderName, senderPassword, host, port, ssl);
            if (!MailManager.MailSenders.ContainsKey(MailSenderID)) MailManager.MailSenders.Add(MailSenderID, this);
        }

        /// <summary>
        /// 创建完整邮件对象
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="priority"></param>
        /// <param name="html"></param>
        /// <param name="toList"></param>
        /// <param name="ccList"></param>
        /// <param name="bccList"></param>
        /// <returns></returns>
        public MailObject CreateMail(string subject, string body, MailPriority priority, bool html, string[] toList, string[]? ccList = null, string[]? bccList = null)
        {
            return new MailObject(this, subject, body, priority, html, toList, ccList, bccList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public MailSendResult Send(MailObject mail)
        {
            LastestResult = MailManager.Send(this, mail, out string errorMsg);
            if (!string.IsNullOrWhiteSpace(errorMsg)) ErrorMsg = errorMsg;
            return LastestResult;
        }

        private bool _isDisposed = false;

        /// <summary>
        /// 关闭邮件服务
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 关闭邮件服务
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    MailManager.Dispose(this);
                }
            }
            _isDisposed = true;
        }
    }
}
