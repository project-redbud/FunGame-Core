namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 角色的魔法抗性，对不同的魔法类型有不同抗性
    /// </summary>
    public class MagicResistance()
    {
        public double None { get; set; } = 0;
        public double Starmark { get; set; } = 0;
        public double PurityNatural { get; set; } = 0;
        public double PurityContemporary { get; set; } = 0;
        public double Bright { get; set; } = 0;
        public double Shadow { get; set; } = 0;
        public double Element { get; set; } = 0;
        public double Fleabane { get; set; } = 0;
        public double Particle { get; set; } = 0;
    }
}
