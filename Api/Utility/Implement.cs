using System.Reflection;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// See: <see cref="InterfaceType"/>, <see cref="InterfaceSet"/>, <see cref="InterfaceMethod"/>
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
        /// 获取接口方法名
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
                _ => ""
            };
        }

        /// <summary>
        /// 公开方法：获取FunGame.Implement.DLL中指定方法的返回值
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <param name="Method">方法代号</param>
        /// <returns></returns>
        public static object? GetFunGameImplValue(InterfaceType Interface, InterfaceMethod Method)
        {
            MethodInfo? MethodInfo;

            Assembly? Assembly = System.Reflection.Assembly.LoadFile(ReflectionSet.EXEFolderPath + ReflectionSet.FUNGAME_IMPL + ".dll");
            Type? Type = GetFunGameImplementType(Assembly, Interface); // 通过类名获取命名空间+类名称
            string MethodName = GetImplementMethodName(Method); // 获取方法名

            if (Assembly != null && Type != null) MethodInfo = Type.GetMethod(MethodName); // 从Type中查找方法名
            else return null;

            object? Instance = Assembly.CreateInstance(Type.Namespace + "." + Type.Name);
            if (Instance != null && MethodInfo != null)
            {
                object? value = MethodInfo.Invoke(Instance, Array.Empty<object>()); // 实例方法的调用
                if (value != null)
                    return value;
                else return null;
            }

            return null;
        }
    }
}
