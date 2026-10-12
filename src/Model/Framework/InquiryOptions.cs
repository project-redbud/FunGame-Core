using FunGame.Core.Library.Constant;
using FunGame.Core.Model.EffectContext;

namespace FunGame.Core.Model.Framework
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
        public bool CanCancel { get; set; } = true;
        public Dictionary<string, object> CustomArgs { get; set; } = [];

        /// <summary>
        /// 兜底决策器：当外部事件与特效钩子都未给出答复时调用，供模组或上层介入决策
        /// <para>返回 null 表示不介入，继续下沉到框架内置的默认规则</para>
        /// </summary>
        public Func<InquiryContext, InquiryResponse?>? FallbackResolver { get; set; } = null;

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
