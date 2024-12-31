using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Quest : BaseEntity
    {
        public string Description { get; set; } = "";
        public int EstimatedMinutes { get; set; } = 0;
        public int Status { get; set; } = 0;
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
                    (Status == 1 ?
                    $"\r\n预计在 {Math.Max(Math.Round((StartTime.Value.AddMinutes(EstimatedMinutes) - DateTime.Now).TotalMinutes, MidpointRounding.ToPositiveInfinity), 1)} 分钟后完成" : "")
                    + "\r\n"
                : "") +
                $"完成奖励：{string.Join("，", Awards.Select(kv=> kv.Key + " * " + kv.Value))}\r\n" +
                $"任务状态：{GetStatus()}" +
                (SettleTime.HasValue ? $"\r\n结算时间：{SettleTime.Value.ToString(General.GeneralDateTimeFormatChinese)}" : "");
        }

        public string GetIdName()
        {
            return Id + "." + Name;
        }

        private string GetStatus()
        {
            return Status switch
            {
                1 => "进行中",
                2 => "已完成",
                3 => "已结算",
                _ => "未开始"
            };
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Quest && other.Id == Id;
        }
    }
}
