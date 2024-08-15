namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 服务器需要实现此接口
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// 一个服务器标识秘钥
        /// </summary>
        public string SecretKey { get; }
    }
}
