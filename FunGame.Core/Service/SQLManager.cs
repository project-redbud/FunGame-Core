using System.Data;
using System.Text;
using Milimoe.FunGame.Core.Api.Data;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Service
{
    /// <summary>
    /// 需要在Server中继承此类实现
    /// </summary>
    public abstract class SQLManager
    {
        public SQLHelper? SQLHelper { get; }

        public abstract int Add(StringBuilder sql, ref SQLResult result);

        public abstract int Add(string sql, ref SQLResult result);

        public abstract SQLResult Execute();

        public abstract SQLResult Execute(StringBuilder sql);

        public abstract SQLResult Execute(string sql);

        public abstract DataSet ExecuteDataSet(StringBuilder sql);

        public abstract DataSet ExecuteDataSet(string sql);

        public abstract object Query(EntityType type, StringBuilder sql);

        public abstract object Query(EntityType type, string sql);

        public abstract int Remove(StringBuilder sql, ref SQLResult result);

        public abstract int Remove(string sql, ref SQLResult result);

        public abstract int Update(StringBuilder sql, ref SQLResult result);
        
        public abstract int Update(string sql, ref SQLResult result);
    }
}
