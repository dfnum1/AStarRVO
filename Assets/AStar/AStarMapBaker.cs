using UnityEngine;
using System.Collections.Generic;

namespace AStarPathfinding
{
    public class AStarMapBaker : MonoBehaviour
    {
        [Header("烘焙设置")]
        [SerializeField] private float cellSize = 1f; // 格子大小
        [SerializeField] private LayerMask obstacleLayer; // 阻挡层
        [SerializeField] private bool includeTerrain = true; // 是否包含地形
        [SerializeField] private float raycastHeight = 100f; // 射线检测高度
        [SerializeField] private float raycastMaxDistance = 200f; // 射线检测最大距离

        [Header("可视化")]
        [SerializeField] private bool showGizmos = true; // 是否显示 gizmos
        [SerializeField] private Color emptyColor = Color.green;
        [SerializeField] private Color obstacleColor = Color.red;

        private Map m_map;
        private Bounds m_sceneBounds;
        private int m_mapWidth;
        private int m_mapHeight;

        // 属性访问器
        public float CellSize { get { return cellSize; } set { cellSize = value; } }
        public LayerMask ObstacleLayer { get { return obstacleLayer; } set { obstacleLayer = value; } }
        public bool IncludeTerrain { get { return includeTerrain; } set { includeTerrain = value; } }
        public float RaycastHeight { get { return raycastHeight; } set { raycastHeight = value; } }
        public float RaycastMaxDistance { get { return raycastMaxDistance; } set { raycastMaxDistance = value; } }
        public bool ShowGizmos { get { return showGizmos; } set { showGizmos = value; } }
        public Color EmptyColor { get { return emptyColor; } set { emptyColor = value; } }
        public Color ObstacleColor { get { return obstacleColor; } set { obstacleColor = value; } }
        public Bounds SceneBounds { get { return m_sceneBounds; } }
        public int MapWidth { get { return m_mapWidth; } }
        public int MapHeight { get { return m_mapHeight; } }

        // 计算场景边界
        public void CalculateSceneBounds()
        {
            // 初始化边界
            m_sceneBounds = new Bounds();
            bool hasBounds = false;

            // 查找所有带Renderer的物体
            Renderer[] renderers = FindObjectsOfType<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                // 跳过地形（如果不包含）
                if (!includeTerrain && renderer.GetComponent<Terrain>() != null)
                    continue;

                if (!hasBounds)
                {
                    m_sceneBounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    m_sceneBounds.Encapsulate(renderer.bounds);
                }
            }

            // 如果没有找到任何物体，使用默认边界
            if (!hasBounds)
            {
                m_sceneBounds = new Bounds(Vector3.zero, new Vector3(10, 0, 10));
            }

            // 扩展边界以确保覆盖所有物体
            m_sceneBounds.Expand(cellSize * 2);

            // 计算地图大小
            m_mapWidth = Mathf.CeilToInt(m_sceneBounds.size.x / cellSize);
            m_mapHeight = Mathf.CeilToInt(m_sceneBounds.size.z / cellSize);

            Debug.Log($"场景边界: {m_sceneBounds}");
            Debug.Log($"地图大小: {m_mapWidth}x{m_mapHeight}");
        }

        // 烘焙地图
        public Map BakeMap()
        {
            // 计算场景边界
            CalculateSceneBounds();

            // 创建地图
            m_map = new Map(m_mapWidth, m_mapHeight, cellSize);

            // 烘焙每个格子
            for (int x = 0; x < m_mapWidth; x++)
            {
                for (int z = 0; z < m_mapHeight; z++)
                {
                    // 计算格子世界坐标
                    Vector3 cellCenter = GetWorldPosition(x, z);
                    
                    // 检测是否有阻挡
                    bool isWalkable = !IsObstacle(cellCenter);
                    
                    // 检测高度
                    float height = GetHeightAtPosition(cellCenter);
                    
                    // 设置格子属性
                    int blockType = isWalkable ? (int)EBlockType.Walkable : (int)EBlockType.Unwalkable;
                    m_map.SetGrid(x, z, height, 1f, blockType);
                }
            }

            Debug.Log("地图烘焙完成!");
            return m_map;
        }

        // 获取格子的世界坐标
        private Vector3 GetWorldPosition(int x, int z)
        {
            float worldX = m_sceneBounds.min.x + x * cellSize + cellSize * 0.5f;
            float worldZ = m_sceneBounds.min.z + z * cellSize + cellSize * 0.5f;
            return new Vector3(worldX, 0, worldZ);
        }

        // 获取网格坐标
        private Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - m_sceneBounds.min.x) / cellSize);
            int z = Mathf.FloorToInt((worldPosition.z - m_sceneBounds.min.z) / cellSize);
            return new Vector2Int(x, z);
        }

        // 检测位置是否有阻挡
        private bool IsObstacle(Vector3 position)
        {
            // 从高处向下发射射线
            Vector3 rayStart = new Vector3(position.x, raycastHeight, position.z);
            Vector3 rayDirection = Vector3.down;

            // 射线检测
            if (Physics.Raycast(rayStart, rayDirection, raycastMaxDistance, obstacleLayer))
            {
                return true;
            }

            // 盒子检测
            Collider[] colliders = Physics.OverlapBox(position, new Vector3(cellSize * 0.4f, 0.5f, cellSize * 0.4f), Quaternion.identity, obstacleLayer);
            if (colliders.Length > 0)
            {
                return true;
            }

            return false;
        }

        // 获取位置的高度
        private float GetHeightAtPosition(Vector3 position)
        {
            Vector3 rayStart = new Vector3(position.x, raycastHeight, position.z);
            Vector3 rayDirection = Vector3.down;

            RaycastHit hit;
            if (Physics.Raycast(rayStart, rayDirection, out hit, raycastMaxDistance))
            {
                return hit.point.y;
            }

            return 0f;
        }

        // 手动添加阻挡
        public void AddObstacle(Vector3 worldPosition)
        {
            if (m_map == null)
            {
                Debug.LogError("地图未烘焙!");
                return;
            }

            Vector2Int gridPos = GetGridPosition(worldPosition);
            if (IsValidGridPosition(gridPos))
            {
                Grid grid = m_map.GetGrid(gridPos.x, gridPos.y);
                m_map.SetGrid(gridPos.x, gridPos.y, grid.Y, 1f, (int)EBlockType.Unwalkable);
                Debug.Log($"添加阻挡: {gridPos}");
            }
        }

        // 手动移除阻挡
        public void RemoveObstacle(Vector3 worldPosition)
        {
            if (m_map == null)
            {
                Debug.LogError("地图未烘焙!");
                return;
            }

            Vector2Int gridPos = GetGridPosition(worldPosition);
            if (IsValidGridPosition(gridPos))
            {
                Grid grid = m_map.GetGrid(gridPos.x, gridPos.y);
                m_map.SetGrid(gridPos.x, gridPos.y, grid.Y, 1f, (int)EBlockType.Walkable);
                Debug.Log($"移除阻挡: {gridPos}");
            }
        }

        // 检查网格坐标是否有效
        private bool IsValidGridPosition(Vector2Int gridPos)
        {
            return gridPos.x >= 0 && gridPos.x < m_mapWidth && gridPos.y >= 0 && gridPos.y < m_mapHeight;
        }

        // 获取烘焙的地图
        public Map GetBakedMap()
        {
            return m_map;
        }

        //-------------------------------------------

        // 保存地图为二进制文件
        public bool SaveMapToBinary(string filePath)
        {
            if (m_map == null)
            {
                Debug.LogError("地图未烘焙!");
                return false;
            }

            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(fs))
                {
                    // 写入地图基本信息
                    writer.Write(m_map.Width);
                    writer.Write(m_map.Height);
                    writer.Write(m_map.CellSize);

                    // 写入场景边界信息
                    writer.Write(m_sceneBounds.min.x);
                    writer.Write(m_sceneBounds.min.y);
                    writer.Write(m_sceneBounds.min.z);
                    writer.Write(m_sceneBounds.max.x);
                    writer.Write(m_sceneBounds.max.y);
                    writer.Write(m_sceneBounds.max.z);

                    // 写入每个格子的数据
                    for (int x = 0; x < m_map.Width; x++)
                    {
                        for (int z = 0; z < m_map.Height; z++)
                        {
                            Grid grid = m_map.GetGrid(x, z);
                            if (grid != null)
                            {
                                writer.Write(grid.X);
                                writer.Write(grid.Z);
                                writer.Write(grid.Y);
                                writer.Write(grid.Cost);
                                writer.Write(grid.BlockType);
                            }
                        }
                    }
                }

                Debug.Log($"地图保存成功: {filePath}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"保存地图失败: {e.Message}");
                return false;
            }
        }

        // 绘制 gizmos
        private void OnDrawGizmos()
        {
            if (!showGizmos || m_map == null)
                return;

            // 绘制网格
            for (int x = 0; x < m_mapWidth; x++)
            {
                for (int z = 0; z < m_mapHeight; z++)
                {
                    Grid grid = m_map.GetGrid(x, z);
                    if (grid != null)
                    {
                        Vector3 worldPos = GetWorldPosition(x, z);
                        worldPos.y = grid.Y + 0.01f;

                        // 设置颜色
                        Gizmos.color = grid.IsWalkable ? emptyColor : obstacleColor;

                        // 绘制格子
                        Gizmos.DrawCube(worldPos, new Vector3(cellSize * 0.9f, 0.02f, cellSize * 0.9f));
                    }
                }
            }

            // 绘制场景边界
            if (m_sceneBounds.size != Vector3.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(m_sceneBounds.center, m_sceneBounds.size);
            }
        }
    }
}
