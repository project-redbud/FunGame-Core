using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Exception
{
    public class SystemError : System.Exception
    {
        public string Name { get; } = "";

        public new string StackTrace { get => base.StackTrace ?? ""; }

        public SystemError() { }

        public SystemError(string Name)
        {
            this.Name = Name;
        }

        public string GetStackTrace()
        {
            return Name + "\r\n" + StackTrace;
        }
    }
}
