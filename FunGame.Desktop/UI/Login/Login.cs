using Milimoe.FunGame.Core.Entity;
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
        private LoginController LoginController;

        public Login()
        {
            InitializeComponent();
            LoginController = new LoginController(this);
        }

        private void Login_Handler()
        {
            try
            {
                string username = UsernameText.Text.Trim();
                string password = PasswordText.Text.Trim();
                if (username == "" || password == "")
                {
                    ShowMessage.ErrorMessage("账号或密码不能为空！");
                    UsernameText.Focus();
                    return;
                }
                password = Core.Api.Utility.Encryption.HmacSha512(password, username);
                if (!LoginController.LoginAccount(username, password))
                    ShowMessage.Message("登录失败！！", "登录失败");
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
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
            Login_Handler();
        }

        private void ForgetPassword_Click(object sender, EventArgs e)
        {
            ShowMessage.TipMessage("暂不支持找回密码~");
        }
    }
}
