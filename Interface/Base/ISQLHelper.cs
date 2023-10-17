using System.Data;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISQLHelper
    {
        public FunGameInfo.FunGame FunGameType { get; }
        public string Script { get; set; }
        public CommandType CommandType { get; set; }
        public SQLResult Result { get; }
        public SQLServerInfo ServerInfo { get; }
        public int UpdateRows { get; }
        public DataSet DataSet { get; }
        public bool Success { get; }

        public int Execute();
        public DataSet ExecuteDataSet();
        public DataRow? ExecuteDataRow();
        public void Close();
    }
}
