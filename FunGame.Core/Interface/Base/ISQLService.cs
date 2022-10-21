using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Interface.Base
{
    internal interface ISQLService
    {
        internal Entity.Enum.ProxyResult Execute();
        internal Entity.Enum.ProxyResult Execute(object[]? objs = null);
        internal Entity.Enum.ProxyResult Execute(StringBuilder script);
        internal int UpdateRow(object[]? objs = null);
        internal int UpdateRow(StringBuilder script);
        internal int DeleteRow(object[]? objs = null);
        internal int DeleteRow(StringBuilder script);
        internal int AddRow(object[]? objs = null);
        internal int AddRow(StringBuilder script);
        internal System.Data.DataSet GetData(Entity.Enum.EntityType type, object[]? objs = null);
        internal System.Data.DataSet GetData(object[]? objs = null);
        internal System.Data.DataSet GetData(StringBuilder script);
        internal System.Data.DataTable GetDataTable(Entity.Enum.EntityType type, object[]? objs = null);
        internal System.Data.DataTable GetDataTable(object[]? objs);
        internal System.Data.DataTable GetDataTable(StringBuilder script);
        internal System.Data.DataRow GetDataRow(Entity.Enum.EntityType type, object[]? objs = null);
        internal System.Data.DataRow GetDataRow(System.Data.DataSet set, object[]? objs = null);
        internal System.Data.DataRow GetDataRow(System.Data.DataTable table, object[]? objs = null);
        internal System.Data.DataRow GetDataRow(StringBuilder script);
    }
}
