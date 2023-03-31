namespace Milimoe.FunGame.Core.Interface
{
    public interface IClient
    {
        public string FunGameIcon { get; }
        public string FunGameBackGround { get; }
        public string FunGameMainMusic { get; }
        public string FunGameMusic1 { get; }
        public string FunGameMusic2 { get; }
        public string FunGameMusic3 { get; }

        public string RemoteServerIP();
    }
}
