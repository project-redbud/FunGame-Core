using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Base
{
    internal interface ISocket
    {
        internal int Send();
        internal int Read();
    }
}
