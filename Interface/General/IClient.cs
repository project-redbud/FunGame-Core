namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 这是最基本的接口，要求客户端实现
    /// </summary>
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
