using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;

namespace Milimoe.FunGame.Core.Library.Common.Addon.Example
{
    /// <summary>
    /// 必须继承基类：<see cref="Plugin"/><para/>
    /// 继承事件接口并实现其方法来使插件生效。例如继承：<seealso cref="ILoginEvent"/>
    /// </summary>
    public class ExamplePlugin : Plugin, ILoginEvent
    {
        public override string Name => "fungame.example.plugin";

        public override string Description => "My First Plugin";

        public override string Version => "1.0.0";

        public override string Author => "FunGamer";

        public void AfterLoginEvent(object sender, LoginEventArgs e)
        {

        }

        public void BeforeLoginEvent(object sender, LoginEventArgs e)
        {
            // 如果这里设置Cancel = true，将终止登录
            e.Cancel = true;
        }

        public void FailedLoginEvent(object sender, LoginEventArgs e)
        {

        }

        public void SucceedLoginEvent(object sender, LoginEventArgs e)
        {

        }
    }
}
