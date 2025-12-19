using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model
{
    /// <summary>
    /// 核心属性增强结构
    /// </summary>
    public readonly struct AttributeBoost(PrimaryAttribute pa, double value)
    {
        public PrimaryAttribute PrimaryAttribute => pa;
        public double Value => value;
    }
}
