using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    /// <summary>
    /// 模组的依赖集合<para/>
    /// <paramref name="Maps"></paramref>（地图名字（<see cref="GameMap.Name"/>）的数组）<para/>
    /// <paramref name="Characters"></paramref>（程序集（*.dll）名字（*）的数组）<para/>
    /// <paramref name="Skills"></paramref>（程序集（*.dll）名字（*）的数组）<para/>
    /// <paramref name="Items"></paramref>（程序集（*.dll）名字（*）的数组）
    /// </summary>
    public readonly struct GameModuleDepend(string[] Maps, string[] Characters, string[] Skills, string[] Items)
    {
        /// <summary>
        /// 模组所使用的地图组（地图名字（<see cref="GameMap.Name"/>）的数组）
        /// </summary>
        public string[] MapsDepend { get; } = Maps;

        /// <summary>
        /// 模组所使用的角色组（程序集（*.dll）名字（*）的数组）
        /// </summary>
        public string[] CharactersDepend { get; } = Characters;

        /// <summary>
        /// 模组所使用的技能组（程序集（*.dll）名字（*）的数组）
        /// </summary>
        public string[] SkillsDepend { get; } = Skills;

        /// <summary>
        /// 模组所使用的物品组（程序集（*.dll）名字（*）的数组）
        /// </summary>
        public string[] ItemsDepend { get; } = Items;

        /// <summary>
        /// 实际使用的地图组对象<para/>
        /// 请使用 <see cref="GetDependencies"/> 自动填充，不要自己添加
        /// </summary>
        public List<GameMap> Maps { get; } = [];

        /// <summary>
        /// 实际使用的角色组对象<para/>
        /// 请使用 <see cref="GetDependencies"/> 自动填充，不要自己添加
        /// </summary>
        public List<Character> Characters { get; } = [];

        /// <summary>
        /// 实际使用的技能组对象<para/>
        /// 请使用 <see cref="GetDependencies"/> 自动填充，不要自己添加
        /// </summary>
        public List<Skill> Skills { get; } = [];

        /// <summary>
        /// 实际使用的物品组对象<para/>
        /// 请使用 <see cref="GetDependencies"/> 自动填充，不要自己添加
        /// </summary>
        public List<Item> Items { get; } = [];

        /// <summary>
        /// 获得所有的依赖项<para/>
        /// 此方法会自动填充 <see cref="Maps"/> <see cref="Characters"/> <see cref="Skills"/> <see cref="Items"/>
        /// </summary>
        public void GetDependencies(GameModuleLoader loader)
        {
            Maps.Clear();
            Characters.Clear();
            Skills.Clear();
            Items.Clear();
            Maps.AddRange(loader.Maps.Keys.Where(MapsDepend.Contains).Select(str => loader.Maps[str]));
            foreach (List<Character> list in loader.Characters.Keys.Where(CharactersDepend.Contains).Select(str => loader.Characters[str]))
            {
                Characters.AddRange(list);
            }
            foreach (List<Skill> list in loader.Skills.Keys.Where(SkillsDepend.Contains).Select(str => loader.Skills[str]))
            {
                Skills.AddRange(list);
            }
            foreach (List<Item> list in loader.Items.Keys.Where(ItemsDepend.Contains).Select(str => loader.Items[str]))
            {
                Items.AddRange(list);
            }
        }
    }
}
