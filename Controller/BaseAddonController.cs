using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Controller
{
    /// <summary>
    /// 这是通用的控制器，仅提供基本功能
    /// </summary>
    /// <typeparam name="T">Addon的类型，如 <see cref="GameModule"/> 或者 <see cref="Plugin"/> / <see cref="ServerPlugin"/> / <see cref="WebAPIPlugin"/></typeparam>
    public class BaseAddonController<T> where T : IAddon
    {
        /// <summary>
        /// 控制器的本体
        /// </summary>
        public T Addon { get; }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        protected Action<string, string, LogLevel, bool> MaskMethod_WriteLine { get; set; }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        protected Action<Exception> MaskMethod_Error { get; set; }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="useLevel"></param>
        /// <returns></returns>
        public void WriteLine(string msg, LogLevel level = LogLevel.Info, bool useLevel = true) => MaskMethod_WriteLine(Addon.Name, msg, level, useLevel);

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public void Error(Exception e) => MaskMethod_Error(e);

        /// <summary>
        /// JSON 工具类对象
        /// </summary>
        public JsonTool JSON { get; } = new();

        /// <summary>
        /// 新建一个BaseAddonController
        /// </summary>
        /// <param name="addon"></param>
        /// <param name="delegates"></param>
        public BaseAddonController(IAddon addon, Dictionary<string, object> delegates)
        {
            Addon = (T)addon;
            if (delegates.TryGetValue("WriteLine", out object? value)) MaskMethod_WriteLine = value != null ? (Action<string, string, LogLevel, bool>)value : new(DefaultPrint);
            if (delegates.TryGetValue("Error", out value)) MaskMethod_Error = value != null ? (Action<Exception>)value : new(DefaultPrint);
            MaskMethod_WriteLine ??= new(DefaultPrint);
            MaskMethod_Error ??= new(DefaultPrint);
        }

        /// <summary>
        /// 默认的输出错误消息方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="useLevel"></param>
        /// <returns></returns>
        private void DefaultPrint(string name, string msg, LogLevel level = LogLevel.Info, bool useLevel = true)
        {
            DateTime now = DateTime.Now;
            Console.Write("\r" + now.AddMilliseconds(-now.Millisecond).ToString() + $" {CommonSet.GetLogLevelPrefix(level)}/[Addon] {Addon.Name}：\n\r> ");
        }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private void DefaultPrint(Exception e) => DefaultPrint(Addon.Name, e.ToString(), LogLevel.Error);
    }
}
