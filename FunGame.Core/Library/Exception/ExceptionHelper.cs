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
        public static string GetErrorInfo(this System.Exception e)
        {
            return (e.InnerException != null) ? $"InnerExceoption: {e.InnerException}\n{e}" : e.ToString();
        }
    }
}
