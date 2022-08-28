using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FunGame.Models.Config;
using FunGame.Models.Enum;
using System.Diagnostics;

namespace FunGame.Utils.DataHelper
{
    /// <summary>
    /// 在FunGame.Core中添加新接口和新实现时，需要：
    /// 1、在这里定义类名和方法名
    /// 2、在FunGame.Models.Enum.CommonEnums里同步添加InterfaceType和InterfaceMethod
    /// </summary>
    public class AssemblyHelper
    {
        /**
         * 定义类名
         */
        public const string ServerInterface = "ServerInterface";

        public const string Implement = "Impl"; // 实现类的后缀（无需修改）

        /**
         * 定义方法名
         */
        public const string GetRemoteServerIP = "RemoteServerIP";

        /**
         * 定义反射变量
         */
        private Assembly? Assembly;
        private Type? Type;
        private MethodInfo? Method;
        private object? Instance;

        /// <summary>
        /// 获取FunGame.Core.dll中接口的实现方法
        /// </summary>
        /// <param name="Interface">类名</param>
        /// <returns></returns>
        private Type? GetFunGameCoreImplement(int Interface)
        {
            // 通过类名获取获取命名空间+类名称
            string ClassName = Interface switch
            {
                (int)CommonEnums.InterfaceType.ServerInterface => ServerInterface + Implement,
                _ => "",
            };
            List<Type>? Classes = null;
            if (Assembly != null)
            {
                Classes = Assembly.GetTypes().Where(w =>
                    w.Namespace == "FunGame.Core.Implement" &&
                    w.Name.Contains(ClassName)
                ).ToList();
                if (Classes != null && Classes.Count > 0)
                {
                    Debug.WriteLine(Classes);
                    return Classes[0];
                }
                else return null;
            }
            else return null;
        }

        private string GetMethodName(int Method)
        {
            // 通过AssemblyHelperType来获取方法名
            switch (Method)
            {
                case (int)CommonEnums.InterfaceMethod.GetServerIP:
                    return GetRemoteServerIP;
            }
            return "";
        }

        public object? GetFunGameCoreValue(int Interface, int Method)
        {
            Assembly = Assembly.LoadFile(System.Windows.Forms.Application.StartupPath + @"FunGame.Core.dll"); // 要绝对路径
            Type = GetFunGameCoreImplement(Interface); // 通过类名获取获取命名空间+类名称
            string MethodName = GetMethodName(Method); // 获取方法名
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
