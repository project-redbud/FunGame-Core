using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.Library.Interface
{
    public interface IMain
    {
        public bool Connected { get; }

        public bool GetServerConnection();
        public ConnectResult Connect();
        public void Disconnect();
        public void Disconnected();
        public void SetWaitConnectAndSetYellow();
        public void SetWaitLoginAndSetYellow();
        public void SetGreenAndPing();
        public void SetGreen();
        public void SetYellow();
        public void SetRed();
        public bool LogOut();
        public bool Close();
        public bool IntoRoom();
    }
}
