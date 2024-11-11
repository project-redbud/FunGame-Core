using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.SQLScript.Common;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class WebAPIPlugin : IAddon, IServerAddon, IAddonController<IAddon>
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 插件作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 包含了一些常用方法的控制器
        /// </summary>
        public BaseAddonController<IAddon> Controller
        {
            get => _controller ?? throw new NotImplementedException();
            set => _controller = value;
        }

        /// <summary>
        /// 控制器内部变量
        /// </summary>
        private BaseAddonController<IAddon>? _controller;

        /// <summary>
        /// 全局数据库连接器
        /// </summary>
        public SQLHelper? SQLHelper { get; set; } = null;

        /// <summary>
        /// 全局邮件发送器
        /// </summary>
        public MailSender? MailSender { get; set; } = null;

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool _isLoaded = false;

        /// <summary>
        /// SQL 线程
        /// </summary>
        private Task? _sqlPolling = null;
        
        /// <summary>
        /// SQL 线程取消标记
        /// </summary>
        private CancellationTokenSource? _cts = null;

        /// <summary>
        /// 加载插件
        /// </summary>
        public bool Load(params object[] objs)
        {
            if (_isLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此插件
            if (BeforeLoad(objs))
            {
                // 创建持久化 SQLHelper
                _cts = new();
                _sqlPolling = Task.Run(async () =>
                {
                    await Task.Delay(30);
                    SQLHelper = Factory.OpenFactory.GetSQLHelper();
                    if (SQLHelper != null)
                    {
                        while (true)
                        {
                            if (_cts.Token.IsCancellationRequested)
                            {
                                break;
                            }
                            // 每两小时触发一次SQL服务器的心跳查询，防止SQL服务器掉线
                            try
                            {
                                await Task.Delay(2 * 1000 * 3600);
                                SQLHelper?.ExecuteDataSet(ServerLoginLogs.Select_GetLastLoginTime());
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (System.Exception e)
                            {
                                Controller.Error(e);
                            }
                        }
                    }
                }, _cts.Token);
                // 插件加载后，不允许再次加载此插件
                _isLoaded = true;
            }
            return _isLoaded;
        }

        /// <summary>
        /// 接收服务器控制台的输入
        /// </summary>
        /// <param name="input"></param>
        public abstract void ProcessInput(string input);

        /// <summary>
        /// 插件完全加载后需要做的事
        /// </summary>
        public virtual void AfterLoad(params object[] objs)
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此插件
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad(params object[] objs)
        {
            return true;
        }

        /// <summary>
        /// 关闭插件的服务
        /// </summary>
        public async void Close()
        {
            _cts?.Cancel();
            if (_sqlPolling != null)
            {
                await _sqlPolling;
                _sqlPolling.Dispose();
                _sqlPolling = null;
            }
            _cts?.Dispose();
        }
    }
}
