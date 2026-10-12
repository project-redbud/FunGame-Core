using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
{
    public class InquiryResponse
    {
        public InquiryType InquiryType { get; } = InquiryType.None;
        public string Topic { get; set; } = "";
        public List<string> Choices { get; set; } = [];
        public string TextResult { get; set; } = "";
        public double NumberResult { get; set; } = 0;
        public bool Cancel { get; set; } = false;
        public Dictionary<string, object> CustomResponse { get; set; } = [];

        /// <summary>
        /// 答复的来源，由询问链路在各阶段标注，用于日志与回放归因
        /// </summary>
        public InquiryResponseSource Source { get; set; } = InquiryResponseSource.Default;

        public InquiryResponse(InquiryType type, string topic, InquiryResponseSource source = InquiryResponseSource.Default)
        {
            InquiryType = type;
            Topic = topic;
            Source = source;
        }

        /// <summary>
        /// 按 <paramref name="options"/> 内置的默认规则构造答复：优先取 <see cref="InquiryOptions.DefaultChoice"/>，否则取首个选项
        /// </summary>
        /// <param name="options">询问选项</param>
        /// <param name="source">答复来源，默认 <see cref="InquiryResponseSource.Default"/></param>
        public InquiryResponse(InquiryOptions options, InquiryResponseSource source = InquiryResponseSource.Default)
        {
            InquiryType = options.InquiryType;
            Topic = options.Topic;
            Source = source;
            switch (options.InquiryType)
            {
                case InquiryType.Choice:
                case InquiryType.MultipleChoice:
                case InquiryType.BinaryChoice:
                    // 默认值必须确实存在于候选中，否则回退到首个选项
                    // 这保证内置默认答复本身一定合法，上层校验失败时回退到它才不会得到同样非法的结果
                    if (options.DefaultChoice != "" && options.Choices.ContainsKey(options.DefaultChoice))
                    {
                        Choices.Add(options.DefaultChoice);
                    }
                    else if (options.Choices.Count > 0)
                    {
                        Choices.Add(options.Choices.Keys.First());
                    }
                    break;
                case InquiryType.TextInput:
                    TextResult = "";
                    break;
                case InquiryType.NumberInput:
                    NumberResult = options.DefaultNumberValue;
                    break;
                default:
                    break;
            }
        }
    }
}
