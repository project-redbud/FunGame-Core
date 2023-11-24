using System.Data;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    /// <summary>
    /// 需要在Server中继承此类实现
    /// </summary>
    public abstract class SQLHelper : ISQLHelper
    {
        public abstract FunGameInfo.FunGame FunGameType { get; }
        public abstract string Script { get; set; }
        public abstract CommandType CommandType { get; set; }
        public abstract SQLResult Result { get; }
        public abstract SQLServerInfo ServerInfo { get; }
        public abstract int UpdateRows { get; }
        public abstract DataSet DataSet { get; }
        public bool Success => Result == SQLResult.Success;

        /// <summary>
        /// 执行一个命令
        /// </summary>
        /// <param name="Result">执行结果</param>
        /// <returns>影响的行数</returns>
        public abstract int Execute();

        /// <summary>
        /// 执行一个指定的命令
        /// </summary>
        /// <param name="Script">命令</param>
        /// <param name="Result">执行结果</param>
        /// <returns>影响的行数</returns>
        public abstract int Execute(string Script);

        /// <summary>
        /// 查询DataSet
        /// </summary>
        /// <param name="Result">执行结果</param>
        /// <returns>结果集</returns>
        public abstract DataSet ExecuteDataSet();

        /// <summary>
        /// 执行指定的命令查询DataSet
        /// </summary>
        /// <param name="Script">命令</param>
        /// <param name="Result">执行结果</param>
        /// <returns>结果集</returns>
        public abstract DataSet ExecuteDataSet(string Script);

        /// <summary>
        /// 执行指定的命令查询DataRow（可选实现）
        /// </summary>
        /// <returns>结果行</returns>
        public virtual DataRow? ExecuteDataRow()
        {
            return null;
        }

        /// <summary>
        /// 执行指定的命令查询DataRow（可选实现）
        /// </summary>
        /// <param name="Script">命令</param>
        /// <returns>结果行</returns>
        public virtual DataRow? ExecuteDataRow(string Script)
        {
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
    }
}
