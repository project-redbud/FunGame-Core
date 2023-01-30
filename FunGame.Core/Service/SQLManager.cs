using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Api.Data;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Service
{
    internal class SQLManager : BaseSQLService
    {
        internal SQLHelper SQLHelper { get; }

        internal SQLManager(SQLHelper SQLHelper)
        {
            this.SQLHelper = SQLHelper;
        }

        internal override int Add(StringBuilder sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        internal override int Add(string sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        internal override SQLResult Execute()
        {
            throw new NotImplementedException();
        }

        internal override SQLResult Execute(StringBuilder sql)
        {
            throw new NotImplementedException();
        }

        internal override SQLResult Execute(string sql)
        {
            throw new NotImplementedException();
        }

        internal override object Query(EntityType type, StringBuilder sql)
        {
            throw new NotImplementedException();
        }

        internal override T Query<T>(StringBuilder sql)
        {
            throw new NotImplementedException();
        }

        internal override object Query(EntityType type, string sql)
        {
            throw new NotImplementedException();
        }

        internal override T Query<T>(string sql)
        {
            throw new NotImplementedException();
        }

        internal override int Remove(StringBuilder sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        internal override int Remove(string sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        internal override int Update(StringBuilder sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        internal override int Update(string sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }
    }
}
