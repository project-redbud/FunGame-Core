namespace Milimoe.FunGame.Core.Interface
{
    /// <summary>
    /// 服务端和客户端都应该实现这个接口，用于初始化支持的Mod列表
    /// </summary>
    public interface IGameModeSupported
    {
        public string[] GameModeList { get; }
        public string[] GameMapList { get; }
    }
}
