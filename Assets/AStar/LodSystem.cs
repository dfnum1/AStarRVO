/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	LODSystem
作    者:	HappLI
描    述:	LOD系统，根据距离使用不同精度的网格
*********************************************************************/
using ExternEngine;
using System.Collections.Generic;
using UnityEngine;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector2 = UnityEngine.Vector2;
using FVector3 = UnityEngine.Vector3;
#endif
namespace Framework.Pathfinding.Runtime
{
    //-------------------------------------------
    //! LOD级别
    //-------------------------------------------
    public enum LODLevel
    {
        High = 0,    // 完整精度
        Medium = 1,  // 2x2网格合并
        Low = 2,     // 4x4网格合并
        VeryLow = 3  // 8x8网格合并
    }
    //-------------------------------------------
    //!  LOD系统，根据距离使用不同精度的网格
    //-------------------------------------------
    public class LODSystem
    {
        private Map                         m_originalMap;
        private Dictionary<LODLevel, Map>   m_lodMaps;
        private FFloat                      m_cellSize;
        //-------------------------------------------
        public LODSystem(Map originalMap)
        {
            m_originalMap = originalMap;
            m_lodMaps = new Dictionary<LODLevel, Map>();
            m_cellSize = originalMap.CellSize;
            
            // 预计算不同LOD级别的地图
            PrecomputeLODMaps();
        }
        //-------------------------------------------
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
        //-------------------------------------------
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
        //-------------------------------------------
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
        //-------------------------------------------
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
        //-------------------------------------------
        // 根据距离获取地图
        public Map GetMapByDistance(float distance)
        {
            LODLevel lodLevel = GetLODLevel(distance);
            return GetMap(lodLevel);
        }
        //-------------------------------------------
        // 将LOD坐标转换为原始坐标
        public Vector2Int LODToOriginal(LODLevel lodLevel, Vector2Int lodCoord)
        {
            int scale = GetLODScale(lodLevel);
            return new Vector2Int(lodCoord.x * scale, lodCoord.y * scale);
        }
        //-------------------------------------------
        // 将原始坐标转换为LOD坐标
        public Vector2Int OriginalToLOD(LODLevel lodLevel, Vector2Int originalCoord)
        {
            int scale = GetLODScale(lodLevel);
            return new Vector2Int(originalCoord.x / scale, originalCoord.y / scale);
        }
        //-------------------------------------------
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
        //-------------------------------------------
        // 清除LOD地图
        public void ClearLODMaps()
        {
            m_lodMaps.Clear();
        }
    }
}
