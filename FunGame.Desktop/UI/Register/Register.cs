using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Base;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Library.Interface;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Register : BaseReg, ISocketCallBack
    {
        public bool CheckReg { get; set; } = false;
        public RegisterEventArgs EventArgs { get; set; } = new RegisterEventArgs();

        private readonly RegisterController RegController;

        public Register()
        {
            InitializeComponent();
            RegController = new RegisterController(this);
        }

        protected override void BindEvent()
        {
            base.BindEvent();
            SucceedReg += SucceedRegEvent;
        }

        private bool Reg_Handler()
        {
            try
            {
                string username = UsernameText.Text.Trim();
                string password = PasswordText.Text.Trim();
                string checkpassword = CheckPasswordText.Text.Trim();
                string email = EmailText.Text.Trim();
                if (username != "")
                {
                    int length = General.DefaultEncoding.GetBytes(username).Length;
                    if (length >= 3 && length <= 12) // 字节范围 3~12
                    {
                        if (password != checkpassword)
                        {
                            ShowMessage.ErrorMessage("两个密码不相同，请重新输入！");
                            CheckPasswordText.Focus();
                            return false;
                        }
                    }
                    else
                    {
                        ShowMessage.ErrorMessage("账号名长度不符合要求：2~6个字符数");
                        UsernameText.Focus();
                        return false;
                    }
                }
                if (password != "")
                {
                    int length = password.Length;
                    if (length < 6 || length > 15) // 字节范围 3~12
                    {
                        ShowMessage.ErrorMessage("密码长度不符合要求：6~15个字符数");
                        PasswordText.Focus();
                        return false;
                    }
                }
                if (username == "" || password == "" || checkpassword == "")
                {
                    ShowMessage.ErrorMessage("请将账号和密码填写完整！");
                    UsernameText.Focus();
                    return false;
                }
                if (email == "")
                {
                    ShowMessage.ErrorMessage("邮箱不能为空！");
                    EmailText.Focus();
                    return false;
                }
                if (!NetworkUtility.IsEmail(email))
                {
                    ShowMessage.ErrorMessage("这不是一个邮箱地址！");
                    EmailText.Focus();
                    return false;
                }
                EventArgs = new RegisterEventArgs(username, password, email);
                if (!RegController.Reg(username, email))
                {
                    ShowMessage.Message("注册失败！", "注册失败");
                    return false;
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
            return true;
        }

        public void SocketHandler(SocketMessageType type, params object[]? objs)
        {
            RegController.SocketHandler(type, objs);
        }

        public void UpdateUI(RegInvokeType type)
        {
            try
            {
                void Action()
                {
                    switch (type)
                    {
                        case RegInvokeType.InputVerifyCode:
                            string username = UsernameText.Text.Trim();
                            string password = PasswordText.Text.Trim();
                            string email = EmailText.Text.Trim();
                            string verifycode = ShowMessage.InputMessageCancel("请输入注册邮件中的6位数字验证码", "注册验证码", out MessageResult cancel);
                            if (cancel != MessageResult.Cancel) RegController.CheckReg(username, password, email, verifycode);
                            else RegButton.Enabled = true;
                            break;
                        case RegInvokeType.DuplicateUserName:
                            ShowMessage.WarningMessage("此账号名已被注册，请使用其他账号名。");
                            RegButton.Enabled = true;
                            break;
                        case RegInvokeType.DuplicateEmail:
                            ShowMessage.WarningMessage("此邮箱已被使用，请使用其他邮箱注册。");
                            RegButton.Enabled = true;
                            break;
                    }
                };
                if (!IsDisposed)
                {
                    if (InvokeRequired) Invoke(Action);
                    else Action();
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private EventResult SucceedRegEvent(object sender, GeneralEventArgs e)
        {
            string username = ((RegisterEventArgs)e).Username;
            string password = ((RegisterEventArgs)e).Password;
            if (LoginController.LoginAccount(username, password)) Close();
            return EventResult.Success;
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            RegButton.Enabled = false;
            if (!Reg_Handler()) RegButton.Enabled = true;
        }

        private void GoToLogin_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
