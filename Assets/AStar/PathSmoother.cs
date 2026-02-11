/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	PathSmoother
作    者:	HappLI
描    述:	路径平滑器，优化寻路路径的视觉效果
*********************************************************************/
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
    public class PathSmoother
    {
        private Map         m_map;
        private FFloat      m_cellSize;

        private List<FVector3> m_vPath = null;
        private List<FVector3> m_vPathResult = null;

        public PathSmoother(Map map)
        {
            m_map = map;
            m_cellSize = map.CellSize;
        }
        //-------------------------------------------
        void CheckPaths()
        {
            if (m_vPath == null) m_vPath = new List<FVector3>(8);
            if (m_vPathResult == null) m_vPathResult = new List<FVector3>(8);
            m_vPathResult.Clear();
            m_vPath.Clear();
        }
        //-------------------------------------------
        // 平滑路径，使用贝塞尔曲线
        public List<FVector3> SmoothPath(List<Grid> originalPath, float smoothness = 0.5f)
        {
            if (originalPath == null || originalPath.Count < 2)
            {
                return null;
            }

            // 转换为世界坐标
            CheckPaths();
            foreach (var grid in originalPath)
            {
                FFloat x = grid.X * m_cellSize + m_cellSize * 0.5f;
                FFloat z = grid.Z * m_cellSize + m_cellSize * 0.5f;
                m_vPath.Add(new FVector3(x, grid.Y, z));
            }
            
            // 简化路径
            List<FVector3> simplifiedPath = SimplifyPath(m_vPath, m_vPathResult, 0.1f);
            
            if (simplifiedPath.Count < 2)
            {
                return simplifiedPath;
            }

            // 添加起点
            m_vPath.Clear();
            m_vPath.Add(simplifiedPath[0]);
            
            // 处理中间点
            for (int i = 0; i < simplifiedPath.Count - 1; i++)
            {
                FVector3 start = simplifiedPath[i];
                FVector3 end = simplifiedPath[i + 1];
                
                // 计算控制点
                FVector3 control1, control2;
                if (i > 0)
                {
                    FVector3 prev = simplifiedPath[i - 1];
                    control1 = start + (start - prev) * smoothness;
                }
                else
                {
                    control1 = start;
                }
                
                if (i < simplifiedPath.Count - 2)
                {
                    FVector3 next = simplifiedPath[i + 2];
                    control2 = end - (next - end) * smoothness;
                }
                else
                {
                    control2 = end;
                }
                
                // 生成贝塞尔曲线上的点
                int segments = Mathf.Max(2, Mathf.FloorToInt((start- end).magnitude / m_cellSize));
                for (int j = 1; j < segments; j++)
                {
                    FFloat t = (FFloat)j / segments;
                    FVector3 point = CalculateBezierPoint(t, start, control1, control2, end);
                    
                    // 检查点是否可行走
                    if (IsWalkable(point))
                    {
                        m_vPath.Add(point);
                    }
                    else
                    {
                        // 如果点不可行走，使用原始点
                        m_vPath.Add(start + (end - start) * t);
                    }
                }
            }

            // 添加终点
            m_vPath.Add(simplifiedPath[simplifiedPath.Count - 1]);

            // 墙角处理
            m_vPathResult.Clear();
            m_vPathResult = HandleCorners(m_vPath, m_vPathResult);
            
            return m_vPathResult;
        }
        //-------------------------------------------
        // 计算贝塞尔曲线上的点
        private FVector3 CalculateBezierPoint(float t, FVector3 p0, FVector3 p1, FVector3 p2, FVector3 p3)
        {
            FFloat u = 1 - t;
            FFloat tt = t * t;
            FFloat uu = u * u;
            FFloat uuu = uu * u;
            FFloat ttt = tt * t;

            FVector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;
            
            return p;
        }
        //-------------------------------------------
        // 简化路径，移除冗余点
        private List<FVector3> SimplifyPath(List<FVector3> path, List<FVector3> simplified, float tolerance)
        {
            if (path.Count < 3)
            {
                return path;
            }
            simplified.Clear();
            simplified.Add(path[0]);
            
            for (int i = 1; i < path.Count - 1; i++)
            {
                FVector3 prev = path[i - 1];
                FVector3 current = path[i];
                FVector3 next = path[i + 1];
                
                // 计算点到线段的距离
                FFloat distance = DistanceToLine(current, prev, next);
                if (distance > tolerance)
                {
                    simplified.Add(current);
                }
            }
            
            simplified.Add(path[path.Count - 1]);
            return simplified;
        }
        //-------------------------------------------
        // 计算点到线段的距离
        private FFloat DistanceToLine(FVector3 point, FVector3 lineStart, FVector3 lineEnd)
        {
            FVector3 lineDirection = lineEnd - lineStart;
            float lineLength = lineDirection.magnitude;
            
            if (lineLength < 0.001f)
            {
                return (point- lineStart).magnitude;
            }
            
            lineDirection.Normalize();
            FFloat t = Mathf.Clamp01(FVector3.Dot(point - lineStart, lineDirection));
            FVector3 closestPoint = lineStart + lineDirection * t * lineLength;
            
            return (point - closestPoint).magnitude;
        }
        //-------------------------------------------
        // 墙角处理，避免单位贴墙行走
        private List<FVector3> HandleCorners(List<FVector3> path, List<FVector3> handledPath)
        {
            if (path.Count < 3)
            {
                return path;
            }
            handledPath.Clear();
            handledPath.Add(path[0]);
            
            for (int i = 1; i < path.Count - 1; i++)
            {
                FVector3 prev = path[i - 1];
                FVector3 current = path[i];
                FVector3 next = path[i + 1];
                
                // 检查是否是墙角
                FVector3 direction1 = (current - prev).normalized;
                FVector3 direction2 = (next - current).normalized;

                FFloat dot = Vector3.Dot(direction1, direction2);
                if (dot < -0.5f) // 接近直角
                {
                    // 计算墙角的圆角
                    FVector3 corner = current;
                    FFloat cornerRadius = m_cellSize * 0.3f;
                    
                    // 检查墙角是否可以圆角处理
                    FVector3 roundedPoint = current + (direction1 + direction2).normalized * cornerRadius;
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
        //-------------------------------------------
        // 检查位置是否可行走
        private bool IsWalkable(FVector3 position)
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
        //-------------------------------------------
        // 从平滑路径中获取网格路径（用于AStar算法）
        public List<Grid> GetGridPath(List<FVector3> smoothPath)
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
        //-------------------------------------------
        // 计算路径长度
        public FFloat CalculatePathLength(List<FVector3> path)
        {
            FFloat length = 0.0f;
            for (int i = 1; i < path.Count; i++)
            {
                length += Vector3.Distance(path[i - 1], path[i]);
            }
            return length;
        }
        //-------------------------------------------
        // 检查路径是否有效
        public bool IsPathValid(List<FVector3> path)
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
