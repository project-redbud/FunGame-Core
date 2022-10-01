using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 在FunGame.Core.Interface中添加新接口和新实现时，需要：
    /// 在FunGame.Core.Entity.Enum.CommonEnums里同步添加InterfaceType、InterfaceMethod
    /// </summary>
    public class ReflectionHelper
    {
        /**
         * 定义需要反射的DLL
         */
        public const string FUNGAME_IMPL = "FunGame.Implement";

        /**
         * 无需二次修改的
         */
        public static string EXEDocPath = Environment.CurrentDirectory.ToString() + "\\"; // 程序目录
        public static string PluginDocPath = Environment.CurrentDirectory.ToString() + "\\plugins\\"; // 插件目录

        ////////////////////////////////////////////////////////////////////
        /////////////// * 下 面 是 工 具 类 实 现 * ////////////////
        ///////////////////////////////////////////////////////////////////

        /**
         * 定义反射变量
         */
        private Assembly? Assembly;
        private Type? Type;
        private MethodInfo? Method;
        private object? Instance;

        /// <summary>
        /// 获取FunGame.Implement.dll中接口的实现方法
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <returns></returns>
        private Type? GetFunGameImplementType(int Interface)
        {
            // 通过类名获取获取命名空间+类名称
            string ClassName = EnumHelper.GetImplementClassName(Interface);
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
        /// 公开方法：获取FunGame.Implement.DLL中指定方法的返回值
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <param name="Method">方法代号</param>
        /// <returns></returns>
        public object? GetFunGameImplValue(int Interface, int Method)
        {
            Assembly = Assembly.LoadFile(EXEDocPath + FUNGAME_IMPL + ".dll");
            Type = GetFunGameImplementType(Interface); // 通过类名获取获取命名空间+类名称
            string MethodName = EnumHelper.GetImplementMethodName(Method); // 获取方法名
            if (Assembly != null && Type != null) this.Method = Type.GetMethod(MethodName); // 从Type中查找方法名
            else return null;
            Instance = Assembly.CreateInstance(Type.Namespace + "." + Type.Name);
            if (Instance != null && this.Method != null)
            {
                object? value = this.Method.Invoke(Instance, Array.Empty<object>()); // 实例方法的调用
                if (value != null)
                    return value;
                else return null;
            }
            else return null;
        }
    }
}
