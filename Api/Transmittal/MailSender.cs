using System.Net.Mail;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
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
        public SmtpClientInfo SmtpClientInfo => _SmtpClientInfo;

        /// <summary>
        /// 上一个邮件发送的结果
        /// </summary>
        public MailSendResult LastestResult => _LastestResult;

        /// <summary>
        /// 上一个邮件的发送错误信息（如果发送失败）
        /// </summary>
        public string ErrorMsg => _ErrorMsg;

        /**
         * 内部变量
         */
        private readonly SmtpClientInfo _SmtpClientInfo;
        private MailSendResult _LastestResult = MailSendResult.NotSend;
        private string _ErrorMsg = "";

        /// <summary>
        /// 创建邮件服务
        /// </summary>
        /// <param name="SenderMailAddress"></param>
        /// <param name="SenderName"></param>
        /// <param name="SenderPassword"></param>
        /// <param name="Host"></param>
        /// <param name="Port"></param>
        /// <param name="OpenSSL"></param>
        public MailSender(string SenderMailAddress, string SenderName, string SenderPassword, string Host, int Port, bool OpenSSL)
        {
            MailSenderID = Guid.NewGuid();
            _SmtpClientInfo = new SmtpClientInfo(SenderMailAddress, SenderName, SenderPassword, Host, Port, OpenSSL);
            if (!MailManager.MailSenders.ContainsKey(MailSenderID)) MailManager.MailSenders.Add(MailSenderID, this);
        }

        /// <summary>
        /// 创建完整邮件对象
        /// </summary>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="Priority"></param>
        /// <param name="HTML"></param>
        /// <param name="ToList"></param>
        /// <param name="CCList"></param>
        /// <param name="BCCList"></param>
        /// <returns></returns>
        public MailObject CreateMail(string Subject, string Body, MailPriority Priority, bool HTML, string[] ToList, string[]? CCList = null, string[]? BCCList = null)
        {
            return new MailObject(this, Subject, Body, Priority, HTML, ToList, CCList, BCCList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="Mail"></param>
        /// <returns></returns>
        public MailSendResult Send(MailObject Mail)
        {
            _LastestResult = MailManager.Send(this, Mail, out _ErrorMsg);
            return _LastestResult;
        }

        private bool IsDisposed = false;

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
        /// <param name="Disposing"></param>
        protected void Dispose(bool Disposing)
        {
            if (!IsDisposed)
            {
                if (Disposing)
                {
                    MailManager.Dispose(this);
                }
            }
            IsDisposed = true;
        }
    }
}
