using System.Collections;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 单例表：表中的对象以类名作为Key保存，并以Key获取该对象，Key具有唯一约束
    /// 用于储存单例对象使用
    /// </summary>
    public class Singleton
    {
        private static readonly Hashtable SingletonTable = new();

        /// <summary>
        /// 查询目标的类是否已经有实例
        /// </summary>
        /// <param name="single">单例对象</param>
        /// <returns></returns>
        public static bool IsExist(object single)
        {
            return SingletonTable.ContainsKey(single.GetType().ToString());
        }

        /// <summary>
        /// 将目标和目标的类添加至单例表
        /// </summary>
        /// <param name="single">单例对象</param>
        /// <returns></returns>
        /// <exception cref="SingletonAddException">添加单例到单例表时遇到错误</exception>
        public static bool Add(object single)
        {
            string type = single.GetType().ToString();
            if (!SingletonTable.ContainsKey(type))
            {
                try
                {
                    SingletonTable.Add(type, single);
                }
                catch
                {
                    throw new SingletonAddException();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将目标和目标的类从单例表中移除
        /// </summary>
        /// <param name="single">单例对象</param>
        /// <returns></returns>
        public static bool Remove(object single)
        {
            string type = single.GetType().ToString();
            if (!SingletonTable.ContainsKey(type))
            {
                return false;
            }
            else
            {
                SingletonTable.Remove(type);
                return true;
            }
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <typeparam name="T">目标类</typeparam>
        /// <returns></returns>
        /// <exception cref="SingletonGetException">不能从单例表中获取到指定的单例</exception>
        public static T? Get<T>()
        {
            T? single = default;
            string type = typeof(T).ToString();
            if (SingletonTable.ContainsKey(type))
            {
                try
                {
                    single = (T?)SingletonTable[type];
                }
                catch
                {
                    throw new SingletonGetException();
                }
                if (single != null) return single;
            }
            return single;
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <param name="type">目标类</param>
        /// <returns></returns>
        /// <exception cref="SingletonGetException">不能从单例表中获取到指定的单例</exception>
        public static object? Get(Type type)
        {
            object? single = default;
            if (SingletonTable.ContainsKey(type))
            {
                try
                {
                    single = SingletonTable[type];
                }
                catch
                {
                    throw new SingletonGetException();
                }
                if (single != null) return single;
            }
            return single;
        }
    }
}
