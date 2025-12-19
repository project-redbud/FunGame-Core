using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 继承此类以表示神经校准特效
    /// </summary>
    public class NeuralCalibrationEffect : Effect
    {
        public WeaponType SupportedWeaponType { get; set; } = WeaponType.None;
    }
}
