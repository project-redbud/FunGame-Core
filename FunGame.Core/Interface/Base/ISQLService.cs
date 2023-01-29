using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    internal interface ISQLService
    {
        internal SQLResult Execute();
        internal SQLResult Execute(StringBuilder sql);
        internal SQLResult Execute(string sql);
        internal int Update(StringBuilder sql, ref SQLResult result);
        internal int Remove(StringBuilder sql, ref SQLResult result);
        internal int Add(StringBuilder sql, ref SQLResult result);
        internal int Update(string sql, ref SQLResult result);
        internal int Remove(string sql, ref SQLResult result);
        internal int Add(string sql, ref SQLResult result);
        internal object Query(EntityType type, StringBuilder sql);
        internal T Query<T>(StringBuilder sql);
        internal object Query(EntityType type, string sql);
        internal T Query<T>(string sql);
    }
}
