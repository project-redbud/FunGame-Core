using System.Reflection;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// See: <see cref="InterfaceMethod"/>, <see cref="InterfaceType"/>, <see cref="InterfaceSet"/>
    /// </summary>
    public class Implement
    {
        /// <summary>
        /// 获取FunGame.Implement.dll中接口的实现方法
        /// </summary>
        /// <param name="Assembly">程序集</param>
        /// <param name="Interface">接口代号</param>
        /// <returns></returns>
        private static Type? GetFunGameImplementType(Assembly Assembly, InterfaceType Interface)
        {
            // 通过类名获取命名空间+类名称
            string ClassName = GetImplementClassName(Interface);
            List<Type>? Classes = null;
            if (Assembly != null)
            {
                Classes = Assembly.GetTypes().Where(w =>
                    w.Namespace == "Milimoe.FunGame.Core.Implement" &&
                    w.Name.Contains(ClassName)
                ).ToList();
                if (Classes != null && Classes.Count > 0)
                    return Classes[0];
                else return null;
            }
            else return null;
        }

        /// <summary>
        /// 获取接口实现类类名
        /// </summary>
        /// <param name="Interface">接口类型</param>
        /// <returns></returns>
        private static string GetImplementClassName(InterfaceType Interface)
        {
            return Interface switch
            {
                InterfaceType.IClient => InterfaceSet.Type.IClient,
                InterfaceType.IServer => InterfaceSet.Type.IServer,
                _ => ""
            };
        }

        /// <summary>
        /// 获取接口方法名（支持属性）
        /// </summary>
        /// <param name="Method">方法</param>
        /// <returns></returns>
        private static string GetImplementMethodName(InterfaceMethod Method)
        {
            return Method switch
            {
                InterfaceMethod.RemoteServerIP => InterfaceSet.Method.RemoteServerIP,
                InterfaceMethod.DBConnection => InterfaceSet.Method.DBConnection,
                InterfaceMethod.GetServerSettings => InterfaceSet.Method.GetServerSettings,
                InterfaceMethod.GameModuleList => InterfaceSet.Method.GameModuleList,
                InterfaceMethod.GameMapList => InterfaceSet.Method.GameMapList,
                _ => ""
            };
        }

        /// <summary>
        /// 公开方法：获取FunGame.Implement.DLL中指定方法（属性）的返回值
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <param name="Method">方法代号（支持属性）</param>
        /// <param name="IsMethod">是否是方法（如是属性请传入false）</param>
        /// <returns></returns>
        public static object? GetFunGameImplValue(InterfaceType Interface, InterfaceMethod Method, bool IsMethod = true)
        {
            MethodInfo? MethodInfo;
            PropertyInfo? PropertyInfo;

            // 反射读取程序集
            Assembly? Assembly = System.Reflection.Assembly.LoadFile(ReflectionSet.EXEFolderPath + ReflectionSet.FUNGAME_IMPL + ".dll");
            // 通过类名获取命名空间+类名称
            Type? Type = GetFunGameImplementType(Assembly, Interface);

            if (Assembly != null && Type != null)
            {
                // 创建类对象
                object? Instance = Assembly.CreateInstance(Type.Namespace + "." + Type.Name);
                // 获取方法/属性名
                string MethodName = GetImplementMethodName(Method);
                if (IsMethod)
                {
                    // 从Type中查找方法名
                    MethodInfo = Type.GetMethod(MethodName);
                    if (Instance != null && MethodInfo != null)
                    {
                        object? value = MethodInfo.Invoke(Instance, []);
                        if (value != null) return value;
                    }
                }
                else
                {
                    PropertyInfo = Type.GetProperty(MethodName);
                    if (Instance != null && PropertyInfo != null)
                    {
                        object? value = PropertyInfo.GetValue(Instance);
                        if (value != null) return value;
                    }
                }
            }

            return null;
        }
    }
}
