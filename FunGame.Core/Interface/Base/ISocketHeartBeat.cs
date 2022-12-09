using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISocketHeartBeat
    {
        public int HeartBeatFaileds { get; }
        public bool SendingHeartBeat { get; }
    }
}
