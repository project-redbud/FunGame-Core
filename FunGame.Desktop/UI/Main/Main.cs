using FunGame.Desktop.Models.Component;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Text;
using FunGame.Core.Api.Model.Entity;
using FunGame.Desktop.Models.Config;
using FunGame.Desktop.Utils;
using FunGame.Core.Api.Model.Enum;
using static FunGame.Core.Api.Model.Enum.CommonEnums;

namespace FunGame.Desktop.UI
{
    public partial class Main : Form
    {

        #region 变量定义

        /**
         * 定义全局变量
         */
        private int LOCATION_X, LOCATION_Y; // 窗口当前坐标
        private int MAX_CONNECTEDRETRY = 20; // 最大重试连接次数
        private int NOW_CONNECTEDRETRY = -1; // 当前重试连接次数

        /**
         * 定义全局对象
         */
        private Task? MatchFunGame = null; // 匹配线程
        private WebHelper? WebHelper = null; // WebHelper

        /**
         * 定义委托
         * 子线程操作窗体控件时，先实例化Action，然后Invoke传递出去。
         */
        Action<int, object[]?>? StartMatch_Action = null;
        Action<int, object[]?>? CreateRoom_Action = null;
        Action<Main?>? WebHelper_Action = null;
        Action<Main?>? Main_Action = null;

        public Main()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 所有自定义初始化的内容
        /// </summary>
        public void Init()
        {
            SetButtonEnableIfLogon(false);
            SetRoomid("-1"); // 房间号初始化
            ShowFunGameInfo(); // 显示FunGame信息
            GetServerConnection(); // 开始连接服务器
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 提供公共方法给WebHelper
        /// </summary>
        /// <param name="webHelper"></param>
        /// <param name="msg"></param>
        /// <param name="needTime"></param>
        /// <returns></returns>
        public object? GetMessage(WebHelper webHelper, string? msg, bool needTime = false, object[]? objs = null)
        {
            try
            {
                if (msg != null)
                {
                    switch (msg)
                    {
                        case Config.WebHelper_SetGreen:
                            Usercfg.FunGame_isRetrying = false;
                            WebHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)CommonEnums.LightType.Green);
                                SetButtonEnableIfLogon(true);
                            };
                            if (InvokeRequired)
                                BeginInvoke(WebHelper_Action, this);
                            else
                                WebHelper_Action(this);
                            Usercfg.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            break;
                        case Config.WebHelper_SetGreenAndPing:
                            Usercfg.FunGame_isRetrying = false;
                            WebHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)CommonEnums.LightType.Green, GetServerPing(Config.SERVER_IPADRESS));
                                SetButtonEnableIfLogon(true);
                            };
                            if (InvokeRequired)
                                BeginInvoke(WebHelper_Action, this);
                            else
                                WebHelper_Action(this);
                            Usercfg.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            break;
                        case Config.WebHelper_SetYellow:
                            Usercfg.FunGame_isRetrying = false;
                            WebHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)CommonEnums.LightType.Yellow);
                                SetButtonEnableIfLogon(false);
                            };
                            if (InvokeRequired)
                                BeginInvoke(WebHelper_Action, this);
                            else
                                WebHelper_Action(this);
                            Usercfg.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            break;
                        case Config.WebHelper_SetRed:
                            WebHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)CommonEnums.LightType.Red);
                                SetButtonEnableIfLogon(false);
                            };
                            if (InvokeRequired)
                                BeginInvoke(WebHelper_Action, this);
                            else
                                WebHelper_Action(this);
                            Usercfg.FunGame_isConnected = false;
                            break;
                        case Config.WebHelper_Disconnected:
                            Usercfg.FunGame_isRetrying = false;
                            WebHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)CommonEnums.LightType.Red);
                                SetButtonEnableIfLogon(false);
                            };
                            if (InvokeRequired)
                                BeginInvoke(WebHelper_Action, this);
                            else
                                WebHelper_Action(this);
                            Usercfg.FunGame_isConnected = false;
                            if (Usercfg.FunGame_isAutoRetry && NOW_CONNECTEDRETRY <= MAX_CONNECTEDRETRY)
                            {
                                Task.Run(() =>
                                {
                                    Thread.Sleep(5000);
                                    Connect();
                                });
                                if (needTime)
                                    throw new Exception(GetNowShortTime() + "\nERROR：连接服务器失败，5秒后自动尝试重连。");
                                else
                                    throw new Exception("ERROR：连接服务器失败，5秒后自动尝试重连。");
                            }
                            else
                                if (needTime)
                                throw new Exception(GetNowShortTime() + "\nERROR：无法连接至服务器，请检查你的网络连接。");
                            else
                                throw new Exception("ERROR：无法连接至服务器，请检查你的网络连接。");
                        case Config.WebHelper_GetUser:
                            if (Usercfg.LoginUser != null)
                                return Usercfg.LoginUser;
                            return null;
                        case Config.WebHelper_SetUser:
                            if (objs != null && objs.Length > 1)
                            {
                                if (InvokeRequired)
                                    BeginInvoke(SetLoginUser, objs);
                                else
                                    SetLoginUser(objs);
                            }
                            return null;
                        default:
                            if (needTime)
                                WritelnGameInfo(webHelper, GetNowShortTime() + msg);
                            else
                                WritelnGameInfo(webHelper, msg);
                            return null;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                WritelnGameInfo(webHelper, e.Message != null ? e.Message + "\n" + e.StackTrace : "" + e.StackTrace);
                GetMessage(webHelper, Config.WebHelper_SetRed);
            }
            return null;
        }

        #endregion

        #region 实现

        /// <summary>
        /// 反射获取服务器IP和Port
        /// </summary>
        private void GetServerConnection()
        {
            try
            {
                string? ipaddress = (string?)Config.DefaultAssemblyHelper.GetFunGameCoreValue((int)InterfaceType.ClientConnectInterface, (int)InterfaceMethod.RemoteServerIP); // 获取服务器IP
                if (ipaddress != null)
                {
                    string[] s = ipaddress.Split(':');
                    if (s != null && s.Length > 1)
                    {
                        Config.SERVER_IPADRESS = s[0];
                        Config.SERVER_PORT = Convert.ToInt32(s[1]);
                        Connect(); // 连接服务器
                    }
                    else throw new Exception();
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                WritelnGameInfo(">> 查找可用的服务器失败，请重启FunGame。\n" + e.StackTrace);
                ShowMessage.ErrorMessage("查找可用的服务器失败！");
            }
        }

        /// <summary>
        /// 在服务器IP获取成功后，尝试连接服务器
        /// </summary>
        private void Connect()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (IsHandleCreated)
                    {
                        // 检查是否创建了窗口句柄，再Invoke委托。
                        break;
                    }
                }
                Main_Action = (main) =>
                {
                    if (!Usercfg.FunGame_isConnected)
                    {
                        NOW_CONNECTEDRETRY++;
                        if (NOW_CONNECTEDRETRY == 0)
                            WritelnGameInfo(GetNowTime() + " >> 开始连接服务器...");
                        else
                            WritelnGameInfo(GetNowTime() + " >> 第" + NOW_CONNECTEDRETRY + "次重试连接服务器...");
                        if (NOW_CONNECTEDRETRY + 1 > MAX_CONNECTEDRETRY) // 判断重连次数是否达到上限
                        {
                            WritelnGameInfo("ERROR：无法连接至服务器，请检查网络并重启游戏再试。");
                            return;
                        }
                        WebHelper_Action = (main) =>
                        {
                            try
                            {
                                if (main != null)
                                {
                                    if (WebHelper != null)
                                    {
                                        WebHelper.WebHelpMethod((int)WebHelperMethod.CloseSocket);
                                        WebHelper = null;
                                    }
                                    Usercfg.FunGame_isRetrying = true;
                                    Application.DoEvents();
                                    WebHelper = new WebHelper(main);
                                    WebHelper.WebHelpMethod((int)WebHelperMethod.CreateSocket); // Invoke -> CreateSocket
                                }
                            }
                            catch
                            {

                            }
                        };
                        Task.Factory.StartNew(() =>
                        {
                            if (InvokeRequired)
                            {
                                BeginInvoke(WebHelper_Action, main);
                            }
                            else
                            {
                                WebHelper_Action(main);
                            }
                        });
                    }
                };
                if (InvokeRequired)
                {
                    Invoke(Main_Action, this);
                }
                else
                {
                    Main_Action(this);
                }
            });
        }

        /// <summary>
        /// 设置房间号和显示信息
        /// </summary>
        /// <param name="roomid"></param>
        private void SetRoomid(string roomid)
        {
            Usercfg.FunGame_roomid = roomid;
            if (!roomid.Equals("-1"))
            {
                WritelnGameInfo(GetNowShortTime() + " 加入房间");
                WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已加入房间 -> [ " + Usercfg.FunGame_roomid + " ]");
                Room.Text = "[ 当前房间 ]\n" + Convert.ToString(Usercfg.FunGame_roomid);
            }
            else
                Room.Text = "暂未进入房间";
        }

        /// <summary>
        /// 设置登录信息
        /// </summary>
        /// <param name="objs"></param>
        private void SetLoginUser(object[]? objs = null)
        {
            if (InvokeRequired)
                BeginInvoke(LoginAccount, objs);
            else
                LoginAccount(objs);
        }

        /// <summary>
        /// 向消息队列输出换行符
        /// </summary>
        private void WritelnGameInfo()
        {
            GameInfo.Text += "\n";
            GameInfo.SelectionStart = GameInfo.Text.Length - 1;
            GameInfo.ScrollToCaret();
        }

        /// <summary>
        /// 由WebHelper委托向消息队列输出一行文字
        /// </summary>
        /// <param name="webHelper"></param>
        /// <param name="msg"></param>
        private void WritelnGameInfo(WebHelper webHelper, string msg)
        {
            if (webHelper != null && msg.Trim() != "")
            {
                Action tempAction = new Action(() =>
                {
                    GameInfo.Text += msg + "\n";
                    GameInfo.SelectionStart = GameInfo.Text.Length - 1;
                    GameInfo.ScrollToCaret();
                });
                if (this.InvokeRequired)
                    Invoke(tempAction);
                else
                    tempAction();
            }
        }

        /// <summary>
        /// 向消息队列输出一行文字
        /// </summary>
        /// <param name="msg"></param>
        private void WritelnGameInfo(string msg)
        {
            if (msg.Trim() != "")
            {
                GameInfo.Text += msg + "\n";
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
                GameInfo.Text += msg;
                GameInfo.SelectionStart = GameInfo.Text.Length - 1;
                GameInfo.ScrollToCaret();
            }
        }

        /// <summary>
        /// 在大厅中，设置按钮的显示和隐藏
        /// </summary>
        private void InMain()
        {
            // 显示：匹配、创建房间
            // 隐藏：退出房间、房间设定
            WritelnGameInfo(GetNowShortTime() + " 离开房间");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已离开房间 -> [ " + Usercfg.FunGame_roomid + " ]");
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
        /// </summary>
        private void SetButtonEnableIfLogon(bool isLogon)
        {
            if (isLogon)
            {
                PresetText.Items.Clear();
                PresetText.Items.AddRange(Config.PresetOnineItems);
            }
            else
            {
                PresetText.Items.Clear();
                PresetText.Items.AddRange(Config.PresetNoLoginItems);
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
                        if (Usercfg.FunGame_roomid.Equals("-1"))
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
                }
            else
            {
                if (CheckRoomIDExist(roomid))
                {
                    if (Usercfg.FunGame_roomid.Equals("-1"))
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
                case (int)StartMatch_State.Matching:
                    // 开始匹配
                    Usercfg.FunGame_isMatching = true;
                    int loop = 0;
                    string roomid = Convert.ToString(new Random().Next(1, 10000));
                    // 匹配中 匹配成功返回房间号
                    Task.Factory.StartNew(() =>
                    {
                        // 创建新线程，防止主界面阻塞
                        Thread.Sleep(3000);
                        while (loop < 10000 && Usercfg.FunGame_isMatching)
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
                                    Invoke(StartMatch_Action, (int)StartMatch_State.Success, new object[] { roomid });
                                }
                                else
                                {
                                    StartMatch_Action((int)StartMatch_State.Success, new object[] { roomid });
                                }
                                break;
                            }
                        }
                    });
                    break;
                case (int)StartMatch_State.Success:
                    Usercfg.FunGame_isMatching = false;
                    // 匹配成功返回房间号
                    roomid = "-1";
                    if (objs != null) roomid = (string)objs[0];
                    if (!roomid.Equals(-1))
                    {
                        WritelnGameInfo(GetNowShortTime() + " 匹配成功");
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
                        Invoke(StartMatch_Action, (int)StartMatch_State.Enable, new object[] { true });
                    }
                    else
                    {
                        StartMatch_Action((int)StartMatch_State.Enable, new object[] { true });
                    }
                    MatchFunGame = null;
                    break;
                case (int)StartMatch_State.Enable:
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
                case (int)StartMatch_State.Cancel:
                    WritelnGameInfo(GetNowShortTime() + " 终止匹配");
                    WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已终止匹配。");
                    Usercfg.FunGame_isMatching = false;
                    StartMatch_Action = (i, objs) =>
                    {
                        StartMatch_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(StartMatch_Action, (int)StartMatch_State.Enable, new object[] { true });
                    }
                    else
                    {
                        StartMatch_Action((int)StartMatch_State.Enable, new object[] { true });
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
        private void LoginAccount(object[]? objs = null)
        {
            if (objs != null && objs.Length > 0)
            {
                Usercfg.LoginUser = (User)objs[2];
                Usercfg.LoginUserName = Usercfg.LoginUser.Userame;
            }
            NowAccount.Text = "[ID] " + Usercfg.LoginUserName;
            Login.Visible = false;
            Logout.Visible = true;
            SetServerStatusLight((int)LightType.Green);
            Task.Run(() =>
            { // DEBUG
                ShowMessage.TipMessage("欢迎回来， " + Usercfg.LoginUserName + "！", "登录成功", 5000);
            });
        }

        /// <summary>
        /// 登出账号，显示登录按钮
        /// </summary>
        private void LogoutAccount()
        {
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
                Invoke(StartMatch_Action, (int)StartMatch_State.Cancel, new object[] { true });
            }
            else
            {
                StartMatch_Action((int)StartMatch_State.Cancel, new object[] { true });
            }
        }

        /// <summary>
        /// 发送消息实现
        /// </summary>
        /// <param name="isLeave">是否离开焦点</param>
        private void SendTalkText_Click(bool isLeave)
        {
            // 向消息队列发送消息
            if (!TalkText.Text.Trim().Equals("") && !TalkText.ForeColor.Equals(Color.DarkGray))
            {
                WritelnGameInfo(GetNowShortTime() + " [ " + (!Usercfg.LoginUserName.Equals("") ? Usercfg.LoginUserName : "尚未登录") + " ] 说： " + TalkText.Text);
                SwitchTalkMessage(TalkText.Text);
                TalkText.Text = "";
                if (isLeave) TalkText_Leave(); // 回车不离开焦点
            }
            else
            {
                TalkText.Enabled = false;
                ShowMessage.TipMessage("消息不能为空，请重新输入。");
                TalkText.Enabled = true;
            }
        }

        /// <summary>
        /// 发送消息实现，往消息队列发送消息
        /// </summary>
        /// <param name="msg"></param>
        private void SendTalkText_Click(string msg)
        {
            WritelnGameInfo(GetNowShortTime() + " [ " + (!Usercfg.LoginUserName.Equals("") ? Usercfg.LoginUserName : "尚未登录") + " ] 说： " + msg);
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
            if (!Usercfg.FunGame_roomid.Equals("-1"))
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
                case (int)CreateRoom_State.Creating:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoom_State.Success, new object[] { roomtype });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoom_State.Success, new object[] { roomtype });
                    }
                    break;
                case (int)CreateRoom_State.Success:
                    roomid = Convert.ToString(new Random().Next(1, 10000));
                    SetRoomid(roomid);
                    InRoom();
                    WritelnGameInfo(GetNowShortTime() + " 创建" + roomtype + "房间");
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
        private void SetServerStatusLight(int light, int ping = 0)
        {
            switch(light)
            {
                case (int)CommonEnums.LightType.Green:
                    Connection.Text = "服务器连接成功";
                    this.Light.Image = Properties.Resources.green;
                    break;
                case (int)CommonEnums.LightType.Yellow:
                    Connection.Text = "等待登录账号";
                    this.Light.Image = Properties.Resources.yellow;
                    break;
                case (int)CommonEnums.LightType.Red:
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
            WritelnGameInfo(FunGameEnums.GetInfo(Config.FunGameType));
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
                if (WebHelper != null)
                {
                    WebHelper.WebHelpMethod((int)WebHelperMethod.CloseSocket);
                    WebHelper = null;
                }
                Environment.Exit(0);
            }
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
                LOCATION_X = e.Location.X;
                LOCATION_Y = e.Location.Y;
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
                Left += e.Location.X - LOCATION_X;
                Top += e.Location.Y - LOCATION_Y;
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
            WritelnGameInfo(GetNowShortTime() + " 开始匹配");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 开始匹配");
            WriteGameInfo(">> 匹配参数：");
            if (!Usercfg.Match_Mix && !Usercfg.Match_Team && !Usercfg.Match_HasPass)
                WritelnGameInfo("无");
            else
            {
                WriteGameInfo((Usercfg.Match_Mix ? " 混战房间 " : "") + (Usercfg.Match_Team ? " 团队房间 " : "") + (Usercfg.Match_HasPass ? " 密码房间 " : ""));
                WritelnGameInfo();
            }
            // 显示停止匹配按钮
            StartMatch.Visible = false;
            StopMatch.Visible = true;
            // 暂停其他按钮
            StartMatch_Method((int)StartMatch_State.Enable, new object[] { false });
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
                    Invoke(StartMatch_Action, (int)StartMatch_State.Matching, null);
                }
                else
                {
                    StartMatch_Action((int)StartMatch_State.Matching, null);
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
            if (Usercfg.Match_Mix && Usercfg.Match_Team)
            {
                ShowMessage.WarningMessage("创建房间不允许同时勾选混战和团队！");
                return;
            }
            else if (Usercfg.Match_Mix && !Usercfg.Match_Team && !Usercfg.Match_HasPass)
            {
                roomtype = Config.GameMode_Mix;
            }
            else if (!Usercfg.Match_Mix && Usercfg.Match_Team && !Usercfg.Match_HasPass)
            {
                roomtype = Config.GameMode_Team;
            }
            else if (Usercfg.Match_Mix && !Usercfg.Match_Team && Usercfg.Match_HasPass)
            {
                roomtype = Config.GameMode_MixHasPass;
            }
            else if (!Usercfg.Match_Mix && Usercfg.Match_Team && Usercfg.Match_HasPass)
            {
                roomtype = Config.GameMode_TeamHasPass;
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
                Invoke(CreateRoom_Action, (int)CreateRoom_State.Creating, new object[] { roomtype });
            }
            else
            {
                CreateRoom_Action((int)CreateRoom_State.Creating, new object[] { roomtype });
            }
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitRoom_Click(object sender, EventArgs e)
        {
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
            LogoutAccount();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, EventArgs e)
        {
            if (WebHelper != null)
                WebHelper.WebHelpMethod((int)CommonEnums.WebHelperMethod.Login);
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
            if (CheckMix.Checked) Usercfg.Match_Mix = true;
            else Usercfg.Match_Mix = false;
        }

        /// <summary>
        /// 勾选团队选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTeam_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckTeam.Checked) Usercfg.Match_Team = true;
            else Usercfg.Match_Team = false;
        }

        /// <summary>
        /// 勾选密码选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckHasPass_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckHasPass.Checked) Usercfg.Match_HasPass = true;
            else Usercfg.Match_HasPass = false;
        }

        /// <summary>
        /// 点击房间号输入框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomText_Click(object sender, EventArgs e)
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
        /// 点击聊天框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalkText_Click(object sender, EventArgs e)
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
            // Copyright 2022 mili.cyou
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

        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinForm_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
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
        private void SwitchTalkMessage(string s)
        {
            switch (s)
            {
                case Config.FunGame_SignIn:
                    break;
                case Config.FunGame_ShowCredits:
                    break;
                case Config.FunGame_ShowStock:
                    break;
                case Config.FunGame_ShowStore:
                    break;
                case Config.FunGame_CreateMix:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoom_State.Creating, new object[] { Config.GameMode_Mix });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoom_State.Creating, new object[] { Config.GameMode_Mix });
                    }
                    break;
                case Config.FunGame_CreateTeam:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoom_State.Creating, new object[] { Config.GameMode_Team });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoom_State.Creating, new object[] { Config.GameMode_Team });
                    }
                    break;
                case Config.FunGame_StartGame:
                    break;
                case Config.FunGame_AutoRetryOn:
                    WritelnGameInfo(">> 自动重连开启");
                    Usercfg.FunGame_isAutoRetry = true;
                    break;
                case Config.FunGame_AutoRetryOff:
                    WritelnGameInfo(">> 自动重连关闭");
                    Usercfg.FunGame_isAutoRetry = false;
                    break;
                case Config.FunGame_Retry:
                    if (!Usercfg.FunGame_isRetrying)
                    {
                        NOW_CONNECTEDRETRY = -1;
                        Connect();
                    }
                    else
                        WritelnGameInfo(">> 你不能在连接服务器的同时重试连接！");
                    break;
            }
        }

        /// <summary>
        /// 获取系统日期
        /// </summary>
        /// <returns></returns>
        private string GetNowTime()
        {
            DateTime now = DateTime.Now;
            return now.AddMilliseconds(-now.Millisecond).ToString();
        }

        /// <summary>
        /// 获取系统短日期
        /// </summary>
        /// <returns></returns>
        private string GetNowShortTime()
        {
            DateTime now = DateTime.Now;
            return now.AddMilliseconds(-now.Millisecond).ToString("T");
        }

        /// <summary>
        /// 获取服务器的延迟
        /// </summary>
        /// <param name="addr">服务器IP地址</param>
        /// <returns></returns>
        private int GetServerPing(string addr)
        {
            Ping pingSender = new();
            PingOptions options = new()
            {
                DontFragment = true
            };
            string data = "getserverping";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(addr, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                return Convert.ToInt32(reply.RoundtripTime);
            }
            return -1;
        }

        #endregion
    }
}