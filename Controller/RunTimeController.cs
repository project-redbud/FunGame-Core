using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Controller
{
    public abstract class RunTimeController
    {
        public abstract Task<ConnectResult> Connect();
        public abstract void Disconnect();
    }
}
