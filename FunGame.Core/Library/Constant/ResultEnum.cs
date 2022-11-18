using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Library.Constant
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

    public enum SocketResult
    {
        Success,
        Fail,
        NotSent,
        NotReceived
    }

    public enum ProxyResult
    {
        Success,
        Fail,
        NotFound
    }
}
