using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 回合奖励事件类型
    /// </summary>
    public enum RoundRewardEventKind
    {
        /// <summary>
        /// 发放（获得）
        /// </summary>
        Gained,

        /// <summary>
        /// 移除：回合结束回收、吟唱顺延清算、被打断、被剥夺类效果清除
        /// </summary>
        Lost,

        /// <summary>
        /// 夺取：归属由原持有者转移给夺取者
        /// </summary>
        Stolen
    }

    /// <summary>
    /// 一次回合奖励事件的记录，供回合日志与回放展示
    /// </summary>
    public class RoundRewardRecord
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public RoundRewardEventKind Kind { get; set; } = RoundRewardEventKind.Gained;

        /// <summary>
        /// 奖励的绑定方式
        /// </summary>
        public RoundRewardBinding Binding { get; set; } = RoundRewardBinding.Round;

        /// <summary>
        /// 奖励键：<see cref="RoundRewardBinding.Round"/> 时为全局回合；<see cref="RoundRewardBinding.Character"/> 时为该角色的行动回合序号
        /// </summary>
        public int TurnKey { get; set; } = 0;

        /// <summary>
        /// 归属角色：<see cref="RoundRewardEventKind.Gained"/> / <see cref="RoundRewardEventKind.Lost"/> 为奖励归属方；
        /// <see cref="RoundRewardEventKind.Stolen"/> 为原持有者
        /// </summary>
        public Character Character { get; set; } = new();

        /// <summary>
        /// 对方角色：仅 <see cref="RoundRewardEventKind.Stolen"/> 有值（夺取者）
        /// </summary>
        public Character? Counterpart { get; set; } = null;

        /// <summary>
        /// 涉及的奖励；夺取时为全部被夺取项
        /// </summary>
        public List<Skill> Skills { get; set; } = [];

        /// <summary>
        /// 是否为「吟唱回合顺延到结算回合」的被动奖励（仅 <see cref="RoundRewardEventKind.Lost"/> 有意义）
        /// </summary>
        public bool IsCarryOver { get; set; } = false;

        /// <summary>
        /// 绑定方式与键位的中文描述，用于日志与回放展示
        /// </summary>
        public string BindingText => Binding == RoundRewardBinding.Round ? $"全局回合 {TurnKey}" : $"行动回合 {TurnKey}";

        /// <summary>
        /// 渲染为一行日志文本
        /// </summary>
        public override string ToString()
        {
            string skills = string.Join(" / ", Skills.Select(s => s.Name));
            return Kind switch
            {
                RoundRewardEventKind.Gained => $"[ {Character} ] 获得回合奖励（{BindingText}）：{skills}",
                RoundRewardEventKind.Stolen => $"[ {Character} ] 的回合奖励（{BindingText}）被 [ {Counterpart} ] 夺取：{skills}",
                _ => $"[ {Character} ] 失去回合奖励（{BindingText}{(IsCarryOver ? "，顺延结算" : "")}）：{skills}",
            };
        }
    }
}
