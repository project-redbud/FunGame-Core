using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.UI
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Exit = new FunGame.Desktop.Library.Component.ExitButton(this.components);
            this.MinForm = new Library.Component.MinButton();
            this.Connection = new System.Windows.Forms.Label();
            this.Light = new System.Windows.Forms.Label();
            this.SendTalkText = new System.Windows.Forms.Button();
            this.TalkText = new System.Windows.Forms.TextBox();
            this.StartMatch = new System.Windows.Forms.Button();
            this.CheckMix = new System.Windows.Forms.CheckBox();
            this.CheckTeam = new System.Windows.Forms.CheckBox();
            this.RoomSetting = new System.Windows.Forms.Button();
            this.Login = new System.Windows.Forms.Button();
            this.NowAccount = new System.Windows.Forms.Label();
            this.AccountSetting = new System.Windows.Forms.Button();
            this.About = new System.Windows.Forms.Button();
            this.Room = new System.Windows.Forms.Label();
            this.RoomText = new System.Windows.Forms.TextBox();
            this.PresetText = new System.Windows.Forms.ComboBox();
            this.RoomBox = new System.Windows.Forms.GroupBox();
            this.QueryRoom = new System.Windows.Forms.Button();
            this.RoomList = new System.Windows.Forms.ListBox();
            this.Notice = new System.Windows.Forms.GroupBox();
            this.NoticeText = new FunGame.Desktop.Library.Component.TextArea();
            this.InfoBox = new System.Windows.Forms.GroupBox();
            this.TransparentRectControl = new FunGame.Desktop.Library.Component.TransparentRect();
            this.GameInfo = new FunGame.Desktop.Library.Component.TextArea();
            this.QuitRoom = new System.Windows.Forms.Button();
            this.CreateRoom = new System.Windows.Forms.Button();
            this.Logout = new System.Windows.Forms.Button();
            this.CheckHasPass = new System.Windows.Forms.CheckBox();
            this.Stock = new System.Windows.Forms.Button();
            this.Store = new System.Windows.Forms.Button();
            this.Copyright = new System.Windows.Forms.LinkLabel();
            this.StopMatch = new System.Windows.Forms.Button();
            this.RoomBox.SuspendLayout();
            this.Notice.SuspendLayout();
            this.InfoBox.SuspendLayout();
            this.TransparentRectControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // Exit
            // 
            this.Exit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Exit.BackColor = System.Drawing.Color.White;
            this.Exit.BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.exit;
            this.Exit.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Exit.FlatAppearance.BorderSize = 0;
            this.Exit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Exit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Exit.Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Exit.ForeColor = System.Drawing.Color.Red;
            this.Exit.Location = new System.Drawing.Point(750, 3);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(47, 47);
            this.Exit.TabIndex = 15;
            this.Exit.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Exit.UseVisualStyleBackColor = false;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.Font = new System.Drawing.Font("LanaPixel", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Title.Location = new System.Drawing.Point(3, 3);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(689, 47);
            this.Title.TabIndex = 96;
            this.Title.Text = "FunGame By Mili.cyou";
            this.Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MinForm
            // 
            this.MinForm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.MinForm.BackColor = System.Drawing.Color.White;
            this.MinForm.BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.min;
            this.MinForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.MinForm.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.MinForm.FlatAppearance.BorderSize = 0;
            this.MinForm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.MinForm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.MinForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MinForm.Font = new System.Drawing.Font("LanaPixel", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MinForm.ForeColor = System.Drawing.Color.Red;
            this.MinForm.Location = new System.Drawing.Point(698, 3);
            this.MinForm.Name = "MinForm";
            this.MinForm.Size = new System.Drawing.Size(47, 47);
            this.MinForm.TabIndex = 14;
            this.MinForm.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.MinForm.UseVisualStyleBackColor = false;
            this.MinForm.RelativeForm = this;
            // 
            // Connection
            // 
            this.Connection.BackColor = System.Drawing.Color.Transparent;
            this.Connection.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Connection.Location = new System.Drawing.Point(649, 424);
            this.Connection.Margin = new System.Windows.Forms.Padding(3);
            this.Connection.Name = "Connection";
            this.Connection.Size = new System.Drawing.Size(130, 23);
            this.Connection.TabIndex = 92;
            this.Connection.Text = "等待连接服务器";
            this.Connection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Light
            // 
            this.Light.BackColor = System.Drawing.Color.Transparent;
            this.Light.Image = global::Milimoe.FunGame.Desktop.Properties.Resources.yellow;
            this.Light.Location = new System.Drawing.Point(777, 426);
            this.Light.Name = "Light";
            this.Light.Size = new System.Drawing.Size(18, 18);
            this.Light.TabIndex = 93;
            // 
            // SendTalkText
            // 
            this.SendTalkText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SendTalkText.BackColor = System.Drawing.Color.Transparent;
            this.SendTalkText.BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.send;
            this.SendTalkText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.SendTalkText.FlatAppearance.BorderSize = 0;
            this.SendTalkText.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Teal;
            this.SendTalkText.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.SendTalkText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SendTalkText.Font = new System.Drawing.Font("LanaPixel", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SendTalkText.Location = new System.Drawing.Point(608, 421);
            this.SendTalkText.Name = "SendTalkText";
            this.SendTalkText.Size = new System.Drawing.Size(51, 27);
            this.SendTalkText.TabIndex = 3;
            this.SendTalkText.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.SendTalkText.UseVisualStyleBackColor = false;
            this.SendTalkText.Click += new System.EventHandler(this.SendTalkText_Click);
            // 
            // TalkText
            // 
            this.TalkText.AllowDrop = true;
            this.TalkText.Font = new System.Drawing.Font("LanaPixel", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TalkText.ForeColor = System.Drawing.Color.DarkGray;
            this.TalkText.Location = new System.Drawing.Point(317, 422);
            this.TalkText.Name = "TalkText";
            this.TalkText.Size = new System.Drawing.Size(289, 26);
            this.TalkText.TabIndex = 2;
            this.TalkText.Text = "向消息队列发送消息...";
            this.TalkText.WordWrap = false;
            this.TalkText.Click += new System.EventHandler(this.TalkText_ClickAndFocused);
            this.TalkText.GotFocus += new System.EventHandler(this.TalkText_ClickAndFocused);
            this.TalkText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TalkText_KeyUp);
            this.TalkText.Leave += new System.EventHandler(this.TalkText_Leave);
            // 
            // StartMatch
            // 
            this.StartMatch.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartMatch.Location = new System.Drawing.Point(665, 184);
            this.StartMatch.Name = "StartMatch";
            this.StartMatch.Size = new System.Drawing.Size(132, 35);
            this.StartMatch.TabIndex = 9;
            this.StartMatch.Text = "开始匹配";
            this.StartMatch.UseVisualStyleBackColor = true;
            this.StartMatch.Click += new System.EventHandler(this.StartMatch_Click);
            // 
            // CheckMix
            // 
            this.CheckMix.BackColor = System.Drawing.Color.Transparent;
            this.CheckMix.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CheckMix.Location = new System.Drawing.Point(675, 94);
            this.CheckMix.Name = "CheckMix";
            this.CheckMix.Size = new System.Drawing.Size(123, 24);
            this.CheckMix.TabIndex = 6;
            this.CheckMix.Text = "混战模式房间";
            this.CheckMix.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.CheckMix.UseVisualStyleBackColor = false;
            this.CheckMix.CheckedChanged += new System.EventHandler(this.CheckMix_CheckedChanged);
            // 
            // CheckTeam
            // 
            this.CheckTeam.BackColor = System.Drawing.Color.Transparent;
            this.CheckTeam.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CheckTeam.Location = new System.Drawing.Point(675, 124);
            this.CheckTeam.Name = "CheckTeam";
            this.CheckTeam.Size = new System.Drawing.Size(123, 24);
            this.CheckTeam.TabIndex = 7;
            this.CheckTeam.Text = "团队模式房间";
            this.CheckTeam.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.CheckTeam.UseVisualStyleBackColor = false;
            this.CheckTeam.CheckedChanged += new System.EventHandler(this.CheckTeam_CheckedChanged);
            // 
            // RoomSetting
            // 
            this.RoomSetting.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RoomSetting.Location = new System.Drawing.Point(665, 225);
            this.RoomSetting.Name = "RoomSetting";
            this.RoomSetting.Size = new System.Drawing.Size(132, 35);
            this.RoomSetting.TabIndex = 10;
            this.RoomSetting.Text = "房间设置";
            this.RoomSetting.UseVisualStyleBackColor = true;
            this.RoomSetting.Visible = false;
            this.RoomSetting.Click += new System.EventHandler(this.RoomSetting_Click);
            // 
            // Login
            // 
            this.Login.Font = new System.Drawing.Font("LanaPixel", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Login.Location = new System.Drawing.Point(665, 380);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(132, 39);
            this.Login.TabIndex = 13;
            this.Login.Text = "登录账号";
            this.Login.UseVisualStyleBackColor = true;
            this.Login.Click += new System.EventHandler(this.Login_Click);
            // 
            // NowAccount
            // 
            this.NowAccount.BackColor = System.Drawing.Color.Transparent;
            this.NowAccount.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NowAccount.Location = new System.Drawing.Point(659, 352);
            this.NowAccount.Name = "NowAccount";
            this.NowAccount.Size = new System.Drawing.Size(141, 25);
            this.NowAccount.TabIndex = 91;
            this.NowAccount.Text = "请登录账号";
            this.NowAccount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AccountSetting
            // 
            this.AccountSetting.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.AccountSetting.Location = new System.Drawing.Point(665, 317);
            this.AccountSetting.Name = "AccountSetting";
            this.AccountSetting.Size = new System.Drawing.Size(65, 32);
            this.AccountSetting.TabIndex = 11;
            this.AccountSetting.Text = "设置";
            this.AccountSetting.UseVisualStyleBackColor = true;
            // 
            // About
            // 
            this.About.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.About.Location = new System.Drawing.Point(732, 317);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(65, 32);
            this.About.TabIndex = 12;
            this.About.Text = "关于";
            this.About.UseVisualStyleBackColor = true;
            // 
            // Room
            // 
            this.Room.BackColor = System.Drawing.Color.Transparent;
            this.Room.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Room.Location = new System.Drawing.Point(665, 263);
            this.Room.Name = "Room";
            this.Room.Size = new System.Drawing.Size(132, 45);
            this.Room.TabIndex = 90;
            this.Room.Text = "房间号：114514";
            this.Room.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RoomText
            // 
            this.RoomText.AllowDrop = true;
            this.RoomText.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RoomText.ForeColor = System.Drawing.Color.DarkGray;
            this.RoomText.Location = new System.Drawing.Point(6, 226);
            this.RoomText.Name = "RoomText";
            this.RoomText.Size = new System.Drawing.Size(114, 25);
            this.RoomText.TabIndex = 1;
            this.RoomText.Text = "键入房间代号...";
            this.RoomText.WordWrap = false;
            this.RoomText.Click += new System.EventHandler(this.RoomText_ClickAndFocused);
            this.RoomText.GotFocus += new System.EventHandler(this.RoomText_ClickAndFocused);
            this.RoomText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RoomText_KeyUp);
            this.RoomText.Leave += new System.EventHandler(this.RoomText_Leave);
            // 
            // PresetText
            // 
            this.PresetText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PresetText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PresetText.Font = new System.Drawing.Font("LanaPixel", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PresetText.FormattingEnabled = true;
            this.PresetText.Items.AddRange(new object[] { "- 快捷消息 -" });
            this.PresetText.Location = new System.Drawing.Point(195, 422);
            this.PresetText.Name = "PresetText";
            this.PresetText.Size = new System.Drawing.Size(121, 26);
            this.PresetText.TabIndex = 1;
            this.PresetText.SelectedIndexChanged += new System.EventHandler(this.PresetText_SelectedIndexChanged);
            // 
            // RoomBox
            // 
            this.RoomBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RoomBox.BackColor = System.Drawing.Color.Transparent;
            this.RoomBox.Controls.Add(this.QueryRoom);
            this.RoomBox.Controls.Add(this.RoomList);
            this.RoomBox.Controls.Add(this.RoomText);
            this.RoomBox.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RoomBox.Location = new System.Drawing.Point(3, 56);
            this.RoomBox.Name = "RoomBox";
            this.RoomBox.Size = new System.Drawing.Size(186, 258);
            this.RoomBox.TabIndex = 0;
            this.RoomBox.TabStop = false;
            this.RoomBox.Text = "房间列表";
            // 
            // QueryRoom
            // 
            this.QueryRoom.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.QueryRoom.Location = new System.Drawing.Point(126, 225);
            this.QueryRoom.Name = "QueryRoom";
            this.QueryRoom.Size = new System.Drawing.Size(51, 27);
            this.QueryRoom.TabIndex = 2;
            this.QueryRoom.Text = "加入";
            this.QueryRoom.UseVisualStyleBackColor = true;
            this.QueryRoom.Click += new System.EventHandler(this.QueryRoom_Click);
            // 
            // RoomList
            // 
            this.RoomList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RoomList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RoomList.FormattingEnabled = true;
            this.RoomList.ItemHeight = 19;
            this.RoomList.Location = new System.Drawing.Point(0, 26);
            this.RoomList.Name = "RoomList";
            this.RoomList.Size = new System.Drawing.Size(186, 192);
            this.RoomList.TabIndex = 0;
            this.RoomList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RoomList_MouseDoubleClick);
            // 
            // Notice
            // 
            this.Notice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Notice.BackColor = System.Drawing.Color.Transparent;
            this.Notice.Controls.Add(this.NoticeText);
            this.Notice.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Notice.Location = new System.Drawing.Point(3, 317);
            this.Notice.Name = "Notice";
            this.Notice.Size = new System.Drawing.Size(186, 110);
            this.Notice.TabIndex = 94;
            this.Notice.TabStop = false;
            this.Notice.Text = "通知公告";
            // 
            // NoticeText
            // 
            this.NoticeText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NoticeText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NoticeText.EmptyTextTip = null;
            this.NoticeText.Location = new System.Drawing.Point(6, 24);
            this.NoticeText.Name = "NoticeText";
            this.NoticeText.ReadOnly = true;
            this.NoticeText.Size = new System.Drawing.Size(174, 86);
            this.NoticeText.TabIndex = 0;
            this.NoticeText.Text = "";
            // 
            // InfoBox
            // 
            this.InfoBox.BackColor = System.Drawing.Color.Transparent;
            this.InfoBox.Controls.Add(this.TransparentRectControl);
            this.InfoBox.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InfoBox.Location = new System.Drawing.Point(195, 56);
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.Size = new System.Drawing.Size(464, 363);
            this.InfoBox.TabIndex = 95;
            this.InfoBox.TabStop = false;
            this.InfoBox.Text = "消息队列";
            // 
            // TransparentRectControl
            // 
            this.TransparentRectControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TransparentRectControl.BackColor = System.Drawing.Color.Transparent;
            this.TransparentRectControl.BorderColor = System.Drawing.Color.Transparent;
            this.TransparentRectControl.Controls.Add(this.GameInfo);
            this.TransparentRectControl.Location = new System.Drawing.Point(0, 20);
            this.TransparentRectControl.Name = "TransparentRectControl";
            this.TransparentRectControl.Opacity = 125;
            this.TransparentRectControl.Radius = 20;
            this.TransparentRectControl.ShapeBorderStyle = Milimoe.FunGame.Desktop.Library.Component.TransparentRect.ShapeBorderStyles.ShapeBSNone;
            this.TransparentRectControl.Size = new System.Drawing.Size(464, 343);
            this.TransparentRectControl.TabIndex = 2;
            this.TransparentRectControl.TabStop = false;
            // 
            // GameInfo
            // 
            this.GameInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GameInfo.EmptyTextTip = null;
            this.GameInfo.EmptyTextTipColor = System.Drawing.Color.Transparent;
            this.GameInfo.Location = new System.Drawing.Point(6, 6);
            this.GameInfo.Name = "GameInfo";
            this.GameInfo.ReadOnly = true;
            this.GameInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.GameInfo.Size = new System.Drawing.Size(452, 331);
            this.GameInfo.TabIndex = 1;
            this.GameInfo.Text = "";
            // 
            // QuitRoom
            // 
            this.QuitRoom.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.QuitRoom.Location = new System.Drawing.Point(665, 184);
            this.QuitRoom.Name = "QuitRoom";
            this.QuitRoom.Size = new System.Drawing.Size(132, 35);
            this.QuitRoom.TabIndex = 9;
            this.QuitRoom.Text = "退出房间";
            this.QuitRoom.UseVisualStyleBackColor = true;
            this.QuitRoom.Visible = false;
            this.QuitRoom.Click += new System.EventHandler(this.QuitRoom_Click);
            // 
            // CreateRoom
            // 
            this.CreateRoom.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CreateRoom.Location = new System.Drawing.Point(665, 225);
            this.CreateRoom.Name = "CreateRoom";
            this.CreateRoom.Size = new System.Drawing.Size(132, 35);
            this.CreateRoom.TabIndex = 10;
            this.CreateRoom.Text = "创建房间";
            this.CreateRoom.UseVisualStyleBackColor = true;
            this.CreateRoom.Click += new System.EventHandler(this.CreateRoom_Click);
            // 
            // Logout
            // 
            this.Logout.Font = new System.Drawing.Font("LanaPixel", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Logout.Location = new System.Drawing.Point(665, 380);
            this.Logout.Name = "Logout";
            this.Logout.Size = new System.Drawing.Size(132, 39);
            this.Logout.TabIndex = 13;
            this.Logout.Text = "退出登录";
            this.Logout.UseVisualStyleBackColor = true;
            this.Logout.Visible = false;
            this.Logout.Click += new System.EventHandler(this.Logout_Click);
            // 
            // CheckHasPass
            // 
            this.CheckHasPass.BackColor = System.Drawing.Color.Transparent;
            this.CheckHasPass.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CheckHasPass.Location = new System.Drawing.Point(675, 154);
            this.CheckHasPass.Name = "CheckHasPass";
            this.CheckHasPass.Size = new System.Drawing.Size(123, 24);
            this.CheckHasPass.TabIndex = 8;
            this.CheckHasPass.Text = "带密码的房间";
            this.CheckHasPass.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.CheckHasPass.UseVisualStyleBackColor = false;
            this.CheckHasPass.CheckedChanged += new System.EventHandler(this.CheckHasPass_CheckedChanged);
            // 
            // Stock
            // 
            this.Stock.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Stock.Location = new System.Drawing.Point(661, 56);
            this.Stock.Name = "Stock";
            this.Stock.Size = new System.Drawing.Size(65, 32);
            this.Stock.TabIndex = 4;
            this.Stock.Text = "库存";
            this.Stock.UseVisualStyleBackColor = true;
            // 
            // Store
            // 
            this.Store.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Store.Location = new System.Drawing.Point(732, 56);
            this.Store.Name = "Store";
            this.Store.Size = new System.Drawing.Size(65, 32);
            this.Store.TabIndex = 5;
            this.Store.Text = "商店";
            this.Store.UseVisualStyleBackColor = true;
            // 
            // Copyright
            // 
            this.Copyright.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Copyright.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Copyright.BackColor = System.Drawing.Color.Transparent;
            this.Copyright.Font = new System.Drawing.Font("LanaPixel", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Copyright.LinkArea = new System.Windows.Forms.LinkArea(6, 10);
            this.Copyright.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.Copyright.LinkColor = System.Drawing.Color.Teal;
            this.Copyright.Location = new System.Drawing.Point(3, 430);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(186, 23);
            this.Copyright.TabIndex = 97;
            this.Copyright.TabStop = true;
            this.Copyright.Text = FunGameInfo.FunGame_CopyRight;
            this.Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Copyright.UseCompatibleTextRendering = true;
            this.Copyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Copyright_LinkClicked);
            // 
            // StopMatch
            // 
            this.StopMatch.Font = new System.Drawing.Font("LanaPixel", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StopMatch.Location = new System.Drawing.Point(665, 184);
            this.StopMatch.Name = "StopMatch";
            this.StopMatch.Size = new System.Drawing.Size(132, 35);
            this.StopMatch.TabIndex = 9;
            this.StopMatch.Text = "停止匹配";
            this.StopMatch.UseVisualStyleBackColor = true;
            this.StopMatch.Visible = false;
            this.StopMatch.Click += new System.EventHandler(this.StopMatch_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Milimoe.FunGame.Desktop.Properties.Resources.back;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StopMatch);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.Store);
            this.Controls.Add(this.Stock);
            this.Controls.Add(this.Logout);
            this.Controls.Add(this.RoomSetting);
            this.Controls.Add(this.QuitRoom);
            this.Controls.Add(this.InfoBox);
            this.Controls.Add(this.CreateRoom);
            this.Controls.Add(this.Notice);
            this.Controls.Add(this.RoomBox);
            this.Controls.Add(this.PresetText);
            this.Controls.Add(this.SendTalkText);
            this.Controls.Add(this.TalkText);
            this.Controls.Add(this.Room);
            this.Controls.Add(this.About);
            this.Controls.Add(this.AccountSetting);
            this.Controls.Add(this.NowAccount);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.CheckHasPass);
            this.Controls.Add(this.CheckTeam);
            this.Controls.Add(this.CheckMix);
            this.Controls.Add(this.StartMatch);
            this.Controls.Add(this.Light);
            this.Controls.Add(this.Connection);
            this.Controls.Add(this.MinForm);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.Exit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FunGame";
            this.RoomBox.ResumeLayout(false);
            this.RoomBox.PerformLayout();
            this.Notice.ResumeLayout(false);
            this.InfoBox.ResumeLayout(false);
            this.TransparentRectControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExitButton Exit;
        private MinButton MinForm;
        private Label Connection;
        private Label Light;
        private Button StartMatch;
        private CheckBox CheckMix;
        private CheckBox CheckTeam;
        private Button RoomSetting;
        private Button Login;
        private Label NowAccount;
        private Button AccountSetting;
        private Button About;
        private Label Room;
        private Button SendTalkText;
        private TextBox TalkText;
        private TextBox RoomText;
        private ComboBox PresetText;
        private GroupBox RoomBox;
        private ListBox RoomList;
        private GroupBox Notice;
        private GroupBox InfoBox;
        private Button QuitRoom;
        private Button CreateRoom;
        private Button QueryRoom;
        private Button Logout;
        private CheckBox CheckHasPass;
        private Button Stock;
        private Button Store;
        private LinkLabel Copyright;
        private Button StopMatch;
        private Library.Component.TextArea GameInfo;
        private Library.Component.TextArea NoticeText;
        private Library.Component.TransparentRect TransparentRectControl;
    }
}