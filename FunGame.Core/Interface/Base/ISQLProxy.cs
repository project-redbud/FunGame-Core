using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISQLProxy
    {
        public Entity.Enum.ProxyResult Execute();
        public Entity.Enum.ProxyResult Execute(object[]? objs = null);
        public Entity.Enum.ProxyResult Execute(StringBuilder script);
        public int ExecuteRow();
        public int ExecuteRow(object[]? objs = null);
        public int ExecuteRow(StringBuilder script);
        public System.Data.DataSet GetData(Entity.Enum.EntityType type, object[]? objs = null);
        public System.Data.DataSet GetData(object[]? objs = null);
        public System.Data.DataSet GetData(StringBuilder script);
        public System.Data.DataTable GetDataTable(Entity.Enum.EntityType type, object[]? objs = null);
        public System.Data.DataTable GetDataTable(object[]? objs = null);
        public System.Data.DataTable GetDataTable(StringBuilder script);
        public System.Data.DataRow GetDataRow(Entity.Enum.EntityType type, object[]? objs = null);
        public System.Data.DataRow GetDataRow(System.Data.DataSet set, object[]? objs = null);
        public System.Data.DataRow GetDataRow(System.Data.DataTable table, object[]? objs = null);
        public System.Data.DataRow GetDataRow(StringBuilder script);
    }
}
