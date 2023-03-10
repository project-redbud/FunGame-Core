using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    public partial class GeneralForm : Form
    {
        protected int loc_x, loc_y; // 窗口当前坐标

        public GeneralForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 绑定事件，子类需要重写
        /// </summary>
        protected virtual void BindEvent()
        {

        }

        /// <summary>
        /// 鼠标按下，开始移动主窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Title_MouseDown(object sender, MouseEventArgs e)
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
        protected virtual void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //计算鼠标移动距离
                Left += e.Location.X - loc_x;
                Top += e.Location.Y - loc_y;
            }
        }

        /// <summary>
        /// 自定义窗体销毁方法
        /// </summary>
        protected virtual void FormClosedEvent(object? sender, FormClosedEventArgs e)
        {
            if (GetType() != typeof(ShowMessage))
            {
                Singleton.Remove(this);
                if (GetType() == typeof(Main))
                {
                    RunTime.Main = null;
                }
                else if (GetType() == typeof(Login))
                {
                    RunTime.Login = null;
                }
                else if (GetType() == typeof(Register))
                {
                    RunTime.Register = null;
                }
                else if (GetType() == typeof(InventoryUI))
                {
                    RunTime.Inventory = null;
                }
                else if (GetType() == typeof(StoreUI))
                {
                    RunTime.Store = null;
                }
                else if (GetType() == typeof(RoomSetting))
                {
                    RunTime.RoomSetting = null;
                }
                else if (GetType() == typeof(UserCenter))
                {
                    RunTime.UserCenter = null;
                }
                Dispose();
            }
        }

        /// <summary>
        /// 窗体加载事件，触发BindEvent()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void LoadEvent(object? sender, EventArgs e)
        {
            BindEvent();
        }
    }
}
