using Milimoe.FunGame.Desktop.Library.Component;

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
            new Register().ShowDialog();
        }
    }
}
