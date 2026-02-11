/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	Node
作    者:	HappLI
描    述:	路径缓存键
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pathfinding.Runtime
{
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
    //-------------------------------------------
    //! PathCacheItem
    //-------------------------------------------
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
    //-------------------------------------------
    //! 路径缓存系统
    //-------------------------------------------
    public class PathCache
    {
        private Dictionary<PathCacheKey, PathCacheItem> m_cache;
        private int                                     m_maxCacheSize;
        private float                                   m_cacheLifetime;
        //-------------------------------------------
        public PathCache(int maxCacheSize = 1000, float cacheLifetime = 30f)
        {
            m_cache = new Dictionary<PathCacheKey, PathCacheItem>();
            m_maxCacheSize = maxCacheSize;
            m_cacheLifetime = cacheLifetime;
        }
        //-------------------------------------------
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
        //-------------------------------------------
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
        //-------------------------------------------
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
        //-------------------------------------------
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
        //-------------------------------------------
        // 清除所有缓存
        public void ClearAllCache()
        {
            m_cache.Clear();
        }
        //-------------------------------------------
        // 获取缓存大小
        public int CacheSize
        {
            get { return m_cache.Count; }
        }
        //-------------------------------------------
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
        //-------------------------------------------
        // 设置缓存生命周期
        public void SetCacheLifetime(float lifetime)
        {
            m_cacheLifetime = lifetime;
        }
    }
}
