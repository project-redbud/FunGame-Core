using System.Collections;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

// 此演示包含GameModule、GameModuleServer、GameMap
namespace Milimoe.FunGame.Core.Library.Common.Addon.Example
{
    /// <summary>
    /// 建议使用一个类来存储常量，方便重用
    /// </summary>
    public class ExampleGameModuleConstant
    {
        public static GameModuleDepend GameModuleDepend => _depends;

        private static readonly string[] Maps = ["Example GameMap"];
        private static readonly string[] Characters = [];
        private static readonly string[] Items = [];
        private static readonly string[] Skills = [];
        private static readonly GameModuleDepend _depends = new(Maps, Characters, Items, Skills);
    }

    /// <summary>
    /// 模组：必须继承基类：<see cref="GameModule"/><para/>
    /// 继承事件接口并实现其方法来使模组生效。例如继承：<seealso cref="IGamingConnectEvent"/><para/>
    /// </summary>
    public class ExampleGameModule : GameModule, IGamingConnectEvent
    {
        public override string Name => "FunGame Example GameModule";

        public override string Description => "My First GameModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override string DefaultMap => GameModuleDepend.Maps.Length > 0 ? GameModuleDepend.Maps[0] : "";

        public override GameModuleDepend GameModuleDepend => ExampleGameModuleConstant.GameModuleDepend;

        public override RoomType RoomType => RoomType.Mix;

        protected Gaming? Instance;
        protected Room room = General.HallInstance;
        protected List<User> users = [];
        protected Dictionary<string, Character> characters = [];

        public override void StartGame(Gaming instance, params object[] args)
        {
            Instance = instance;
            // 取得房间玩家等信息
            GamingEventArgs eventArgs = instance.EventArgs;
            room = eventArgs.Room;
            users = eventArgs.Users;
            characters = eventArgs.Characters;
            // 客户端做好准备后，等待服务器的消息通知，下面可以根据需求进一步处理
        }

        public override void StartUI(params object[] args)
        {
            // 如果你是一个WPF或者Winform项目，可以在这里启动你的界面
            // 如果没有，则不需要重写此方法
        }

        public void BeforeGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            // 此方法预处理攻击消息
            // 如果这里将Cancel设置为true，那么这个方法结束后，后续的事件就会终止
            e.Cancel = true;
        }

        public void AfterGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {

        }

        public void SucceedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {

        }

        public void FailedGamingConnectEvent(object sender, GamingEventArgs e, Hashtable data, Hashtable result)
        {
            _ = DiscountGameModuleServer();
        }

        public async Task DiscountGameModuleServer()
        {
            // 这是一个主动请求服务器的示例：
            Api.Transmittal.DataRequest request = Controller.NewDataRequest(DataRequestType.Gaming);
            request.AddRequestData("type", GamingType.Disconnect);
            if (await request.SendRequestAsync() == RequestResult.Success)
            {
                string msg = request.GetResult<string>("msg") ?? string.Empty;
                Controller.WriteLine(msg);
            }
        }
    }

    /// <summary>
    /// 模组服务器：必须继承基类：<see cref="GameModuleServer"/><para/>
    /// 使用switch块分类处理 <see cref="GamingType"/>。
    /// </summary>
    public class ExampleGameModuleServer : GameModuleServer
    {
        public override string Name => "FunGame Example GameModule";

        public override string Description => "My First GameModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override string DefaultMap => GameModuleDepend.Maps.Length > 0 ? GameModuleDepend.Maps.First() : "";

        public override GameModuleDepend GameModuleDepend => ExampleGameModuleConstant.GameModuleDepend;

        protected Room Room = General.HallInstance;
        protected List<User> Users = [];
        protected IServerModel? RoomMaster;
        protected Dictionary<string, IServerModel> Others = [];
        protected Dictionary<string, IServerModel> All = [];

        public override bool StartServer(string GameModule, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> OthersServerModel, params object[] Args)
        {
            // 将参数转为本地属性
            this.Room = Room;
            this.Users = Users;
            RoomMaster = RoomMasterServerModel;
            Others = OthersServerModel;
            if (RoomMaster != null)
            {
                // 这里获得了每名玩家的服务线程，保存为一个字典
                All = OthersServerModel.ToDictionary(k => k.Key, v => v.Value);
                All.Add(RoomMaster.User.Username, RoomMaster);
            }
            // 创建一个线程执行Test()
            TaskUtility.NewTask(Test).OnError(Controller.Error);
            return true;
        }

        private readonly List<User> ConnectedUser = [];

        private async Task Test()
        {
            // 通常，我们可以对客户端的连接状态进行确认，此方法展示如何确认客户端的连接
            Controller.WriteLine("欢迎各位玩家进入房间 " + Room.Roomid + " 。");
            SendAll(SocketMessageType.Gaming, GamingType.Connect);
            // 新建一个线程等待所有玩家确认
            while (true)
            {
                if (ConnectedUser.Count == Users.Count) break;
                // 每200ms确认一次，不需要太频繁
                await Task.Delay(200);
            }
            Controller.WriteLine("所有玩家都已经连接。");
        }

        public override Hashtable GamingMessageHandler(string username, GamingType type, Hashtable data)
        {
            Hashtable result = [];

            switch (type)
            {
                case GamingType.Connect:
                    // 编写处理“连接”命令的逻辑
                    ConnectedUser.Add(Users.Where(u => u.Username == username).First());
                    Controller.WriteLine(username + "已经连接。");
                    break;
            }

            return result;
        }

        private void SendAll(SocketMessageType type, params object[] args)
        {
            // 循环服务线程，向所有玩家发送消息
            foreach (IServerModel s in All.Values)
            {
                if (s != null && s.Socket != null)
                {
                    s.Send(s.Socket, type, args);
                }
            }
        }
    }

    /// <summary>
    /// 地图：必须继承基类：<see cref="GameMap"/><para/>
    /// </summary>
    public class ExampleGameMap : GameMap
    {
        public override string Name => "Example GameMap";

        public override string Description => "My First GameMap";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override float Width => 12.0f;

        public override float Height => 12.0f;

        public override float Size => 4.0f;
    }
}
