using Milimoe.FunGame.Core.Entity.Enum;
using Milimoe.FunGame.Core.Interface.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Proxy
{
    public class SQLProxy : ISQLProxy
    {
        public ProxyResult Execute()
        {
            throw new NotImplementedException();
        }

        public ProxyResult Execute(object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public ProxyResult Execute(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        public int ExecuteRow()
        {
            throw new NotImplementedException();
        }

        public int ExecuteRow(object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public int ExecuteRow(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        public DataSet GetData(EntityType type, object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public DataSet GetData(object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public DataSet GetData(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        public DataRow GetDataRow(EntityType type, object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public DataRow GetDataRow(DataSet set, object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public DataRow GetDataRow(DataTable table, object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public DataRow GetDataRow(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDataTable(EntityType type, object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDataTable(object[]? objs = null)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDataTable(StringBuilder script)
        {
            throw new NotImplementedException();
        }
    }
}
