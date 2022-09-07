using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Core.Api.Model.Enum
{
    public static class FunGameEnums
    {
        private const string FunGame_Core = "FunGame Core";
        private const string FunGame_Core_Api = "FunGame Core Api";
        private const string FunGame_Console = "FunGame Console";
        private const string FunGame_Desktop = "FunGame Desktop";
        private const string FunGame_Server = "FunGame Server Console";

        private const string FunGame_Version = "v1.0";
        private const string FunGame_VersionPatch = "";

        public enum FunGame
        {
            FunGame_Core,
            FunGame_Core_Api,
            FunGame_Console,
            FunGame_Desktop,
            FunGame_Server
        }

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
            if (type.Equals(FunGame_Desktop))
                return type + " [ 版本: " + FunGame_Version + FunGame_VersionPatch + " ]\n©2022 Mili.cyou. 保留所有权利\n";
            else
                return type + " [ 版本: " + FunGame_Version + FunGame_VersionPatch + " ]\n(C)2022 Mili.cyou. 保留所有权利\n";
        }

        /**
         * 更新日志
         * 
         * 
         */
    }
}
