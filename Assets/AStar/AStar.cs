/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	AStar
作    者:	HappLI
描    述:	AStar寻路类，实现AStar算法的核心逻辑
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Framework.Pathfinding.Runtime
{
    // 
    public class AStar
    {
        private AStarPathfinding m_System;
        private Map m_map;
        private PriorityQueue m_openSet;
        private HashSet<Node> m_closedSet;
        private Node[,] m_nodes;
        private int m_unitWidth;
        private int m_unitHeight;
        private int m_ignoredBlockTypes; // 使用位掩码存储忽略的阻挡类型
        private List<Grid> m_path;
        private Node m_endNode;
        private bool m_useMultiThreading;
        private PathCache m_pathCache;

        public Map Map { get { return m_map; } }
        public bool UseMultiThreading { get { return m_useMultiThreading; } }
        public AStar(AStarPathfinding system, Map map)
        {
            m_System = system;
            m_map = map;
            m_openSet = new PriorityQueue();
            m_closedSet = new HashSet<Node>();
            m_nodes = new Node[map.Width, map.Height];
            m_unitWidth = 1;
            m_unitHeight = 1;
            m_ignoredBlockTypes = 0;
            m_path = new List<Grid>();
            m_endNode = new Node();
            m_useMultiThreading = false;
            m_pathCache = new PathCache();

            // 预分配所有节点
            for (int x = 0; x < map.Width; x++)
            {
                for (int z = 0; z < map.Height; z++)
                {
                    m_nodes[x, z] = new Node();
                }
            }
        }
        //-------------------------------------------
        // 设置是否使用多线程
        public void SetUseMultiThreading(bool useMultiThreading)
        {
            m_useMultiThreading = useMultiThreading;
        }
        //-------------------------------------------
        // 设置单位体积
        public void SetUnitSize(int width, int height)
        {
            m_unitWidth = width;
            m_unitHeight = height;
        }
        //-------------------------------------------
        // 添加忽略的阻挡类型
        public void AddIgnoredBlockType(int blockType)
        {
            m_ignoredBlockTypes |= (1 << blockType);
        }
        //-------------------------------------------
        // 移除忽略的阻挡类型
        public void RemoveIgnoredBlockType(int blockType)
        {
            m_ignoredBlockTypes &= ~(1 << blockType);
        }
        //-------------------------------------------
        public void ClearIgnoredBlockTypes()
        {
            m_ignoredBlockTypes = 0;
        }
        //-------------------------------------------
        // 检查阻挡类型是否被忽略
        public bool IsBlockTypeIgnored(int blockType)
        {
            return (m_ignoredBlockTypes & (1 << blockType)) != 0;
        }
        //-------------------------------------------
        // 寻路方法
        public List<Grid> FindPath(int startX, int startZ, int endX, int endZ)
        {
            // 检查路径缓存
            PathCacheKey cacheKey = new PathCacheKey(startX, startZ, endX, endZ, m_unitWidth, m_unitHeight);
            List<Grid> cachedPath = m_pathCache.GetPath(cacheKey);
            if (cachedPath != null)
            {
                return cachedPath;
            }
            
            List<Grid> path;
            
            if (m_useMultiThreading)
            {
                // 使用线程池执行多线程寻路
                Task<List<Grid>> task = m_System.threadPoolManager.ExecuteTask(() =>
                {
                    return FindPathInternal(startX, startZ, endX, endZ);
                });
                
                // 等待任务完成
                path = task.Result;
            }
            else
            {
                // 单线程寻路
                path = FindPathInternal(startX, startZ, endX, endZ);
            }
            
            // 保存路径到缓存
            if (path != null && path.Count > 0)
            {
                // 计算路径代价
                float pathCost = CalculatePathCost(path);
                m_pathCache.AddPath(cacheKey, path, pathCost);
            }
            
            return path;
        }
        //-------------------------------------------
        // 计算路径代价
        private float CalculatePathCost(List<Grid> path)
        {
            float cost = 0;
            for (int i = 1; i < path.Count; i++)
            {
                Grid prev = path[i - 1];
                Grid current = path[i];
                float distance = GetDistance(prev.X, prev.Z, current.X, current.Z);
                cost += distance * current.Cost;
            }
            return cost;
        }
        //-------------------------------------------
        // 内部寻路方法
        private List<Grid> FindPathInternal(int startX, int startZ, int endX, int endZ)
        {
            // 重置节点
            for (int x = 0; x < m_map.Width; x++)
            {
                for (int z = 0; z < m_map.Height; z++)
                {
                    Node node = m_nodes[x, z];
                    node.Reset(x, z);
                }
            }

            m_openSet.Clear();
            m_closedSet.Clear();
            m_path.Clear();

            // 重置终点节点
            m_endNode.Reset(endX, endZ);

            // 重置起点节点
            Node startNode = m_nodes[startX, startZ];
            startNode.Reset(startX, startZ);

            m_openSet.Enqueue(startNode);

            while (m_openSet.Count > 0)
            {
                Node currentNode = m_openSet.Dequeue();
                m_closedSet.Add(currentNode);

                // 如果到达终点，回溯路径
                if (currentNode.X == endX && currentNode.Z == endZ)
                {
                    return RetracePath(startNode, currentNode);
                }

                // 遍历相邻节点
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    for (int zOffset = -1; zOffset <= 1; zOffset++)
                    {
                        // 跳过自己
                        if (xOffset == 0 && zOffset == 0)
                            continue;

                        // 计算相邻节点坐标
                        int neighborX = currentNode.X + xOffset;
                        int neighborZ = currentNode.Z + zOffset;

                        // 检查是否在地图范围内
                        if (neighborX < 0 || neighborX >= m_map.Width || neighborZ < 0 || neighborZ >= m_map.Height)
                            continue;

                        // 检查单位体积是否可以通过
                        if (!IsUnitCanFit(neighborX, neighborZ))
                            continue;

                        // 检查是否可行走
                        Grid neighborGrid = m_map.GetGrid(neighborX, neighborZ);
                        if (!neighborGrid.IsWalkable && !IsBlockTypeIgnored(neighborGrid.BlockType))
                            continue;

                        // 计算移动代价
                        float moveCost = currentNode.GCost + GetDistance(currentNode, neighborX, neighborZ) * neighborGrid.Cost;

                        // 检查是否已经在开放集或关闭集中
                        Node neighborNode = m_nodes[neighborX, neighborZ];
                        if (!m_closedSet.Contains(neighborNode))
                        {
                            if (neighborNode.Parent == null || moveCost < neighborNode.GCost)
                            {
                                neighborNode.Reset(neighborX, neighborZ, currentNode, moveCost, GetDistance(neighborX, neighborZ, endX, endZ));
                                if (!m_openSet.Contains(neighborNode))
                                {
                                    m_openSet.Enqueue(neighborNode);
                                }
                                else
                                {
                                    m_openSet.UpdateNode(neighborNode);
                                }
                            }
                        }
                    }
                }
            }
            // 没有找到路径
            return null;
        }
        //-------------------------------------------
        // 检查单位体积是否可以通过
        private bool IsUnitCanFit(int x, int z)
        {
            for (int i = 0; i < m_unitWidth; i++)
            {
                for (int j = 0; j < m_unitHeight; j++)
                {
                    int checkX = x + i;
                    int checkZ = z + j;

                    if (checkX < 0 || checkX >= m_map.Width || checkZ < 0 || checkZ >= m_map.Height)
                        return false;

                    Grid grid = m_map.GetGrid(checkX, checkZ);
                if (!grid.IsWalkable && !IsBlockTypeIgnored(grid.BlockType))
                    return false;
                }
            }

            return true;
        }
        //-------------------------------------------
        // 计算两个节点之间的距离
        private float GetDistance(Node a, int bX, int bZ)
        {
            int dx = Math.Abs(a.X - bX);
            int dz = Math.Abs(a.Z - bZ);

            if (dx > dz)
                return 14 * dz + 10 * (dx - dz);
            return 14 * dx + 10 * (dz - dx);
        }
        //-------------------------------------------
        // 计算两个坐标之间的距离
        private float GetDistance(int aX, int aZ, int bX, int bZ)
        {
            int dx = Math.Abs(aX - bX);
            int dz = Math.Abs(aZ - bZ);

            if (dx > dz)
                return 14 * dz + 10 * (dx - dz);
            return 14 * dx + 10 * (dz - dx);
        }
        //-------------------------------------------
        // 回溯路径
        private List<Grid> RetracePath(Node startNode, Node endNode)
        {
            m_path.Clear();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                m_path.Add(m_map.GetGrid(currentNode.X, currentNode.Z));
                currentNode = currentNode.Parent;
            }

            m_path.Add(m_map.GetGrid(startNode.X, startNode.Z));
            m_path.Reverse();
            return m_path;
        }
    }
}
