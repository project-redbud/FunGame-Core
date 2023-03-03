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
        public Register()
        {
            InitializeComponent();
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
                if (!RegisterController.Reg(username, email))
                {
                    ShowMessage.Message("注册失败！！", "注册失败");
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
            return EventResult.Success;
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            Reg_Handler();
        }

        private void GoToLogin_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
