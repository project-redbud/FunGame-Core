using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Entity.Enum
{
    public enum MessageResult
    {
        OK,
        Cancel,
        Yes,
        No,
        Retry
    }

    public enum EventResult
    {
        Success,
        Fail
    }

    public enum ProxyResult
    {
        Success,
        Fail,
        NotFound
    }
}
