using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface ICharacter
    {
        public string FirstName { get; set; }
        public string NickName { get; set; }
    }
}
