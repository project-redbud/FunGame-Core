using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    /// <summary>
    /// 模组的依赖集合<para/>
    /// <paramref name="maps"></paramref>（地图名称（<see cref="GameMap.Name"/>）的数组）<para/>
    /// <paramref name="characters"></paramref>（角色模组名称（<see cref="CharacterModule.Name"/>）的数组）<para/>
    /// <paramref name="skills"></paramref>（技能模组名称（<see cref="SkillModule.Name"/>）的数组）<para/>
    /// <paramref name="items"></paramref>（物品模组名称（<see cref="ItemModule.Name"/>）的数组）
    /// </summary>
    public readonly struct GameModuleDepend(string[] maps, string[] characters, string[] skills, string[] items)
    {
        /// <summary>
        /// 模组所使用的地图组
        /// </summary>
        public string[] MapsDepend { get; } = maps;

        /// <summary>
        /// 模组所使用的角色组
        /// </summary>
        public string[] CharactersDepend { get; } = characters;

        /// <summary>
        /// 模组所使用的技能组
        /// </summary>
        public string[] SkillsDepend { get; } = skills;

        /// <summary>
        /// 模组所使用的物品组
        /// </summary>
        public string[] ItemsDepend { get; } = items;

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
            foreach (CharacterModule modules in loader.Characters.Keys.Where(CharactersDepend.Contains).Select(str => loader.Characters[str]))
            {
                Characters.AddRange(modules.Characters);
            }
            foreach (SkillModule modules in loader.Skills.Keys.Where(SkillsDepend.Contains).Select(str => loader.Skills[str]))
            {
                Skills.AddRange(modules.Skills);
            }
            foreach (ItemModule modules in loader.Items.Keys.Where(ItemsDepend.Contains).Select(str => loader.Items[str]))
            {
                Items.AddRange(modules.Items);
            }
        }
    }
}
