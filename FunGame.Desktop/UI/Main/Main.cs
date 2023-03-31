using System.Diagnostics;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
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
    public partial class Main : BaseMain
    {

        #region 变量定义

        /**
         * 定义全局变量
         */
        public int MaxRetryTimes { get; } = SocketSet.MaxRetryTimes; // 最大重试连接次数
        public int CurrentRetryTimes { get; set; } = -1; // 当前重试连接次数

        /**
         * 定义全局对象
         */
        private Task? MatchFunGame = null; // 匹配线程
        private MainController? MainController = null;

        /**
         * 定义委托
         * 子线程操作窗体控件时，先实例化Action，然后Invoke传递出去。
         */
        Action<int, object[]?>? StartMatch_Action = null;
        Action<int, object[]?>? CreateRoom_Action = null;

        public Main()
        {
            InitializeComponent();
            InitAsync();
        }

        /// <summary>
        /// 所有自定义初始化的内容
        /// </summary>
        public async void InitAsync()
        {
            RunTime.Main = this;
            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
            SetRoomid("-1"); // 房间号初始化
            ShowFunGameInfo(); // 显示FunGame信息
            GetFunGameConfig(); // 获取FunGame配置
            // 创建RunTime
            RunTime.Connector = new RunTimeController(this);
            // 窗口句柄创建后，进行委托
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (IsHandleCreated)
                    {
                        break;
                    }
                }
                if (Config.FunGame_isAutoConnect) RunTime.Connector?.GetServerConnection();
            });
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        protected override void BindEvent()
        {
            base.BindEvent();
            Disposed += Main_Disposed;
            FailedConnect += FailedConnectEvent;
            SucceedConnect += SucceedConnectEvent;
            SucceedLogin += SucceedLoginEvent;
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 提供公共方法给Controller更新UI
        /// </summary>
        /// <param name="updatetype">string?</param>
        /// <param name="objs">object[]?</param>
        public void UpdateUI(MainInvokeType type, params object[]? objs)
        {
            void action()
            {
                try
                {
                    switch (type)
                    {
                        case MainInvokeType.SetGreen:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Green);
                            SetButtonEnableIfLogon(true, ClientState.Online);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetGreenAndPing:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Green, ping: NetworkUtility.GetServerPing(Constant.Server_Address));
                            SetButtonEnableIfLogon(true, ClientState.Online);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.WaitConnectAndSetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            if (MainController != null && Config.FunGame_isAutoConnect)
                            {
                                // 自动连接服务器
                                RunTime.Connector?.Connect();
                            }
                            break;

                        case MainInvokeType.WaitLoginAndSetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Yellow, true);
                            SetButtonEnableIfLogon(false, ClientState.WaitLogin);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetRed:
                            SetServerStatusLight((int)LightType.Red);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = false;
                            break;

                        case MainInvokeType.Disconnected:
                            RoomList.Items.Clear();
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isConnected = false;
                            SetServerStatusLight((int)LightType.Red);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            LogoutAccount();
                            MainController?.Dispose();
                            CloseConnectedWindows();
                            break;

                        case MainInvokeType.Disconnect:
                            RoomList.Items.Clear();
                            Config.FunGame_isAutoRetry = false;
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isAutoConnect = false;
                            Config.FunGame_isAutoLogin = false;
                            Config.FunGame_isConnected = false;
                            SetServerStatusLight((int)LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            LogoutAccount();
                            MainController?.Dispose();
                            break;

                        case MainInvokeType.LogIn:
                            break;

                        case MainInvokeType.LogOut:
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isAutoLogin = false;
                            SetServerStatusLight((int)LightType.Yellow, true);
                            SetButtonEnableIfLogon(false, ClientState.WaitLogin);
                            LogoutAccount();
                            if (objs != null && objs.Length > 0)
                            {
                                if (objs[0].GetType() == typeof(string))
                                {
                                    WritelnSystemInfo((string)objs[0]);
                                    ShowMessage.Message((string)objs[0], "退出登录", 5);
                                }
                            }
                            break;

                        case MainInvokeType.SetUser:
                            if (objs != null && objs.Length > 0)
                            {
                                LoginAccount(objs);
                            }
                            break;

                        case MainInvokeType.Connected:
                            NoticeText.Text = Config.FunGame_Notice;
                            break;

                        case MainInvokeType.UpdateRoom:
                            if (objs != null && objs.Length > 0)
                            {
                                List<string> list = (List<string>)objs[0];
                                RoomList.Items.AddRange(list.ToArray());
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    WritelnGameInfo(e.GetErrorInfo());
                    UpdateUI(MainInvokeType.SetRed);
                }
            }
            InvokeUpdateUI(action);
        }

        /// <summary>
        /// 提供公共方法给Controller发送系统信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timetype"></param>
        public void GetMessage(string? msg, TimeType timetype = TimeType.TimeOnly)
        {
            void action()
            {
                try
                {
                    if (msg == null || msg == "") return;
                    if (timetype != TimeType.None)
                    {
                        WritelnGameInfo(DateTimeUtility.GetDateTimeToString(timetype) + " >> " + msg);
                    }
                    else
                    {
                        WritelnGameInfo(msg);
                    }
                }
                catch (Exception e)
                {
                    WritelnGameInfo(e.GetErrorInfo());
                }
            };
            InvokeUpdateUI(action);
        }

        #endregion

        #region 实现

        /// <summary>
        /// 委托更新UI
        /// </summary>
        /// <param name="action"></param>
        private void InvokeUpdateUI(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }

        /// <summary>
        /// 获取FunGame配置文件设定
        /// </summary>
        private void GetFunGameConfig()
        {
            try
            {
                if (INIHelper.ExistINIFile())
                {
                    string isAutoConnect = INIHelper.ReadINI("Config", "AutoConnect");
                    string isAutoLogin = INIHelper.ReadINI("Config", "AutoLogin");
                    string strUserName = INIHelper.ReadINI("Account", "UserName");
                    string strPassword = INIHelper.ReadINI("Account", "Password");
                    string strAutoKey = INIHelper.ReadINI("Account", "AutoKey");

                    if (isAutoConnect != null && isAutoConnect.Trim() != "" && (isAutoConnect.ToLower().Equals("false") || isAutoConnect.ToLower().Equals("true")))
                        Config.FunGame_isAutoConnect = Convert.ToBoolean(isAutoConnect);
                    else throw new ReadConfigException();

                    if (isAutoLogin != null && isAutoLogin.Trim() != "" && (isAutoLogin.ToLower().Equals("false") || isAutoLogin.ToLower().Equals("true")))
                        Config.FunGame_isAutoLogin = Convert.ToBoolean(isAutoLogin);
                    else throw new ReadConfigException();

                    if (strUserName != null && strUserName.Trim() != "")
                        Config.FunGame_AutoLoginUser = strUserName.Trim();

                    if (strPassword != null && strPassword.Trim() != "")
                        Config.FunGame_AutoLoginPassword = strPassword.Trim();

                    if (strAutoKey != null && strAutoKey.Trim() != "")
                        Config.FunGame_AutoLoginKey = strAutoKey.Trim();
                }
                else
                {
                    INIHelper.Init((FunGameInfo.FunGame)Constant.FunGameType);
                    WritelnGameInfo(">> 首次启动，已自动为你创建配置文件。");
                    GetFunGameConfig();
                }
            }
            catch (System.Exception e)
            {
                WritelnGameInfo(e.GetErrorInfo());
            }
        }

        /// <summary>
        /// 设置房间号和显示信息
        /// </summary>
        /// <param name="roomid"></param>
        private void SetRoomid(string roomid)
        {
            Config.FunGame_Roomid = roomid;
            if (!roomid.Equals("-1"))
            {
                WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 加入房间");
                WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已加入房间 -> [ " + Config.FunGame_Roomid + " ]");
                Room.Text = "[ 当前房间 ]\n" + Convert.ToString(Config.FunGame_Roomid);
            }
            else
                Room.Text = "暂未进入房间";
        }

        /// <summary>
        /// 向消息队列输出换行符
        /// </summary>
        private void WritelnGameInfo()
        {
            GameInfo.AppendText("\n");
            GameInfo.SelectionStart = GameInfo.Text.Length - 1;
            GameInfo.ScrollToCaret();
        }

        /// <summary>
        /// 向消息队列输出一行文字
        /// </summary>
        /// <param name="msg"></param>
        private void WritelnGameInfo(string msg)
        {
            if (msg.Trim() != "")
            {
                GameInfo.AppendText(msg + "\n");
                GameInfo.SelectionStart = GameInfo.Text.Length - 1;
                GameInfo.ScrollToCaret();
            }
        }

        /// <summary>
        /// 向消息队列输出文字
        /// </summary>
        /// <param name="msg"></param>
        private void WriteGameInfo(string msg)
        {
            if (msg.Trim() != "")
            {
                GameInfo.AppendText(msg);
                GameInfo.SelectionStart = GameInfo.Text.Length - 1;
                GameInfo.ScrollToCaret();
            }
        }
        
        /// <summary>
        /// 向消息队列输出一行系统信息
        /// </summary>
        /// <param name="msg"></param>
        private void WritelnSystemInfo(string msg)
        {
            msg = DateTimeUtility.GetDateTimeToString(TimeType.TimeOnly) + " >> " + msg;
            WritelnGameInfo(msg);
        }

        /// <summary>
        /// 在大厅中，设置按钮的显示和隐藏
        /// </summary>
        private void InMain()
        {
            // 显示：匹配、创建房间
            // 隐藏：退出房间、房间设定
            SetRoomid("-1");
            QuitRoom.Visible = false;
            StartMatch.Visible = true;
            RoomSetting.Visible = false;
            CreateRoom.Visible = true;
        }

        /// <summary>
        /// 在房间中，设置按钮的显示和隐藏
        /// </summary>
        private void InRoom()
        {
            // 显示：退出房间、房间设置
            // 隐藏：停止匹配、创建房间
            StopMatch.Visible = false;
            QuitRoom.Visible = true;
            CreateRoom.Visible = false;
            RoomSetting.Visible = true;
        }

        /// <summary>
        /// 未登录和离线时，停用按钮
        /// 登录的时候要激活按钮
        /// </summary>
        /// <param name="isLogon">是否登录</param>
        /// <param name="status">客户端状态</param>
        private void SetButtonEnableIfLogon(bool isLogon, ClientState status)
        {
            switch (status)
            {
                case ClientState.Online:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetOnineItems);
                    break;
                case ClientState.WaitConnect:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetNoConnectItems);
                    break;
                case ClientState.WaitLogin:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetNoLoginItems);
                    break;
            }
            this.PresetText.SelectedIndex = 0;
            CheckMix.Enabled = isLogon;
            CheckTeam.Enabled = isLogon;
            CheckHasPass.Enabled = isLogon;
            StartMatch.Enabled = isLogon;
            CreateRoom.Enabled = isLogon;
            RoomBox.Enabled = isLogon;
            AccountSetting.Enabled = isLogon;
            Stock.Enabled = isLogon;
            Store.Enabled = isLogon;
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="isDouble"></param>
        /// <param name="roomid"></param>
        private void JoinRoom(bool isDouble, string roomid)
        {
            if (!isDouble)
                if (!RoomText.Text.Equals("") && !RoomText.ForeColor.Equals(Color.DarkGray))
                {
                    if (CheckRoomIDExist(roomid))
                    {
                        if (Config.FunGame_Roomid.Equals("-1"))
                        {
                            if (ShowMessage.YesNoMessage("已找到房间 -> [ " + roomid + " ]\n是否加入？", "已找到房间") == MessageResult.Yes)
                            {
                                SetRoomid(roomid);
                                InRoom();
                            }
                        }
                        else
                        {
                            ShowMessage.TipMessage("你需要先退出房间才可以加入新的房间。");
                        }
                    }
                    else
                    {
                        ShowMessage.WarningMessage("未找到此房间！");
                    }
                }
                else
                {
                    RoomText.Enabled = false;
                    ShowMessage.TipMessage("请输入房间号。");
                    RoomText.Enabled = true;
                    RoomText.Focus();
                }
            else
            {
                if (CheckRoomIDExist(roomid))
                {
                    if (Config.FunGame_Roomid.Equals("-1"))
                    {
                        if (ShowMessage.YesNoMessage("已找到房间 -> [ " + roomid + " ]\n是否加入？", "已找到房间") == MessageResult.Yes)
                        {
                            SetRoomid(roomid);
                            InRoom();
                        }
                    }
                    else
                    {
                        ShowMessage.TipMessage("你需要先退出房间才可以加入新的房间。");
                    }
                }
                else
                {
                    ShowMessage.WarningMessage("未找到此房间！");
                }
            }
        }

        /// <summary>
        /// 这里实现匹配相关的方法
        /// </summary>
        /// <param name="i">主要参数：触发方法的哪一个分支</param>
        /// <param name="objs">可传多个参数</param>
        private void StartMatch_Method(int i, object[]? objs = null)
        {
            switch (i)
            {
                case (int)StartMatchState.Matching:
                    // 开始匹配
                    Config.FunGame_isMatching = true;
                    int loop = 0;
                    string roomid = Convert.ToString(new Random().Next(1, 10000));
                    // 匹配中 匹配成功返回房间号
                    Task.Factory.StartNew(() =>
                    {
                        // 创建新线程，防止主界面阻塞
                        Thread.Sleep(3000);
                        while (loop < 10000 && Config.FunGame_isMatching)
                        {
                            loop++;
                            if (loop == Convert.ToInt32(roomid))
                            {
                                // 创建委托，操作主界面
                                StartMatch_Action = (int i, object[]? objs) =>
                                {
                                    StartMatch_Method(i, objs);
                                };
                                if (InvokeRequired)
                                {
                                    Invoke(StartMatch_Action, (int)StartMatchState.Success, new object[] { roomid });
                                }
                                else
                                {
                                    StartMatch_Action((int)StartMatchState.Success, new object[] { roomid });
                                }
                                break;
                            }
                        }
                    });
                    break;
                case (int)StartMatchState.Success:
                    Config.FunGame_isMatching = false;
                    // 匹配成功返回房间号
                    roomid = "-1";
                    if (objs != null) roomid = (string)objs[0];
                    if (!roomid.Equals(-1))
                    {
                        WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 匹配成功");
                        WritelnGameInfo(">> 房间号： " + roomid);
                        SetRoomid(roomid);
                    }
                    else
                    {
                        WritelnGameInfo("ERROR：匹配失败！");
                        break;
                    }
                    // 设置按钮可见性
                    InRoom();
                    // 创建委托，操作主界面
                    StartMatch_Action = (i, objs) =>
                    {
                        StartMatch_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(StartMatch_Action, (int)StartMatchState.Enable, new object[] { true });
                    }
                    else
                    {
                        StartMatch_Action((int)StartMatchState.Enable, new object[] { true });
                    }
                    MatchFunGame = null;
                    break;
                case (int)StartMatchState.Enable:
                    // 设置匹配过程中的各种按钮是否可用
                    bool isPause = false;
                    if (objs != null) isPause = (bool)objs[0];
                    CheckMix.Enabled = isPause;
                    CheckTeam.Enabled = isPause;
                    CheckHasPass.Enabled = isPause;
                    CreateRoom.Enabled = isPause;
                    RoomBox.Enabled = isPause;
                    Login.Enabled = isPause;
                    break;
                case (int)StartMatchState.Cancel:
                    WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 终止匹配");
                    WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已终止匹配。");
                    Config.FunGame_isMatching = false;
                    StartMatch_Action = (i, objs) =>
                    {
                        StartMatch_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(StartMatch_Action, (int)StartMatchState.Enable, new object[] { true });
                    }
                    else
                    {
                        StartMatch_Action((int)StartMatchState.Enable, new object[] { true });
                    }
                    MatchFunGame = null;
                    StopMatch.Visible = false;
                    StartMatch.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// 登录账号，显示登出按钮
        /// </summary>
        private void LoginAccount(params object[]? objs)
        {
            if (objs != null && objs.Length > 0)
            {
                Usercfg.LoginUser = (User)objs[0];
                if (Usercfg.LoginUser is null)
                {
                    throw new NoUserLogonException();
                }
                Usercfg.LoginUserName = Usercfg.LoginUser.Username;
            }
            NowAccount.Text = "[ID] " + Usercfg.LoginUserName;
            Login.Visible = false;
            Logout.Visible = true;
            UpdateUI(MainInvokeType.SetGreenAndPing);
            string welcome = $"欢迎回来， {Usercfg.LoginUserName}！";
            ShowMessage.Message(welcome, "登录成功", 5);
            WritelnSystemInfo(welcome);
        }

        /// <summary>
        /// 登出账号，显示登录按钮
        /// </summary>
        private void LogoutAccount()
        {
            InMain();
            Usercfg.LoginUser = null;
            Usercfg.LoginUserName = "";
            NowAccount.Text = "请登录账号";
            Logout.Visible = false;
            Login.Visible = true;
        }

        /// <summary>
        /// 终止匹配实现方法
        /// </summary>
        private void StopMatch_Click()
        {
            StartMatch_Action = (i, objs) =>
            {
                StartMatch_Method(i, objs);
            };
            if (InvokeRequired)
            {
                Invoke(StartMatch_Action, (int)StartMatchState.Cancel, new object[] { true });
            }
            else
            {
                StartMatch_Action((int)StartMatchState.Cancel, new object[] { true });
            }
        }

        /// <summary>
        /// 发送消息实现
        /// </summary>
        /// <param name="isLeave">是否离开焦点</param>
        private void SendTalkText_Click(bool isLeave)
        {
            // 向消息队列发送消息
            string text = TalkText.Text;
            if (text.Trim() != "" && !TalkText.ForeColor.Equals(Color.DarkGray))
            {
                string msg;
                if (Usercfg.LoginUserName == "")
                {
                    msg = ":> " + text;
                }
                else
                {
                    msg = DateTimeUtility.GetNowShortTime() + " [ " + Usercfg.LoginUserName + " ] 说： " + text;
                }
                WritelnGameInfo(msg);
                if (Usercfg.LoginUser != null && !SwitchTalkMessage(text))
                {
                    MainController?.Chat(" [ " + Usercfg.LoginUserName + " ] 说： " + text);
                }
                TalkText.Text = "";
                if (isLeave) TalkText_Leave(); // 回车不离开焦点
            }
            else
            {
                TalkText.Enabled = false;
                ShowMessage.TipMessage("消息不能为空，请重新输入。");
                TalkText.Enabled = true;
                TalkText.Focus();
            }
        }

        /// <summary>
        /// 发送消息实现，往消息队列发送消息
        /// </summary>
        /// <param name="msg"></param>
        private void SendTalkText_Click(string msg)
        {
            WritelnGameInfo((!Usercfg.LoginUserName.Equals("") ? DateTimeUtility.GetNowShortTime() + " [ " + Usercfg.LoginUserName + " ] 说： " : ":> ") + msg);
        }

        /// <summary>
        /// 焦点离开聊天框时设置灰色文本
        /// </summary>
        private void TalkText_Leave()
        {
            if (TalkText.Text.Equals(""))
            {
                TalkText.ForeColor = Color.DarkGray;
                TalkText.Text = "向消息队列发送消息...";
            }
        }

        /// <summary>
        /// 这里实现创建房间相关的方法
        /// </summary>
        /// <param name="i">主要参数：触发方法的哪一个分支</param>
        /// <param name="objs">可传多个参数</param>
        private void CreateRoom_Method(int i, object[]? objs = null)
        {
            if (!Config.FunGame_Roomid.Equals("-1"))
            {
                ShowMessage.WarningMessage("已在房间中，无法创建房间。");
                return;
            }
            string roomid = "";
            string roomtype = "";
            if (objs != null)
            {
                roomtype = (string)objs[0];
            }
            switch (i)
            {
                case (int)CreateRoomState.Creating:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoomState.Success, new object[] { roomtype });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoomState.Success, new object[] { roomtype });
                    }
                    break;
                case (int)CreateRoomState.Success:
                    roomid = Convert.ToString(new Random().Next(1, 10000));
                    SetRoomid(roomid);
                    InRoom();
                    WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 创建" + roomtype + "房间");
                    WritelnGameInfo(">> 创建" + roomtype + "房间成功！房间号： " + roomid);
                    ShowMessage.Message("创建" + roomtype + "房间成功！\n房间号是 -> [ " + roomid + " ]", "创建成功");
                    break;
            }
        }

        /// <summary>
        /// 设置服务器连接状态指示灯
        /// </summary>
        /// <param name="light"></param>
        /// <param name="ping"></param>
        private void SetServerStatusLight(int light, bool waitlogin = false, int ping = 0)
        {
            switch(light)
            {
                case (int)LightType.Green:
                    Connection.Text = "服务器连接成功";
                    this.Light.Image = Properties.Resources.green;
                    break;
                case (int)LightType.Yellow:
                    Connection.Text = waitlogin ? "等待登录账号" : "等待连接服务器";
                    this.Light.Image = Properties.Resources.yellow;
                    break;
                case (int)LightType.Red:
                default:
                    Connection.Text = "服务器连接失败";
                    this.Light.Image = Properties.Resources.red;
                    break;
            }
            if (ping > 0)
            {
                Connection.Text = "心跳延迟  " + ping + "ms";
                if (ping < 100)
                    this.Light.Image = Properties.Resources.green;
                else if (ping >= 100 && ping < 200)
                    this.Light.Image = Properties.Resources.yellow;
                else if (ping >= 200)
                    this.Light.Image = Properties.Resources.red;
            }
        }

        /// <summary>
        /// 显示FunGame信息
        /// </summary>
        private void ShowFunGameInfo()
        {
            WritelnGameInfo(FunGameInfo.GetInfo((FunGameInfo.FunGame)Constant.FunGameType));
        }

        /// <summary>
        /// 关闭所有登录后才能访问的窗口
        /// </summary>
        private static void CloseConnectedWindows()
        {
            RunTime.Login?.Close();
            RunTime.Register?.Close();
            RunTime.Store?.Close();
            RunTime.Inventory?.Close();
            RunTime.RoomSetting?.Close();
            RunTime.UserCenter?.Close();
        }

        #endregion

        #region 事件

        /// <summary>
        /// 关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e)
        {
            if (ShowMessage.OKCancelMessage("你确定关闭游戏？", "退出") == (int)MessageResult.OK)
            {
                RunTime.Connector?.Close();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMatch_Click(object sender, EventArgs e)
        {
            // 开始匹配
            WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 开始匹配");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 开始匹配");
            WriteGameInfo(">> 匹配参数：");
            if (!Config.Match_Mix && !Config.Match_Team && !Config.Match_HasPass)
                WritelnGameInfo("无");
            else
            {
                WriteGameInfo((Config.Match_Mix ? " 混战房间 " : "") + (Config.Match_Team ? " 团队房间 " : "") + (Config.Match_HasPass ? " 密码房间 " : ""));
                WritelnGameInfo();
            }
            // 显示停止匹配按钮
            StartMatch.Visible = false;
            StopMatch.Visible = true;
            // 暂停其他按钮
            StartMatch_Method((int)StartMatchState.Enable, new object[] { false });
            // 创建委托，开始匹配
            StartMatch_Action = (i, objs) =>
            {
                StartMatch_Method(i, objs);
            };
            // 创建新线程匹配
            MatchFunGame = Task.Factory.StartNew(() =>
            {

                if (InvokeRequired)
                {
                    Invoke(StartMatch_Action, (int)StartMatchState.Matching, null);
                }
                else
                {
                    StartMatch_Action((int)StartMatchState.Matching, null);
                }
            });
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateRoom_Click(object sender, EventArgs e)
        {
            string roomtype = "";
            if (Config.Match_Mix && Config.Match_Team)
            {
                ShowMessage.WarningMessage("创建房间不允许同时勾选混战和团队！");
                return;
            }
            else if (Config.Match_Mix && !Config.Match_Team && !Config.Match_HasPass)
            {
                roomtype = Constant.GameMode_Mix;
            }
            else if (!Config.Match_Mix && Config.Match_Team && !Config.Match_HasPass)
            {
                roomtype = Constant.GameMode_Team;
            }
            else if (Config.Match_Mix && !Config.Match_Team && Config.Match_HasPass)
            {
                roomtype = Constant.GameMode_MixHasPass;
            }
            else if (!Config.Match_Mix && Config.Match_Team && Config.Match_HasPass)
            {
                roomtype = Constant.GameMode_TeamHasPass;
            }
            if (roomtype.Equals(""))
            {
                ShowMessage.WarningMessage("请勾选你要创建的房间类型！");
                return;
            }
            CreateRoom_Action = (i, objs) =>
            {
                CreateRoom_Method(i, objs);
            };
            if (InvokeRequired)
            {
                Invoke(CreateRoom_Action, (int)CreateRoomState.Creating, new object[] { roomtype });
            }
            else
            {
                CreateRoom_Action((int)CreateRoomState.Creating, new object[] { roomtype });
            }
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitRoom_Click(object sender, EventArgs e)
        {
            WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 离开房间");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已离开房间 -> [ " + Config.FunGame_Roomid + " ]");
            InMain();
        }

        /// <summary>
        /// 房间设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomSetting_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 查找房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryRoom_Click(object sender, EventArgs e)
        {
            JoinRoom(false, RoomText.Text);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, EventArgs e)
        {
            if (ShowMessage.OKCancelMessage("你确定要退出登录吗？", "退出登录") == MessageResult.OK)
            {
                if (MainController == null || !MainController.LogOut())
                    ShowMessage.WarningMessage("请求无效：退出登录失败！");
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, EventArgs e)
        {
            if (MainController != null && Config.FunGame_isConnected)
                OpenForm.SingleForm(FormType.Login, OpenFormType.Dialog);
            else
                ShowMessage.WarningMessage("请先连接服务器！");
        }

        /// <summary>
        /// 终止匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMatch_Click(object sender, EventArgs e)
        {
            StopMatch_Click();
        }

        /// <summary>
        /// 双击房间列表中的项可以加入房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            #pragma warning disable CS8600, CS8604
            if (RoomList.SelectedItem != null)
                JoinRoom(true, RoomList.SelectedItem.ToString());
        }

        /// <summary>
        /// 点击发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendTalkText_Click(object sender, EventArgs e)
        {
            SendTalkText_Click(true);
        }

        /// <summary>
        /// 勾选混战选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckMix_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckMix.Checked) Config.Match_Mix = true;
            else Config.Match_Mix = false;
        }

        /// <summary>
        /// 勾选团队选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTeam_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckTeam.Checked) Config.Match_Team = true;
            else Config.Match_Team = false;
        }

        /// <summary>
        /// 勾选密码选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckHasPass_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckHasPass.Checked) Config.Match_HasPass = true;
            else Config.Match_HasPass = false;
        }

        /// <summary>
        /// 房间号输入框点击/焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomText_ClickAndFocused(object sender, EventArgs e)
        {
            if (RoomText.Text.Equals("键入房间代号..."))
            {
                RoomText.ForeColor = Color.DarkGray;
                RoomText.Text = "";
            }
        }

        /// <summary>
        /// 焦点离开房间号输入框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomText_Leave(object sender, EventArgs e)
        {
            if (RoomText.Text.Equals(""))
            {
                RoomText.ForeColor = Color.DarkGray;
                RoomText.Text = "键入房间代号...";
            }
        }

        /// <summary>
        /// 房间号输入框内容改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomText_KeyUp(object sender, KeyEventArgs e)
        {
            RoomText.ForeColor = Color.Black;
            if (e.KeyCode.Equals(Keys.Enter))
            {
                // 按下回车加入房间
                JoinRoom(false, RoomText.Text);
            }
        }

        /// <summary>
        /// 聊天框点击/焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalkText_ClickAndFocused(object sender, EventArgs e)
        {
            if (TalkText.Text.Equals("向消息队列发送消息..."))
            {
                TalkText.ForeColor = Color.DarkGray;
                TalkText.Text = "";
            }
        }

        /// <summary>
        /// TalkText离开焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalkText_Leave(object sender, EventArgs e)
        {
            TalkText_Leave();
        }

        /// <summary>
        /// 聊天框内容改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalkText_KeyUp(object sender, KeyEventArgs e)
        {
            TalkText.ForeColor = Color.Black;
            if (e.KeyCode.Equals(Keys.Enter))
            {
                // 按下回车发送
                SendTalkText_Click(false);
            }
        }

        /// <summary>
        /// 版权链接点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Copyright 2023 mili.cyou
            Process.Start(new ProcessStartInfo("https://mili.cyou/fungame") { UseShellExecute = true });
        }

        /// <summary>
        /// 点击快捷消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetText_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 发送快捷消息并执行功能
            if (PresetText.SelectedIndex != 0)
            {
                string s = PresetText.SelectedItem.ToString();
                SendTalkText_Click(s);
                SwitchTalkMessage(s);
                PresetText.SelectedIndex = 0;
            }
        }

        private void Main_Disposed(object? sender, EventArgs e)
        {
            MainController?.Dispose();
        }

        /// <summary>
        /// 连接服务器失败后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public EventResult FailedConnectEvent(object sender, GeneralEventArgs e)
        {
            // 自动重连
            if (Config.FunGame_isConnected && Config.FunGame_isAutoRetry && CurrentRetryTimes <= MaxRetryTimes)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    if (Config.FunGame_isConnected && Config.FunGame_isAutoRetry) RunTime.Connector?.Connect(); // 再次判断是否开启自动重连
                });
                GetMessage("连接服务器失败，5秒后自动尝试重连。");
            }
            else GetMessage("无法连接至服务器，请检查你的网络连接。");
            return EventResult.Success;
        }

        /// <summary>
        /// 连接服务器成功后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public EventResult SucceedConnectEvent(object sender, GeneralEventArgs e)
        {
            // 创建MainController
            MainController = new MainController(this);
            if (MainController != null && Config.FunGame_isAutoLogin && Config.FunGame_AutoLoginUser != "" && Config.FunGame_AutoLoginPassword != "" && Config.FunGame_AutoLoginKey != "")
            {
                // 自动登录
                _ = RunTime.Connector?.AutoLogin(Config.FunGame_AutoLoginUser, Config.FunGame_AutoLoginPassword, Config.FunGame_AutoLoginKey);
            }
            return EventResult.Success;
        }

        /// <summary>
        /// 登录成功后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private EventResult SucceedLoginEvent(object sender, GeneralEventArgs e)
        {
            // 接入-1号房间聊天室
            MainController?.IntoRoom("-1");
            // 获取在线的房间列表
            MainController?.UpdateRoom();
            return EventResult.Success;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 判断是否存在这个房间
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        private bool CheckRoomIDExist(string roomid)
        {
            foreach (string BoxText in RoomList.Items)
            {
                if (roomid.Equals(BoxText))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断快捷消息
        /// </summary>
        /// <param name="s"></param>
        private bool SwitchTalkMessage(string s)
        {
            switch (s)
            {
                case Constant.FunGame_SignIn:
                    break;
                case Constant.FunGame_ShowCredits:
                    break;
                case Constant.FunGame_ShowStock:
                    break;
                case Constant.FunGame_ShowStore:
                    break;
                case Constant.FunGame_CreateMix:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoomState.Creating, new object[] { Constant.GameMode_Mix });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoomState.Creating, new object[] { Constant.GameMode_Mix });
                    }
                    break;
                case Constant.FunGame_CreateTeam:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoomState.Creating, new object[] { Constant.GameMode_Team });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoomState.Creating, new object[] { Constant.GameMode_Team });
                    }
                    break;
                case Constant.FunGame_StartGame:
                    break;
                case Constant.FunGame_AutoRetryOn:
                    WritelnGameInfo(">> 自动重连开启");
                    Config.FunGame_isAutoRetry = true;
                    break;
                case Constant.FunGame_AutoRetryOff:
                    WritelnGameInfo(">> 自动重连关闭");
                    Config.FunGame_isAutoRetry = false;
                    break;
                case Constant.FunGame_Retry:
                    if (!Config.FunGame_isRetrying)
                    {
                        CurrentRetryTimes = -1;
                        Config.FunGame_isAutoRetry = true;
                        RunTime.Connector?.Connect();
                    }
                    else
                        WritelnGameInfo(">> 你不能在连接服务器的同时重试连接！");
                    break;
                case Constant.FunGame_Connect:
                    if (!Config.FunGame_isConnected)
                    {
                        CurrentRetryTimes = -1;
                        Config.FunGame_isAutoRetry = true;
                        RunTime.Connector?.GetServerConnection();
                    }
                    break;
                case Constant.FunGame_Disconnect:
                    if (Config.FunGame_isConnected && MainController != null)
                    {
                        // 先退出登录再断开连接
                        if (MainController?.LogOut() ?? false) RunTime.Connector?.Disconnect();
                    }
                    break;
                case Constant.FunGame_DisconnectWhenNotLogin:
                    if (Config.FunGame_isConnected && MainController != null)
                    {
                        RunTime.Connector?.Disconnect();
                    }
                    break;
                case Constant.FunGame_ConnectTo:
                    string msg = ShowMessage.InputMessage("请输入服务器IP地址和端口号，如: 127.0.0.1:22222。", "连接指定服务器");
                    if (msg.Equals("")) return true;
                    string[] addr = msg.Split(':');
                    string ip;
                    int port;
                    if (addr.Length < 2)
                    {
                        ip = addr[0];
                        port = 22222;
                    }
                    else if (addr.Length < 3)
                    {
                        ip = addr[0];
                        port = Convert.ToInt32(addr[1]);
                    }
                    else
                    {
                        ShowMessage.ErrorMessage("格式错误！\n这不是一个服务器地址。");
                        return true;
                    }
                    ErrorType ErrorType = NetworkUtility.IsServerAddress(ip, port);
                    if (ErrorType == Core.Library.Constant.ErrorType.None)
                    {
                        Constant.Server_Address = ip;
                        Constant.Server_Port = port;
                        CurrentRetryTimes = -1;
                        Config.FunGame_isAutoRetry = true;
                        RunTime.Connector?.Connect();
                    }
                    else if (ErrorType == Core.Library.Constant.ErrorType.IsNotIP) ShowMessage.ErrorMessage("这不是一个IP地址！");
                    else if (ErrorType == Core.Library.Constant.ErrorType.IsNotPort) ShowMessage.ErrorMessage("这不是一个端口号！\n正确范围：1~65535");
                    else ShowMessage.ErrorMessage("格式错误！\n这不是一个服务器地址。");
                    break;
                default:
                    break;
            }
            return false;
        }

        #endregion
    }
}