using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Base;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Utility;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Login : BaseLogin
    {
        public Login()
        {
            InitializeComponent();
        }

        protected override void BindEvent()
        {
            base.BindEvent();
            SucceedLogin += SucceedLoginEvent;
            FailedLogin += FailedLoginEvent;
        }

        private bool Login_Handler()
        {
            try
            {
                string username = UsernameText.Text.Trim();
                string password = PasswordText.Text.Trim();
                if (username == "" || password == "")
                {
                    ShowMessage.ErrorMessage("账号或密码不能为空！");
                    UsernameText.Focus();
                    return false;
                }
                if (!LoginController.LoginAccount(username, password))
                {
                    ShowMessage.Message("登录失败！！", "登录失败");
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
        /// 打开注册界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegButton_Click(object sender, EventArgs e)
        {
            OpenForm.SingleForm(Core.Library.Constant.FormType.Register, Core.Library.Constant.OpenFormType.Dialog);
        }

        private void FastLogin_Click(object sender, EventArgs e)
        {
            ShowMessage.TipMessage("与No.16对话即可获得快速登录秘钥，快去试试吧！");
        }

        private void GoToLogin_Click(object sender, EventArgs e)
        {
            GoToLogin.Enabled = false;
            if (!Login_Handler()) GoToLogin.Enabled = true;
        }

        private void ForgetPassword_Click(object sender, EventArgs e)
        {
            ShowMessage.TipMessage("暂不支持找回密码~");
        }

        public EventResult FailedLoginEvent(object sender, GeneralEventArgs e)
        {
            GoToLogin.Enabled = true;
            RunTime.Main?.OnFailedLoginEvent(e);
            return EventResult.Success;
        }

        private EventResult SucceedLoginEvent(object sender, GeneralEventArgs e)
        {
            RunTime.Main?.OnSucceedLoginEvent(e);
            return EventResult.Success;
        }

        private EventResult BeforeLoginEvent(object sender, GeneralEventArgs e)
        {
            RunTime.Main?.OnBeforeLoginEvent(e);
            return EventResult.Success;
        }

        private EventResult AfterLoginEvent(object sender, GeneralEventArgs e)
        {
            RunTime.Main?.OnAfterLoginEvent(e);
            return EventResult.Success;
        }
    }
}
