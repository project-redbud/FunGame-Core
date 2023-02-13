using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IRelateCharacter
    {
        public Character? Character { get; set; }
    }
}
