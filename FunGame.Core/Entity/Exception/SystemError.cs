using Milimoe.FunGame.Core.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Exception
{
    public class SystemError : System.Exception
    {
        public string Name { get; set; } = "";

        public new string StackTrace { get => base.StackTrace ?? ""; }

        public string GetStackTrace()
        {
            return Name + "\r\n" + StackTrace;
        }
    }
}
