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
        int ISQLService.AddRow(object[]? objs)
        {
            throw new NotImplementedException();
        }

        int ISQLService.AddRow(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        int ISQLService.DeleteRow(object[]? objs)
        {
            throw new NotImplementedException();
        }

        int ISQLService.DeleteRow(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        ProxyResult ISQLService.Execute()
        {
            throw new NotImplementedException();
        }

        ProxyResult ISQLService.Execute(object[]? objs)
        {
            throw new NotImplementedException();
        }

        ProxyResult ISQLService.Execute(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        DataSet ISQLService.GetData(EntityType type, object[]? objs)
        {
            throw new NotImplementedException();
        }

        DataSet ISQLService.GetData(object[]? objs)
        {
            throw new NotImplementedException();
        }

        DataSet ISQLService.GetData(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        DataRow ISQLService.GetDataRow(EntityType type, object[]? objs)
        {
            throw new NotImplementedException();
        }

        DataRow ISQLService.GetDataRow(DataSet set, object[]? objs)
        {
            throw new NotImplementedException();
        }

        DataRow ISQLService.GetDataRow(DataTable table, object[]? objs)
        {
            throw new NotImplementedException();
        }

        DataRow ISQLService.GetDataRow(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        DataTable ISQLService.GetDataTable(EntityType type, object[]? objs)
        {
            throw new NotImplementedException();
        }

        DataTable ISQLService.GetDataTable(object[]? objs)
        {
            throw new NotImplementedException();
        }

        DataTable ISQLService.GetDataTable(StringBuilder script)
        {
            throw new NotImplementedException();
        }

        int ISQLService.UpdateRow(object[]? objs)
        {
            throw new NotImplementedException();
        }

        int ISQLService.UpdateRow(StringBuilder script)
        {
            throw new NotImplementedException();
        }
    }
}
