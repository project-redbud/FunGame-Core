using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Base;
using Milimoe.FunGame.Desktop.Library.Component;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Register : BaseReg
    {
        public bool CheckReg { get; set; } = false;
        public RegisterEventArgs EventArgs { get; set; } = new RegisterEventArgs();

        public Register()
        {
            InitializeComponent();
        }

        protected override void BindEvent()
        {
            base.BindEvent();
            SucceedReg += SucceedRegEvent;
            FailedReg += FailedRegEvent;
        }

        private bool Reg_Handler()
        {
            try
            {
                string username = UsernameText.Text.Trim();
                string password = PasswordText.Text.Trim();
                string checkpassword = CheckPasswordText.Text.Trim();
                string email = EmailText.Text.Trim();
                if (username == "" || password == "" || checkpassword == "")
                {
                    ShowMessage.ErrorMessage("账号或密码不能为空！");
                    UsernameText.Focus();
                    return false;
                }
                if (password != checkpassword)
                {
                    ShowMessage.ErrorMessage("两个密码不相同，请重新输入！");
                    CheckPasswordText.Focus();
                    return false;
                }
                if (email == "")
                {
                    ShowMessage.ErrorMessage("邮箱不能为空！");
                    UsernameText.Focus();
                    return false;
                }
                EventArgs = new RegisterEventArgs(username, password, email);
                if (!RegisterController.Reg(username, email))
                {
                    ShowMessage.Message("注册失败！！", "注册失败");
                    return false;
                }
                else
                {
                    CheckReg = false;
                    // 成功发送注册请求后
                    CheckReg_Handler(username, password, email);
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
            return true;
        }

        public void CheckReg_Handler(string username, string password, string email)
        {
            if (!CheckReg)
            {
                string verifycode = ShowMessage.InputMessageCancel("请输入注册邮件中的6位数字验证码", "注册验证码", out MessageResult cancel);
                if (cancel == MessageResult.Cancel)
                {
                    CheckReg = true;
                    RegButton.Enabled = true;
                }
                RegisterController.CheckReg(username, password, email, verifycode);
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
            if (LoginController.LoginAccount(username, password))
            Dispose();
            return EventResult.Success;
        }
        
        private EventResult FailedRegEvent(object sender, GeneralEventArgs e)
        {
            string username = ((RegisterEventArgs)e).Username;
            string password = ((RegisterEventArgs)e).Password;
            string email = ((RegisterEventArgs)e).Email;
            CheckReg_Handler(username, password, email);
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
