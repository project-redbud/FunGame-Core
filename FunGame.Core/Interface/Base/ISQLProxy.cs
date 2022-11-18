using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISQLProxy
    {
        public ProxyResult Execute();
        public ProxyResult Execute(object[]? objs = null);
        public ProxyResult Execute(StringBuilder script);
        public int ExecuteRow();
        public int ExecuteRow(object[]? objs = null);
        public int ExecuteRow(StringBuilder script);
        public System.Data.DataSet GetData(EntityType type, object[]? objs = null);
        public System.Data.DataSet GetData(object[]? objs = null);
        public System.Data.DataSet GetData(StringBuilder script);
        public System.Data.DataTable GetDataTable(EntityType type, object[]? objs = null);
        public System.Data.DataTable GetDataTable(object[]? objs = null);
        public System.Data.DataTable GetDataTable(StringBuilder script);
        public System.Data.DataRow GetDataRow(EntityType type, object[]? objs = null);
        public System.Data.DataRow GetDataRow(System.Data.DataSet set, object[]? objs = null);
        public System.Data.DataRow GetDataRow(System.Data.DataTable table, object[]? objs = null);
        public System.Data.DataRow GetDataRow(StringBuilder script);
    }
}
