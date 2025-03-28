using System.Data;
using System.Data.Common;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Api.Transmittal
{
    /// <summary>
    /// 数据库助手类。这是一个抽象类，需要在Server中继承此类实现
    /// </summary>
    public abstract class SQLHelper : ISQLHelper, IDisposable
    {
        /// <summary>
        /// FunGame 类型
        /// </summary>
        public abstract FunGameInfo.FunGame FunGameType { get; }

        /// <summary>
        /// 使用的数据库类型
        /// </summary>
        public abstract SQLMode Mode { get; }

        /// <summary>
        /// SQL 脚本
        /// </summary>
        public abstract string Script { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public abstract CommandType CommandType { get; set; }

        /// <summary>
        /// 数据库事务
        /// </summary>
        public abstract DbTransaction? Transaction { get; }

        /// <summary>
        /// 执行结果
        /// </summary>
        public abstract SQLResult Result { get; }

        /// <summary>
        /// SQL 服务器信息
        /// </summary>
        public abstract SQLServerInfo ServerInfo { get; }

        /// <summary>
        /// 上一次执行命令影响的行数
        /// </summary>
        public abstract int AffectedRows { get; }

        /// <summary>
        /// 上一次执行的命令是 Insert 时，返回的自增 ID，大于 0 有效
        /// </summary>
        public abstract long LastInsertId { get; }

        /// <summary>
        /// 上一次执行命令的查询结果集
        /// </summary>
        public abstract DataSet DataSet { get; }

        /// <summary>
        /// SQL 语句参数
        /// </summary>
        public abstract Dictionary<string, object> Parameters { get; }

        /// <summary>
        /// 上一次执行命令是否成功
        /// </summary>
        public bool Success => Result == SQLResult.Success;

        /// <summary>
        /// 是否在每次执行命令后清除参数，默认为 true
        /// </summary>
        public bool ClearParametersAfterExecute { get; set; } = true;

        /// <summary>
        /// 执行现有命令（<see cref="Script"/>）
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
        /// 异步执行现有命令（<see cref="Script"/>）
        /// </summary>
        /// <returns>影响的行数</returns>
        public abstract Task<int> ExecuteAsync();

        /// <summary>
        /// 异步执行一个指定的命令
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>影响的行数</returns>
        public abstract Task<int> ExecuteAsync(string script);

        /// <summary>
        /// 执行现有命令（<see cref="Script"/>）查询 DataSet
        /// </summary>
        /// <returns>结果集</returns>
        public abstract DataSet ExecuteDataSet();

        /// <summary>
        /// 执行指定的命令查询 DataSet
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>结果集</returns>
        public abstract DataSet ExecuteDataSet(string script);

        /// <summary>
        /// 异步执行现有命令（<see cref="Script"/>）查询 DataSet
        /// </summary>
        /// <returns>结果集</returns>
        public abstract Task<DataSet> ExecuteDataSetAsync();

        /// <summary>
        /// 异步执行指定的命令查询 DataSet
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>结果集</returns>
        public abstract Task<DataSet> ExecuteDataSetAsync(string script);

        /// <summary>
        /// 执行现有命令（<see cref="Script"/>）查询 DataRow（可选实现）
        /// </summary>
        /// <returns>结果行</returns>
        public virtual DataRow? ExecuteDataRow()
        {
            return ExecuteDataRow(Script);
        }

        /// <summary>
        /// 执行指定的命令查询 DataRow（可选实现）
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>结果行</returns>
        public virtual DataRow? ExecuteDataRow(string script)
        {
            ExecuteDataSet(script);
            if (Success)
            {
                return DataSet.Tables[0].Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 异步执行现有命令（<see cref="Script"/>）查询 DataRow（可选实现）
        /// </summary>
        /// <returns>结果行</returns>
        public virtual async Task<DataRow?> ExecuteDataRowAsync()
        {
            return await ExecuteDataRowAsync(Script);
        }

        /// <summary>
        /// 异步执行指定的命令查询 DataRow（可选实现）
        /// </summary>
        /// <param name="script">命令</param>
        /// <returns>结果行</returns>
        public virtual async Task<DataRow?> ExecuteDataRowAsync(string script)
        {
            await ExecuteDataSetAsync(script);
            if (Success)
            {
                return DataSet.Tables[0].Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 查询数据库是否存在
        /// </summary>
        /// <returns></returns>
        public abstract bool DatabaseExists();

        /// <summary>
        /// 执行一个 sql 脚本文件
        /// </summary>
        /// <param name="path"></param>
        public virtual void ExecuteSqlFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("SQL 脚本文件不存在", path);
            }

            string content = File.ReadAllText(path);
            string[] commands = content.Split([";"], StringSplitOptions.RemoveEmptyEntries);

            foreach (string command in commands)
            {
                string sql = command.Trim();
                if (!string.IsNullOrEmpty(sql))
                {
                    Execute(sql);
                }
            }
        }
        
        /// <summary>
        /// 异步执行一个 sql 脚本文件
        /// </summary>
        /// <param name="path"></param>
        public virtual async Task ExecuteSqlFileAsync(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("SQL 脚本文件不存在", path);
            }

            string content = File.ReadAllText(path);
            string[] commands = content.Split([";"], StringSplitOptions.RemoveEmptyEntries);

            foreach (string command in commands)
            {
                string sql = command.Trim();
                if (!string.IsNullOrEmpty(sql))
                {
                    await ExecuteAsync(sql);
                }
            }
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
