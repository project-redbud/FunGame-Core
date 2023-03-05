using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    public partial class ShowMessage : GeneralForm
    {
        private MessageResult MessageResult = MessageResult.Cancel;
        private string InputResult = "";
        private readonly int AutoClose = 0;
        private readonly Task? TaskAutoClose;

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
                            InputText.Visible = false;
                            InputButton.Visible = false;
                            LeftButton.Visible = false;
                            RightButton.Visible = false;
                            MidButton.Visible = true;
                            break;
                        case MessageButtonType.OKCancel:
                            LeftButton.Text = BUTTON_OK;
                            RightButton.Text = BUTTON_CANCEL;
                            InputText.Visible = false;
                            InputButton.Visible = false;
                            LeftButton.Visible = true;
                            RightButton.Visible = true;
                            MidButton.Visible = false;
                            break;
                        case MessageButtonType.YesNo:
                            LeftButton.Text = BUTTON_YES;
                            RightButton.Text = BUTTON_NO;
                            InputText.Visible = false;
                            InputButton.Visible = false;
                            LeftButton.Visible = true;
                            RightButton.Visible = true;
                            MidButton.Visible = false;
                            break;
                        case MessageButtonType.RetryCancel:
                            LeftButton.Text = BUTTON_RETRY;
                            RightButton.Text = BUTTON_CANCEL;
                            InputText.Visible = false;
                            InputButton.Visible = false;
                            LeftButton.Visible = true;
                            RightButton.Visible = true;
                            MidButton.Visible = false;
                            break;
                        case MessageButtonType.Input:
                            InputButton.Text = BUTTON_OK;
                            LeftButton.Visible = false;
                            RightButton.Visible = false;
                            MidButton.Visible = false;
                            InputText.Visible = true;
                            InputButton.Visible = true;
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
                TaskAutoClose = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1);
                    string msg = MsgText.Text;
                    int s = AutoClose;
                    BeginInvoke(() => ChangeSecond(msg, s));
                    while (s > 0)
                    {
                        Thread.Sleep(1000);
                        s--;
                    if (IsHandleCreated) BeginInvoke(() => ChangeSecond(msg, s));
                    }
                    MessageResult = MessageResult.OK;
                    Close();
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
            TaskAutoClose?.Wait(1);
            Dispose();
        }

        public static MessageResult Message(string msg, string title, int autoclose = 0)
        {
            object[] objs = { title, msg, autoclose, MessageButtonType.OK, BUTTON_OK};
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult TipMessage(string msg, string? title = null, int autoclose = 0)
        {
            object[] objs = { title ?? TITLE_TIP, msg, autoclose, MessageButtonType.OK, BUTTON_OK };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult WarningMessage(string msg, string? title = null, int autoclose = 0)
        {
            object[] objs = { title ?? TITLE_WARNING, msg, autoclose, MessageButtonType.OK, BUTTON_OK };
            MessageResult result = new ShowMessage(objs).MessageResult;
            return result;
        }

        public static MessageResult ErrorMessage(string msg, string? title = null, int autoclose = 0)
        {
            object[] objs = { title ?? TITLE_ERROR, msg, autoclose, MessageButtonType.OK, BUTTON_OK };
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

        public static string InputMessage(string msg, string title)
        {
            object[] objs = { title, msg, 0, MessageButtonType.Input };
            string result = new ShowMessage(objs).InputResult;
            return result;
        }
        
        public static string InputMessageCancel(string msg, string title, out MessageResult cancel)
        {
            object[] objs = { title, msg, 0, MessageButtonType.Input };
            ShowMessage window = new ShowMessage(objs);
            string result = window.InputResult;
            cancel = window.MessageResult;
            return result;
        }

        private void ChangeSecond(string msg, int s)
        {
            MsgText.Text = msg + "\n[ " + s + " 秒后自动关闭 ]";
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

        private void InputButton_Click()
        {
            MessageResult = MessageResult.OK;
            if (InputText.Text != null && !InputText.Text.Equals(""))
            {
                InputResult = InputText.Text;
                Dispose();
            }
            else
            {
                InputText.Enabled = false;
                WarningMessage("不能输入空值！");
                InputText.Enabled = true;
            }
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            InputButton_Click();
        }

        private void InputText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                InputButton_Click();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            MessageResult = MessageResult.Cancel;
            Dispose();
        }
    }
}
