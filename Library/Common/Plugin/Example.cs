using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Plugin
{
    /// <summary>
    /// 必须继承基类：<see cref="BasePlugin"/><para/>
    /// 继承事件接口并实现其方法来使插件生效。例如继承：<seealso cref="ILoginEvent"/>
    /// </summary>
    public class Example : BasePlugin, ILoginEvent
    {
        public override string Name => "FunGame Example Plugin";

        public override string Description => "My First Plugin";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public EventResult AfterLoginEvent(object sender, LoginEventArgs e)
        {
            return EventResult.Success;
        }

        public EventResult BeforeLoginEvent(object sender, LoginEventArgs e)
        {
            return EventResult.Success;
        }

        public EventResult FailedLoginEvent(object sender, LoginEventArgs e)
        {
            return EventResult.Success;
        }

        public EventResult SucceedLoginEvent(object sender, LoginEventArgs e)
        {
            return EventResult.Success;
        }
    }
}
