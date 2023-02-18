namespace Milimoe.FunGame.Core.Library.Constant
{
    public enum MessageResult
    {
        OK,
        Cancel,
        Yes,
        No,
        Retry
    }

    public enum EventResult
    {
        Success,
        Fail,
        NoEventImplement
    }

    public enum SocketResult
    {
        Success,
        Fail,
        NotSent,
        NotReceived
    }

    public enum ConnectResult
    {
        Success,
        ConnectFailed,
        CanNotConnect,
        FindServerFailed
    }

    public enum SQLResult
    {
        Success,
        Fail,
        NotFound,
        SQLError,
        IsExist
    }
}
