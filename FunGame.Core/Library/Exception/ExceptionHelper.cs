using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Exception
{
    public static class ExceptionHelper
    {
        public static string GetStackTrace(this System.Exception e)
        {
            if (e.Message != null && e.Message != "")
            {
                return $"ERROR: {e.Message}\n{e.StackTrace}";
            }
            else
            {
                return $"ERROR: \n{e.StackTrace}";
            }
        }
    }
}
