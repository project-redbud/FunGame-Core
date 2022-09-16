using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FunGame.Core.Api.Model.Enum;
using static FunGame.Core.Api.Model.Enum.EnumHelper;

namespace FunGame.Desktop.Models.Component
{
    public partial class ShowMessage : Form
    {
        private int Location_x, Location_y;
        private MessageResult MessageResult = MessageResult.Cancel;
        private int AutoClose = 0;

        private const string TITLE_TIP = "提示";
        private const string TITLE_WARNING = "警告";
        private const string TITLE_ERROR = "错误";
        private const string BUTTON_OK = "确定";
        private const string BUTTON_CANCEL = "取消";
        private const string BUTTON_YES = "是";
        private const string BUTTON_NO = "否";
        private const string BUTTON_RETRY = "重试";

        /// <summary>
        /// 构造方法
        /// </summary>
        /// /// <param name="objs">参数数组，按下面的注释传参，不要乱传</param>
        private ShowMessage(params object[] objs)
        {
            InitializeComponent();
            Opacity = 0.85; // 透明度
            Title.MouseDown += new MouseEventHandler(Title_MouseDown);
            Title.MouseMove += new MouseEventHandler(Title_MouseMove);
            if (objs != null)
            {
                /**
                 * objs:
                 * 0 = title
                 * 1 = msg
                 * 2 = autoclose(second)
                 * 3 = button type
                 * 4 = mid text
                 * 5 = left text
                 * 6 = right text
                 */
                int length = objs.Length;
                if (length > 0 && objs[0] != null)
                {
                    Title.Text = (string)objs[0];
                    Text = Title.Text;
                }
                if (length > 1 && objs[1] != null)
                {
                    MsgText.Text = (string)objs[1];
                }
                if (length > 2 && objs[2] != null)
                {
                    AutoClose = (int)objs[2];
                }
                if (length > 3 && objs[3] != null)
                {
                    MessageButtonType type = (MessageButtonType)objs[3];
                    switch (type)
                    {
                        case MessageButtonType.OK:
                            MidButton.Text = BUTTON_OK;
                            LeftButton.Visible = false;
                            RightButton.Visible = false;
                            MidButton.Visible = true;
                            break;
                        case MessageButtonType.OKCancel:
                            LeftButton.Text = BUTTON_OK;
                            RightButton.Text = BUTTON_CANCEL;
                            LeftButton.Visible = true;
                            RightButton.Visible = true;
                            MidButton.Visible = false;
                            break;
                        case MessageButtonType.YesNo:
                            LeftButton.Text = BUTTON_YES;
                            RightButton.Text = BUTTON_NO;
                            LeftButton.Visible = true;
                            RightButton.Visible = true;
                            MidButton.Visible = false;
                            break;
                        case MessageButtonType.RetryCancel:
                            LeftButton.Text = BUTTON_RETRY;
                            RightButton.Text = BUTTON_CANCEL;
                            LeftButton.Visible = true;
                            RightButton.Visible = true;
                            MidButton.Visible = false;
                            break;
                    }
                    if (length > 4 && objs[4] != null) MidButton.Text = (string)objs[4];
                    if (length > 5 && objs[5] != null) LeftButton.Text = (string)objs[5];
                    if (length > 6 && objs[6] != null) RightButton.Text = (string)objs[6];
                }
            }
            else
            {
                MessageResult = MessageResult.Cancel;
                Dispose();
            }
            if (AutoClose > 0)
            {
                Action action = new(() =>
                {
                    string msg = MsgText.Text;
                    MsgText.Text = msg + "\n[ " + AutoClose + " 秒后自动关闭 ]";
                    while (AutoClose > 0)
                    {
                        Thread.Sleep(1000);
                        AutoClose--;
                        MsgText.Text = msg + "\n[ " + AutoClose + " 秒后自动关闭 ]";
                    }
                    MessageResult = MessageResult.OK;
                    Dispose();
                });
                Task.Run(() =>
                {
                    if (InvokeRequired)
                        BeginInvoke(action);
                    else
                        action();
                });
            }
            ShowDialog();
        }

        /// <summary>
        /// 设置窗体按钮返回值
        /// </summary>
        /// <param name="text"></param>
        private void SetButtonResult(string text)
        {
            MessageResult = text switch
            {
                BUTTON_OK => MessageResult.OK,
                BUTTON_CANCEL => MessageResult.Cancel,
                BUTTON_YES => MessageResult.Yes,
                BUTTON_NO => MessageResult.No,
                BUTTON_RETRY => MessageResult.Retry,
                _ => MessageResult.Cancel
            };
            Dispose();
        }

        /// <summary>
        /// 设置窗口可拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_MouseDown(object? sender, MouseEventArgs e)
        {
            //判断是否为鼠标左键
            if (e.Button == MouseButtons.Left)
            {
                //获取鼠标左键按下时的位置
                this.Location_x = e.Location.X;
                this.Location_y = e.Location.Y;
            }
        }

        /// <summary>
        /// 设置窗口移动时的偏移距离
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //计算鼠标移动距离
                this.Left += e.Location.X - this.Location_x;
                this.Top += e.Location.Y - this.Location_y;
            }
        }

        public static MessageResult Message(string msg, string title, int autoclose = 0)
        {
            object[] objs = { title, msg, autoclose, MessageButtonType.OK, BUTTON_OK};
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult TipMessage(string msg, string? title = null, int autoclose = 0)
        {
            object[] objs = { (title != null) ? title : TITLE_TIP, msg, autoclose, MessageButtonType.OK, BUTTON_OK };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult WarningMessage(string msg, string? title = null, int autoclose = 0)
        {
            object[] objs = { (title != null) ? title : TITLE_WARNING, msg, autoclose, MessageButtonType.OK, BUTTON_OK };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult ErrorMessage(string msg, string? title = null, int autoclose = 0)
        {
            object[] objs = { (title != null) ? title : TITLE_ERROR, msg, autoclose, MessageButtonType.OK, BUTTON_OK };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult YesNoMessage(string msg, string title)
        {
            object[] objs = { title, msg, 0, MessageButtonType.YesNo, BUTTON_CANCEL, BUTTON_YES, BUTTON_NO };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult OKCancelMessage(string msg, string title)
        {
            object[] objs = { title, msg, 0, MessageButtonType.OKCancel, BUTTON_CANCEL, BUTTON_OK, BUTTON_CANCEL };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult RetryCancelMessage(string msg, string title)
        {
            object[] objs = { title, msg, 0, MessageButtonType.RetryCancel, BUTTON_CANCEL, BUTTON_RETRY, BUTTON_CANCEL };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            SetButtonResult(LeftButton.Text);
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            SetButtonResult(RightButton.Text);
        }

        private void MidButton_Click(object sender, EventArgs e)
        {
            SetButtonResult(MidButton.Text);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            MessageResult = MessageResult.Cancel;
            Dispose();
        }
    }
}
