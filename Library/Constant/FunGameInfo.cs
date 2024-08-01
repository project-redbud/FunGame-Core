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

        public const string FunGame_CopyRight = @"©2024 Milimoe. 米粒的糖果屋";

        /// <summary>
        /// 添加-debug启动项将开启DebugMode（仅适用于Desktop或Console）
        /// <para>目前支持禁用心跳检测功能</para>
        /// </summary>
        public static bool FunGame_DebugMode { get; set; } = false;

        public const string FunGame_Core = "FunGame Core";
        public const string FunGame_Core_Api = "FunGame Core Api";
        public const string FunGame_Console = "FunGame Console";
        public const string FunGame_Desktop = "FunGame Desktop";
        public const string FunGame_Server = "FunGame Server Console";

        public const string FunGame_Version = "v1.0";
        public const string FunGame_VersionPatch = "";

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
            return type + " [版本: " + FunGame_Version + FunGame_VersionPatch + "]\n" + (type.Equals(FunGame_Desktop) ? @"©" : "(C)") + "2024 Milimoe. 保留所有权利\n";
        }
    }
}
