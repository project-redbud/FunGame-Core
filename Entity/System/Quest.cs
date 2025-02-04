using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Quest : BaseEntity
    {
        public string Description { get; set; } = "";
        public QuestState Status { get; set; } = 0;
        public long CharacterId { get; set; } = 0;
        public long RegionId { get; set; } = 0;
        public double CreditsAward { get; set; } = 0;
        public double MaterialsAward { get; set; } = 0;
        public HashSet<Item> Awards { get; set; } = [];
        public Dictionary<string, int> AwardsCount { get; set; } = [];
        public string AwardsString { get; set; } = "";
        public DateTime? StartTime { get; set; } = null;
        public DateTime? SettleTime { get; set; } = null;
        public QuestType QuestType { get; set; } = QuestType.Continuous;
        public int EstimatedMinutes { get; set; } = 0;
        public int Progress { get; set; } = 0;
        public int MaxProgress { get; set; } = 100;

        public override string ToString()
        {
            string progressString = "";
            if (QuestType == QuestType.Progressive)
            {
                progressString = $"\r\n当前进度：{Progress}/{MaxProgress}";
            }

            List<string> awards = [];
            if (CreditsAward > 0)
            {
                awards.Add($"{General.GameplayEquilibriumConstant.InGameCurrency} * {CreditsAward}");
            }
            if (MaterialsAward > 0)
            {
                awards.Add($"{General.GameplayEquilibriumConstant.InGameMaterial} * {MaterialsAward}");
            }
            foreach (Item item in Awards)
            {
                awards.Add($"[{ItemSet.GetQualityTypeName(item.QualityType)}|{ItemSet.GetItemTypeName(item.ItemType)}] {item.Name} * {AwardsCount[item.Name]}");
            }
            AwardsString = string.Join("，", awards);

            return $"{Id}. {Name}\r\n" +
                   $"{Description}\r\n" +
                   (QuestType == QuestType.Continuous ? $"需要时间：{EstimatedMinutes} 分钟\r\n" : "") +
                   (StartTime.HasValue ? $"开始时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)}" +
                       (Status == QuestState.InProgress && QuestType == QuestType.Continuous ?
                       $"\r\n预计在 {Math.Max(Math.Round((StartTime.Value.AddMinutes(EstimatedMinutes) - DateTime.Now).TotalMinutes, MidpointRounding.ToPositiveInfinity), 1)} 分钟后完成" : "")
                       + "\r\n"
                   : "") +
                   $"完成奖励：{AwardsString}\r\n" +
                   $"任务状态：{GetStatus()}" + progressString +
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
