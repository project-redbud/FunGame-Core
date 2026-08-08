using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FunGame.Core.Entity;
using FunGame.Core.Interface.Base;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Api
{
    /// <summary>
    /// 即时外发事件 id（<see cref="RoundRecordPayload.E"/> 的取值）
    /// </summary>
    public static class RoundRecordSinkEventIds
    {
        /// <summary>
        /// "0" ActionRecord：当前角色触发的操作（每次角色操作结束后发送）
        /// </summary>
        public const string Action = "0";

        /// <summary>
        /// "1" RoundRecord：当前回合的数据（每次角色操作结束后发送）
        /// </summary>
        public const string Round = "1";

        /// <summary>
        /// "2" RoundRecord：当前回合的数据，相比事件 "1" 内容更多（在回合结束时，且当前回合为检查点时才会发送）
        /// </summary>
        public const string CheckpointRound = "2";

        /// <summary>
        /// "3" CharacterStatistics：包含现阶段所有角色的统计数据（在回合结束时发送）
        /// </summary>
        public const string CharacterStatistics = "3";

        /// <summary>
        /// "4" Character[]：所有角色的完整数据（在回合结束时发送）
        /// </summary>
        public const string Characters = "4";

        /// <summary>
        /// "5" Team[]：团队的完整数据（在回合结束时发送）
        /// </summary>
        public const string Teams = "5";

        /// <summary>
        /// "6" Dictionary&lt;string, double&gt;：行动顺序表的数据（每次角色操作结束后发送），key: string=角色的Guid，value: double=当前等待时间
        /// </summary>
        public const string QueueData = "6";

        /// <summary>
        /// "7" string[]：已淘汰/处于死亡的角色名单（每次角色操作结束后发送），value: string=角色的Guid
        /// </summary>
        public const string EliminatedCharacters = "7";

        /// <summary>
        /// "8" string[]：已淘汰的团队名单（每次角色操作结束后发送），value: string=团队的Name
        /// </summary>
        public const string EliminatedTeams = "8";

        /// <summary>
        /// "13" string：验证签名事件
        /// </summary>
        public const string VerifySignature = "13";
    }

    /// <summary>
    /// 即时主动 POST 外发数据包的默认实现<para/>
    /// 将游戏过程中产生的数据包即时 POST 到外部专用服务器，服务器需存在一个 POST 方法且 body 接收与 <see cref="RoundRecordPayload"/> 相同格式的模型<para/>
    /// 若设置了 <see cref="Secret"/>，则在 <see cref="Attach"/> 后立即发送事件 "13" 验证签名，成功前其他事件不会外发，且每隔 <see cref="HandshakeRetryIntervalSeconds"/> 秒重发一次
    /// </summary>
    public class DefaultRoundRecordSink : IRoundRecordSink, IDisposable
    {
        /// <summary>
        /// 服务器的 accessToken（Bearer 认证），默认为空
        /// </summary>
        public string AccessToken { get; set; } = "";

        /// <summary>
        /// 用于验证签名的 secret，默认为空（为空时不做签名验证，直接外发；已在实例化后赋值时立即发起签名验证）
        /// </summary>
        public string Secret
        {
            get => _secret;
            set
            {
                _secret = value;
                if (_queueId != Guid.Empty && !string.IsNullOrEmpty(value))
                {
                    BeginHandshake();
                }
            }
        }

        /// <summary>
        /// 签名验证失败后重发事件 "13" 的间隔（秒），默认 60 秒，直到游戏结束
        /// </summary>
        public int HandshakeRetryIntervalSeconds { get; set; } = 60;

        /// <summary>
        /// 可访问 POST 方法的 URL
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// 会外发什么事件（事件 id，见 <see cref="RoundRecordSinkEventIds"/>）
        /// </summary>
        private readonly HashSet<string> _intents;

        /// <summary>
        /// 用于外发的 HttpClient（所有 POST 请求必须在 5 秒内返回结果，否则挂起多个请求会影响游戏性能）
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// 外发数据包的序列化选项（与 <see cref="JsonService.GeneralOptions"/> 一致，但不缩进）
        /// </summary>
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// 用于验证签名的 secret
        /// </summary>
        private string _secret = "";

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly Lock _lock = new();

        /// <summary>
        /// 所属 GamingQueue 的 Guid
        /// </summary>
        private Guid _queueId = Guid.Empty;

        /// <summary>
        /// 握手成功后的签名（null 表示未握手成功）
        /// </summary>
        private string? _signature = null;

        /// <summary>
        /// 签名验证重试定时器
        /// </summary>
        private Timer? _handshakeTimer = null;

        /// <summary>
        /// 是否正在发送签名验证请求（防止定时器重入）
        /// </summary>
        private int _handshaking = 0;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 新建一个即时主动 POST 外发数据包实现
        /// </summary>
        /// <param name="url">可访问 POST 方法的 URL</param>
        /// <param name="intents">会外发什么事件（事件 id，见 <see cref="RoundRecordSinkEventIds"/>）</param>
        public DefaultRoundRecordSink(string url, string[] intents)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _intents = [.. intents ?? []];
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
            _serializerOptions = new JsonSerializerOptions(JsonService.GeneralOptions)
            {
                WriteIndented = false
            };
        }

        /// <summary>
        /// 绑定所属队列（GamingQueue 设置 <see cref="Model.Queue.GamingQueue.RoundRecordSink"/> 属性时调用）
        /// </summary>
        /// <param name="queueId">所属 GamingQueue 的 Guid</param>
        public void Attach(Guid queueId)
        {
            _queueId = queueId;
            if (!string.IsNullOrEmpty(Secret))
            {
                BeginHandshake();
            }
        }

        /// <summary>
        /// 游戏结束通知（停止签名验证重试）
        /// </summary>
        public void End()
        {
            lock (_lock)
            {
                _handshakeTimer?.Dispose();
                _handshakeTimer = null;
            }
        }

        /// <summary>
        /// 外发单次操作记录
        /// </summary>
        public void SendAction(ActionRecord action) => TrySend(RoundRecordSinkEventIds.Action, action);

        /// <summary>
        /// 外发当前回合数据
        /// </summary>
        public void SendRound(RoundRecord round) => TrySend(RoundRecordSinkEventIds.Round, round);

        /// <summary>
        /// 外发检查点回合记录
        /// </summary>
        public void SendCheckpointRound(RoundRecord round) => TrySend(RoundRecordSinkEventIds.CheckpointRound, round);

        /// <summary>
        /// 外发现阶段所有角色的统计数据
        /// </summary>
        public void SendCharacterStatistics(Dictionary<Guid, CharacterStatistics> statistics) => TrySend(RoundRecordSinkEventIds.CharacterStatistics, statistics);

        /// <summary>
        /// 外发所有角色的完整数据
        /// </summary>
        public void SendCharacters(IEnumerable<Character> characters) => TrySend(RoundRecordSinkEventIds.Characters, characters);

        /// <summary>
        /// 外发团队的完整数据
        /// </summary>
        public void SendTeams(IEnumerable<Team> teams) => TrySend(RoundRecordSinkEventIds.Teams, teams);

        /// <summary>
        /// 外发行动顺序表的数据
        /// </summary>
        public void SendQueueData(Dictionary<Guid, double> queueData) => TrySend(RoundRecordSinkEventIds.QueueData, queueData);

        /// <summary>
        /// 外发已淘汰/处于死亡的角色名单
        /// </summary>
        public void SendEliminatedCharacters(IEnumerable<string> characterGuids) => TrySend(RoundRecordSinkEventIds.EliminatedCharacters, characterGuids);

        /// <summary>
        /// 外发已淘汰的团队名单
        /// </summary>
        public void SendEliminatedTeams(IEnumerable<string> teamNames) => TrySend(RoundRecordSinkEventIds.EliminatedTeams, teamNames);

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            End();
            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 开始签名验证（立即发送事件 "13"，失败则每隔 <see cref="HandshakeRetryIntervalSeconds"/> 秒重发一次，直到成功或游戏结束）
        /// </summary>
        private void BeginHandshake()
        {
            lock (_lock)
            {
                _signature = null;
                _handshakeTimer?.Dispose();
                _handshakeTimer = new Timer(_ => _ = TryHandshakeAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(Math.Max(1, HandshakeRetryIntervalSeconds)));
            }
        }

        /// <summary>
        /// 发送签名验证请求（事件 "13"）并确认检验结果
        /// </summary>
        private async Task TryHandshakeAsync()
        {
            if (Interlocked.CompareExchange(ref _handshaking, 1, 0) != 0)
            {
                return;
            }
            try
            {
                long t = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                // d 属性：secret 的 SHA256 哈希值，此时 s 属性为空字符串
                string d = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(Secret)));
                RoundRecordPayload payload = new()
                {
                    G = _queueId,
                    T = t,
                    E = RoundRecordSinkEventIds.VerifySignature,
                    D = d,
                    S = ""
                };
                string json = JsonSerializer.Serialize(payload, _serializerOptions);
                using HttpResponseMessage? response = await PostAsync(json).ConfigureAwait(false);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    return;
                }
                // 服务器返回的哈希：用 HMAC-SHA512 算法，将 d 属性值的前 10、后 10 个字符相连接，
                // 并在所得字符串后面补充 t 属性的值作为 key，计算原始 secret 的哈希值
                string key = d[..10] + d[^10..] + t;
                string expected = Convert.ToHexStringLower(HMACSHA512.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(Secret)));
                string? actual = (await response.Content.ReadAsStringAsync().ConfigureAwait(false))?.Trim();
                if (string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                {
                    lock (_lock)
                    {
                        // 握手成功，后续 POST 请求的 s 属性都会携带这个哈希值
                        _signature = expected;
                        _handshakeTimer?.Dispose();
                        _handshakeTimer = null;
                    }
                }
                // 不成功：等下一个周期重发事件 "13"
            }
            catch
            {
                // 服务器临时不可达：等下一个周期重发事件 "13"
            }
            finally
            {
                Interlocked.Exchange(ref _handshaking, 0);
            }
        }

        /// <summary>
        /// 尝试外发一个事件（intents 不包含该事件，或设置了 secret 但签名验证尚未成功时直接丢弃）
        /// </summary>
        private void TrySend(string e, object? d)
        {
            if (!_intents.Contains(e))
            {
                return;
            }
            if (!string.IsNullOrEmpty(Secret) && _signature == null)
            {
                // 签名验证成功之前，其他所有的事件都不会外发
                return;
            }
            try
            {
                RoundRecordPayload payload = new()
                {
                    G = _queueId,
                    T = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    E = e,
                    D = d,
                    S = _signature ?? ""
                };
                string json = JsonSerializer.Serialize(payload, _serializerOptions);
                _ = PostAsync(json);
            }
            catch
            {
                // 序列化失败：丢弃，不补发
            }
        }

        /// <summary>
        /// 异步 POST 外发数据包（不阻塞游戏线程；网络异常时返回 null，错过的消息不会补发）
        /// </summary>
        private async Task<HttpResponseMessage?> PostAsync(string json)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Post, _url);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                if (!string.IsNullOrEmpty(AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                }
                return await _client.SendAsync(request).ConfigureAwait(false);
            }
            catch
            {
                // 发送失败：丢弃，不补发
                return null;
            }
        }
    }
}
