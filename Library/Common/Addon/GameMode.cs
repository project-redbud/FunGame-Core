using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class GameMode : IGameMode
    {
        /// <summary>
        /// 模组名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 模组描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 模组版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 模组作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 模组所使用的地图
        /// </summary>
        public abstract GameMap Map { get; }

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool IsLoaded = false;

        /// <summary>
        /// 加载模组
        /// </summary>
        public bool Load(params object[] objs)
        {
            if (IsLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此模组
            if (BeforeLoad())
            {
                // 模组加载后，不允许再次加载此模组
                IsLoaded = true;
                // 初始化此模组（传入委托或者Model）
                Init(objs);
                // 触发绑定事件
                BindEvent();
                // 如果加载后需要执行代码，请重写AfterLoad方法
                AfterLoad();
            }
            return IsLoaded;
        }

        /// <summary>
        /// 模组加载后需要做的事
        /// </summary>
        protected virtual void AfterLoad()
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此模组
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad()
        {
            return true;
        }

        /// <summary>
        /// 传递委托以便让模组调用
        /// </summary>
        private void Init(params object[] objs)
        {
            if (objs.Length > 0) WritelnSystemInfo = (Action<string>)objs[0];
            if (objs.Length > 1) NewDataRequest = (Func<DataRequestType, DataRequest>)objs[1];
            if (objs.Length > 2) NewLongRunningDataRequest = (Func<DataRequestType, DataRequest>)objs[2];
            if (objs.Length > 3) Session = (Session)objs[3];
            if (objs.Length > 4) Config = (FunGameConfig)objs[4];
        }

        /// <summary>
        /// 输出系统消息
        /// </summary>
        protected Action<string> WritelnSystemInfo = new(msg => Console.Write("\r" + msg + "\n\r> "));

        /// <summary>
        /// 基于本地已连接的Socket创建新的数据请求
        /// </summary>
        protected Func<DataRequestType, DataRequest> NewDataRequest = new(type => throw new ConnectFailedException());

        /// <summary>
        /// 基于本地已连接的Socket创建长时间运行的数据请求
        /// </summary>
        protected Func<DataRequestType, DataRequest> NewLongRunningDataRequest = new(type => throw new ConnectFailedException());

        /// <summary>
        /// Session对象
        /// </summary>
        protected Session Session = new();

        /// <summary>
        /// Config对象
        /// </summary>
        protected FunGameConfig Config = new();

        /// <summary>
        /// 绑定事件。在<see cref="BeforeLoad"/>后触发
        /// </summary>
        private void BindEvent()
        {

        }
    }
}
