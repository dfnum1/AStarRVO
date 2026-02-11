/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	AStarPathfinding
作    者:	HappLI
描    述:	AStar寻路系统核心类，提供地图创建、路径搜索、路径平滑、路径缓存和LOD系统接口，支持从二进制文件加载地图数据。
*********************************************************************/
using System.IO;
namespace Framework.Pathfinding.Runtime
{
    public class AStarPathfinding 
    {
        ThreadPoolManager m_ThreadPool = null;
        //-------------------------------------------
        internal ThreadPoolManager threadPoolManager
        {
            get
            {
                if (m_ThreadPool == null)
                {
                    m_ThreadPool = new ThreadPoolManager();
                }
                return m_ThreadPool;
            }
        }
        //-------------------------------------------
        // 创建地图
        public Map CreateMap(int width, int height, float cellSize = 1f)
        {
            return new Map(width, height, cellSize);
        }
        //-------------------------------------------

        // 创建单位管理器
        public UnitManager CreateUnitManager(Map map)
        {
            return new UnitManager(map);
        }
        //-------------------------------------------

        // 创建RVO算法实例
        public RVOAlgorithm CreateRVOAlgorithm(int timeStep = 16, int neighborDist = 500, int maxNeighbors = 10)
        {
            return new RVOAlgorithm(timeStep, neighborDist, maxNeighbors);
        }
        //-------------------------------------------
        public AStar CreateAStar(Map map)
        {
            return new AStar(this, map);
        }
        //-------------------------------------------
        public PathSmoother CreatePathSmoother(Map map)
        {
            return new PathSmoother(map);
        }
        //-------------------------------------------
        public PathCache CreatePathCache(int maxCacheSize = 1000, float cacheLifetime = 30f)
        {
            return new PathCache(maxCacheSize, cacheLifetime);
        }

        //-------------------------------------------
        // 创建LOD系统
        public LODSystem CreateLODSystem(Map map)
        {
            return new LODSystem(map);
        }
        //-------------------------------------------
        // 从二进制文件加载地图
        public Map LoadMapFromBinary(string filePath)
        {
            if (!File.Exists(filePath))
            {
                UnityEngine.Debug.LogError($"地图文件不存在: {filePath}");
                return null;
            }

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    // 读取地图基本信息
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    float cellSize = reader.ReadSingle();

                    // 创建地图
                    Map map = new Map(width, height, cellSize);

                    // 读取场景边界信息（可选，用于可视化）
                    float minX = reader.ReadSingle();
                    float minY = reader.ReadSingle();
                    float minZ = reader.ReadSingle();
                    float maxX = reader.ReadSingle();
                    float maxY = reader.ReadSingle();
                    float maxZ = reader.ReadSingle();

                    // 读取每个格子的数据
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            int gridX = reader.ReadInt32();
                            int gridZ = reader.ReadInt32();
                            float gridY = reader.ReadSingle();
                            float gridCost = reader.ReadSingle();
                            int gridBlockType = reader.ReadInt32();

                            // 设置格子属性
                            map.SetGrid(gridX, gridZ, gridY, gridCost, gridBlockType);
                        }
                    }

                    UnityEngine.Debug.Log($"地图加载成功: {filePath}");
                    return map;
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"加载地图失败: {e.Message}");
                return null;
            }
        }
    }
}
