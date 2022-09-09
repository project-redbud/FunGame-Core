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
using static FunGame.Core.Api.Model.Enum.CommonEnums;

namespace FunGame.Desktop.Models.Component
{
    public partial class ShowMessage : Form
    {
        private int Location_x, Location_y;
        private static int MessageResult = -1;
        private static ShowMessage? s = null;

        private ShowMessage(string title)
        {
            InitializeComponent();
            Opacity = 0.85;
            Title.MouseDown += new MouseEventHandler(Title_MouseDown);
            Title.MouseMove += new MouseEventHandler(Title_MouseMove);
            this.Text = title;
        }

        private void SetButtonResult(string text)
        {
            MessageResult = text switch
            {
                "确定" => (int)CommonEnums.MessageResult.OK,
                "取消" => (int)CommonEnums.MessageResult.Cancel,
                "是" => (int)CommonEnums.MessageResult.Yes,
                "否" => (int)CommonEnums.MessageResult.No,
                "重试" => (int)CommonEnums.MessageResult.Retry,
                _ => -1
            };
            Dispose();
        }

        // 设置窗口可拖动
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

        private void Title_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //计算鼠标移动距离
                this.Left += e.Location.X - this.Location_x;
                this.Top += e.Location.Y - this.Location_y;
            }
        }

        public static int Message(string msg, string title)
        {
            s = new ShowMessage(title != null && !title.Equals("") ? title : msg);
            s.Title.Text = title;
            s.MsgText.Text = msg;
            s.LeftButton.Visible = false;
            s.RightButton.Visible = false;
            s.MidButton.Visible = true;
            s.MidButton.Text = "确定";
            s.ShowDialog();
            return MessageResult;
        }

        public static int TipMessage(string msg, string? title = null)
        {
            s = new ShowMessage(title != null && !title.Equals("") ? title : msg);
            if (title != null) s.Title.Text = title;
            else s.Title.Text = "提示";
            s.MsgText.Text = msg;
            s.LeftButton.Visible = false;
            s.RightButton.Visible = false;
            s.MidButton.Visible = true;
            s.MidButton.Text = "确定";
            s.ShowDialog();
            return MessageResult;
        }

        public static int WarningMessage(string msg, string? title = null)
        {
            s = new ShowMessage(title != null && !title.Equals("") ? title : msg);
            if (title != null) s.Title.Text = title;
            else s.Title.Text = "警告";
            s.MsgText.Text = msg;
            s.LeftButton.Visible = false;
            s.RightButton.Visible = false;
            s.MidButton.Visible = true;
            s.MidButton.Text = "确定";
            s.ShowDialog();
            return MessageResult;
        }

        public static int ErrorMessage(string msg, string? title = null)
        {
            s = new ShowMessage(title != null && !title.Equals("") ? title : msg);
            if (title != null) s.Title.Text = title;
            else s.Title.Text = "错误";
            s.MsgText.Text = msg;
            s.LeftButton.Visible = false;
            s.RightButton.Visible = false;
            s.MidButton.Visible = true;
            s.MidButton.Text = "确定";
            s.ShowDialog();
            return MessageResult;
        }

        public static int YesNoMessage(string msg, string title)
        {
            s = new ShowMessage(title != null && !title.Equals("") ? title : msg);
            s.Title.Text = title;
            s.MsgText.Text = msg;
            s.LeftButton.Visible = true;
            s.RightButton.Visible = true;
            s.MidButton.Visible = false;
            s.LeftButton.Text = "是";
            s.RightButton.Text = "否";
            s.ShowDialog();
            return MessageResult;
        }

        public static int OKCancelMessage(string msg, string title)
        {
            s = new ShowMessage(title != null && !title.Equals("") ? title : msg);
            s.Title.Text = title;
            s.MsgText.Text = msg;
            s.LeftButton.Visible = true;
            s.RightButton.Visible = true;
            s.MidButton.Visible = false;
            s.LeftButton.Text = "确定";
            s.RightButton.Text = "取消";
            s.ShowDialog();
            return MessageResult;
        }

        public static int RetryCancelMessage(string msg, string title)
        {
            s = new ShowMessage(title != null && !title.Equals("") ? title : msg);
            s.Title.Text = title;
            s.MsgText.Text = msg;
            s.LeftButton.Visible = true;
            s.RightButton.Visible = true;
            s.MidButton.Visible = false;
            s.LeftButton.Text = "重试";
            s.RightButton.Text = "取消";
            s.ShowDialog();
            return MessageResult;
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
            MessageResult = (int)CommonEnums.MessageResult.Cancel;
            Dispose();
        }
    }
}
