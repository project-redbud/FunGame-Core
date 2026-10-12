using System.Collections.Concurrent;
using System.Reflection;
using FunGame.Core.Entity;

namespace FunGame.Core.Api
{
    /// <summary>
    /// 特效钩子重写缓存：在模块加载时反射所有 <see cref="Effect"/> 派生类型，
    /// 缓存每个类型重写了哪些钩子方法，供框架在调用钩子前判断是否需要自动记录 <see cref="Model.Framework.RoundRecord.Effects"/>。
    /// </summary>
    internal static class EffectHookCache
    {
        /// <summary>
        /// 类型 -> 该类型（含中间基类）重写的钩子方法名集合
        /// </summary>
        private static readonly ConcurrentDictionary<Type, HashSet<string>> Cache = new();

        /// <summary>
        /// <see cref="Effect"/> 基类声明的全部虚方法名（钩子），非钩子重写（如 Equals 等）不会包含在内
        /// </summary>
        private static readonly HashSet<string> HookNames = [.. typeof(Effect)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.IsVirtual && m.GetBaseDefinition().DeclaringType == typeof(Effect))
            .Select(m => m.Name)];

        /// <summary>
        /// 判断指定类型的实例是否重写了指定钩子方法（含中间基类重写）。
        /// 缓存未命中时懒计算该类型的重写表，兼容模块加载后动态加载的程序集。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="hookName"></param>
        /// <returns></returns>
        public static bool IsOverridden(Type type, string hookName)
        {
            if (!HookNames.Contains(hookName))
            {
                return false;
            }
            return Cache.GetOrAdd(type, ComputeOverridden).Contains(hookName);
        }

        /// <summary>
        /// 扫描当前已加载程序集中的所有 <see cref="Effect"/> 派生类型，预填充重写缓存（模组加载时调用）。
        /// 加载失败的程序集（如原生程序集）会跳过，不影响后续懒计算兜底。
        /// </summary>
        public static void ScanAllAssemblies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = [.. e.Types.Where(t => t is not null).Select(t => t!)];
                }
                catch
                {
                    continue;
                }
                foreach (Type type in types)
                {
                    if (type != null && type != typeof(Effect) && !type.IsAbstract && typeof(Effect).IsAssignableFrom(type))
                    {
                        Cache.GetOrAdd(type, ComputeOverridden);
                    }
                }
            }
        }

        /// <summary>
        /// 计算一个类型重写了哪些钩子方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static HashSet<string> ComputeOverridden(Type type)
        {
            HashSet<string> set = [];
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.DeclaringType != typeof(Effect) && HookNames.Contains(method.Name))
                {
                    set.Add(method.Name);
                }
            }
            return set;
        }
    }
}
