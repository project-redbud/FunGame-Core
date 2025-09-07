using System.Drawing;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public class Grid(int id, int x, int y, int z)
    {
        /// <summary>
        /// 空格子
        /// </summary>
        public static Grid Empty { get; } = new Grid(-1, 0, 0, 0);

        /// <summary>
        /// 格子编号
        /// </summary>
        public int Id { get; } = id;

        /// <summary>
        /// 格子在地图中的x坐标
        /// </summary>
        public int X { get; } = x;

        /// <summary>
        /// 格子在地图中的y坐标
        /// </summary>
        public int Y { get; } = y;

        /// <summary>
        /// 格子在地图中的z坐标
        /// </summary>
        public int Z { get; } = z;

        /// <summary>
        /// 是谁站在这格子上？
        /// </summary>
        public HashSet<Character> Characters { get; set; } = [];

        /// <summary>
        /// 此格子目前受到了什么影响？
        /// </summary>
        public HashSet<Effect> Effects { get; set; } = [];

        /// <summary>
        /// 此格子呈现的颜色（默认为 <see cref="Color.Gray"/> ）
        /// </summary>
        public Color Color { get; set; } = Color.Gray;

        /// <summary>
        /// 默认的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Grid: {Id} ({X}, {Y}, {Z})";
        }
    }
}
