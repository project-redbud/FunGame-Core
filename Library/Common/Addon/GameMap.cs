using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Addons;
using Milimoe.FunGame.Core.Interface.Base;

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
        public abstract int Length { get; }

        /// <summary>
        /// 宽度
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// 高度
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// 格子大小
        /// </summary>
        public abstract float Size { get; }

        /// <summary>
        /// 格子集
        /// </summary>
        public Dictionary<long, Grid> Grids { get; } = [];
        
        /// <summary>
        /// 格子集（基于坐标）
        /// </summary>
        public Dictionary<(int x, int y, int z), Grid> GridsByCoordinate { get; } = [];
        
        /// <summary>
        /// 角色集
        /// </summary>
        public Dictionary<Character, Grid> Characters { get; } = [];

        /// <summary>
        /// 使用坐标获取格子，0号格子的坐标是(0, 0)，如果你还有高度的话，则是(0, 0, 0)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Grid? this[int x, int y, int z = 0]
        {
            get
            {
                if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? grid))
                {
                    return grid;
                }
                return null;
            }
        }

        /// <summary>
        /// 使用编号获取格子，从0号开始
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Grid? this[long id]
        {
            get
            {
                if (Grids.TryGetValue(id, out Grid? grid))
                {
                    return grid;
                }
                return null;
            }
        }

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
                for (int x = 0; x < Length; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        for (int z = 0; z < Height; z++)
                        {
                            Grid grid = new(Grids.Count, x, y, z);
                            Grids.Add(Grids.Count, grid);
                            GridsByCoordinate.Add((x, y, z), grid);
                        }
                    }
                }
            }
            return IsLoaded;
        }

        /// <summary>
        /// 地图完全加载后需要做的事
        /// </summary>
        public virtual void AfterLoad(GameModuleLoader loader, params object[] args)
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

        /// <summary>
        /// 获取角色当前所在的格子
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Grid? GetCharacterCurrentGrid(Character character)
        {
            if (Characters.TryGetValue(character, out Grid? current))
            {
                return current;
            }
            return null;
        }

        /// <summary>
        /// 强制设置角色当前所在的格子
        /// </summary>
        /// <param name="character"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool SetCharacterCurrentGrid(Character character, Grid target)
        {
            Grid? current = GetCharacterCurrentGrid(character);
            current?.Characters.Remove(character);
            if (Grids.ContainsValue(target))
            {
                target.Characters.Add(character);
                Characters[character] = target;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将角色从地图中移除
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public void RemoveCharacter(Character character)
        {
            Grid? current = GetCharacterCurrentGrid(character);
            current?.Characters.Remove(character);
            Characters[character] = Grid.Empty;
        }

        /// <summary>
        /// 获取以某个格子为中心，一定范围内的格子（曼哈顿距离），只考虑同一平面的格子，不包含中心格子。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual List<Grid> GetGridsByRange(Grid grid, int range)
        {
            List<Grid> grids = [];

            for (int dx = -range; dx <= range; ++dx)
            {
                for (int dy = -range; dy <= range; ++dy)
                {
                    //限制在中心点周围范围内
                    if (Math.Abs(dx) + Math.Abs(dy) <= range)
                    {
                        //检查是否在棋盘范围内
                        int x = grid.X;
                        int y = grid.Y;
                        int z = grid.Z;
                        if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? select) && select != null)
                        {
                            grids.Add(select);
                        }
                    }
                }
            }
            grids.RemoveAll(g => g.Id == grid.Id);

            return grids;
        }

        /// <summary>
        /// 设置角色移动
        /// </summary>
        /// <param name="character"></param>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns>移动的步数，只算平面移动步数</returns>
        public virtual int CharacterMove(Character character, Grid? current, Grid target)
        {
            if (current is null || current.Id < 0 || target.Id < 0 || !Grids.ContainsValue(target))
            {
                return -1;
            }

            Grid? realGrid = GetCharacterCurrentGrid(character);

            if (current.Id == target.Id)
            {
                return 0;
            }

            // 记录走到某个格子时的步数
            Queue<(Grid grid, int steps)> queue = new();
            // 记录已访问的格子
            HashSet<long> visited = [];

            // 将起始格子加入队列，步数为0，并标记为已访问
            queue.Enqueue((current, 0));
            visited.Add(current.Id);

            while (queue.Count > 0)
            {
                var (currentGrid, currentSteps) = queue.Dequeue();

                // 如果当前格子就是目标格子，则找到了最短路径
                if (currentGrid.Id == target.Id)
                {
                    realGrid?.Characters.Remove(character);
                    current.Characters.Remove(character);
                    target.Characters.Add(character);
                    Characters[character] = target;
                    return currentSteps;
                }

                // 定义平面移动的四个方向
                (int dx, int dy)[] directions = [
                    (0, 1), // 上
                    (0, -1), // 下
                    (1, 0), // 右
                    (-1, 0) // 左
                ];

                foreach (var (dx, dy) in directions)
                {
                    int nextX = currentGrid.X + dx;
                    int nextY = currentGrid.Y + dy;
                    int nextZ = currentGrid.Z;

                    // 尝试获取相邻格子
                    Grid? neighborGrid = this[nextX, nextY, nextZ];

                    // 如果相邻格子存在且未被访问过
                    if (neighborGrid != null && !visited.Contains(neighborGrid.Id))
                    {
                        visited.Add(neighborGrid.Id);
                        queue.Enqueue((neighborGrid, currentSteps + 1));
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 初始化游戏队列
        /// </summary>
        /// <param name="queue"></param>
        public virtual GameMap InitGamingQueue(IGamingQueue queue)
        {
            return this;
        }

        /// <summary>
        /// 在事件流逝前处理
        /// </summary>
        /// <param name="timeToReduce"></param>
        protected virtual void BeforeTimeElapsed(ref double timeToReduce)
        {

        }
        
        /// <summary>
        /// 在事件流逝后处理
        /// </summary>
        /// <param name="timeToReduce"></param>
        protected virtual void AfterTimeElapsed(ref double timeToReduce)
        {

        }

        /// <summary>
        /// 时间流逝时，处理格子上的特效
        /// </summary>
        /// <param name="timeToReduce"></param>
        public void OnTimeElapsed(double timeToReduce)
        {
            BeforeTimeElapsed(ref timeToReduce);

            foreach (Grid grid in Grids.Values)
            {
                List<Effect> effects = [.. grid.Effects];
                foreach (Effect effect in effects)
                {
                    if (effect.Durative)
                    {
                        if (effect.RemainDuration < timeToReduce)
                        {
                            // 移除特效前也完成剩余时间内的效果
                            effect.OnTimeElapsed(grid, effect.RemainDuration);
                            effect.RemainDuration = 0;
                            grid.Effects.Remove(effect);
                        }
                        else
                        {
                            effect.RemainDuration -= timeToReduce;
                            effect.OnTimeElapsed(grid, timeToReduce);
                        }
                    }
                    else
                    {
                        effect.OnTimeElapsed(grid, timeToReduce);
                    }
                }
            }

            AfterTimeElapsed(ref timeToReduce);
        }
    }
}
