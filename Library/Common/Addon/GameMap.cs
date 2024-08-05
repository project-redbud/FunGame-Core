using Milimoe.FunGame.Core.Interface.Addons;

namespace Milimoe.FunGame.Core.Library.Common.Addon
{
    public abstract class GameMap : IGameMap
    {
        /// <summary>
        /// 地图名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 地图描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 地图版本
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// 地图作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 长度
        /// </summary>
        public abstract float Length { get; }
        
        /// <summary>
        /// 宽度
        /// </summary>
        public abstract float Width { get; }

        /// <summary>
        /// 高度
        /// </summary>
        public abstract float Height { get; }

        /// <summary>
        /// 格子大小
        /// </summary>
        public abstract float Size { get; }

        /// <summary>
        /// 格子集
        /// </summary>
        public Dictionary<long, Grid> Grids { get; } = [];

        /// <summary>
        /// 使用坐标获取格子，0号格子的坐标是(0, 0)，如果你还有高度的话，则是(0, 0, 0)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Grid this[float x, float y, float z = 0] => Grids.Values.Where(g => g.X == x && g.Y == y && g.Z == z).FirstOrDefault();
        
        /// <summary>
        /// 使用坐标获取格子，从0号开始
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Grid this[int id] => Grids[id];

        /// <summary>
        /// 加载标记
        /// </summary>
        private bool IsLoaded = false;

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public bool Load(params object[] objs)
        {
            if (IsLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此地图
            if (BeforeLoad())
            {
                // 地图加载后，不允许再次加载此地图
                IsLoaded = true;
                // 生成格子
                for (float x = 0; x < Length; x++)
                {
                    for (float y = 0; y< Width; y++)
                    {
                        for (float z = 0; z < Height; z++)
                        {
                            Grids.Add(Grids.Count, new(Grids.Count, x, y, z));
                        }
                    }
                }
                // 如果加载后需要执行代码，请重写AfterLoad方法
                AfterLoad();
            }
            return IsLoaded;
        }

        /// <summary>
        /// 加载后需要做的事
        /// </summary>
        protected virtual void AfterLoad()
        {
            // override
        }

        /// <summary>
        /// 允许返回false来阻止加载此地图
        /// </summary>
        /// <returns></returns>
        protected virtual bool BeforeLoad()
        {
            return true;
        }
    }
}
