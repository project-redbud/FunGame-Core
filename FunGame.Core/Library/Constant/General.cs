using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Constant
{
    public class General
    {
        // Static Variable
        public static Empty EntityInstance { get; } = new();
        public static Encoding DefaultEncoding { get; } = Encoding.UTF8;

        // Const
        public const int MaxRetryTimes = 20;
        public const int MaxTask_1C2G = 10;
        public const int MaxTask_General = 20;
        public const int MaxTask_4C4G = 40;
    }
}
