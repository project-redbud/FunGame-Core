using System.Collections;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Common.Addon;

namespace Milimoe.FunGame.Core.Controller
{
    /// <summary>
    /// 这是通用的控制器，仅提供基本功能
    /// </summary>
    /// <typeparam name="T">Addon的类型，如<see cref="GameModule"/>或者<see cref="Plugin"/></typeparam>
    public class BaseAddonController<T> where T : IAddon
    {
        /// <summary>
        /// 控制器的本体
        /// </summary>
        public T Addon { get; }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        protected Action<string> MaskMethod_WriteLine { get; set; }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        protected Action<Exception> MaskMethod_Error { get; set; }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void WriteLine(string msg) => MaskMethod_WriteLine(msg);

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public void Error(Exception e) => MaskMethod_Error(e);

        /// <summary>
        /// 新建一个BaseAddonController
        /// </summary>
        /// <param name="addon"></param>
        /// <param name="delegates"></param>
        public BaseAddonController(IAddon addon, Dictionary<string, object> delegates)
        {
            Addon = (T)addon;
            if (delegates.ContainsKey("WriteLine")) MaskMethod_WriteLine = delegates["WriteLine"] != null ? (Action<string>)delegates["WriteLine"]! : new(DefaultPrint);
            if (delegates.ContainsKey("Error")) MaskMethod_Error = delegates["Error"] != null ? (Action<Exception>)delegates["Error"]! : new(DefaultPrint);
            MaskMethod_WriteLine ??= new(DefaultPrint);
            MaskMethod_Error ??= new(DefaultPrint);
        }

        /// <summary>
        /// 默认的输出错误消息方法
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private void DefaultPrint(string msg) => Console.Write("\r" + msg + "\n\r> ");

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private void DefaultPrint(Exception e) => DefaultPrint(e.ToString());
    }
}
