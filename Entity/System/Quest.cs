namespace Milimoe.FunGame.Core.Entity
{
    public class Quest()
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int EstimatedMinutes { get; set; } = 0;
        public int Status { get; set; } = 0;
        public int CharacterIndex { get; set; } = 0;
        public Dictionary<string, int> Awards { get; set; } = [];

        public override string ToString()
        {
            return $"{Id}. {Name}\r\n" +
                $"{Description}\r\n" +
                $"需要时间：{EstimatedMinutes} 分钟\r\n" +
                $"奖励：{string.Join("，", Awards.Select(kv=> kv.Key + " * " + kv.Value))}\r\n" +
                $"是否完成：{GetStatus()}";
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
    }
}
