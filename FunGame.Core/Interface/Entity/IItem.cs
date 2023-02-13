using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IItem : IActiveEnable, IRelateCharacter
    {
        public string Describe { get; set; }
        public decimal Price { get; set; }
        public char Key { get; set; }
    }
}
