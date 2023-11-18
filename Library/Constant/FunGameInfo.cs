namespace Milimoe.FunGame.Core.Library.Constant
{
    public class FunGameInfo
    {
        public enum FunGame
        {
            FunGame_Core,
            FunGame_Core_Api,
            FunGame_Console,
            FunGame_Desktop,
            FunGame_Server
        }

        public const string FunGame_CopyRight = @"©2023 Milimoe. 米粒的糖果屋";

        /// <summary>
        /// 添加-debug启动项将开启DebugMode（仅适用于Desktop或Console）
        /// <para>目前支持禁用心跳检测功能</para>
        /// </summary>
        public static bool FunGame_DebugMode { get; set; } = false;

        private const string FunGame_Core = "FunGame Core";
        private const string FunGame_Core_Api = "FunGame Core Api";
        private const string FunGame_Console = "FunGame Console";
        private const string FunGame_Desktop = "FunGame Desktop";
        private const string FunGame_Server = "FunGame Server Console";

        private const string FunGame_Version = "v1.0";
        private const string FunGame_VersionPatch = "";

        public static string GetInfo(FunGame FunGameType)
        {
            string type = FunGameType switch
            {
                FunGame.FunGame_Core => FunGame_Core,
                FunGame.FunGame_Core_Api => FunGame_Core_Api,
                FunGame.FunGame_Console => FunGame_Console,
                FunGame.FunGame_Desktop => FunGame_Desktop,
                FunGame.FunGame_Server => FunGame_Server,
                _ => ""
            };
            return type + " [ 版本: " + FunGame_Version + FunGame_VersionPatch + " ]\n" + (type.Equals(FunGame_Desktop) ? @"©" : "(C)") + "2023 Milimoe. 保留所有权利\n";
        }

        /**
         * 更新日志
         * 
         * 
         */
    }
}
