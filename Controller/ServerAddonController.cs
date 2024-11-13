using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Library.SQLScript.Common;

namespace Milimoe.FunGame.Core.Controller
{
    /// <summary>
    /// 这个控制器在 Base 的基础上添加了 SQLHelper 和 MailSender
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 新建一个ServerAddonController
    /// </remarks>
    /// <param name="addon"></param>
    /// <param name="delegates"></param>
    public class ServerAddonController<T>(IAddon addon, Dictionary<string, object> delegates) : BaseAddonController<T>(addon, delegates), IServerAddon where T : IAddon
    {
        /// <summary>
        /// 数据库连接器
        /// </summary>
        public SQLHelper? SQLHelper => _sqlHelper;

        /// <summary>
        /// 邮件发送器
        /// </summary>
        public MailSender? MailSender => _mailSender;

        private SQLHelper? _sqlHelper = null;
        private MailSender? _mailSender = null;
        private Task? _sqlPolling = null;
        private CancellationTokenSource? _cts = null;

        /// <summary>
        /// 新建 SQLHelper
        /// </summary>
        public void NewSQLHelper()
        {
            if (_sqlHelper is null)
            {
                // 创建持久化 SQLHelper
                _sqlHelper = Factory.OpenFactory.GetSQLHelper();
                if (_sqlHelper != null)
                {
                    _cts = new();
                    _sqlPolling = Task.Run(async () =>
                    {
                        await Task.Delay(30);
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
                                _sqlHelper?.ExecuteDataSet(ServerLoginLogs.Select_GetLastLoginTime());
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                            catch (Exception e)
                            {
                                Error(e);
                            }
                        }
                    }, _cts.Token);
                }
            }
            else
            {
                WriteLine("已经创建过 SQLHelper 实例。");
            }
        }

        /// <summary>
        /// 新建 MailSender
        /// </summary>
        public void NewMailSender()
        {
            if (_mailSender is null)
            {
                _mailSender = Factory.OpenFactory.GetMailSender();
            }
            else
            {
                WriteLine("已经创建过 MailSender 实例。");
            }
        }

        /// <summary>
        /// 关闭插件的服务
        /// </summary>
        public async void Close()
        {
            _mailSender?.Dispose();
            _mailSender = null;
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
