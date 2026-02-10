using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
    // 单位基类
    public class Unit
    {
        public Vector3 Position { get; set; }
        public Vector3 TargetPosition { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int UnitId { get; set; }
        public float Radius { get; set; }
        public bool IsMoving { get; set; }
        
        public Unit(int unitId, Vector3 position, int width = 1, int height = 1)
        {
            UnitId = unitId;
            Position = position;
            Width = width;
            Height = height;
            Radius = Mathf.Max(width, height) * 0.5f;
            IsMoving = false;
        }
    }
    
    // 单位管理器，处理单位碰撞避免和动态阻挡
    public class UnitManager
    {
        private Dictionary<int, Unit> m_units;
        private HashSet<Vector2Int> m_occupiedGrids;
        private Map m_map;
        private float m_cellSize;
        
        public UnitManager(Map map)
        {
            m_units = new Dictionary<int, Unit>();
            m_occupiedGrids = new HashSet<Vector2Int>();
            m_map = map;
            m_cellSize = map.CellSize;
        }
        
        // 添加单位
        public void AddUnit(Unit unit)
        {
            if (!m_units.ContainsKey(unit.UnitId))
            {
                m_units[unit.UnitId] = unit;
                UpdateOccupiedGrids(unit);
            }
        }
        
        // 移除单位
        public void RemoveUnit(int unitId)
        {
            if (m_units.ContainsKey(unitId))
            {
                Unit unit = m_units[unitId];
                ClearOccupiedGrids(unit);
                m_units.Remove(unitId);
            }
        }
        
        // 更新单位位置
        public void UpdateUnitPosition(int unitId, Vector3 newPosition)
        {
            if (m_units.ContainsKey(unitId))
            {
                Unit unit = m_units[unitId];
                ClearOccupiedGrids(unit);
                unit.Position = newPosition;
                UpdateOccupiedGrids(unit);
            }
        }
        
        // 检查位置是否被其他单位占据
        public bool IsPositionOccupied(Vector3 position, int unitWidth = 1, int unitHeight = 1, int excludeUnitId = -1)
        {
            Vector2Int gridPos = WorldToGrid(position);
            
            // 检查单位占据的所有格子
            for (int x = 0; x < unitWidth; x++)
            {
                for (int z = 0; z < unitHeight; z++)
                {
                    Vector2Int checkPos = new Vector2Int(gridPos.x + x, gridPos.y + z);
                    if (m_occupiedGrids.Contains(checkPos))
                    {
                        // 检查是否是排除的单位
                        if (!IsPositionOccupiedByUnit(checkPos, excludeUnitId))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        // 检查格子是否被指定单位以外的单位占据
        private bool IsPositionOccupiedByUnit(Vector2Int gridPos, int excludeUnitId)
        {
            foreach (var unit in m_units.Values)
            {
                if (unit.UnitId == excludeUnitId)
                    continue;
                
                Vector2Int unitGridPos = WorldToGrid(unit.Position);
                for (int x = 0; x < unit.Width; x++)
                {
                    for (int z = 0; z < unit.Height; z++)
                    {
                        Vector2Int unitCheckPos = new Vector2Int(unitGridPos.x + x, unitGridPos.y + z);
                        if (unitCheckPos == gridPos)
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        // 更新单位占据的格子
        private void UpdateOccupiedGrids(Unit unit)
        {
            Vector2Int gridPos = WorldToGrid(unit.Position);
            
            for (int x = 0; x < unit.Width; x++)
            {
                for (int z = 0; z < unit.Height; z++)
                {
                    Vector2Int checkPos = new Vector2Int(gridPos.x + x, gridPos.y + z);
                    if (IsValidGrid(checkPos))
                    {
                        m_occupiedGrids.Add(checkPos);
                    }
                }
            }
        }
        
        // 清除单位占据的格子
        private void ClearOccupiedGrids(Unit unit)
        {
            Vector2Int gridPos = WorldToGrid(unit.Position);
            
            for (int x = 0; x < unit.Width; x++)
            {
                for (int z = 0; z < unit.Height; z++)
                {
                    Vector2Int checkPos = new Vector2Int(gridPos.x + x, gridPos.y + z);
                    if (IsValidGrid(checkPos))
                    {
                        // 检查是否还有其他单位占据此格子
                        bool stillOccupied = false;
                        foreach (var otherUnit in m_units.Values)
                        {
                            if (otherUnit.UnitId != unit.UnitId)
                            {
                                Vector2Int otherGridPos = WorldToGrid(otherUnit.Position);
                                for (int ox = 0; ox < otherUnit.Width; ox++)
                                {
                                    for (int oz = 0; oz < otherUnit.Height; oz++)
                                    {
                                        Vector2Int otherCheckPos = new Vector2Int(otherGridPos.x + ox, otherGridPos.y + oz);
                                        if (otherCheckPos == checkPos)
                                        {
                                            stillOccupied = true;
                                            break;
                                        }
                                    }
                                    if (stillOccupied)
                                        break;
                                }
                            }
                            if (stillOccupied)
                                break;
                        }
                        
                        if (!stillOccupied)
                        {
                            m_occupiedGrids.Remove(checkPos);
                        }
                    }
                }
            }
        }
        
        // 世界坐标转网格坐标
        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / m_cellSize);
            int z = Mathf.FloorToInt(worldPos.z / m_cellSize);
            return new Vector2Int(x, z);
        }
        
        // 网格坐标转世界坐标
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            float x = gridPos.x * m_cellSize + m_cellSize * 0.5f;
            float z = gridPos.y * m_cellSize + m_cellSize * 0.5f;
            return new Vector3(x, 0, z);
        }
        
        // 检查网格坐标是否有效
        private bool IsValidGrid(Vector2Int gridPos)
        {
            return gridPos.x >= 0 && gridPos.x < m_map.Width && gridPos.y >= 0 && gridPos.y < m_map.Height;
        }
        
        // 获取附近的单位
        public List<Unit> GetNearbyUnits(Vector3 position, float radius)
        {
            List<Unit> nearbyUnits = new List<Unit>();
            
            foreach (var unit in m_units.Values)
            {
                if (Vector3.Distance(position, unit.Position) <= radius)
                {
                    nearbyUnits.Add(unit);
                }
            }
            
            return nearbyUnits;
        }
        
        // 避免碰撞，调整单位位置
        public Vector3 AvoidCollisions(Unit unit, Vector3 desiredPosition)
        {
            Vector3 adjustedPosition = desiredPosition;
            float avoidanceRadius = unit.Radius * 2;
            
            foreach (var otherUnit in m_units.Values)
            {
                if (otherUnit.UnitId == unit.UnitId)
                    continue;
                
                float distance = Vector3.Distance(adjustedPosition, otherUnit.Position);
                if (distance < avoidanceRadius)
                {
                    Vector3 direction = (adjustedPosition - otherUnit.Position).normalized;
                    float pushDistance = avoidanceRadius - distance;
                    adjustedPosition += direction * pushDistance * 0.5f;
                }
            }
            
            // 确保调整后的位置在地图范围内
            Vector2Int gridPos = WorldToGrid(adjustedPosition);
            if (!IsValidGrid(gridPos))
            {
                adjustedPosition = unit.Position;
            }
            
            return adjustedPosition;
        }
        
        // 检查路径是否被其他单位阻挡
        public bool IsPathBlocked(List<Grid> path, int unitId)
        {
            foreach (var grid in path)
            {
                Vector3 gridPosition = GridToWorld(new Vector2Int(grid.X, grid.Z));
                if (IsPositionOccupied(gridPosition, 1, 1, unitId))
                {
                    return true;
                }
            }
            return false;
        }
        
        // 获取所有单位
        public Dictionary<int, Unit> GetAllUnits()
        {
            return m_units;
        }
        
        // 清除所有单位
        public void ClearAllUnits()
        {
            m_units.Clear();
            m_occupiedGrids.Clear();
        }
    }
}
