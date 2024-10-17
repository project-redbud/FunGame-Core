using System.Collections.Concurrent;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 单例表：表中的对象以类名作为Key保存，并以Key获取该对象，Key具有唯一约束
    /// 用于储存单例对象使用
    /// </summary>
    public class Singleton
    {
        private static readonly ConcurrentDictionary<string, object> SingletonTable = [];

        /// <summary>
        /// 查询目标的类是否已经有实例
        /// </summary>
        /// <param name="single">单例对象</param>
        /// <returns></returns>
        public static bool IsExist(object single)
        {
            Type type = single.GetType();
            string name = type.FullName ?? type.ToString();
            return SingletonTable.ContainsKey(name);
        }

        /// <summary>
        /// 将目标和目标的类添加至单例表，如果存在，将更新此类单例
        /// </summary>
        /// <param name="single">单例对象</param>
        /// <param name="baseClass">存入基类</param>
        /// <returns></returns>
        public static void AddOrUpdate(object single, bool baseClass = false)
        {
            if (single != null)
            {
                Type? type = baseClass ? single.GetType().BaseType : single.GetType();
                string name = type?.FullName ?? type?.ToString() ?? "";
                if (name != "") SingletonTable.AddOrUpdate(name, single, (key, oldValue) => single);
            }
        }

        /// <summary>
        /// 将目标和目标的类从单例表中移除
        /// </summary>
        /// <param name="single">单例对象</param>
        /// <returns></returns>
        public static bool Remove(object single)
        {
            Type type = single.GetType();
            string name = type.FullName ?? type.ToString();
            return SingletonTable.TryRemove(name, out _);
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <typeparam name="T">目标类</typeparam>
        /// <returns></returns>
        public static T? Get<T>()
        {
            string name = typeof(T).FullName ?? typeof(T).ToString();
            if (SingletonTable.TryGetValue(name, out object? value) && value is T single)
            {
                return single;
            }
            return default;
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <param name="type">目标类</param>
        /// <returns></returns>
        public static object? Get(Type type)
        {
            string name = type.FullName ?? type.ToString();
            if (SingletonTable.TryGetValue(name, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
