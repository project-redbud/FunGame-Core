using System.Collections;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.Addon.Example
{
    /// <summary>
    /// 建议使用一个类来存储常量，方便重用
    /// </summary>
    public class ExampleGameModuleConstant
    {
        public static GameModuleDepend GameModuleDepend => _depends;
        public const string ExampleGameModule = "fungame.example.gamemodule";
        public const string ExampleMap = "fungame.example.gamemap";
        public const string ExampleCharacter = "fungame.example.character";
        public const string ExampleSkill = "fungame.example.skill";
        public const string ExampleItem = "fungame.example.item";

        private static readonly string[] Maps = [ExampleMap];
        private static readonly string[] Characters = [ExampleCharacter];
        private static readonly string[] Skills = [ExampleSkill];
        private static readonly string[] Items = [ExampleItem];
        private static readonly GameModuleDepend _depends = new(Maps, Characters, Skills, Items);
    }

    /// <summary>
    /// 模组：必须继承基类：<see cref="GameModule"/><para/>
    /// 继承事件接口并实现其方法来使模组生效。例如继承：<seealso cref="IGamingUpdateInfoEvent"/><para/>
    /// </summary>
    public class ExampleGameModule : GameModule, IGamingUpdateInfoEvent
    {
        public override string Name => ExampleGameModuleConstant.ExampleGameModule;

        public override string Description => "My First GameModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override string DefaultMap => GameModuleDepend.MapsDepend.Length > 0 ? GameModuleDepend.MapsDepend[0] : "";

        public override GameModuleDepend GameModuleDepend => ExampleGameModuleConstant.GameModuleDepend;

        public override RoomType RoomType => RoomType.Mix;

        public ExampleGameModule()
        {
            // 构造函数中可以指定模组连接到哪个模组服务器。
            // 如果你使用自己的，保持默认即可：删除下面两行，并将模组服务器的名称设置为与此模组的名称相同
            IsConnectToOtherServerModule = true;
            AssociatedServerModuleName = ExampleGameModuleConstant.ExampleGameModule;
        }

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
            // 客户端做好准备后，等待服务器的消息通知，下面可以根据需求进一步处理
        }

        public override void StartUI(params object[] args)
        {
            // 如果你是一个WPF或者Winform项目，可以在这里启动你的界面
            // 如果没有，则不需要重写此方法
        }

        public void GamingUpdateInfoEvent(object sender, GamingEventArgs e, Hashtable data)
        {
            // 在下方的Server示例中，服务器发来的data中，包含check字符串，因此客户端要主动发起确认连接的请求。
            if (data.ContainsKey("info_type"))
            {
                // 反序列化得到指定key的值
                string info_type = DataRequest.GetHashtableJsonObject<string>(data, "info_type") ?? "";
                if (info_type == "check")
                {
                    Guid token = DataRequest.GetHashtableJsonObject<Guid>(data, "connect_token");
                    // 发起连接确认请求
                    DataRequest request = Controller.NewDataRequest(GamingType.Connect);
                    // 传递参数
                    request.AddRequestData("username", ((Gaming)sender).CurrentUser.Username);
                    request.AddRequestData("connect_token", token);
                    if (request.SendRequest() == RequestResult.Success)
                    {
                        string msg = request.GetResult<string>("msg") ?? "";
                        Controller.WriteLine(msg);
                    }
                    request.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// 模组服务器：必须继承基类：<see cref="GameModuleServer"/><para/>
    /// 使用switch块分类处理 <see cref="GamingType"/>。
    /// </summary>
    public class ExampleGameModuleServer : GameModuleServer
    {
        /// <summary>
        /// 注意：服务器模组的名称必须和模组名称相同。除非你指定了 <see cref="GameModule.IsConnectToOtherServerModule"/> 和 <see cref="GameModule.AssociatedServerModuleName"/>
        /// </summary>
        public override string Name => ExampleGameModuleConstant.ExampleGameModule;

        public override string Description => "My First GameModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override string DefaultMap => GameModuleDepend.MapsDepend.Length > 0 ? GameModuleDepend.MapsDepend.First() : "";

        public override GameModuleDepend GameModuleDepend => ExampleGameModuleConstant.GameModuleDepend;

        protected Room Room = General.HallInstance;
        protected List<User> Users = [];
        protected IServerModel? RoomMaster;
        protected Dictionary<string, IServerModel> All = [];

        public override bool StartServer(string GameModule, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> ServerModels, params object[] Args)
        {
            // 将参数转为本地属性
            this.Room = Room;
            this.Users = Users;
            RoomMaster = RoomMasterServerModel;
            All = ServerModels;
            // 创建一个线程执行Test()
            TaskUtility.NewTask(Test).OnError(Controller.Error);
            return true;
        }

        private readonly List<User> ConnectedUser = [];
        private readonly Dictionary<string, Hashtable> UserData = [];

        private async Task Test()
        {
            Controller.WriteLine("欢迎各位玩家进入房间 " + Room.Roomid + " 。");

            // 通常，我们可以对客户端的连接状态进行确认，此方法展示如何确认客户端的连接
            // 有两种确认的方式，1是服务器主动确认，2是客户端发起确认
            // 在FunGame项目中，建议永远使用客户端主动发起请求，因为服务器主动发起的实现难度较高
            // 下面的演示基于综合的两种情况：服务器主动发送通知，客户端收到后，发起确认
            // UpdateInfo是一个灵活的类型。如果发送check字符串，意味着服务器要求客户端发送确认
            Hashtable data = [];
            data.Add("info_type", "check");

            // 进阶示例：传递一个token，让客户端返回
            Guid token = Guid.NewGuid();
            data.Add("connect_token", token);

            // 我们保存到字典UserData中，这样可以方便跨方法检查变量
            foreach (string username in Users.Select(u => u.Username).Distinct())
            {
                if (UserData.TryGetValue(username, out Hashtable? value))
                {
                    value.Add("connect_token", token);
                }
                else
                {
                    UserData.Add(username, []);
                    UserData[username].Add("connect_token", token);
                }
            }
            SendAllGamingMessage(GamingType.UpdateInfo, data);

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
                    // 如果需要处理客户端传递的参数：获取与客户端约定好的参数key对应的值
                    string un = NetworkUtility.JsonDeserializeFromHashtable<string>(data, "username") ?? "";
                    Guid token = NetworkUtility.JsonDeserializeFromHashtable<Guid>(data, "connect_token");
                    if (un == username && UserData.TryGetValue(username, out Hashtable? value) && value != null && (value["connect_token"]?.Equals(token) ?? false))
                    {
                        ConnectedUser.Add(Users.Where(u => u.Username == username).First());
                        Controller.WriteLine(username + " 已经连接。");
                    }
                    else Controller.WriteLine(username + " 确认连接失败！");
                    break;
            }

            return result;
        }

        // === 下面是一些常用的工具方法，用于服务器给客户端发送消息，可以直接添加到你的项目中 === //

        protected void SendAllGamingMessage(GamingType type, Hashtable data)
        {
            // 循环服务线程，向所有玩家发送局内消息
            foreach (IServerModel s in All.Values)
            {
                if (s != null && s.Socket != null)
                {
                    s.Send(s.Socket, SocketMessageType.Gaming, type, data);
                }
            }
        }

        protected void SendGamingMessage(string username, GamingType type, Hashtable data)
        {
            // 向指定玩家发送局内消息
            IServerModel s = All[username];
            if (s != null && s.Socket != null)
            {
                s.Send(s.Socket, SocketMessageType.Gaming, type, data);
            }
        }

        protected void SendAll(SocketMessageType type, params object[] args)
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

        protected void Send(string username, SocketMessageType type, params object[] args)
        {
            // 向指定玩家发送消息
            IServerModel s = All[username];
            if (s != null && s.Socket != null)
            {
                s.Send(s.Socket, type, args);
            }
        }
    }

    /// <summary>
    /// 地图：必须继承基类：<see cref="GameMap"/><para/>
    /// </summary>
    public class ExampleGameMap : GameMap
    {
        public override string Name => ExampleGameModuleConstant.ExampleMap;

        public override string Description => "My First GameMap";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override float Length => 12.0f;

        public override float Width => 12.0f;

        public override float Height => 6.0f;

        public override float Size => 4.0f;
    }

    /// <summary>
    /// 角色：必须继承基类：<see cref="CharacterModule"/><para/>
    /// </summary>
    public class ExampleCharacterModule : CharacterModule
    {
        public override string Name => ExampleGameModuleConstant.ExampleCharacter;

        public override string Description => "My First CharacterModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override List<Character> Characters
        {
            get
            {
                List<Character> list = [];
                // 构建一个你想要的角色
                Character c = Factory.GetCharacter();
                c.Name = "Oshima";
                c.FirstName = "Shiya";
                c.NickName = "OSM";
                c.MagicType = MagicType.PurityNatural;
                c.InitialHP = 30;
                c.InitialSTR = 20;
                c.InitialAGI = 10;
                c.InitialINT = 5;
                c.InitialATK = 100;
                c.InitialDEF = 10;
                list.Add(c);
                return list;
            }
        }
    }

    /// <summary>
    /// 技能：必须继承基类：<see cref="SkillModule"/><para/>
    /// </summary>
    public class ExampleSkillModule : SkillModule
    {
        public override string Name => ExampleGameModuleConstant.ExampleSkill;

        public override string Description => "My First SkillModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override List<Skill> Skills
        {
            get
            {
                List<Skill> list = [];
                // 技能应该在GameModule中新建类继承Skill实现，再自行构造。
                return list;
            }
        }

        public override Skill? GetSkill(long id, string name)
        {
            // 此方法将根据id和name，返回一个你继承实现了的类对象。
            return Factory.GetSkill();
        }
    }

    /// <summary>
    /// 物品：必须继承基类：<see cref="ItemModule"/><para/>
    /// </summary>
    public class ExampleItemModule : ItemModule
    {
        public override string Name => ExampleGameModuleConstant.ExampleItem;

        public override string Description => "My First ItemModule";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override List<Item> Items
        {
            get
            {
                List<Item> list = [];
                // 物品应该在GameModule中新建类继承Item实现，再自行构造。
                return list;
            }
        }

        public override Item? GetItem(long id, string name)
        {
            // 此方法将根据id和name，返回一个你继承实现了的类对象。
            return Factory.GetItem();
        }
    }
}
