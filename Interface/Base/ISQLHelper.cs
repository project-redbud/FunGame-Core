using System.Data;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Interface.Base
{
    public interface ISQLHelper
    {
        public FunGameInfo.FunGame FunGameType { get; }
        public string Script { get; set; }
        public CommandType CommandType { get; set; }
        public SQLResult Result { get; }
        public bool Success { get; }
        public SQLServerInfo ServerInfo { get; }
        public int UpdateRows { get; }
        public DataSet DataSet { get; }

        public int Execute();
        public DataSet ExecuteDataSet();
        public void Close();
    }
}
