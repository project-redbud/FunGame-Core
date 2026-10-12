using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 继承此类以表示神经校准特效
    /// </summary>
    public abstract class NeuralCalibrationEffect : Effect
    {
        public WeaponType SupportedWeaponType { get; set; } = WeaponType.None;
    }
}
