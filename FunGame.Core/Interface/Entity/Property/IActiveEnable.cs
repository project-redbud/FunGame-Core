using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IActiveEnable
    {
        public bool Active { get; set; }
        public bool Enable { get; set; }
    }
}
