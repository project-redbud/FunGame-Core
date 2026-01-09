using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    public class InquiryOptions
    {
        public InquiryType InquiryType { get; } = InquiryType.None;
        public string Topic { get; set; } = "";
        public string Description { get; set; } = "";
        public Dictionary<string, string> Choices { get; set; } = [];
        public string DefaultChoice { get; set; } = "";
        public double MinNumberValue { get; set; } = 0;
        public double MaxNumberValue { get; set; } = 0;
        public double DefaultNumberValue { get; set; } = 0;
        public Dictionary<string, object> CustomArgs { get; set; } = [];

        public InquiryOptions(InquiryType type, string topic)
        {
            InquiryType = type;
            Topic = topic;
            if (type == InquiryType.BinaryChoice)
            {
                Choices.Add("是", "");
                Choices.Add("否", "");
                DefaultChoice = "否";
            }
        }
    }
}
