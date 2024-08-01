namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    /// <summary>
    /// 模组的依赖集合
    /// </summary>
    public readonly struct GameModuleDepend(string[] Maps, string[] Characters, string[] Items, string[] Skills)
    {
        /// <summary>
        /// 模组所使用的地图组
        /// </summary>
        public string[] Maps { get; } = Maps;

        /// <summary>
        /// 模组所使用的角色组
        /// </summary>
        public string[] Characters { get; } = Characters;

        /// <summary>
        /// 模组所使用的物品组
        /// </summary>
        public string[] Items { get; } = Items;

        /// <summary>
        /// 模组所使用的技能组
        /// </summary>
        public string[] Skills { get; } = Skills;
    }
}
