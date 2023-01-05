using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Login : Form
    {
        private int loc_x, loc_y; // 窗口当前坐标

        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 鼠标按下，开始移动主窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_MouseDown(object sender, MouseEventArgs e)
        {
            //判断是否为鼠标左键
            if (e.Button == MouseButtons.Left)
            {
                //获取鼠标左键按下时的位置
                loc_x = e.Location.X;
                loc_y = e.Location.Y;
            }
        }

        /// <summary>
        /// 鼠标移动，正在移动主窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //计算鼠标移动距离
                Left += e.Location.X - loc_x;
                Top += e.Location.Y - loc_y;
            }
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

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
