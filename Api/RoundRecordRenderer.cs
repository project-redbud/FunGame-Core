using System.Text;
using FunGame.Core.Entity;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Api
{
    /// <summary>
    /// 展示型回放渲染器<para/>
    /// 只读取回合/操作记录数据渲染文本（不重跑引擎），供复盘、观战等消费端使用
    /// </summary>
    public static class RoundRecordRenderer
    {
        /// <summary>
        /// 渲染单个回合的完整文本（操作流 + 回合汇总）
        /// </summary>
        /// <param name="round">回合记录</param>
        /// <returns></returns>
        public static string RenderRound(RoundRecord round)
        {
            StringBuilder builder = new();
            builder.AppendLine($"=== Round {round.Round} ===");
            builder.AppendLine($"[ {round.Actor} ] 的回合");

            // 操作流（DP 系统下逐条操作，按执行顺序）
            foreach (ActionRecord action in round.Actions)
            {
                builder.AppendLine("  " + action);
            }

            // 回合汇总
            if (round.RoundRewards.Count > 0)
            {
                builder.AppendLine($"[ {round.Actor} ] 回合奖励 -> {string.Join(" / ", round.RoundRewards.Select(s => s.Name)).Trim()}");
            }
            if (round.DeathContinuousKilling.Count > 0)
            {
                builder.AppendLine(string.Join("\r\n", round.DeathContinuousKilling));
            }
            if (round.ActorContinuousKilling.Count > 0)
            {
                builder.AppendLine(string.Join("\r\n", round.ActorContinuousKilling));
            }
            if (round.Assists.Count > 0)
            {
                builder.AppendLine($"本回合助攻：[ {string.Join(" ] / [ ", round.Assists)} ]");
            }
            if (round.OtherMessages.Count > 0)
            {
                builder.AppendLine(string.Join("\r\n", round.OtherMessages));
            }

            if (round.CastTime > 0)
            {
                builder.AppendLine($"[ {round.Actor} ] 吟唱持续时间：{round.CastTime:0.##}");
            }
            else
            {
                builder.AppendLine($"[ {round.Actor} ] 回合结束，硬直时间：{round.HardnessTime:0.##}");
            }

            foreach (Character character in round.RespawnCountdowns.Keys)
            {
                builder.AppendLine($"[ {character} ] 进入复活倒计时：{round.RespawnCountdowns[character]:0.##}");
            }

            foreach (Character character in round.Respawns)
            {
                builder.AppendLine($"[ {character} ] 复活了");
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// 渲染整个对局的所有回合
        /// </summary>
        /// <param name="rounds">全部回合记录（按顺序）</param>
        /// <returns></returns>
        public static string RenderAll(IEnumerable<RoundRecord> rounds)
        {
            StringBuilder builder = new();
            foreach (RoundRecord round in rounds)
            {
                builder.AppendLine(RenderRound(round));
                builder.AppendLine();
            }
            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// 渲染指定角色的全部操作（按角色过滤）
        /// </summary>
        /// <param name="rounds">全部回合记录</param>
        /// <param name="character">目标角色（按 Guid 匹配）</param>
        /// <returns></returns>
        public static string RenderCharacterActions(IEnumerable<RoundRecord> rounds, Character character)
        {
            StringBuilder builder = new();
            foreach (RoundRecord round in rounds)
            {
                foreach (ActionRecord action in round.Actions.Where(a => a.Actor.Guid == character.Guid))
                {
                    builder.AppendLine($"[Round {round.Round}] " + action);
                }
            }
            return builder.ToString().TrimEnd();
        }
    }
}
