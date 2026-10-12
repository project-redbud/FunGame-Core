using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    /// <summary>
    /// 一次询问的概要记录，供回合回放与日志展示
    /// <para>由 <see cref="RoundRecord.AddInquiry"/> 在询问取得答复后写入</para>
    /// </summary>
    public class InquiryRecord
    {
        /// <summary>
        /// 被询问的角色
        /// </summary>
        public Character Character { get; set; } = new();

        /// <summary>
        /// 询问主题
        /// </summary>
        public string Topic { get; set; } = "";

        /// <summary>
        /// 询问描述
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 询问类型
        /// </summary>
        public InquiryType InquiryType { get; set; } = InquiryType.None;

        /// <summary>
        /// 可选项（键 -> 说明）
        /// </summary>
        public Dictionary<string, string> Choices { get; set; } = [];

        /// <summary>
        /// 最终选定的项（选择类询问）
        /// </summary>
        public List<string> Selected { get; set; } = [];

        /// <summary>
        /// 文本结果（文本输入类询问）
        /// </summary>
        public string TextResult { get; set; } = "";

        /// <summary>
        /// 数值结果（数值输入类询问）
        /// </summary>
        public double NumberResult { get; set; } = 0;

        /// <summary>
        /// 是否被取消
        /// </summary>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// 答复来源：区分是玩家/服务器、特效钩子、自定义决策器还是内置默认给出的结果
        /// </summary>
        public InquiryResponseSource Source { get; set; } = InquiryResponseSource.None;

        public InquiryRecord() { }

        /// <summary>
        /// 依据询问选项与最终答复构造概要
        /// </summary>
        /// <param name="character">被询问的角色</param>
        /// <param name="options">发起询问时的选项</param>
        /// <param name="response">最终答复</param>
        public InquiryRecord(Character character, InquiryOptions options, InquiryResponse response)
        {
            Character = character;
            Topic = options.Topic;
            Description = options.Description;
            InquiryType = options.InquiryType;
            Choices = new(options.Choices);
            Selected = [.. response.Choices];
            TextResult = response.TextResult;
            NumberResult = response.NumberResult;
            Cancel = response.Cancel;
            Source = response.Source;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string topic = Topic.Length > 0 ? $"（{Topic}）" : "";
            if (Cancel) return $"[ {Character} ] 取消了询问{topic}";

            string source = GetSourceName(Source);
            return InquiryType switch
            {
                InquiryType.Choice or InquiryType.MultipleChoice or InquiryType.BinaryChoice => Selected.Count > 0 ? $"[ {Character} ] 询问{topic} -> {string.Join(" / ", Selected)}（来源：{source}）" : $"[ {Character} ] 询问{topic} -> 无结果（来源：{source}）",
                InquiryType.TextInput => $"[ {Character} ] 询问{topic} -> 输入：{TextResult}（来源：{source}）",
                InquiryType.NumberInput => $"[ {Character} ] 询问{topic} -> 输入：{NumberResult:0.##}（来源：{source}）",
                _ => $"[ {Character} ] 询问{topic}（来源：{source}）",
            };
        }

        private static string GetSourceName(InquiryResponseSource source) => source switch
        {
            InquiryResponseSource.External => "外部",
            InquiryResponseSource.Effect => "特效",
            InquiryResponseSource.Custom => "自定义",
            InquiryResponseSource.AI => "AI",
            InquiryResponseSource.Default => "默认",
            _ => "未设置"
        };
    }
}
