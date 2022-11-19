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

        private System.Exception e { get; }

        public SystemError()
        {
            e = new System.Exception();
        }

        public SystemError(string Name)
        {
            this.Name = Name;
            e = new System.Exception(Name);
        }

        public string GetStackTrace()
        {
            return e.GetStackTrace();
        }
    }
}
