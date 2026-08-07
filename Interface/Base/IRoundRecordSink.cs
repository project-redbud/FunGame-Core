using FunGame.Core.Entity;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Interface.Base
{
    /// <summary>
    /// 回合记录外发通道<para/>
    /// 实现方负责将记录序列化并发送（如 POST 到远程服务、写入消息队列等）<para/>
    /// 事件 id 与方法的对应关系："0" <see cref="SendAction"/>、"1" <see cref="SendRound"/>、"2" <see cref="SendCheckpointRound"/>、
    /// "3" <see cref="SendCharacterStatistics"/>、"4" <see cref="SendCharacters"/>、"5" <see cref="SendTeams"/>、
    /// "6" <see cref="SendQueueData"/>、"7" <see cref="SendEliminatedCharacters"/>、"8" <see cref="SendEliminatedTeams"/>
    /// </summary>
    public interface IRoundRecordSink
    {
        /// <summary>
        /// 绑定所属队列（GamingQueue 设置 <see cref="Model.Queue.GamingQueue.RoundRecordSink"/> 属性时调用，提供队列 Guid；自定义协议实现可忽略）
        /// </summary>
        /// <param name="queueId">所属 GamingQueue 的 Guid</param>
        void Attach(Guid queueId);

        /// <summary>
        /// 游戏结束通知（停止握手重试等；自定义协议实现可忽略）
        /// </summary>
        void End();

        /// <summary>
        /// 外发单次操作记录（每次角色操作完成后即时调用，用于实时增量推送）
        /// </summary>
        /// <param name="action">操作记录（结构快照，实体引用共享）</param>
        void SendAction(ActionRecord action);

        /// <summary>
        /// 外发当前回合数据（每次角色操作完成后即时调用，包含本回合已发生的操作流与汇总）
        /// </summary>
        /// <param name="round">回合记录（结构快照，实体引用共享）</param>
        void SendRound(RoundRecord round);

        /// <summary>
        /// 外发检查点回合记录（回合结束时且当前回合为检查点时调用，相比 <see cref="SendRound"/> 内容更多，附带全角色状态快照）
        /// </summary>
        /// <param name="round">回合记录（结构快照，实体引用共享）</param>
        void SendCheckpointRound(RoundRecord round);

        /// <summary>
        /// 外发现阶段所有角色的统计数据（回合结束时调用）
        /// </summary>
        /// <param name="statistics">角色 Guid -> 统计数据</param>
        void SendCharacterStatistics(Dictionary<Guid, CharacterStatistics> statistics);

        /// <summary>
        /// 外发所有角色的完整数据（回合结束时调用）
        /// </summary>
        /// <param name="characters">参与本次游戏的所有角色</param>
        void SendCharacters(IEnumerable<Character> characters);

        /// <summary>
        /// 外发团队的完整数据（回合结束时调用；非团队模式为空）
        /// </summary>
        /// <param name="teams">当前存活的团队</param>
        void SendTeams(IEnumerable<Team> teams);

        /// <summary>
        /// 外发行动顺序表的数据（每次角色操作完成后即时调用）
        /// </summary>
        /// <param name="queueData">角色 Guid -> 当前等待时间</param>
        void SendQueueData(Dictionary<Guid, double> queueData);

        /// <summary>
        /// 外发已淘汰/处于死亡的角色名单（每次角色操作完成后即时调用）
        /// </summary>
        /// <param name="characterGuids">角色的 Guid</param>
        void SendEliminatedCharacters(IEnumerable<string> characterGuids);

        /// <summary>
        /// 外发已淘汰的团队名单（回合结束时调用；非团队模式为空）
        /// </summary>
        /// <param name="teamNames">团队的 Name</param>
        void SendEliminatedTeams(IEnumerable<string> teamNames);
    }
}
