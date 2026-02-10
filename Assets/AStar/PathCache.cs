using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
    // 路径缓存键
    public struct PathCacheKey
    {
        public int StartX;
        public int StartZ;
        public int EndX;
        public int EndZ;
        public int UnitWidth;
        public int UnitHeight;
        
        public PathCacheKey(int startX, int startZ, int endX, int endZ, int unitWidth = 1, int unitHeight = 1)
        {
            StartX = startX;
            StartZ = startZ;
            EndX = endX;
            EndZ = endZ;
            UnitWidth = unitWidth;
            UnitHeight = unitHeight;
        }
        
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + StartX;
            hash = hash * 31 + StartZ;
            hash = hash * 31 + EndX;
            hash = hash * 31 + EndZ;
            hash = hash * 31 + UnitWidth;
            hash = hash * 31 + UnitHeight;
            return hash;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is PathCacheKey)
            {
                PathCacheKey other = (PathCacheKey)obj;
                return StartX == other.StartX && StartZ == other.StartZ &&
                       EndX == other.EndX && EndZ == other.EndZ &&
                       UnitWidth == other.UnitWidth && UnitHeight == other.UnitHeight;
            }
            return false;
        }
    }
    
    // 路径缓存项
    public class PathCacheItem
    {
        public List<Grid> Path;
        public float Timestamp;
        public float Cost;
        
        public PathCacheItem(List<Grid> path, float cost)
        {
            Path = path;
            Timestamp = Time.time;
            Cost = cost;
        }
    }
    
    // 路径缓存系统
    public class PathCache
    {
        private Dictionary<PathCacheKey, PathCacheItem> m_cache;
        private int m_maxCacheSize;
        private float m_cacheLifetime;
        
        public PathCache(int maxCacheSize = 1000, float cacheLifetime = 30f)
        {
            m_cache = new Dictionary<PathCacheKey, PathCacheItem>();
            m_maxCacheSize = maxCacheSize;
            m_cacheLifetime = cacheLifetime;
        }
        
        // 添加路径到缓存
        public void AddPath(PathCacheKey key, List<Grid> path, float cost)
        {
            // 检查缓存大小
            if (m_cache.Count >= m_maxCacheSize)
            {
                RemoveOldestPath();
            }
            
            // 添加或更新缓存项
            m_cache[key] = new PathCacheItem(path, cost);
        }
        
        // 从缓存获取路径
        public List<Grid> GetPath(PathCacheKey key)
        {
            if (m_cache.TryGetValue(key, out PathCacheItem item))
            {
                // 检查缓存是否过期
                if (Time.time - item.Timestamp < m_cacheLifetime)
                {
                    // 更新时间戳
                    item.Timestamp = Time.time;
                    return new List<Grid>(item.Path);
                }
                else
                {
                    // 缓存过期，移除
                    m_cache.Remove(key);
                }
            }
            return null;
        }
        
        // 移除最旧的路径
        private void RemoveOldestPath()
        {
            PathCacheKey oldestKey = default(PathCacheKey);
            float oldestTime = float.MaxValue;
            
            foreach (var pair in m_cache)
            {
                if (pair.Value.Timestamp < oldestTime)
                {
                    oldestTime = pair.Value.Timestamp;
                    oldestKey = pair.Key;
                }
            }
            
            if (oldestTime != float.MaxValue)
            {
                m_cache.Remove(oldestKey);
            }
        }
        
        // 清除过期的缓存
        public void ClearExpiredCache()
        {
            List<PathCacheKey> keysToRemove = new List<PathCacheKey>();
            
            foreach (var pair in m_cache)
            {
                if (Time.time - pair.Value.Timestamp >= m_cacheLifetime)
                {
                    keysToRemove.Add(pair.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                m_cache.Remove(key);
            }
        }
        
        // 清除所有缓存
        public void ClearAllCache()
        {
            m_cache.Clear();
        }
        
        // 获取缓存大小
        public int CacheSize
        {
            get { return m_cache.Count; }
        }
        
        // 设置最大缓存大小
        public void SetMaxCacheSize(int maxSize)
        {
            m_maxCacheSize = maxSize;
            // 如果当前缓存大小超过最大值，移除多余的缓存
            while (m_cache.Count > m_maxCacheSize)
            {
                RemoveOldestPath();
            }
        }
        
        // 设置缓存生命周期
        public void SetCacheLifetime(float lifetime)
        {
            m_cacheLifetime = lifetime;
        }
    }
    
    // LOD级别
    public enum LODLevel
    {
        High = 0,    // 完整精度
        Medium = 1,  // 2x2网格合并
        Low = 2,     // 4x4网格合并
        VeryLow = 3  // 8x8网格合并
    }
    
    // LOD系统，根据距离使用不同精度的网格
    public class LODSystem
    {
        private Map m_originalMap;
        private Dictionary<LODLevel, Map> m_lodMaps;
        private float m_cellSize;
        
        public LODSystem(Map originalMap)
        {
            m_originalMap = originalMap;
            m_lodMaps = new Dictionary<LODLevel, Map>();
            m_cellSize = originalMap.CellSize;
            
            // 预计算不同LOD级别的地图
            PrecomputeLODMaps();
        }
        
        // 预计算LOD地图
        private void PrecomputeLODMaps()
        {
            // 计算Medium LOD (2x2)
            Map mediumLOD = CreateLODMap(LODLevel.Medium, 2);
            if (mediumLOD != null)
            {
                m_lodMaps[LODLevel.Medium] = mediumLOD;
            }
            
            // 计算Low LOD (4x4)
            Map lowLOD = CreateLODMap(LODLevel.Low, 4);
            if (lowLOD != null)
            {
                m_lodMaps[LODLevel.Low] = lowLOD;
            }
            
            // 计算VeryLow LOD (8x8)
            Map veryLowLOD = CreateLODMap(LODLevel.VeryLow, 8);
            if (veryLowLOD != null)
            {
                m_lodMaps[LODLevel.VeryLow] = veryLowLOD;
            }
        }
        
        // 创建LOD地图
        private Map CreateLODMap(LODLevel lodLevel, int scale)
        {
            int originalWidth = m_originalMap.Width;
            int originalHeight = m_originalMap.Height;
            
            int lodWidth = Mathf.CeilToInt((float)originalWidth / scale);
            int lodHeight = Mathf.CeilToInt((float)originalHeight / scale);
            float lodCellSize = m_cellSize * scale;
            
            Map lodMap = new Map(lodWidth, lodHeight, lodCellSize);
            
            // 合并网格
            for (int x = 0; x < lodWidth; x++)
            {
                for (int z = 0; z < lodHeight; z++)
                {
                    // 计算原始网格范围
                    int startX = x * scale;
                    int startZ = z * scale;
                    int endX = Mathf.Min(startX + scale, originalWidth);
                    int endZ = Mathf.Min(startZ + scale, originalHeight);
                    
                    // 统计网格信息
                    float totalY = 0;
                    float totalCost = 0;
                    int walkableCount = 0;
                    int totalGrids = 0;
                    
                    for (int ox = startX; ox < endX; ox++)
                    {
                        for (int oz = startZ; oz < endZ; oz++)
                        {
                            Grid originalGrid = m_originalMap.GetGrid(ox, oz);
                            if (originalGrid != null)
                            {
                                totalY += originalGrid.Y;
                                totalCost += originalGrid.Cost;
                                if (originalGrid.IsWalkable)
                                {
                                    walkableCount++;
                                }
                                totalGrids++;
                            }
                        }
                    }
                    
                    // 计算平均值
                    float avgY = totalGrids > 0 ? totalY / totalGrids : 0;
                    float avgCost = totalGrids > 0 ? totalCost / totalGrids : 1;
                    bool isWalkable = totalGrids > 0 && walkableCount == totalGrids;
                    int blockType = isWalkable ? (int)EBlockType.Walkable : (int)EBlockType.Unwalkable;
                    
                    // 设置LOD网格
                    lodMap.SetGrid(x, z, avgY, avgCost, blockType);
                }
            }
            
            return lodMap;
        }
        
        // 根据距离获取合适的LOD级别
        public LODLevel GetLODLevel(float distance)
        {
            if (distance < 50f)
            {
                return LODLevel.High;
            }
            else if (distance < 150f)
            {
                return LODLevel.Medium;
            }
            else if (distance < 300f)
            {
                return LODLevel.Low;
            }
            else
            {
                return LODLevel.VeryLow;
            }
        }
        
        // 根据LOD级别获取地图
        public Map GetMap(LODLevel lodLevel)
        {
            if (lodLevel == LODLevel.High)
            {
                return m_originalMap;
            }
            
            if (m_lodMaps.TryGetValue(lodLevel, out Map lodMap))
            {
                return lodMap;
            }
            
            return m_originalMap;
        }
        
        // 根据距离获取地图
        public Map GetMapByDistance(float distance)
        {
            LODLevel lodLevel = GetLODLevel(distance);
            return GetMap(lodLevel);
        }
        
        // 将LOD坐标转换为原始坐标
        public Vector2Int LODToOriginal(LODLevel lodLevel, Vector2Int lodCoord)
        {
            int scale = GetLODScale(lodLevel);
            return new Vector2Int(lodCoord.x * scale, lodCoord.y * scale);
        }
        
        // 将原始坐标转换为LOD坐标
        public Vector2Int OriginalToLOD(LODLevel lodLevel, Vector2Int originalCoord)
        {
            int scale = GetLODScale(lodLevel);
            return new Vector2Int(originalCoord.x / scale, originalCoord.y / scale);
        }
        
        // 获取LOD缩放因子
        private int GetLODScale(LODLevel lodLevel)
        {
            switch (lodLevel)
            {
                case LODLevel.High:
                    return 1;
                case LODLevel.Medium:
                    return 2;
                case LODLevel.Low:
                    return 4;
                case LODLevel.VeryLow:
                    return 8;
                default:
                    return 1;
            }
        }
        
        // 清除LOD地图
        public void ClearLODMaps()
        {
            m_lodMaps.Clear();
        }
    }
}
