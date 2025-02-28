﻿namespace Milimoe.FunGame.Core.Library.Constant
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

        public const string FunGame_CopyRight = @"©2025 Milimoe. 米粒的糖果屋";

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
            return type + " [版本: " + FunGame_Version + FunGame_VersionPatch + "]\n" + (type.Equals(FunGame_Desktop) ? @"©" : "(C)") + "2022-Present Milimoe. 保留所有权利\n";
        }
    }
}
