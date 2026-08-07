using FunGame.Core.Model.Framework;

namespace FunGame.Core.Interface.Base
{
    /// <summary>
    /// 回合记录外发通道<para/>
    /// 实现方负责将记录序列化并发送（如 POST 到远程服务、写入消息队列等）
    /// </summary>
    public interface IRoundRecordSink
    {
        /// <summary>
        /// 外发单次操作记录（操作完成后即时调用，用于实时增量推送）
        /// </summary>
        /// <param name="action">操作记录（结构快照，实体引用共享）</param>
        void SendAction(ActionRecord action);

        /// <summary>
        /// 外发回合记录（回合决策完成后调用，包含本回合操作流与回合汇总）
        /// </summary>
        /// <param name="round">回合记录（结构快照，实体引用共享）</param>
        void SendRound(RoundRecord round);
    }
}
