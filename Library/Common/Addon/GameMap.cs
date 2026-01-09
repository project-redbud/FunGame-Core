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
        private bool _isLoaded = false;

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public bool Load(params object[] objs)
        {
            if (_isLoaded)
            {
                return false;
            }
            // BeforeLoad可以阻止加载此地图
            if (BeforeLoad())
            {
                // 地图加载后，不允许再次加载此地图
                _isLoaded = true;
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
            return _isLoaded;
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
        /// 初始化游戏队列，要求返回一个新的地图实例，而不是 this
        /// </summary>
        /// <param name="queue"></param>
        public abstract GameMap InitGamingQueue(IGamingQueue queue);

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
        /// 获取以某个格子为中心，一定范围内的格子（曼哈顿距离），只考虑同一平面的格子。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        /// <param name="includeCharacter"></param>
        /// <returns></returns>
        public virtual List<Grid> GetGridsByRange(Grid grid, int range, bool includeCharacter = false)
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
                        int x = grid.X + dx;
                        int y = grid.Y + dy;
                        int z = grid.Z;
                        if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? select) && select != null)
                        {
                            if (includeCharacter || select.Characters.Count == 0)
                            {
                                grids.Add(select);
                            }
                        }
                    }
                }
            }

            return grids;
        }

        /// <summary>
        /// 获取以某个格子为中心，最远距离的格子（曼哈顿距离），只考虑同一平面的格子。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        /// <param name="includeCharacter"></param>
        /// <returns></returns>
        public virtual List<Grid> GetOuterGridsByRange(Grid grid, int range, bool includeCharacter = false)
        {
            List<Grid> grids = [];

            if (range < 0)
            {
                return grids;
            }

            // 遍历以中心格子为中心的方形区域
            // dx和dy的范围从 -range 到 +range
            for (int dx = -range; dx <= range; ++dx)
            {
                for (int dy = -range; dy <= range; ++dy)
                {
                    // 只有当曼哈顿距离恰好等于 range 时，才认为是最远距离的格子
                    if (Math.Abs(dx) + Math.Abs(dy) == range)
                    {
                        int x = grid.X + dx;
                        int y = grid.Y + dy;
                        int z = grid.Z; // 只考虑同一平面

                        // 检查格子是否存在于地图中
                        if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? select) && select != null)
                        {
                            if (includeCharacter || select.Characters.Count == 0)
                            {
                                grids.Add(select);
                            }
                        }
                    }
                }
            }

            return grids;
        }

        /// <summary>
        /// 获取以某个格子为中心，一定半径内的格子（圆形范围，欧几里得距离），只考虑同一平面的格子。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        /// <param name="includeCharacter"></param>
        /// <returns></returns>
        public virtual List<Grid> GetGridsByCircleRange(Grid grid, int range, bool includeCharacter = false)
        {
            List<Grid> grids = [];

            // 预计算半径的平方
            int rangeSquared = range * range;

            // 遍历以中心格子为中心的区域
            // 范围从 -range 到 +range，覆盖所有可能的圆形区域内的格子
            for (int dx = -range; dx <= range; ++dx)
            {
                for (int dy = -range; dy <= range; ++dy)
                {
                    // 计算当前格子与中心格子的欧几里得距离的平方
                    if ((dx * dx) + (dy * dy) <= rangeSquared)
                    {
                        int x = grid.X + dx;
                        int y = grid.Y + dy;
                        int z = grid.Z;

                        if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? select) && select != null)
                        {
                            if (includeCharacter || select.Characters.Count == 0)
                            {
                                grids.Add(select);
                            }
                        }
                    }
                }
            }

            return grids;
        }

        /// <summary>
        /// 获取以某个格子为中心，最远距离的格子（圆形范围，欧几里得距离），只考虑同一平面的格子。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        /// <param name="includeCharacter"></param>
        /// <returns></returns>
        public virtual List<Grid> GetOuterGridsByCircleRange(Grid grid, int range, bool includeCharacter = false)
        {
            List<Grid> grids = [];

            // 预计算半径的平方
            int rangeSquared = range * range;

            // 遍历以中心格子为中心的区域
            // 范围从 -range 到 +range，覆盖所有可能的圆形区域内的格子
            for (int dx = -range; dx <= range; ++dx)
            {
                for (int dy = -range; dy <= range; ++dy)
                {
                    // 计算当前格子与中心格子的欧几里得距离的平方
                    if ((dx * dx) + (dy * dy) == rangeSquared)
                    {
                        int x = grid.X + dx;
                        int y = grid.Y + dy;
                        int z = grid.Z;

                        if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? select) && select != null)
                        {
                            if (includeCharacter || select.Characters.Count == 0)
                            {
                                grids.Add(select);
                            }
                        }
                    }
                }
            }

            return grids;
        }

        /// <summary>
        /// 获取以某个格子为中心，一定范围内的格子（正方形，切比雪夫距离），只考虑同一平面的格子。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        /// <param name="includeCharacter"></param>
        /// <returns></returns>
        public virtual List<Grid> GetGridsBySquareRange(Grid grid, int range, bool includeCharacter = false)
        {
            List<Grid> grids = [];

            if (range < 0) return grids;

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    // 切比雪夫距离：max(|dx|, |dy|) <= range
                    if (Math.Max(Math.Abs(dx), Math.Abs(dy)) <= range)
                    {
                        int x = grid.X + dx;
                        int y = grid.Y + dy;
                        int z = grid.Z;

                        if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? select) && select != null)
                        {
                            if (includeCharacter || select.Characters.Count == 0)
                            {
                                grids.Add(select);
                            }
                        }
                    }
                }
            }

            return grids;
        }

        /// <summary>
        /// 获取以某个格子为中心，最远距离的格子（正方形，切比雪夫距离），只考虑同一平面的格子。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        /// <param name="includeCharacter"></param>
        /// <returns></returns>
        public virtual List<Grid> GetOuterGridsBySquareRange(Grid grid, int range, bool includeCharacter = false)
        {
            List<Grid> grids = [];

            if (range < 0) return grids;

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (Math.Max(Math.Abs(dx), Math.Abs(dy)) == range)
                    {
                        int x = grid.X + dx;
                        int y = grid.Y + dy;
                        int z = grid.Z;

                        if (GridsByCoordinate.TryGetValue((x, y, z), out Grid? select) && select != null)
                        {
                            if (includeCharacter || select.Characters.Count == 0)
                            {
                                grids.Add(select);
                            }
                        }
                    }
                }
            }

            return grids;
        }

        /// <summary>
        /// 使用布雷森汉姆直线算法获取从起点到终点的所有格子（包含起点和终点）。
        /// 若 passThrough 为 true，则继续向同一方向延伸直到地图边缘。只考虑同一平面的格子。
        /// </summary>
        /// <param name="casterGrid">施法者格子</param>
        /// <param name="targetGrid">目标格子</param>
        /// <param name="passThrough">是否贯穿至地图边缘</param>
        /// <param name="includeCharacter">是否包含有角色的格子</param>
        /// <returns>直线上的格子列表</returns>
        public virtual List<Grid> GetGridsOnLine(Grid casterGrid, Grid targetGrid, bool passThrough = false, bool includeCharacter = false)
        {
            List<Grid> grids = [];

            if (casterGrid == Grid.Empty || targetGrid == Grid.Empty || casterGrid.Z != targetGrid.Z)
            {
                if (targetGrid != Grid.Empty && (includeCharacter || targetGrid.Characters.Count == 0))
                    grids.Add(targetGrid);
                return grids;
            }

            int x0 = casterGrid.X;
            int y0 = casterGrid.Y;
            int x1 = targetGrid.X;
            int y1 = targetGrid.Y;
            int z = casterGrid.Z;

            // 始终包含起点
            if (includeCharacter || casterGrid.Characters.Count == 0)
                grids.Add(casterGrid);

            // 计算直线上的所有整数点
            List<(int x, int y)> points = GetLinePoints(x0, y0, x1, y1);

            // 添加中间点（不包括起点和终点）
            for (int i = 1; i < points.Count - 1; i++)
            {
                (int x, int y) = points[i];
                Grid? current = this[x, y, z];
                if (current != null && (includeCharacter || current.Characters.Count == 0) && !grids.Contains(current))
                {
                    grids.Add(current);
                }
            }

            // 添加终点（如果与起点不同）
            if (!(x0 == x1 && y0 == y1))
            {
                Grid? target = this[x1, y1, z];
                if (target != null && (includeCharacter || target.Characters.Count == 0) && !grids.Contains(target))
                {
                    grids.Add(target);
                }
            }

            // 贯穿模式：继续向目标方向延伸直到地图边缘
            if (passThrough && points.Count >= 2)
            {
                // 获取方向向量（从最后第二个点到最后第一个点）
                int lastIndex = points.Count - 1;
                int dirX = points[lastIndex].x - points[lastIndex - 1].x;
                int dirY = points[lastIndex].y - points[lastIndex - 1].y;

                // 规范化方向（保证每次移动一个单位）
                if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
                {
                    int gcd = GCD(Math.Abs(dirX), Math.Abs(dirY));
                    dirX /= gcd;
                    dirY /= gcd;
                }

                int extendX = x1 + dirX;
                int extendY = y1 + dirY;

                // 设置最大延伸步数，防止无限循环
                int maxSteps = Math.Max(Length, Width) * 2;
                int steps = 0;

                while (steps < maxSteps)
                {
                    // 检查坐标是否在地图边界内
                    if (extendX < 0 || extendX >= Length ||
                        extendY < 0 || extendY >= Width)
                        break;

                    Grid? extendGrid = this[extendX, extendY, z];
                    if (extendGrid == null) break;

                    if ((includeCharacter || extendGrid.Characters.Count == 0) && !grids.Contains(extendGrid))
                    {
                        grids.Add(extendGrid);
                    }

                    extendX += dirX;
                    extendY += dirY;
                    steps++;
                }
            }

            return grids;
        }

        /// <summary>
        /// 使用布雷森汉姆直线算法获取从起点到终点的所有格子（包含起点和终点）并考虑宽度。
        /// 若 passThrough 为 true，则继续向同一方向延伸直到地图边缘。只考虑同一平面的格子。
        /// </summary>
        /// <param name="start"></param>
        /// <param name="directionRef"></param>
        /// <param name="range"></param>
        /// <param name="passThrough"></param>
        /// <param name="includeChar"></param>
        /// <returns></returns>
        public virtual List<Grid> GetGridsOnThickLine(Grid start, Grid directionRef, int range, bool passThrough = false, bool includeChar = false)
        {
            List<Grid> line = GetGridsOnLine(start, directionRef, passThrough, includeCharacter: true);
            List<Grid> result = [];

            foreach (Grid g in line)
            {
                List<Grid> around = GetGridsBySquareRange(g, range / 2, includeCharacter: true);
                foreach (Grid a in around)
                {
                    if (!result.Contains(a) && (includeChar || a.Characters.Count == 0))
                    {
                        result.Add(a);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取扇形范围内的格子
        /// 扇形以 casterGrid 为顶点，向 targetGrid 方向张开
        /// </summary>
        /// <param name="targetGrid">目标格子，即扇形顶点</param>
        /// <param name="casterGrid">施法者格子，用于确定朝向</param>
        /// <param name="range">最大半径</param>
        /// <param name="angleDegrees">扇形角度，默认 90</param>
        /// <param name="includeCharacter">是否包含有角色的格子</param>
        /// <returns></returns>
        public virtual List<Grid> GetGridsInSector(Grid casterGrid, Grid targetGrid, int range, double angleDegrees = 90, bool includeCharacter = false)
        {
            List<Grid> grids = [];

            if (casterGrid == Grid.Empty || targetGrid == Grid.Empty || casterGrid.Z != targetGrid.Z)
                return grids;

            if (range <= 0)
            {
                if (includeCharacter || casterGrid.Characters.Count == 0)
                    grids.Add(casterGrid);
                return grids;
            }

            int z = casterGrid.Z;

            // 计算朝向向量：从施法者指向目标点
            double dirX = targetGrid.X - casterGrid.X;
            double dirY = targetGrid.Y - casterGrid.Y;
            double dirLength = Math.Sqrt(dirX * dirX + dirY * dirY);

            // 如果目标和施法者重合，退化为圆形范围
            if (dirLength < 0.01)
            {
                return GetGridsByCircleRange(casterGrid, range - 1 > 0 ? range - 1 : 0, includeCharacter);
            }

            // 单位朝向向量
            double unitDirX = dirX / dirLength;
            double unitDirY = dirY / dirLength;

            // 半角（弧度）
            double halfAngleRad = (angleDegrees / 2) * (Math.PI / 180.0);

            // 遍历以施法者为中心的一个足够大的区域（-range 到 +range）
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    int x = casterGrid.X + dx;
                    int y = casterGrid.Y + dy;

                    Grid? candidate = this[x, y, z];
                    if (candidate == null) continue;

                    // 向量：从施法者到候选格子
                    double vecX = dx;
                    double vecY = dy;
                    double vecLength = Math.Sqrt(vecX * vecX + vecY * vecY);

                    // 必须在最大范围（半径）内
                    if (vecLength > range + 0.01) continue;

                    // 中心格子（施法者自己）始终包含
                    if (vecLength < 0.01)
                    {
                        if (includeCharacter || candidate.Characters.Count == 0)
                            grids.Add(candidate);
                        continue;
                    }

                    // 单位向量
                    double unitVecX = vecX / vecLength;
                    double unitVecY = vecY / vecLength;

                    // 计算与朝向的夹角
                    double dot = unitDirX * unitVecX + unitDirY * unitVecY;
                    dot = Math.Clamp(dot, -1.0, 1.0);
                    double angleRad = Math.Acos(dot);

                    // 夹角在半角范围内 → 在扇形内
                    if (angleRad <= halfAngleRad)
                    {
                        if (includeCharacter || candidate.Characters.Count == 0)
                        {
                            grids.Add(candidate);
                        }
                    }
                }
            }

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
            Grid startGrid = current;
            if (realGrid != null && current.Id != realGrid.Id)
            {
                startGrid = realGrid;
            }

            if (startGrid.Id == target.Id)
            {
                SetCharacterCurrentGrid(character, startGrid);
                return 0;
            }

            // 记录走到某个格子时的步数
            Queue<(Grid grid, int steps)> queue = new();
            // 记录已访问的格子
            HashSet<long> visited = [];

            // 将起始格子加入队列，步数为0，并标记为已访问
            queue.Enqueue((startGrid, 0));
            visited.Add(startGrid.Id);

            while (queue.Count > 0)
            {
                var (currentGrid, currentSteps) = queue.Dequeue();

                // 如果当前格子就是目标格子，则找到了最短路径
                if (currentGrid.Id == target.Id)
                {
                    realGrid?.Characters.Remove(character);
                    SetCharacterCurrentGrid(character, target);
                    return currentSteps;
                }

                // 定义平面移动的方向
                (int dx, int dy)[] directions = [
                    (0, 1), // 上
                    (0, -1), // 下
                    (1, 0), // 右
                    (-1, 0), // 左
                    (1, 1), // 右上
                    (1, -1), // 右下
                    (-1, 1), // 左上
                    (-1, -1) // 左下
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
        /// 设置角色移动。如果不能达到目标格子，则移动到离目标格子最近的一个可达格子上。
        /// </summary>
        /// <param name="character"></param>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns>移动的步数，只算平面移动步数</returns>
        public virtual int CharacterMoveToClosestReachable(Character character, Grid? current, Grid target)
        {
            if (current is null || current.Id < 0 || target.Id < 0 || !Grids.ContainsValue(target))
            {
                return -1;
            }

            Grid? realGrid = GetCharacterCurrentGrid(character);
            Grid startGrid = current;
            if (realGrid != null && current.Id != realGrid.Id)
            {
                startGrid = realGrid;
            }

            if (startGrid.Id == target.Id)
            {
                SetCharacterCurrentGrid(character, startGrid);
                return 0;
            }

            // 使用 BFS 算法探索所有可达格子，并记录它们到起点的步数
            Queue<(Grid grid, int steps)> queue = new();

            // 记录已访问的格子ID
            HashSet<long> visited = [];

            // 初始化 BFS 队列，将起始格子加入，步数为0
            queue.Enqueue((startGrid, 0));
            visited.Add(startGrid.Id);

            Grid? bestReachableGrid = current;
            int minDistanceToTarget = CalculateManhattanDistance(startGrid, target);
            int stepsToBestReachable = 0;

            // 定义平面移动的方向
            (int dx, int dy)[] directions = [
                (0, 1), (0, -1), (1, 0), (-1, 0),
                (1, 1), (1, -1), (-1, 1), (-1, -1)
            ];

            while (queue.Count > 0)
            {
                var (currentGrid, currentSteps) = queue.Dequeue();

                // 计算当前可达格子到目标格子的曼哈顿距离
                int distToTarget = CalculateManhattanDistance(currentGrid, target);

                // 如果当前格子比之前找到的 bestReachableGrid 更接近目标
                if (distToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distToTarget;
                    bestReachableGrid = currentGrid;
                    stepsToBestReachable = currentSteps;
                }
                // 如果距离相同，优先选择到达步数更少的格子（作为一种 tie-breaking 规则）
                else if (distToTarget == minDistanceToTarget && currentSteps < stepsToBestReachable)
                {
                    bestReachableGrid = currentGrid;
                    stepsToBestReachable = currentSteps;
                }

                // 探索相邻格子
                foreach (var (dx, dy) in directions)
                {
                    int nextX = currentGrid.X + dx;
                    int nextY = currentGrid.Y + dy;
                    int nextZ = currentGrid.Z;

                    Grid? neighborGrid = this[nextX, nextY, nextZ];

                    // 如果相邻格子存在且未被访问过
                    if (neighborGrid != null && !visited.Contains(neighborGrid.Id))
                    {
                        visited.Add(neighborGrid.Id);
                        queue.Enqueue((neighborGrid, currentSteps + 1));
                    }
                }
            }

            // 理论上 bestReachableGrid 不会是 null，因为 current 至少是可达的
            if (bestReachableGrid == null)
            {
                return -1;
            }

            // 更新角色的实际位置
            realGrid?.Characters.Remove(character);
            SetCharacterCurrentGrid(character, bestReachableGrid);

            return stepsToBestReachable;
        }

        /// <summary>
        /// 计算两个格子之间的曼哈顿距离
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns>两个格子之间的曼哈顿距离</returns>
        public static int CalculateManhattanDistance(Grid g1, Grid g2)
        {
            return Math.Abs(g1.X - g2.X) + Math.Abs(g1.Y - g2.Y) + Math.Abs(g1.Z - g2.Z);
        }

        /// <summary>
        /// 计算两个整数的最大公约数（欧几里得算法）
        /// </summary>
        public static int GCD(int a, int b)
        {
            if (a == 0 && b == 0) return 1; // 避免除以零
            if (a == 0) return b;
            if (b == 0) return a;
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        /// <summary>
        /// 获取两点之间直线上的所有整数点（包含起点和终点）
        /// 使用改进的Bresenham算法，确保不遗漏任何点
        /// </summary>
        public static List<(int x, int y)> GetLinePoints(int x0, int y0, int x1, int y1)
        {
            List<(int x, int y)> points = [];

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            // 如果直线是水平的
            if (dy == 0)
            {
                int x = x0;
                while (x != x1 + sx)
                {
                    points.Add((x, y0));
                    x += sx;
                }
                return points;
            }

            // 如果直线是垂直的
            if (dx == 0)
            {
                int y = y0;
                while (y != y1 + sy)
                {
                    points.Add((x0, y));
                    y += sy;
                }
                return points;
            }

            // 对于斜线，使用改进的Bresenham算法
            if (dx >= dy)
            {
                // 斜率小于1
                int err = 2 * dy - dx;
                int y = y0;

                for (int x = x0; x != x1 + sx; x += sx)
                {
                    points.Add((x, y));

                    if (err > 0)
                    {
                        y += sy;
                        err -= 2 * dx;
                    }
                    err += 2 * dy;
                }
            }
            else
            {
                // 斜率大于1
                int err = 2 * dx - dy;
                int x = x0;

                for (int y = y0; y != y1 + sy; y += sy)
                {
                    points.Add((x, y));

                    if (err > 0)
                    {
                        x += sx;
                        err -= 2 * dy;
                    }
                    err += 2 * dx;
                }
            }

            return points;
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
        protected virtual void AfterTimeElapsed(double timeToReduce)
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

            AfterTimeElapsed(timeToReduce);
        }
    }
}
