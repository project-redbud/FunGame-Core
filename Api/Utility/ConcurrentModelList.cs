using System.Collections;
using System.Collections.Concurrent;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class ConcurrentModelList<T> : IEnumerable<T>
    {
        /// <summary>
        /// 目前的Model数量
        /// </summary>
        public int Count => Models.Count;

        /// <summary>
        /// 最大接受的Model数量
        /// </summary>
        private int MaxModel { get; }

        /// <summary>
        /// 可参与高并发的字典，但添加效率较低
        /// </summary>
        private ConcurrentDictionary<string, T> Models { get; } = [];

        /// <summary>
        /// Init ModelManager
        /// </summary>
        /// <param name="MaxModel">MaxModel</param>
        public ConcurrentModelList(int MaxModel = 0)
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
        public T this[string name] => Models[name];

        /// <summary>
        /// 向Model管理器中添加Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <param name="t">Model对象</param>
        /// <returns>True：操作成功</returns>
        public bool Add(string name, T t)
        {
            if (Models.Count + 1 > MaxModel) return false;
            return Models.TryAdd(name, t);
        }

        /// <summary>
        /// 从Model管理器中移除Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <returns>True：操作成功</returns>
        public bool Remove(string name)
        {
            return Models.TryRemove(name, out _);
        }

        /// <summary>
        /// 将Model移除，并取得这个Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <param name="t">Model对象</param>
        /// <returns>被移除的Model</returns>
        public bool Remove(string name, ref T? t)
        {
            return Models.TryRemove(name, out t);
        }

        /// <summary>
        /// 将Model移除，并取得这个Model
        /// </summary>
        /// <param name="name">Model的Key</param>
        /// <returns>被移除的Model</returns>
        public T? RemoveAndGet(string name)
        {
            Models.TryRemove(name, out T? result);
            return result;
        }

        /// <summary>
        /// 判断是否存在指定的Model
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsKey(string name)
        {
            return Models.ContainsKey(name);
        }

        /// <summary>
        /// 清空Model管理器
        /// </summary>
        public void Clear()
        {
            Models.Clear();
        }


        /// <summary>
        /// 获取Model对象的列表
        /// </summary>
        public List<T> GetList()
        {
            return [.. Models.Values];
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T instance in Models.Values)
            {
                yield return instance;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
