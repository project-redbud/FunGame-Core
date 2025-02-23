using System.Data;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    /// <summary>
    /// 需要在Server中继承此类实现
    /// </summary>
    public abstract class SQLHelper : ISQLHelper, IDisposable
    {
        public abstract FunGameInfo.FunGame FunGameType { get; }
        public abstract SQLMode Mode { get; }
        public abstract string Script { get; set; }
        public abstract CommandType CommandType { get; set; }
        public abstract SQLResult Result { get; }
        public abstract SQLServerInfo ServerInfo { get; }
        public abstract int UpdateRows { get; }
        public abstract DataSet DataSet { get; }
        public abstract Dictionary<string, object> Parameters { get; } 
        public bool Success => Result == SQLResult.Success;
        public bool ClearParametersAfterExecute { get; set; } = true;

        /// <summary>
        /// 执行一个命令
        /// </summary>
        /// <returns>影响的行数</returns>
        public abstract int Execute();

        /// <summary>
        /// 执行一个指定的命令
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>影响的行数</returns>
        public abstract int Execute(string script);

        /// <summary>
        /// 查询DataSet
        /// </summary>
        /// <returns>结果集</returns>
        public abstract DataSet ExecuteDataSet();

        /// <summary>
        /// 执行指定的命令查询DataSet
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>结果集</returns>
        public abstract DataSet ExecuteDataSet(string script);

        /// <summary>
        /// 执行指定的命令查询DataRow（可选实现）
        /// </summary>
        /// <returns>结果行</returns>
        public virtual DataRow? ExecuteDataRow()
        {
            return ExecuteDataRow(Script);
        }

        /// <summary>
        /// 执行指定的命令查询DataRow（可选实现）
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>结果行</returns>
        public virtual DataRow? ExecuteDataRow(string script)
        {
            DataSet dataSet = ExecuteDataSet(script);
            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet.Tables[0].Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 创建一个SQL事务
        /// </summary>
        public abstract void NewTransaction();

        /// <summary>
        /// 提交事务
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        public abstract void Rollback();

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Dispose();
    }
}
