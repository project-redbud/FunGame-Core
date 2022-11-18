using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milimoe.FunGame.Core.Entity.Enum;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    public class EnumHelper
    {
        /// <summary>
        /// 获取实现类类名
        /// </summary>
        /// <param name="Interface">接口代号</param>
        /// <returns></returns>
        public static string GetImplementClassName(int Interface)
        {
            foreach (string str in Enum.GetNames(typeof(InterfaceType)))
            {
                InterfaceType temp = (InterfaceType)Enum.Parse(typeof(InterfaceType), Interface.ToString(), true);
                if (temp.ToString() == str)
                    return temp + "Impl";
            }
            return "";
        }

        /// <summary>
        /// 获取实现类的方法名
        /// </summary>
        /// <param name="Method">方法代号</param>
        /// <returns></returns>
        public static string GetImplementMethodName(int Method)
        {
            foreach (string str in Enum.GetNames(typeof(InterfaceMethod)))
            {
                InterfaceMethod temp = (InterfaceMethod)Enum.Parse(typeof(InterfaceMethod), Method.ToString(), true);
                if (temp.ToString() == str)
                    return temp.ToString();
            }
            return "";
        }

        /// <summary>
        /// 获取Socket枚举名
        /// </summary>
        /// <param name="SocketType">Socket枚举</param>
        /// <returns></returns>
        public static string GetSocketTypeName(int SocketType)
        {
            foreach (string str in Enum.GetNames(typeof(SocketMessageType)))
            {
                SocketMessageType temp = (SocketMessageType)Enum.Parse(typeof(SocketMessageType), SocketType.ToString(), true);
                if (temp.ToString() == str)
                    return temp.ToString();
            }
            return "";
        }
    }
}
