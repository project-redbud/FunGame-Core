using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        public int Id { get; set; } = 0;
        public Guid Guid { get; set; } = Guid.Empty;
        public string Name { get; set; } = "";

        public abstract bool Equals(IBaseEntity? other);
        public abstract IEnumerator<IBaseEntity> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
