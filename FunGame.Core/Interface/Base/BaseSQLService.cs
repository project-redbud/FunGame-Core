using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    internal abstract class BaseSQLService
    {
        internal abstract SQLResult Execute();
        internal abstract SQLResult Execute(StringBuilder sql);
        internal abstract SQLResult Execute(string sql);
        internal abstract int Update(StringBuilder sql, ref SQLResult result);
        internal abstract int Remove(StringBuilder sql, ref SQLResult result);
        internal abstract int Add(StringBuilder sql, ref SQLResult result);
        internal abstract int Update(string sql, ref SQLResult result);
        internal abstract int Remove(string sql, ref SQLResult result);
        internal abstract int Add(string sql, ref SQLResult result);
        internal abstract object Query(EntityType type, StringBuilder sql);
        internal abstract T Query<T>(StringBuilder sql);
        internal abstract object Query(EntityType type, string sql);
        internal abstract T Query<T>(string sql);
    }
}
