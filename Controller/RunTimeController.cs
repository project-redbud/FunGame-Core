using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Controller
{
    public abstract class RunTimeController
    {
        public abstract bool Connected { get; }

        public abstract Task<ConnectResult> Connect();

        public abstract bool Disconnect();

        public abstract bool Close(Exception? e = null);

        public abstract bool Error(Exception e);

        public abstract Task AutoLogin(string Username, string Password, string AutoKey);

        public abstract void WritelnSystemInfo(string msg);

        public abstract DataRequest NewDataRequest(DataRequestType RequestType);
    }
}
