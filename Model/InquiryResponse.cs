using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
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

        public InquiryResponse(InquiryType type, string topic)
        {
            InquiryType = type;
            Topic = topic;
        }

        public InquiryResponse(InquiryOptions options)
        {
            InquiryType = options.InquiryType;
            Topic = options.Topic;
            switch (options.InquiryType)
            {
                case InquiryType.Choice:
                case InquiryType.MultipleChoice:
                case InquiryType.BinaryChoice:
                    if (options.DefaultChoice != "")
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
