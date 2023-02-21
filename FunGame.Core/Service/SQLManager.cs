using Milimoe.FunGame.Core.Api.Data;
using Milimoe.FunGame.Core.Library.Constant;
using System.Text;

namespace Milimoe.FunGame.Core.Service
{
    /// <summary>
    /// 需要在Server中继承此类实现
    /// </summary>
    public class SQLManager
    {
        internal SQLHelper SQLHelper { get; }

        internal SQLManager(SQLHelper SQLHelper)
        {
            this.SQLHelper = SQLHelper;
        }

        protected virtual int Add(StringBuilder sql, ref SQLResult result)
        {
            return 0;
        }

        protected virtual int Add(string sql, ref SQLResult result)
        {
            return 0;
        }

        protected virtual SQLResult Execute()
        {
            return SQLResult.NotFound;
        }

        protected virtual SQLResult Execute(StringBuilder sql)
        {
            return SQLResult.NotFound;
        }

        protected virtual SQLResult Execute(string sql)
        {
            return SQLResult.NotFound;
        }

        protected virtual object Query(EntityType type, StringBuilder sql)
        {
            return General.EntityInstance;
        }

        protected virtual T? Query<T>(StringBuilder sql)
        {
            return default;
        }

        protected virtual object Query(EntityType type, string sql)
        {
            return General.EntityInstance;
        }

        protected virtual T? Query<T>(string sql)
        {
            return default;
        }

        protected virtual int Remove(StringBuilder sql, ref SQLResult result)
        {
            return 0;
        }

        protected virtual int Remove(string sql, ref SQLResult result)
        {
            return 0;
        }

        protected virtual int Update(StringBuilder sql, ref SQLResult result)
        {
            return 0;
        }

        protected virtual int Update(string sql, ref SQLResult result)
        {
            return 0;
        }
    }
}
