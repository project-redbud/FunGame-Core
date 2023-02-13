using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface IBaseEntity : IEquatable<IBaseEntity>, IEnumerable<IEnumerable>
    {
        public int Id { get; }
        public Guid Guid { get; }
        public string Name { get; }
    }
}
