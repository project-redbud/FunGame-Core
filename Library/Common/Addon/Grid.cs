using System.Drawing;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public struct Grid(int id, float x, float y, float z)
    {
        /// <summary>
        /// 格子编号
        /// </summary>
        public int Id { get; } = id;

        /// <summary>
        /// 格子在地图中的x坐标
        /// </summary>
        public float X { get; } = x;

        /// <summary>
        /// 格子在地图中的y坐标
        /// </summary>
        public float Y { get; } = y;

        /// <summary>
        /// 格子在地图中的z坐标
        /// </summary>
        public float Z { get; } = z;

        /// <summary>
        /// 是谁站在这格子上？
        /// </summary>
        public Dictionary<string, Character> Characters { get; set; } = [];

        /// <summary>
        /// 此格子目前受到了什么影响？或者它有什么技能…
        /// </summary>
        public Dictionary<string, Skill> Skills { get; set; } = [];

        /// <summary>
        /// 此格子呈现的颜色（默认为 <see cref="Color.Gray"/> ）
        /// </summary>
        public Color Color { get; set; } = Color.Gray;
    }
}
