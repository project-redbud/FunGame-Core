using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Addon.Example
{
    /// <summary>
    /// 客户端插件，必须继承基类：<see cref="Plugin"/><para/>
    /// 继承事件接口并实现其方法来使插件生效。例如继承：<seealso cref="IConnectEvent"/>
    /// </summary>
    public class ExamplePlugin : Plugin, IConnectEvent
    {
        public override string Name => "fungame.example.plugin";

        public override string Description => "My First Plugin";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public void AfterConnectEvent(object sender, ConnectEventArgs e)
        {
            // 每个事件接口都有 Before/After 两个介入点，根据自身需求实现即可
            // 可以在连接服务器完毕后做一些处理...
        }

        public void BeforeConnectEvent(object sender, ConnectEventArgs e)
        {
            // 和服务器插件的联动示例
            // 假设服务器上有个名为“fungame.example.serverplugin”的服务器插件，当然也可以不指定，不指定时服务器会广播所有插件，性能可能较低
            DataRequest request = Controller.NewDataRequest(targetAddon: "fungame.example.serverplugin");
            long tick = DateTime.Now.Ticks;
            request.AddRequestData("event", "ping");
            request.AddRequestData("tick", tick);
            request.SendRequest();
            if (request.Result == RequestResult.Success)
            {
                long newTick = request.GetResult<long>("tick");
                Controller.WriteLine($"服务器延迟：{Math.Min(999, new TimeSpan(newTick - tick).TotalMilliseconds)}ms");
            }
        }
    }

    /// <summary>
    /// 服务器插件，必须继承基类：<see cref="ServerPlugin"/><para/>
    /// 同样需继承事件接口并实现其方法来使插件生效。例如继承：<seealso cref="ILoginEvent"/>
    /// </summary>
    public class ExampleServerPlugin : ServerPlugin, ILoginEvent
    {
        public override string Name => "fungame.example.serverplugin";

        public override string Description => "My First Server Plugin";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public override void ProcessInput(string input)
        {
            // 该方法用于接收服务器控制台的输入并处理
            if (input.Equals("info", StringComparison.CurrentCultureIgnoreCase))
            {
                // 使用PluginController的输出方式
                Controller.WriteLine($"This is {nameof(ExampleServerPlugin)}!!");
            }
        }

        public override void AfterLoad(ServerPluginLoader loader, params object[] objs)
        {
            // 该方法可在插件加载完毕后执行代码
        }

        public override Dictionary<string, object> HandleDataRequest(Dictionary<string, object> data, AddonDataRequestEventArgs e)
        {
            // 此方法接收并处理数据请求，我们先前在客户端插件中发送了数据请求，这里就要处理它
            // 需要注意，data 是通用的，如果服务器将数据请求广播到所有的插件，这些插件是无法区分客户端的请求类型是什么的，所以这一部分更偏向于“约定”
            // 可以约定好插件的名称，也可以约定好event，一切都完全由插件开发者掌控
            if (e.From == "fungame.example.plugin" && e.Target == Name)
            {
                string ent = NetworkUtility.JsonDeserializeFromDictionary<string>(data, "event") ?? "";
                if (ent.Equals("ping", StringComparison.CurrentCultureIgnoreCase))
                {
                    return new()
                    {
                        { "tick", DateTime.Now.Ticks }
                    };
                }
            }
            return [];
        }

        public void AfterLoginEvent(object sender, LoginEventArgs e)
        {

        }

        public void BeforeLoginEvent(object sender, LoginEventArgs e)
        {
            // 如果这里设置Cancel = true，将终止登录（可以加入自定义的验证机制等）
            // 例如SQLHelper的使用：
            if (Controller.SQLHelper != null)
            {
                // 服务器的标准功能会有登录检查，这里仅做一个示例
                DataRow? row = Controller.SQLHelper.ExecuteDataRow(UserQuery.Select_UserByUsername(Controller.SQLHelper, e.Username));
                if (row is null)
                {
                    e.Cancel = true;
                    e.EventMsg = "用户名不存在。";
                }
            }
        }
    }
}
