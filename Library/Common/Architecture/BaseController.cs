namespace Milimoe.FunGame.Core.Library.Common.Architecture
{
    public abstract class BaseController
    {
        /// <summary>
        /// 重写此方法并调用Model的Dispose方法，否则将无法正常将监听Socket的事件移除！
        /// </summary>
        public abstract void Dispose();
    }
}