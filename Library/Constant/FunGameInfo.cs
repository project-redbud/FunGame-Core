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

        public const string FunGame_CopyRight_Core = "©2023-Present Project Redbud and Contributors.\r\n©2022-2023 Milimoe.";
        public const string FunGame_CopyRight_Desktop = "©2025 Milimoe. 米粒的糖果屋";

        /// <summary>
        /// 添加-debug启动项将开启DebugMode（仅适用于Desktop或Console）
        /// <para>目前支持禁用心跳检测功能</para>
        /// </summary>
        public static bool FunGame_DebugMode { get; set; } = false;

        /// <summary>
        /// 核心库的版本号
        /// </summary>
        public static string FunGame_Version
        {
            get
            {
                string patch = FunGame_VersionPatch.StartsWith('.') ? FunGame_VersionPatch : $".{FunGame_VersionPatch}";
                return $"{FunGame_Version_Major}.{FunGame_Version_Minor}{patch}";
            }
        }

        public const string FunGame_Core = "FunGame Core";
        public const string FunGame_Core_Api = "FunGame Core Api";
        public const string FunGame_Console = "FunGame Console";
        public const string FunGame_Desktop = "FunGame Desktop";
        public const string FunGame_Server = "FunGame Server Console";

        public const int FunGame_Version_Major = 2;
        public const int FunGame_Version_Minor = 0;
        public const string FunGame_VersionPatch = "0-dev";
        public const string FunGame_Version_Build = "";

        public const string FunGameCoreTitle = @"  _____ _   _ _   _  ____    _    __  __ _____    ____ ___  ____  _____ 
 |  ___| | | | \ | |/ ___|  / \  |  \/  | ____|  / ___/ _ \|  _ \| ____|
 | |_  | | | |  \| | |  _  / _ \ | |\/| |  _|   | |  | | | | |_) |  _|  
 |  _| | |_| | |\  | |_| |/ ___ \| |  | | |___  | |__| |_| |  _ <| |___ 
 |_|    \___/|_| \_|\____/_/   \_\_|  |_|_____|  \____\___/|_| \_\_____|
                                                                        ";

        public const string FunGameServerTitle = @"  _____ _   _ _   _  ____    _    __  __ _____   ____  _____ ______     _______ ____  
 |  ___| | | | \ | |/ ___|  / \  |  \/  | ____| / ___|| ____|  _ \ \   / / ____|  _ \ 
 | |_  | | | |  \| | |  _  / _ \ | |\/| |  _|   \___ \|  _| | |_) \ \ / /|  _| | |_) |
 |  _| | |_| | |\  | |_| |/ ___ \| |  | | |___   ___) | |___|  _ < \ V / | |___|  _ < 
 |_|    \___/|_| \_|\____/_/   \_\_|  |_|_____| |____/|_____|_| \_\ \_/  |_____|_| \_\
                                                                                      ";

        public static string GetInfo(FunGame FunGameType, int major = 0, int minor = 0, string patch = "")
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
            if (major == 0) major = FunGame_Version_Major;
            if (minor == 0) minor = FunGame_Version_Minor;
            if (patch == "") patch = FunGame_VersionPatch;
            if (patch != "" && !patch.StartsWith('.')) patch = $".{patch}";
            return $"{FunGame_Core} [核心库版本: {FunGame_Version}]\r\n" +
                $"{(type.Equals(FunGame_Desktop) ? @"©" : "(C)")}2023-Present Project Redbud and Contributors.\r\n" +
                $"{(type.Equals(FunGame_Desktop) ? @"©" : "(C)")}2022-2023 Milimoe.\r\n" +
                $"\r\nThis software is released under the LGPLv3 license. See LICENSE for details.\r\n\r\n" +
                $"{type} [版本：{major}.{minor}{patch}]\r\n{(type.Equals(FunGame_Desktop) ? @"©" : "(C)")}2022-Present Milimoe.\r\n" +
                $"\r\nThis software is released under the MIT license. See LICENSE for details.\r\n";
        }
    }
}
