using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Desktop.Entity.Component;
using Milimoe.FunGame.Desktop.Others;
using Milimoe.FunGame.Desktop.Utils;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.UI
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
        private SocketHelper? SocketHelper = null; // ScoketHelper

        /**
         * 定义委托
         * 子线程操作窗体控件时，先实例化Action，然后Invoke传递出去。
         */
        Action<int, object[]?>? StartMatch_Action = null;
        Action<int, object[]?>? CreateRoom_Action = null;
        Action<Main?>? SocketHelper_Action = null;
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
            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
            SetRoomid("-1"); // 房间号初始化
            ShowFunGameInfo(); // 显示FunGame信息
            GetFunGameConfig(); // 获取FunGame配置
            if (Others.Config.FunGame_isAutoConnect) GetServerConnection(); // 开始连接服务器
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 提供公共方法给SocketHelper
        /// </summary>
        /// <param name="SocketHelper"></param>
        /// <param name="msg"></param>
        /// <param name="needTime"></param>
        /// <returns></returns>
        public object? GetMessage(SocketHelper SocketHelper, string? msg, bool needTime = false, object[]? objs = null)
        {
            try
            {
                if (msg != null)
                {
                    switch (msg)
                    {
                        case Others.Constant.SocketHelper_SetGreen:
                            Others.Config.FunGame_isRetrying = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Green);
                                SetButtonEnableIfLogon(true, ClientState.Online);
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            Others.Config.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            break;
                        case Others.Constant.SocketHelper_SetGreenAndPing:
                            Others.Config.FunGame_isRetrying = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Green, ping: NetworkUtility.GetServerPing(Others.Constant.SERVER_IPADRESS));
                                SetButtonEnableIfLogon(true, ClientState.Online);
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            Others.Config.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            break;
                        case Others.Constant.SocketHelper_SetYellow:
                            Others.Config.FunGame_isRetrying = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Yellow);
                                SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            Others.Config.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            break;
                        case Others.Constant.SocketHelper_WaitConnectAndSetYellow:
                            Others.Config.FunGame_isRetrying = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Yellow);
                                SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            Others.Config.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            if (SocketHelper != null && Others.Config.FunGame_isAutoConnect)
                            {
                                // 自动连接服务器
                                GetServerConnection();
                            }
                            break;
                        case Others.Constant.SocketHelper_WaitLoginAndSetYellow:
                            Others.Config.FunGame_isRetrying = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Yellow, true);
                                SetButtonEnableIfLogon(false, ClientState.WaitLogin);
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            Others.Config.FunGame_isConnected = true;
                            NOW_CONNECTEDRETRY = 0;
                            break;
                        case Others.Constant.SocketHelper_SetRed:
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Red);
                                SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            Others.Config.FunGame_isConnected = false;
                            break;
                        case Others.Constant.SocketHelper_Disconnected:
                            Others.Config.FunGame_isRetrying = false;
                            Others.Config.FunGame_isConnected = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Red);
                                SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                                LogoutAccount();
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            if (Others.Config.FunGame_isAutoRetry && NOW_CONNECTEDRETRY <= MAX_CONNECTEDRETRY)
                            {
                                Task.Run(() =>
                                {
                                    Thread.Sleep(5000);
                                    if (Others.Config.FunGame_isAutoRetry) Connect(); // 再次判断是否开启自动重连
                                });
                                if (needTime)
                                    throw new Exception(DateTimeUtility.GetNowShortTime() + "\nERROR：连接服务器失败，5秒后自动尝试重连。");
                                else
                                    throw new Exception("ERROR：连接服务器失败，5秒后自动尝试重连。");
                            }
                            else
                                if (needTime)
                                throw new Exception(DateTimeUtility.GetNowShortTime() + "\nERROR：无法连接至服务器，请检查你的网络连接。");
                            else
                                throw new Exception("ERROR：无法连接至服务器，请检查你的网络连接。");
                        case Others.Constant.SocketHelper_Disconnect:
                            Others.Config.FunGame_isAutoRetry = false;
                            Others.Config.FunGame_isRetrying = false;
                            Others.Config.FunGame_isAutoConnect = false;
                            Others.Config.FunGame_isAutoLogin = false;
                            Others.Config.FunGame_isConnected = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Yellow);
                                SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                                LogoutAccount();
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            break;
                        case Others.Constant.SocketHelper_LogOut:
                            Others.Config.FunGame_isRetrying = false;
                            Others.Config.FunGame_isConnected = false;
                            Others.Config.FunGame_isAutoLogin = false;
                            SocketHelper_Action = (main) =>
                            {
                                SetServerStatusLight((int)LightType.Yellow);
                                SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                                LogoutAccount();
                            };
                            if (InvokeRequired)
                                BeginInvoke(SocketHelper_Action, this);
                            else
                                SocketHelper_Action(this);
                            if (Others.Config.FunGame_isAutoConnect)
                            {
                                NOW_CONNECTEDRETRY = -1;
                                Task.Run(() =>
                                {
                                    Thread.Sleep(1000);
                                    Connect();
                                });
                            }
                            break;
                        case Others.Constant.SocketHelper_GetUser:
                            if (Usercfg.LoginUser != null)
                                return Usercfg.LoginUser;
                            return null;
                        case Others.Constant.SocketHelper_SetUser:
                            if (objs != null && objs.Length > 1)
                            {
                                if (InvokeRequired)
                                    BeginInvoke(SetLoginUser, objs);
                                else
                                    SetLoginUser(objs);
                            }
                            return null;
                        case Others.Constant.SocketHelper_SetNotice:
                            Action action = () =>
                            {
                                NoticeText.Text = Others.Config.FunGame_Notice;
                                if (SocketHelper != null && Others.Config.FunGame_isAutoLogin)
                                {
                                    // 自动登录
                                    SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.Login);
                                }
                            };
                            if (InvokeRequired)
                                BeginInvoke(action);
                            else
                                action();
                            return null;
                        default:
                            if (needTime)
                                WritelnGameInfo(SocketHelper, DateTimeUtility.GetNowShortTime() + msg);
                            else
                                WritelnGameInfo(SocketHelper, msg);
                            return null;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                WritelnGameInfo(SocketHelper, e.Message != null ? e.Message + "\n" + e.StackTrace : "" + e.StackTrace);
                GetMessage(SocketHelper, Others.Constant.SocketHelper_SetRed);
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
                string? ipaddress = (string?)Others.Constant.ReflectionHelper.GetFunGameImplValue((int)InterfaceType.IClient, (int)InterfaceMethod.RemoteServerIP); // 获取服务器IP
                if (ipaddress != null)
                {
                    string[] s = ipaddress.Split(':');
                    if (s != null && s.Length > 1)
                    {
                        Others.Constant.SERVER_IPADRESS = s[0];
                        Others.Constant.SERVER_PORT = Convert.ToInt32(s[1]);
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
            if (Others.Constant.SERVER_IPADRESS.Equals("") || Others.Constant.SERVER_PORT <= 0)
            {
                ShowMessage.ErrorMessage("查找可用的服务器失败！");
                return;
            }
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
                    if (!Others.Config.FunGame_isConnected)
                    {
                        NOW_CONNECTEDRETRY++;
                        if (NOW_CONNECTEDRETRY == 0)
                            WritelnGameInfo(DateTimeUtility.GetNowTime() + " >> 开始连接服务器...");
                        else
                            WritelnGameInfo(DateTimeUtility.GetNowTime() + " >> 第" + NOW_CONNECTEDRETRY + "次重试连接服务器...");
                        if (NOW_CONNECTEDRETRY + 1 > MAX_CONNECTEDRETRY) // 判断重连次数是否达到上限
                        {
                            WritelnGameInfo("ERROR：无法连接至服务器，请检查网络并重启游戏再试。");
                            return;
                        }
                        SocketHelper_Action = (main) =>
                        {
                            try
                            {
                                if (main != null)
                                {
                                    if (SocketHelper != null)
                                    {
                                        SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.CloseSocket);
                                        SocketHelper = null;
                                    }
                                    Others.Config.FunGame_isRetrying = true;
                                    Application.DoEvents();
                                    SocketHelper = new SocketHelper(main);
                                    SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.CreateSocket); // Invoke -> CreateSocket
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
                                BeginInvoke(SocketHelper_Action, main);
                            }
                            else
                            {
                                SocketHelper_Action(main);
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
        /// 获取FunGame配置文件设定
        /// </summary>
        private void GetFunGameConfig()
        {
            try
            {
                if (INIHelper.ExistINIFile())
                {
                    string isAutoConncet = INIHelper.ReadINI("Config", "AutoConnect");
                    string isAutoLogin = INIHelper.ReadINI("Config", "AutoLogin");
                    if (isAutoConncet != null && !isAutoConncet.Equals("") && (isAutoConncet.Equals("false") || isAutoConncet.Equals("true")))
                        Others.Config.FunGame_isAutoConnect = Convert.ToBoolean(isAutoConncet);
                    else throw new Exception("ERROR: 读取配置文件出错，参数格式不正确");
                    if (isAutoLogin != null && !isAutoLogin.Equals("") && (isAutoLogin.Equals("false") || isAutoLogin.Equals("true")))
                        Others.Config.FunGame_isAutoLogin = Convert.ToBoolean(isAutoLogin);
                    else throw new Exception("ERROR: 读取配置文件出错，参数格式不正确");
                }
                else
                {
                    INIHelper.Init(Others.Constant.FunGameType);
                    WritelnGameInfo(">> 首次启动，已自动为你创建配置文件。");
                    GetFunGameConfig();
                }
            }
            catch (Exception e)
            {
                WritelnGameInfo(DateTimeUtility.GetNowTime() + e.Message != null ? e.Message + "\n" + e.StackTrace : "" + e.StackTrace);
            }
        }

        /// <summary>
        /// 设置房间号和显示信息
        /// </summary>
        /// <param name="roomid"></param>
        private void SetRoomid(string roomid)
        {
            Others.Config.FunGame_Roomid = roomid;
            if (!roomid.Equals("-1"))
            {
                WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 加入房间");
                WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已加入房间 -> [ " + Others.Config.FunGame_Roomid + " ]");
                Room.Text = "[ 当前房间 ]\n" + Convert.ToString(Others.Config.FunGame_Roomid);
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
                Invoke(LoginAccount, objs);
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
        /// 由SocketHelper委托向消息队列输出一行文字
        /// </summary>
        /// <param name="SocketHelper"></param>
        /// <param name="msg"></param>
        private void WritelnGameInfo(SocketHelper SocketHelper, string msg)
        {
            if (SocketHelper != null && msg.Trim() != "")
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
            WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 离开房间");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已离开房间 -> [ " + Others.Config.FunGame_Roomid + " ]");
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
        private void SetButtonEnableIfLogon(bool isLogon, ClientState status)
        {
            switch (status)
            {
                case ClientState.Online:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Others.Constant.PresetOnineItems);
                    break;
                case ClientState.WaitConnect:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Others.Constant.PresetNoConnectItems);
                    break;
                case ClientState.WaitLogin:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Others.Constant.PresetNoLoginItems);
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
                        if (Others.Config.FunGame_Roomid.Equals("-1"))
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
                    if (Others.Config.FunGame_Roomid.Equals("-1"))
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
                    Others.Config.FunGame_isMatching = true;
                    int loop = 0;
                    string roomid = Convert.ToString(new Random().Next(1, 10000));
                    // 匹配中 匹配成功返回房间号
                    Task.Factory.StartNew(() =>
                    {
                        // 创建新线程，防止主界面阻塞
                        Thread.Sleep(3000);
                        while (loop < 10000 && Others.Config.FunGame_isMatching)
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
                    Others.Config.FunGame_isMatching = false;
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
                    Others.Config.FunGame_isMatching = false;
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
            ShowMessage.TipMessage("欢迎回来， " + Usercfg.LoginUserName + "！", "登录成功", 5);
        }

        /// <summary>
        /// 登出账号，显示登录按钮
        /// </summary>
        private void LogoutAccount()
        {
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
            if (!TalkText.Text.Trim().Equals("") && !TalkText.ForeColor.Equals(Color.DarkGray))
            {
                WritelnGameInfo((!Usercfg.LoginUserName.Equals("") ? DateTimeUtility.GetNowShortTime() + " [ " + Usercfg.LoginUserName + " ] 说： ": ":> ") + TalkText.Text);
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
            if (!Others.Config.FunGame_Roomid.Equals("-1"))
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
            WritelnGameInfo(FunGameEnum.GetInfo(Others.Constant.FunGameType));
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
                if (SocketHelper != null)
                {
                    SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.CloseSocket);
                    SocketHelper = null;
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
            WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 开始匹配");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 开始匹配");
            WriteGameInfo(">> 匹配参数：");
            if (!Others.Config.Match_Mix && !Others.Config.Match_Team && !Others.Config.Match_HasPass)
                WritelnGameInfo("无");
            else
            {
                WriteGameInfo((Others.Config.Match_Mix ? " 混战房间 " : "") + (Others.Config.Match_Team ? " 团队房间 " : "") + (Others.Config.Match_HasPass ? " 密码房间 " : ""));
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
            if (Others.Config.Match_Mix && Others.Config.Match_Team)
            {
                ShowMessage.WarningMessage("创建房间不允许同时勾选混战和团队！");
                return;
            }
            else if (Others.Config.Match_Mix && !Others.Config.Match_Team && !Others.Config.Match_HasPass)
            {
                roomtype = Others.Constant.GameMode_Mix;
            }
            else if (!Others.Config.Match_Mix && Others.Config.Match_Team && !Others.Config.Match_HasPass)
            {
                roomtype = Others.Constant.GameMode_Team;
            }
            else if (Others.Config.Match_Mix && !Others.Config.Match_Team && Others.Config.Match_HasPass)
            {
                roomtype = Others.Constant.GameMode_MixHasPass;
            }
            else if (!Others.Config.Match_Mix && Others.Config.Match_Team && Others.Config.Match_HasPass)
            {
                roomtype = Others.Constant.GameMode_TeamHasPass;
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
                if (SocketHelper == null || !SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.Logout))
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
            if (SocketHelper != null && Others.Config.FunGame_isConnected)
                SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.Login);
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
            if (CheckMix.Checked) Others.Config.Match_Mix = true;
            else Others.Config.Match_Mix = false;
        }

        /// <summary>
        /// 勾选团队选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTeam_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckTeam.Checked) Others.Config.Match_Team = true;
            else Others.Config.Match_Team = false;
        }

        /// <summary>
        /// 勾选密码选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckHasPass_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckHasPass.Checked) Others.Config.Match_HasPass = true;
            else Others.Config.Match_HasPass = false;
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
                case Others.Constant.FunGame_SignIn:
                    break;
                case Others.Constant.FunGame_ShowCredits:
                    break;
                case Others.Constant.FunGame_ShowStock:
                    break;
                case Others.Constant.FunGame_ShowStore:
                    break;
                case Others.Constant.FunGame_CreateMix:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoomState.Creating, new object[] { Others.Constant.GameMode_Mix });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoomState.Creating, new object[] { Others.Constant.GameMode_Mix });
                    }
                    break;
                case Others.Constant.FunGame_CreateTeam:
                    CreateRoom_Action = (i, objs) =>
                    {
                        CreateRoom_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(CreateRoom_Action, (int)CreateRoomState.Creating, new object[] { Others.Constant.GameMode_Team });
                    }
                    else
                    {
                        CreateRoom_Action((int)CreateRoomState.Creating, new object[] { Others.Constant.GameMode_Team });
                    }
                    break;
                case Others.Constant.FunGame_StartGame:
                    break;
                case Others.Constant.FunGame_AutoRetryOn:
                    WritelnGameInfo(">> 自动重连开启");
                    Others.Config.FunGame_isAutoRetry = true;
                    break;
                case Others.Constant.FunGame_AutoRetryOff:
                    WritelnGameInfo(">> 自动重连关闭");
                    Others.Config.FunGame_isAutoRetry = false;
                    break;
                case Others.Constant.FunGame_Retry:
                    if (!Others.Config.FunGame_isRetrying)
                    {
                        NOW_CONNECTEDRETRY = -1;
                        Connect();
                    }
                    else
                        WritelnGameInfo(">> 你不能在连接服务器的同时重试连接！");
                    break;
                case Others.Constant.FunGame_Connect:
                    if (!Others.Config.FunGame_isConnected)
                    {
                        NOW_CONNECTEDRETRY = -1;
                        GetServerConnection();
                    }
                    break;
                case Others.Constant.FunGame_Disconnect:
                    if (Others.Config.FunGame_isConnected && SocketHelper != null)
                    {
                        SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.Disconnect);
                    }
                    break;
                case Others.Constant.FunGame_DisconnectWhenNotLogin:
                    if (Others.Config.FunGame_isConnected && SocketHelper != null)
                    {
                        SocketHelper.GetSocketHelperMethod((int)SocketHelperMethod.CloseSocket);
                        GetMessage(SocketHelper, Others.Constant.SocketHelper_Disconnect);
                        WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " >> 你已成功断开与服务器的连接。 ");
                    }
                    break;
                case Others.Constant.FunGame_ConnectTo:
                    string msg = ShowMessage.InputMessage("请输入服务器IP地址和端口号，如: 127.0.0.1:22222。", "连接指定服务器");
                    if (msg.Equals("")) return;
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
                        return;
                    }
                    ErrorType ErrorType = NetworkUtility.IsServerAddress(ip, port);
                    if (ErrorType == Core.Library.Constant.ErrorType.None)
                    {
                        Others.Constant.SERVER_IPADRESS = ip;
                        Others.Constant.SERVER_PORT = port;
                        NOW_CONNECTEDRETRY = -1;
                        Connect();
                    }
                    else if (ErrorType == Core.Library.Constant.ErrorType.IsNotIP) ShowMessage.ErrorMessage("这不是一个IP地址！");
                    else if (ErrorType == Core.Library.Constant.ErrorType.IsNotPort) ShowMessage.ErrorMessage("这不是一个端口号！\n正确范围：1~65535");
                    else ShowMessage.ErrorMessage("格式错误！\n这不是一个服务器地址。");
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}