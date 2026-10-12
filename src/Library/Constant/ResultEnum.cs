/**
 * 此文件保存Result（结果）的枚举
 */
namespace FunGame.Core.Library.Constant
{
    public enum ConnectResult
    {
        Success,
        ConnectFailed,
        CanNotConnect,
        FindServerFailed
    }

    public enum DamageResult
    {
        Normal,
        Critical,
        Evaded,
        Shield,
        Immune
    }
}
