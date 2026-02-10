using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
    // 路径平滑器，优化寻路路径的视觉效果
    public class PathSmoother
    {
        private Map m_map;
        private float m_cellSize;
        
        public PathSmoother(Map map)
        {
            m_map = map;
            m_cellSize = map.CellSize;
        }
        
        // 平滑路径，使用贝塞尔曲线
        public List<Vector3> SmoothPath(List<Grid> originalPath, float smoothness = 0.5f)
        {
            if (originalPath == null || originalPath.Count < 2)
            {
                return new List<Vector3>();
            }
            
            // 转换为世界坐标
            List<Vector3> pathPoints = new List<Vector3>();
            foreach (var grid in originalPath)
            {
                float x = grid.X * m_cellSize + m_cellSize * 0.5f;
                float z = grid.Z * m_cellSize + m_cellSize * 0.5f;
                pathPoints.Add(new Vector3(x, grid.Y, z));
            }
            
            // 简化路径
            List<Vector3> simplifiedPath = SimplifyPath(pathPoints, 0.1f);
            
            if (simplifiedPath.Count < 2)
            {
                return simplifiedPath;
            }
            
            // 使用贝塞尔曲线平滑
            List<Vector3> smoothedPath = new List<Vector3>();
            
            // 添加起点
            smoothedPath.Add(simplifiedPath[0]);
            
            // 处理中间点
            for (int i = 0; i < simplifiedPath.Count - 1; i++)
            {
                Vector3 start = simplifiedPath[i];
                Vector3 end = simplifiedPath[i + 1];
                
                // 计算控制点
                Vector3 control1, control2;
                if (i > 0)
                {
                    Vector3 prev = simplifiedPath[i - 1];
                    control1 = start + (start - prev) * smoothness;
                }
                else
                {
                    control1 = start;
                }
                
                if (i < simplifiedPath.Count - 2)
                {
                    Vector3 next = simplifiedPath[i + 2];
                    control2 = end - (next - end) * smoothness;
                }
                else
                {
                    control2 = end;
                }
                
                // 生成贝塞尔曲线上的点
                int segments = Mathf.Max(2, Mathf.FloorToInt(Vector3.Distance(start, end) / m_cellSize));
                for (int j = 1; j < segments; j++)
                {
                    float t = (float)j / segments;
                    Vector3 point = CalculateBezierPoint(t, start, control1, control2, end);
                    
                    // 检查点是否可行走
                    if (IsWalkable(point))
                    {
                        smoothedPath.Add(point);
                    }
                    else
                    {
                        // 如果点不可行走，使用原始点
                        smoothedPath.Add(start + (end - start) * t);
                    }
                }
            }
            
            // 添加终点
            smoothedPath.Add(simplifiedPath[simplifiedPath.Count - 1]);
            
            // 墙角处理
            smoothedPath = HandleCorners(smoothedPath);
            
            return smoothedPath;
        }
        
        // 计算贝塞尔曲线上的点
        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            
            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;
            
            return p;
        }
        
        // 简化路径，移除冗余点
        private List<Vector3> SimplifyPath(List<Vector3> path, float tolerance)
        {
            if (path.Count < 3)
            {
                return path;
            }
            
            List<Vector3> simplified = new List<Vector3>();
            simplified.Add(path[0]);
            
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector3 prev = path[i - 1];
                Vector3 current = path[i];
                Vector3 next = path[i + 1];
                
                // 计算点到线段的距离
                float distance = DistanceToLine(current, prev, next);
                if (distance > tolerance)
                {
                    simplified.Add(current);
                }
            }
            
            simplified.Add(path[path.Count - 1]);
            return simplified;
        }
        
        // 计算点到线段的距离
        private float DistanceToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 lineDirection = lineEnd - lineStart;
            float lineLength = lineDirection.magnitude;
            
            if (lineLength < 0.001f)
            {
                return Vector3.Distance(point, lineStart);
            }
            
            lineDirection.Normalize();
            float t = Mathf.Clamp01(Vector3.Dot(point - lineStart, lineDirection));
            Vector3 closestPoint = lineStart + lineDirection * t * lineLength;
            
            return Vector3.Distance(point, closestPoint);
        }
        
        // 墙角处理，避免单位贴墙行走
        private List<Vector3> HandleCorners(List<Vector3> path)
        {
            if (path.Count < 3)
            {
                return path;
            }
            
            List<Vector3> handledPath = new List<Vector3>();
            handledPath.Add(path[0]);
            
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector3 prev = path[i - 1];
                Vector3 current = path[i];
                Vector3 next = path[i + 1];
                
                // 检查是否是墙角
                Vector3 direction1 = (current - prev).normalized;
                Vector3 direction2 = (next - current).normalized;
                
                float dot = Vector3.Dot(direction1, direction2);
                if (dot < -0.5f) // 接近直角
                {
                    // 计算墙角的圆角
                    Vector3 corner = current;
                    float cornerRadius = m_cellSize * 0.3f;
                    
                    // 检查墙角是否可以圆角处理
                    Vector3 roundedPoint = current + (direction1 + direction2).normalized * cornerRadius;
                    if (IsWalkable(roundedPoint))
                    {
                        handledPath.Add(roundedPoint);
                    }
                    else
                    {
                        handledPath.Add(current);
                    }
                }
                else
                {
                    handledPath.Add(current);
                }
            }
            
            handledPath.Add(path[path.Count - 1]);
            return handledPath;
        }
        
        // 检查位置是否可行走
        private bool IsWalkable(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.x / m_cellSize);
            int z = Mathf.FloorToInt(position.z / m_cellSize);
            
            if (x < 0 || x >= m_map.Width || z < 0 || z >= m_map.Height)
            {
                return false;
            }
            
            Grid grid = m_map.GetGrid(x, z);
            return grid != null && grid.IsWalkable;
        }
        
        // 从平滑路径中获取网格路径（用于AStar算法）
        public List<Grid> GetGridPath(List<Vector3> smoothPath)
        {
            List<Grid> gridPath = new List<Grid>();
            HashSet<Vector2Int> visitedGrids = new HashSet<Vector2Int>();
            
            foreach (var point in smoothPath)
            {
                int x = Mathf.FloorToInt(point.x / m_cellSize);
                int z = Mathf.FloorToInt(point.z / m_cellSize);
                
                Vector2Int gridPos = new Vector2Int(x, z);
                if (!visitedGrids.Contains(gridPos) && x >= 0 && x < m_map.Width && z >= 0 && z < m_map.Height)
                {
                    Grid grid = m_map.GetGrid(x, z);
                    if (grid != null)
                    {
                        gridPath.Add(grid);
                        visitedGrids.Add(gridPos);
                    }
                }
            }
            
            return gridPath;
        }
        
        // 计算路径长度
        public float CalculatePathLength(List<Vector3> path)
        {
            float length = 0;
            for (int i = 1; i < path.Count; i++)
            {
                length += Vector3.Distance(path[i - 1], path[i]);
            }
            return length;
        }
        
        // 检查路径是否有效
        public bool IsPathValid(List<Vector3> path)
        {
            if (path == null || path.Count < 2)
            {
                return false;
            }
            
            for (int i = 0; i < path.Count; i++)
            {
                if (!IsWalkable(path[i]))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
