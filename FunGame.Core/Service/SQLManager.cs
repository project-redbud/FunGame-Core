using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Service
{
    internal class SQLManager : ISQLService
    {
        int ISQLService.Add(StringBuilder sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        int ISQLService.Add(string sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        SQLResult ISQLService.Execute()
        {
            throw new NotImplementedException();
        }

        SQLResult ISQLService.Execute(StringBuilder sql)
        {
            throw new NotImplementedException();
        }

        SQLResult ISQLService.Execute(string sql)
        {
            throw new NotImplementedException();
        }

        object ISQLService.Query(EntityType type, StringBuilder sql)
        {
            throw new NotImplementedException();
        }

        T ISQLService.Query<T>(StringBuilder sql)
        {
            throw new NotImplementedException();
        }

        object ISQLService.Query(EntityType type, string sql)
        {
            throw new NotImplementedException();
        }

        T ISQLService.Query<T>(string sql)
        {
            throw new NotImplementedException();
        }

        int ISQLService.Remove(StringBuilder sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        int ISQLService.Remove(string sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        int ISQLService.Update(StringBuilder sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }

        int ISQLService.Update(string sql, ref SQLResult result)
        {
            throw new NotImplementedException();
        }
    }
}
