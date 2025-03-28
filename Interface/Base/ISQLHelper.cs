using System.Data;
using System.Data.Common;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISQLHelper
    {
        public FunGameInfo.FunGame FunGameType { get; }
        public string Script { get; set; }
        public CommandType CommandType { get; set; }
        public DbTransaction? Transaction { get; }
        public SQLResult Result { get; }
        public SQLServerInfo ServerInfo { get; }
        public int AffectedRows { get; }
        public long LastInsertId { get; }
        public DataSet DataSet { get; }
        public bool Success { get; }

        public int Execute();
        public DataSet ExecuteDataSet();
        public DataRow? ExecuteDataRow();
        public Task<int> ExecuteAsync();
        public Task<DataSet> ExecuteDataSetAsync();
        public Task<DataRow?> ExecuteDataRowAsync();
        public void Close();
        public void NewTransaction();
        public void Commit();
        public void Rollback();
    }
}
