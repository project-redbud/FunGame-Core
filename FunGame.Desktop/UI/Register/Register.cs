using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Base;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Register : BaseReg
    {
        public bool CheckReg { get; set; } = false;

        private readonly RegisterController RegController;

        public Register()
        {
            InitializeComponent();
            RegController = new RegisterController(this);
        }

        protected override void BindEvent()
        {
            base.BindEvent();
            Disposed += Register_Disposed;
            SucceedReg += SucceedRegEvent;
        }

        private void Register_Disposed(object? sender, EventArgs e)
        {
            RegController.Dispose();
        }

        private async Task<bool> Reg_Handler()
        {
            try
            {
                string username = UsernameText.Text.Trim();
                string password = PasswordText.Text.Trim();
                string checkpassword = CheckPasswordText.Text.Trim();
                string email = EmailText.Text.Trim();
                if (username != "")
                {
                    if (NetworkUtility.IsUserName(username))
                    {
                        int length = NetworkUtility.GetUserNameLength(username);
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
                            ShowMessage.ErrorMessage("账号名长度不符合要求：最多6个中文字符或12个英文字符");
                            UsernameText.Focus();
                            return false;
                        }
                    }
                    else
                    {
                        ShowMessage.ErrorMessage("账号名不符合要求：不能包含特殊字符");
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
                return await RegController.Reg(username, password, email);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
                return false;
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
            _ = LoginController.LoginAccount(username, password);
            RunTime.Login?.Close();
            return EventResult.Success;
        }

        private async void RegButton_Click(object sender, EventArgs e)
        {
            RegButton.Enabled = false;
            if (!await Reg_Handler()) RegButton.Enabled = true;
            else Dispose();
        }

        private void GoToLogin_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
