using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.Framework
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
