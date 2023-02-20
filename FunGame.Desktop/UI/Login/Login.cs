using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Utility;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Login : GeneralForm
    {
        public Login()
        {
            InitializeComponent();
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
    }
}
