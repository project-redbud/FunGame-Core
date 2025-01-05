using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Quest : BaseEntity
    {
        public string Description { get; set; } = "";
        public int EstimatedMinutes { get; set; } = 0;
        public QuestState Status { get; set; } = 0;
        public int CharacterIndex { get; set; } = 0;
        public Dictionary<string, int> Awards { get; set; } = [];
        public DateTime? StartTime { get; set; } = null;
        public DateTime? SettleTime { get; set; } = null;

        public override string ToString()
        {
            return $"{Id}. {Name}\r\n" +
                $"{Description}\r\n" +
                $"需要时间：{EstimatedMinutes} 分钟\r\n" +
                (StartTime.HasValue ? $"开始时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)}" +
                    (Status == QuestState.InProgress ?
                    $"\r\n预计在 {Math.Max(Math.Round((StartTime.Value.AddMinutes(EstimatedMinutes) - DateTime.Now).TotalMinutes, MidpointRounding.ToPositiveInfinity), 1)} 分钟后完成" : "")
                    + "\r\n"
                : "") +
                $"完成奖励：{string.Join("，", Awards.Select(kv=> kv.Key + " * " + kv.Value))}\r\n" +
                $"任务状态：{GetStatus()}" +
                (SettleTime.HasValue ? $"\r\n结算时间：{SettleTime.Value.ToString(General.GeneralDateTimeFormatChinese)}" : "");
        }

        private string GetStatus()
        {
            return Status switch
            {
                QuestState.InProgress => "进行中",
                QuestState.Completed => "已完成",
                QuestState.Settled => "已结算",
                _ => "未开始"
            };
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Quest && other.Id == Id;
        }
    }
}
