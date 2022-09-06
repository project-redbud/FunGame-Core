using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunGame.Core.Api.Model.Enum
{
    /// <summary>
    /// 用于记录版本号和更新日志
    /// </summary>
    public static class FunGameEnums
    {
        public const string FunGame_Core = "FunGame Core";
        public const string FunGame_Core_Api = "FunGame Core Api";
        public const string FunGame_Console = "FunGame Console";
        public const string FunGame_Desktop = "FunGame Desktop";
        public const string FunGame_Server = "FunGame Server";
        
        public const int FirstVersion = 1;
        public const int SecondVersion = 0;
        public const int ThirdVersion = 0;

        public enum Patch
        {
            Latest = 20221001,
            Patch20220906 = 20220906
        }

        public enum History
        {
            Latest = 20221001,
            R20220906 = 20220906
        }

        public static string GetVersion()
        {
            return "=/=\\=/=\\=/=\\=/= > FunGame版本信息 < =\\=/=\\=/=\\=/=\\=" + "\n" +
                FunGame_Core + " -> v" + FirstVersion + "." + SecondVersion + ((int)Patch.Latest == (int)History.Latest ? " Patch" + (int)Patch.Latest : "") + "\n" +
                FunGame_Core_Api + " -> v" + FirstVersion + "." + SecondVersion + ((int)Patch.Latest == (int)History.Latest ? " Patch" + (int)Patch.Latest : "") + "\n" +
                FunGame_Desktop + " -> v" + FirstVersion + "." + SecondVersion + ((int)Patch.Latest == (int)History.Latest ? " Patch" + (int)Patch.Latest : "");
        }

        /**
         * 更新日志
         * 
         * 
         */
    }
}
