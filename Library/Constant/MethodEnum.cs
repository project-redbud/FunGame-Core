/**
 * 此文件保存Method（方法）的枚举
 */
namespace Milimoe.FunGame.Core.Library.Constant
{
    /// <summary>
    /// 配合 <see cref="InterfaceType"/> <see cref="InterfaceSet"/> 使用，也别忘了修改 <see cref="Api.Utility.Implement"/>
    /// </summary>
    public enum InterfaceMethod
    {
        RemoteServerIP,
        DBConnection,
        GetServerSettings,
        SecretKey
    }
}
