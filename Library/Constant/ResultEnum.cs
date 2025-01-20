/**
 * 此文件保存Result（结果）的枚举
 */
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

    public enum RequestResult
    {
        Success,
        Fail,
        Missing
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

    public enum MailSendResult
    {
        Success,
        Fail,
        NotSend
    }

    public enum DamageResult
    {
        Normal,
        Critical,
        Evaded
    }

    public enum RedeemResult
    {
        Success,
        StockNotEnough,
        PointsNotEnough
    }
}
