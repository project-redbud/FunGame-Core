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
        public static Empty EntityInstance { get; } = new();
        public static Encoding DEFAULT_ENCODING { get; } = Encoding.UTF8;
    }
}
