using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 单例表：表中的对象以类名作为Key保存，并以Key获取该对象，Key具有唯一约束
    /// 用于储存单例对象使用
    /// </summary>
    public class Singleton
    {
        private static readonly Hashtable SingletonTable = new();

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
                    throw new Exception("添加单例到单例表时遇到错误");
                }
                return true;
            }
            return false;
        }

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
                    throw new Exception("不能从单例表中获取到指定的单例");
                }
                if (single != null) return single;
            }
            return single;
        }
    }
}
