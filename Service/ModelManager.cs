using System.Collections.Concurrent;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Service
{
    internal class ModelManager
    {
        /// <summary>
        /// 目前的Model数量
        /// </summary>
        internal int Count => Models.Count;

        /// <summary>
        /// 最大接受的Model数量
        /// </summary>
        private int MaxModel { get; }

        /// <summary>
        /// 可参与高并发的字典，但添加效率较低
        /// </summary>
        private ConcurrentDictionary<string, IServerModel> Models { get; } = new();

        /// <summary>
        /// Init ModelManager
        /// </summary>
        /// <param name="MaxModel">MaxModel</param>
        internal ModelManager(int MaxModel = 0)
        {
            if (MaxModel <= 0)
                this.MaxModel = Library.Constant.General.MaxTask_2C2G;
            else
            {
                this.MaxModel = MaxModel;
            }
        }

        /// <summary>
        /// 获取Model对象
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <returns>Model对象</returns>
        internal IServerModel this[string name] => Models[name];

        /// <summary>
        /// 向Model管理器中添加Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <param name="t">Model对象</param>
        /// <returns>True：操作成功</returns>
        internal bool Add(string name, IServerModel t)
        {
            if (Models.Count + 1 > MaxModel) return false;
            return Models.TryAdd(name, t);
        }

        /// <summary>
        /// 从Model管理器中移除Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <returns>True：操作成功</returns>
        internal bool Remove(string name)
        {
            return Models.TryRemove(name, out _);
        }

        /// <summary>
        /// 将Model移除，并取得这个Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <param name="t">Model对象</param>
        /// <returns>被移除的Model</returns>
        internal bool Remove(string name, ref IServerModel? t)
        {
            return Models.TryRemove(name, out t);
        }
        
        /// <summary>
        /// 将Model移除，并取得这个Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <returns>被移除的Model</returns>
        internal IServerModel? RemoveAndGet(string name)
        {
            Models.TryRemove(name, out IServerModel? result);
            return result;
        }

        internal bool ContainsKey(string name)
        {
            return Models.ContainsKey(name);
        }

        /// <summary>
        /// 清空Model管理器
        /// </summary>
        internal void Clear()
        {
            Models.Clear();
        }
        

        /// <summary>
        /// 获取Model对象的列表
        /// </summary>
        internal List<IServerModel> GetList()
        {
            return Models.Values.ToList();
        }
    }
}
