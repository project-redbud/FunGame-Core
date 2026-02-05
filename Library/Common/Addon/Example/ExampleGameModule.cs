using System.Collections.Concurrent;
using System.Text;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Interface.Base.Addons;
using Milimoe.FunGame.Core.Library.Common.Architecture;
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
    /// GameModule 是用于客户端的模组。每个模组都有一个对应的服务器模组，可以简单理解为“一种游戏模式”<para/>
    /// 必须继承基类：<see cref="GameModule"/><para/>
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

        public override int MaxUsers => 8;

        public override bool HideMain => false;

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
            /// 如果模组不依附 <see cref="Gaming"/> 类启动，或者没有UI，则不需要重写此方法
        }

        public void GamingUpdateInfoEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            // 在下方的Server示例中，服务器发来的data中，包含check字符串，因此客户端要主动发起确认连接的请求。
            if (data.ContainsKey("info_type"))
            {
                // 反序列化得到指定key的值
                string info_type = DataRequest.GetDictionaryJsonObject<string>(data, "info_type") ?? "";
                if (info_type == "check")
                {
                    Guid token = DataRequest.GetDictionaryJsonObject<Guid>(data, "connect_token");
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
    public class ExampleGameModuleServer : GameModuleServer, IHotReloadAware
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

        /// <summary>
        /// 创建一个工作类，接收服务器启动参数的同时，还能定义一些需要的属性
        /// </summary>
        /// <param name="obj"></param>
        private readonly struct ModuleServerWorker(GamingObject obj)
        {
            public GamingObject GamingObject { get; } = obj;
            public List<User> ConnectedUser { get; } = [];
            public List<Character> CharactersForPick { get; } = [];
            public Dictionary<string, Character> UserCharacters { get; } = [];
            public Dictionary<string, Dictionary<string, object>> UserData { get; } = [];
        }

        private ConcurrentDictionary<string, ModuleServerWorker> Workers { get; } = [];

        public override bool StartServer(GamingObject obj, params object[] args)
        {
            // 因为模组是单例的，需要为这个房间创建一个工作类接收参数，不能直接用本地变量处理
            ModuleServerWorker worker = new(obj);
            Workers[obj.Room.Roomid] = worker;
            // 创建一个线程执行Test()，因为这个方法必须立即返回
            TaskUtility.NewTask(async () => await Test(obj, worker)).OnError(Controller.Error);
            return true;
        }

        private async Task Test(GamingObject obj, ModuleServerWorker worker)
        {
            Controller.WriteLine("欢迎各位玩家进入房间 " + obj.Room.Roomid + " 。");

            // 通常，我们可以对客户端的连接状态进行确认，此方法展示如何确认客户端的连接
            // 有两种确认的方式，1是服务器主动确认，2是客户端发起确认
            // 在FunGame项目中，建议永远使用客户端主动发起请求，因为服务器主动发起的实现难度较高
            // 下面的演示基于综合的两种情况：服务器主动发送通知，客户端收到后，发起确认
            // UpdateInfo是一个灵活的类型。如果发送check字符串，意味着服务器要求客户端发送确认
            Dictionary<string, object> data = [];
            data.Add("info_type", "check");

            // 进阶示例：传递一个token，让客户端返回
            Guid token = Guid.NewGuid();
            data.Add("connect_token", token);

            // 我们保存到字典UserData中，这样可以方便跨方法检查变量
            foreach (string username in obj.Users.Select(u => u.Username).Distinct())
            {
                if (worker.UserData.TryGetValue(username, out Dictionary<string, object>? value))
                {
                    value.Add("connect_token", token);
                }
                else
                {
                    worker.UserData.Add(username, []);
                    worker.UserData[username].Add("connect_token", token);
                }
            }
            await SendGamingMessage(obj.All.Values, GamingType.UpdateInfo, data);

            // 新建一个线程等待所有玩家确认，如果超时则取消游戏，30秒
            // 每200ms确认一次，不需要太频繁
            await WaitForUsers(30, async () =>
            {
                if (worker.ConnectedUser.Count == obj.Users.Count)
                {
                    Controller.WriteLine("所有玩家都已经连接。");
                    return true;
                }
                return false;
            }, 200, async () =>
            {
                Controller.WriteLine("等待玩家连接超时，放弃该局游戏！", LogLevel.Warning);
                await CancelGame(obj, worker, "由于等待超时，游戏已取消!");
            }, async () =>
            {
                // 所有玩家都连接完毕了，可以建立一个回合制游戏了
                await StartGame(obj, worker);
            });
        }

        public override async Task<Dictionary<string, object>> GamingMessageHandler(IServerModel model, GamingType type, Dictionary<string, object> data)
        {
            Dictionary<string, object> result = [];
            // 获取model所在的房间工作类
            ModuleServerWorker worker = Workers[model.InRoom.Roomid];
            GamingObject obj = worker.GamingObject;
            string username = model.User.Username;

            switch (type)
            {
                case GamingType.Connect:
                    {
                        // 编写处理“连接”命令的逻辑
                        // 如果需要处理客户端传递的参数：获取与客户端约定好的参数key对应的值
                        string un = NetworkUtility.JsonDeserializeFromDictionary<string>(data, "username") ?? "";
                        Guid token = NetworkUtility.JsonDeserializeFromDictionary<Guid>(data, "connect_token");
                        if (un == username && worker.UserData.TryGetValue(username, out Dictionary<string, object>? value) && value != null && (value["connect_token"]?.Equals(token) ?? false))
                        {
                            worker.ConnectedUser.Add(obj.Users.Where(u => u.Username == username).First());
                            Controller.WriteLine(username + " 已经连接。");
                        }
                        else Controller.WriteLine(username + " 确认连接失败！", LogLevel.Warning);
                        break;
                    }
                case GamingType.PickCharacter:
                    {
                        // 客户端选完了角色这里就要处理了
                        long id = NetworkUtility.JsonDeserializeFromDictionary<long>(data, "id");
                        if (worker.CharactersForPick.FirstOrDefault(c => c.Id == id) is Character character)
                        {
                            // 如果有人选一样的，你还没有做特殊处理的话，为了防止意外，最好复制一份
                            worker.UserCharacters[username] = character.Copy();
                        }
                        break;
                    }
                case GamingType.Skill:
                    {
                        string e = NetworkUtility.JsonDeserializeFromDictionary<string>(data, "event") ?? "";
                        if (e.Equals("SelectSkillTargets", StringComparison.CurrentCultureIgnoreCase))
                        {
                            long caster = NetworkUtility.JsonDeserializeFromDictionary<long>(data, "caster");
                            long[] targets = NetworkUtility.JsonDeserializeFromDictionary<long[]>(data, "targets") ?? [];
                            // 接收客户端传来的目标序号并记录
                            if (worker.UserData.TryGetValue(username, out Dictionary<string, object>? value) && value != null)
                            {
                                value.Add("SkillTargets", targets);
                            }
                        }
                        break;
                    }
                default:
                    await Task.Delay(1);
                    break;
            }

            return result;
        }

        private async Task CancelGame(GamingObject obj, ModuleServerWorker worker, string reason)
        {
            // 通知所有玩家
            await SendAllTextMessage(obj, reason);
            // 结束
            SendEndGame(obj);
            worker.ConnectedUser.Clear();
            Workers.Remove(obj.Room.Roomid, out _);
        }

        private async Task WaitForUsers(int waitSeconds, Func<Task<bool>> waitSomething, int delay, Func<Task> onTimeout, Func<Task> onCompleted)
        {
            // 这是一个用于等待的通用辅助方法
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(waitSeconds));
            CancellationToken ct = cts.Token;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (await waitSomething())
                    {
                        await onCompleted();
                        return;
                    }
                    await Task.Delay(delay, ct);
                }
                catch (System.Exception e) when (e is not OperationCanceledException)
                {
                    Controller.Error(e);
                    await onTimeout();
                    return;
                }
            }

            // 异常和超时都走超时逻辑
            await onTimeout();
        }

        private async Task StartGame(GamingObject obj, ModuleServerWorker worker)
        {
            Dictionary<User, Character> characters = [];
            List<Character> characterPickeds = [];
            Dictionary<string, object> data = [];

            // 首先，让玩家们选择角色
            // 需要一个待选的角色池
            // 这些角色可以从工厂中获取，比如：
            Character character1 = Factory.OpenFactory.GetInstance<Character>(1, "", []);
            worker.CharactersForPick.Add(character1);
            // 或者在什么地方已经有个列表？则使用复制方法
            if (ModuleLoader != null && ModuleLoader.Characters.Count > 0)
            {
                CharacterModule characterModule = ModuleLoader.Characters.Values.First();
                Character character2 = characterModule.Characters.Values.FirstOrDefault()?.Copy() ?? Factory.GetCharacter();
                if (character2.Id > 0)
                {
                    worker.CharactersForPick.Add(character2);
                }
            }
            // 传整个对象或者id都可以，看你心情，推荐用id，轻量，方便
            data["list"] = worker.CharactersForPick.Select(c => c.Id);
            await SendGamingMessage(obj.All.Values, GamingType.PickCharacter, data);

            // 依然等待
            await WaitForUsers(30, async () =>
            {
                if (worker.UserCharacters.Count == obj.Users.Count)
                {
                    Controller.WriteLine("所有玩家都已经完成选择。");
                    return true;
                }
                return false;
            }, 200, async () =>
            {
                await CancelGame(obj, worker, "由于等待超时，游戏已取消!");
            }, async () =>
            {
                try
                {
                    // 得到一个最终列表
                    List<Character> finalList = [.. worker.UserCharacters.Values];

                    // 这里我们可以随意对角色们进行升级和赋能
                    int clevel = 60;
                    int slevel = 6;
                    int mlevel = 8;

                    foreach (Character c in finalList)
                    {
                        c.Level = clevel;
                        c.NormalAttack.Level = mlevel;
                        // 假设要给所有角色发一个编号为1的技能
                        Skill s = Factory.OpenFactory.GetInstance<Skill>(1, "", []);
                        s.Level = slevel;
                        c.Skills.Add(s);
                    }

                    // 重点，创建一个战斗队列
                    // 注意，整个游戏中，finalList及其内部角色对象的引用始终不变，请放心使用
                    MixGamingQueue queue = new(finalList, (str) =>
                    {
                        // 战斗日志可以直接通过传输信息的方式输出回客户端
                        _ = SendAllTextMessage(obj, str);
                    });

                    // 如果你需要开启战棋地图模式
                    GameMap? map = GameModuleDepend.Maps.FirstOrDefault();
                    if (map != null)
                    {
                        queue.LoadGameMap(map);
                    }

                    // 关键，监听任何事件
                    // 在客户端中，通过事件可以很方便地对UI进行操作以同步界面状态，而在服务端则需要套一层网络层
                    //queue.TurnStartEvent += Queue_TurnStartEvent;
                    //queue.DecideActionEvent += Queue_DecideActionEvent;
                    //queue.SelectNormalAttackTargetsEvent += Queue_SelectNormalAttackTargetsEvent;
                    //queue.SelectSkillEvent += Queue_SelectSkillEvent;
                    //queue.SelectNonDirectionalSkillTargetsEvent += Queue_SelectNonDirectionalSkillTargetsEvent;
                    //queue.SelectItemEvent += Queue_SelectItemEvent;
                    //queue.QueueUpdatedEvent += Queue_QueueUpdatedEvent;
                    //queue.TurnEndEvent += Queue_TurnEndEvent;

                    // 我们示范两个事件，一是选择技能目标，需要和客户端交互的事件
                    queue.SelectSkillTargetsEvent += (queue, caster, skill, enemys, teammates, castRange) =>
                    {
                        /// 如果你的逻辑都写在 <see cref="ModuleServerWorker"/> 里就不用这么麻烦每次都传 obj 和 worker 了。
                        return Queue_SelectSkillTargetsEvent(worker, caster, skill, enemys, teammates, castRange);
                    };

                    // 二是角色行动完毕，需要通知客户端更新状态的事件
                    queue.CharacterActionTakenEvent += Queue_CharacterActionTakenEvent;

                    // 战棋地图模式需要额外绑定的事件（如果你在map类里没有处理的话，这里还可以处理）
                    if (queue.Map != null)
                    {
                        //queue.SelectTargetGridEvent += Queue_SelectTargetGridEvent;
                        //queue.CharacterMoveEvent += Queue_CharacterMoveEvent;
                    }

                    queue.InitActionQueue();
                    // 这里我们仅演示自动化战斗，指令战斗还需要实现其他的消息处理类型/事件
                    // 自动化战斗时上述绑定的事件可能不会触发，参见GamingQueue的内部实现
                    queue.SetCharactersToAIControl(cancel: false, finalList);

                    // 总游戏时长
                    double totalTime = 0;
                    // 总死亡数
                    int deaths = 0;

                    // 总回合数
                    int max = 999;
                    int i = 1;
                    while (i < max)
                    {
                        if (i == (max - 1))
                        {
                            // 为了防止回合数超标，游戏近乎死局，可以设置一个上限，然后随便让一个人赢
                            await SendAllTextMessage(obj, $"=== 终局审判 ===");
                            Dictionary<Character, double> hp = [];
                            foreach (Character c in finalList)
                            {
                                hp.TryAdd(c, Calculation.Round4Digits(c.HP / c.MaxHP));
                            }
                            double maxhp = hp.Values.Max();
                            Character winner = hp.Keys.Where(c => hp[c] == maxhp).First();
                            await SendAllTextMessage(obj, "[ " + winner + " ] 成为了天选之人！！");
                            foreach (Character c in finalList.Where(c => c != winner && c.HP > 0))
                            {
                                await SendAllTextMessage(obj, "[ " + winner + " ] 对 [ " + c + " ] 造成了 99999999999 点真实伤害。");
                                queue.DeathCalculation(winner, c);
                            }
                            queue.EndGameInfo(winner);
                            break;
                        }

                        // 检查是否有角色可以行动
                        Character? characterToAct = queue.NextCharacter();

                        // 处理回合
                        if (characterToAct != null)
                        {
                            await SendAllTextMessage(obj, $"=== Round {i++} ===");
                            await SendAllTextMessage(obj, "现在是 [ " + characterToAct + " ] 的回合！");

                            if (queue.Queue.Count == 0)
                            {
                                break;
                            }

                            bool isGameEnd = queue.ProcessTurn(characterToAct);
                            if (isGameEnd)
                            {
                                break;
                            }

                            queue.DisplayQueue();
                        }

                        // 时间流逝，这样能知道下一个是谁可以行动
                        totalTime += queue.TimeLapse();

                        if (queue.Eliminated.Count > deaths)
                        {
                            deaths = queue.Eliminated.Count;
                        }
                    }

                    await SendAllTextMessage(obj, "--- End ---");
                    await SendAllTextMessage(obj, "总游戏时长：" + Calculation.Round2Digits(totalTime));

                    // 赛后统计，充分利用 GamingQueue 提供的功能
                    await SendAllTextMessage(obj, "=== 伤害排行榜 ===");
                    int top = finalList.Count;
                    int count = 1;
                    foreach (Character character in queue.CharacterStatistics.OrderByDescending(d => d.Value.TotalDamage).Select(d => d.Key))
                    {
                        StringBuilder builder = new();
                        CharacterStatistics stats = queue.CharacterStatistics[character];
                        builder.AppendLine($"{count++}. [ {character.ToStringWithLevel()} ] （{stats.Kills} / {stats.Assists}）");
                        builder.AppendLine($"存活时长：{stats.LiveTime} / 存活回合数：{stats.LiveRound} / 行动回合数：{stats.ActionTurn} / 总计决策数：{stats.TurnDecisions} / 总计决策点：{stats.UseDecisionPoints}");
                        builder.AppendLine($"总计伤害：{stats.TotalDamage} / 总计物理伤害：{stats.TotalPhysicalDamage} / 总计魔法伤害：{stats.TotalMagicDamage}");
                        builder.AppendLine($"总承受伤害：{stats.TotalTakenDamage} / 总承受物理伤害：{stats.TotalTakenPhysicalDamage} / 总承受魔法伤害：{stats.TotalTakenMagicDamage}");
                        builder.Append($"每秒伤害：{stats.DamagePerSecond} / 每回合伤害：{stats.DamagePerTurn}");

                        await SendAllTextMessage(obj, builder.ToString());
                    }
                }
                catch (System.Exception e)
                {
                    TXTHelper.AppendErrorLog(e.ToString());
                    Controller.Error(e);
                }
                finally
                {
                    // 结束
                    SendEndGame(obj);
                    worker.ConnectedUser.Clear();
                    Workers.Remove(obj.Room.Roomid, out _);
                }
            });
        }

        private List<Character> Queue_SelectSkillTargetsEvent(ModuleServerWorker worker, Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange)
        {
            // 这是一个需要与客户端交互的事件，其他的选择事件与之做法相同
            // SyncAwaiter是一个允许同步方法安全等待异步任务完成的工具类
            return SyncAwaiter.WaitResult(RequestClientSelectSkillTargets(worker, caster, skill, enemys, teammates, castRange));
        }

        private async Task<List<Character>> RequestClientSelectSkillTargets(ModuleServerWorker worker, Character caster, Skill skill, List<Character> enemys, List<Character> teammates, List<Grid> castRange)
        {
            List<Character> selectTargets = [];
            Dictionary<string, object> data = [];
            data.Add("event", "SelectSkillTargets");
            data.Add("caster", caster.Id);
            data.Add("skill", skill.Id);
            data.Add("enemys", enemys.Select(c => c.Id));
            data.Add("teammates", teammates.Select(c => c.Id));
            data.Add("castRange", castRange.Select(g => g.Id));
            await SendGamingMessage(_clientModels, GamingType.Skill, data);
            await WaitForUsers(30, async () =>
            {
                string username = worker.UserCharacters.FirstOrDefault(kv => kv.Value == caster).Key;
                return worker.UserData.TryGetValue(username, out Dictionary<string, object>? value) && value != null && value.ContainsKey("SkillTargets");
            }, 200, async () => await Task.CompletedTask, async () =>
            {
                string username = worker.UserCharacters.FirstOrDefault(kv => kv.Value == caster).Key;
                if (worker.UserData.TryGetValue(username, out Dictionary<string, object>? value) && value != null && value.TryGetValue("SkillTargets", out object? value2) && value2 is long[] targets)
                {
                    selectTargets.AddRange(worker.UserCharacters.Values.Where(c => targets.Contains(c.Id)));
                }
            });
            return selectTargets;
        }

        private void Queue_CharacterActionTakenEvent(GamingQueue queue, Character actor, DecisionPoints dp, CharacterActionType type, RoundRecord record)
        {
            Dictionary<string, object> data = [];
            data.Add("event", "CharacterActionTaken");
            data.Add("actor", actor.Id);
            data.Add("dp", dp);
            data.Add("type", type);
            // 通知就行，无需等待
            _ = SendGamingMessage(_clientModels, GamingType.Round, data);
        }

        private async Task SendAllTextMessage(GamingObject obj, string str)
        {
            // 工具方法，向所有人推送文本消息
            Dictionary<string, object> data = [];
            data.Add("showmessage", true);
            data.Add("msg", str);
            await SendGamingMessage(obj.All.Values, GamingType.UpdateInfo, data);
        }

        protected HashSet<IServerModel> _clientModels = [];

        /// <summary>
        /// 匿名服务器允许客户端不经过FunGameServer的登录验证就能建立一个游戏模组连接<para/>
        /// 匿名服务器示例
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool StartAnonymousServer(IServerModel model, Dictionary<string, object> data)
        {
            // 可以做验证处理（这只是个演示，具体实现只需要双方约定，收发什么都无所谓）
            string access_token = NetworkUtility.JsonDeserializeFromDictionary<string>(data, "access_token") ?? "";
            if (access_token == "approval_access_token")
            {
                // 接收连接匿名服务器的客户端
                _clientModels.Add(model);
                return true;
            }
            return false;
        }

        public override void CloseAnonymousServer(IServerModel model)
        {
            // 移除客户端
            _clientModels.Remove(model);
        }

        /// <summary>
        /// 接收并处理匿名服务器消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<Dictionary<string, object>> AnonymousGameServerHandler(IServerModel model, Dictionary<string, object> data)
        {
            Dictionary<string, object> result = [];

            // 根据服务器和客户端的数据传输约定，自行处理 data，并返回。
            if (data.Count > 0)
            {
                await Task.Delay(1);
            }
            result.Add("msg", "匿名服务器已经收到消息了");

            return result;
        }

        /// <summary>
        /// 热更新示例：必须实现 <see cref="IHotReloadAware"/> 接口才会被热更新模式加载这个模组<para/>
        /// 如果想要实现端运行的所有模组都能热更新，那么这些模组都必须实现了这个接口（包括 <see cref="GameModule"/>，<see cref="GameMap"/>，<see cref="CharacterModule"/> 等等……）
        /// </summary>
        public void OnBeforeUnload()
        {
            // 这个方法会在模组被卸载前调用，因此，这里要清理一些状态，让框架可以正确卸载模组
            // 假设，这是个匿名服务器，因此它需要清理匿名连接
            GamingObjects.Clear();
            _ = Send(_clientModels, SocketMessageType.EndGame, Factory.GetRoom(), Factory.GetUser());
            IServerModel[] models = [.. _clientModels];
            foreach (IServerModel model in models)
            {
                model.NowGamingServer = null;
                CloseAnonymousServer(model);
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

        public override int Length => 12;

        public override int Width => 12;

        public override int Height => 6;

        public override float Size => 4.0f;

        public override GameMap InitGamingQueue(IGamingQueue queue)
        {
            // 因为模组在模组管理器中都是单例的，所以每次游戏都需要返回一个新的地图对象给队列
            GameMap map = new ExampleGameMap();
            map.Load();

            // 做一些绑定，以便介入游戏队列
            /// 但是，传入的 queue 可能不是 <see cref="GamingQueue"/>，要做类型检查
            // 不使用框架的实现时，需要地图作者与游戏队列的作者做好适配
            if (queue is GamingQueue gq)
            {
                gq.SelectTargetGridEvent += Gq_SelectTargetGrid;
            }

            return map;
        }

        private Grid Gq_SelectTargetGrid(GamingQueue queue, Character character, List<Character> enemys, List<Character> teammates, GameMap map, List<Grid> canMoveGrids)
        {
            // 介入选择，假设这里更新界面，让玩家选择目的地
            return Grid.Empty;
        }
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

        public override Dictionary<string, Character> Characters
        {
            get
            {
                Dictionary<string, Character> dict = [];
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
                dict.Add(c.Name, c);
                return dict;
            }
        }

        protected override Factory.EntityFactoryDelegate<Character> CharacterFactory()
        {
            // 上面示例用 Characters 是预定义的
            // 这里的工厂模式则是根据传进来的参数定制生成角色，只要重写这个方法就能注册工厂了
            return (id, name, args) =>
            {
                return null;
            };
        }

        public static Character CreateCharacter(long id, string name, Dictionary<string, object> args)
        {
            // 注册工厂后，后续创建角色只需要这样调用
            return Factory.OpenFactory.GetInstance<Character>(id, name, args);
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

        public override Dictionary<string, Skill> Skills
        {
            get
            {
                Dictionary<string, Skill> dict = [];
                /// 技能应该在新建类继承Skill实现，再自行构造并加入此列表。
                /// 技能的实现示例参见：<see cref="ExampleSkill"/>
                return dict;
            }
        }

        protected override Factory.EntityFactoryDelegate<Skill> SkillFactory()
        {
            // 注册一个工厂，根据id和name，返回一个你继承实现了的类对象。所有的工厂使用方法参考 Character，都是一样的
            return (id, name, args) =>
            {
                return null;
            };
        }

        protected override Factory.EntityFactoryDelegate<Effect> EffectFactory()
        {
            return (id, name, args) =>
            {
                // 以下是一个示例，实际开发中 id,name,args 怎么处置，看你心情
                Skill? skill = null;
                if (args.TryGetValue("skill", out object? value) && value is Skill s)
                {
                    skill = s;
                }
                skill ??= new OpenSkill(id, name, args);
                /// 如 <see cref="ExampleOpenItemByJson"/> 中所说，特效需要在工厂中注册，方便重用
                if (id == 1001)
                {
                    return new ExampleOpenEffectExATK2(skill, args);
                }
                return null;
            };
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

        public override Dictionary<string, Item> Items
        {
            get
            {
                Dictionary<string, Item> dict = [];
                /// 物品应该新建类继承Item实现，再自行构造并加入此列表。
                /// 物品的实现示例参见：<see cref="ExampleItem"/>
                return dict;
            }
        }

        protected override Factory.EntityFactoryDelegate<Item> ItemFactory()
        {
            // 注册一个工厂，根据id和name，返回一个你继承实现了的类对象。
            return (id, name, args) =>
            {
                return null;
            };
        }
    }
}
