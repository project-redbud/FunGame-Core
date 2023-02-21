using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Desktop.Library
{
    public class Usercfg
    {
        /**
         * 玩家设定内容
         */
        public static User? LoginUser { get; set; } = null; // 已登录的用户
        public static string LoginUserName { get; set; } = ""; // 已登录用户名
    }
}
