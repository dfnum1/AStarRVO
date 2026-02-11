using Framework.Pathfinding.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pathfinding.Runtime
{
    public class AStarMapBakerTest : MonoBehaviour
    {
        private AStarPathfinding m_System = new AStarPathfinding();
        public AStarMapBaker mapBaker = new AStarMapBaker();
        public bool autoBakeOnStart = false;

        private Map m_bakedMap;
        private AStar m_astar;
        private List<Grid> m_currentPath;
        private Vector3 m_startPos;
        private Vector3 m_endPos;

        // 颜色设置
        private Color m_emptyColor = Color.white;
        private Color m_obstacleColor = Color.black;
        private Color m_pathColor = Color.green;
        private Color m_startColor = Color.blue;
        private Color m_endColor = Color.red;

        private GameObject[,] m_gridObjects;

        private void Start()
        {
            if (autoBakeOnStart)
            {
                BakeMap();
            }

            // 设置默认起点和终点
            m_startPos = transform.position + new Vector3(1, 0, 1);
            m_endPos = transform.position + new Vector3(10, 0, 10);
        }



        public void BakeMap()
        {
            m_bakedMap = mapBaker.BakeMap();
            m_astar = new AStar(m_System,m_bakedMap);
            CreateGridVisualization();
            Debug.Log("地图烘焙完成，按C键计算路径");
        }

        private void CalculatePath()
        {
            if (m_bakedMap == null || m_astar == null)
            {
                Debug.LogError("地图未烘焙!");
                return;
            }

            // 转换坐标
            Vector2Int startGrid = GetGridPosition(m_startPos);
            Vector2Int endGrid = GetGridPosition(m_endPos);

            // 检查坐标是否有效
            if (!IsValidGridPosition(startGrid) || !IsValidGridPosition(endGrid))
            {
                Debug.LogError("起点或终点不在地图范围内!");
                return;
            }

            // 计算路径
            m_currentPath = m_astar.FindPath(startGrid.x, startGrid.y, endGrid.x, endGrid.y);

            // 显示路径
            if (m_currentPath != null)
            {
                Debug.Log("路径计算完成，长度: " + m_currentPath.Count);
                UpdateGridVisualization();
            }
            else
            {
                Debug.Log("无法找到路径");
            }
        }

        private Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            Bounds bounds = mapBaker.SceneBounds;
            float cellSize = mapBaker.CellSize;

            int x = Mathf.FloorToInt((worldPosition.x - bounds.min.x) / cellSize);
            int z = Mathf.FloorToInt((worldPosition.z - bounds.min.z) / cellSize);

            return new Vector2Int(x, z);
        }

        private bool IsValidGridPosition(Vector2Int gridPos)
        {
            if (m_bakedMap == null)
                return false;

            return gridPos.x >= 0 && gridPos.x < m_bakedMap.Width && gridPos.y >= 0 && gridPos.y < m_bakedMap.Height;
        }

        private void CreateGridVisualization()
        {
            if (m_bakedMap == null)
                return;

            // 清理旧的网格对象
            if (m_gridObjects != null)
            {
                for (int x = 0; x < m_bakedMap.Width; x++)
                {
                    for (int z = 0; z < m_bakedMap.Height; z++)
                    {
                        if (m_gridObjects[x, z] != null)
                        {
                            Destroy(m_gridObjects[x, z]);
                        }
                    }
                }
            }

            // 创建新的网格对象
            m_gridObjects = new GameObject[m_bakedMap.Width, m_bakedMap.Height];
            Bounds bounds = mapBaker.SceneBounds;
            float cellSize = mapBaker.CellSize;

            for (int x = 0; x < m_bakedMap.Width; x++)
            {
                for (int z = 0; z < m_bakedMap.Height; z++)
                {
                    Grid grid = m_bakedMap.GetGrid(x, z);
                    if (grid != null)
                    {
                        // 计算世界坐标
                        float worldX = bounds.min.x + x * cellSize + cellSize * 0.5f;
                        float worldZ = bounds.min.z + z * cellSize + cellSize * 0.5f;
                        Vector3 worldPos = new Vector3(worldX, grid.Y, worldZ);

                        // 创建网格对象
                        GameObject gridObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        gridObj.transform.position = worldPos;
                        gridObj.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
                        gridObj.name = "Grid_" + x + "_" + z;
                        gridObj.AddComponent<BoxCollider>();

                        // 设置颜色
                        Color color = grid.IsWalkable ? m_emptyColor : m_obstacleColor;
                        gridObj.GetComponent<Renderer>().material.color = color;

                        // 保存网格对象
                        m_gridObjects[x, z] = gridObj;
                    }
                }
            }
        }

        private void UpdateGridVisualization()
        {
            if (m_bakedMap == null || m_gridObjects == null)
                return;

            // 重置网格颜色
            for (int x = 0; x < m_bakedMap.Width; x++)
            {
                for (int z = 0; z < m_bakedMap.Height; z++)
                {
                    Grid grid = m_bakedMap.GetGrid(x, z);
                    if (grid != null && m_gridObjects[x, z] != null)
                    {
                        Color color = grid.IsWalkable ? m_emptyColor : m_obstacleColor;
                        m_gridObjects[x, z].GetComponent<Renderer>().material.color = color;
                    }
                }
            }

            // 显示路径
            if (m_currentPath != null)
            {
                foreach (Grid grid in m_currentPath)
                {
                    if (grid != null && grid.X >= 0 && grid.X < m_bakedMap.Width && grid.Z >= 0 && grid.Z < m_bakedMap.Height)
                    {
                        if (m_gridObjects[grid.X, grid.Z] != null)
                        {
                            m_gridObjects[grid.X, grid.Z].GetComponent<Renderer>().material.color = m_pathColor;
                        }
                    }
                }
            }

            // 显示起点和终点
            Vector2Int startGrid = GetGridPosition(m_startPos);
            Vector2Int endGrid = GetGridPosition(m_endPos);

            if (IsValidGridPosition(startGrid) && m_gridObjects[startGrid.x, startGrid.y] != null)
            {
                m_gridObjects[startGrid.x, startGrid.y].GetComponent<Renderer>().material.color = m_startColor;
            }

            if (IsValidGridPosition(endGrid) && m_gridObjects[endGrid.x, endGrid.y] != null)
            {
                m_gridObjects[endGrid.x, endGrid.y].GetComponent<Renderer>().material.color = m_endColor;
            }
        }

        private void Update()
        {
            if (mapBaker == null)
            {
                mapBaker = GetComponent<AStarMapBaker>();
                if (mapBaker == null)
                {
                    mapBaker = new AStarMapBaker();
                }
            }

            if (autoBakeOnStart)
            {
                BakeMap();
                autoBakeOnStart = false;
            }

            // 烘焙地图
            if (Input.GetKeyDown(KeyCode.B))
            {
                BakeMap();
            }

            // 计算路径
            if (Input.GetKeyDown(KeyCode.C) && m_bakedMap != null)
            {
                CalculatePath();
            }

            // 保存地图
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveMap();
            }

            // 加载地图
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadMap();
            }

            // 鼠标点击设置起点
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    m_startPos = hit.point;
                    Debug.Log("设置起点: " + m_startPos);
                }
            }

            // 鼠标点击设置终点
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    m_endPos = hit.point;
                    Debug.Log("设置终点: " + m_endPos);
                }
            }

            // 手动添加阻挡
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    mapBaker.AddObstacle(hit.point);
                }
            }

            // 手动移除阻挡
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    mapBaker.RemoveObstacle(hit.point);
                }
            }
        }

        private void OnGUI()
        {
            // 显示控制信息
            GUI.Box(new Rect(10, 10, 300, 250), "地图烘焙测试");
            GUI.Label(new Rect(20, 40, 280, 20), "B键: 烘焙地图");
            GUI.Label(new Rect(20, 60, 280, 20), "C键: 计算路径");
            GUI.Label(new Rect(20, 80, 280, 20), "S键: 保存地图");
            GUI.Label(new Rect(20, 100, 280, 20), "L键: 加载地图");
            GUI.Label(new Rect(20, 120, 280, 20), "左键: 设置起点");
            GUI.Label(new Rect(20, 140, 280, 20), "右键: 设置终点");
            GUI.Label(new Rect(20, 160, 280, 20), "1键: 添加阻挡");
            GUI.Label(new Rect(20, 180, 280, 20), "2键: 移除阻挡");

            // 显示地图信息
            if (m_bakedMap != null)
            {
                GUI.Box(new Rect(10, 270, 300, 100), "地图信息");
                GUI.Label(new Rect(20, 290, 280, 20), "地图大小: " + m_bakedMap.Width + "x" + m_bakedMap.Height);
                GUI.Label(new Rect(20, 310, 280, 20), "格子大小: " + mapBaker.CellSize + "m");
            }

            // 显示路径信息
            if (m_currentPath != null)
            {
                GUI.Box(new Rect(10, 380, 300, 50), "路径信息");
                GUI.Label(new Rect(20, 400, 280, 20), "路径长度: " + m_currentPath.Count);
            }
        }

        // 保存地图
        private void SaveMap()
        {
            if (m_bakedMap == null)
            {
                Debug.LogError("地图未烘焙!");
                return;
            }

            string filePath = UnityEngine.Application.persistentDataPath + "/map.bin";
            bool success = mapBaker.SaveMapToBinary(filePath);
            if (success)
            {
                Debug.Log("地图保存成功: " + filePath);
            }
        }

        // 加载地图
        private void LoadMap()
        {
            string filePath = UnityEngine.Application.persistentDataPath + "/map.bin";
            m_bakedMap = m_System.LoadMapFromBinary(filePath);
            if (m_bakedMap != null)
            {
                m_astar = new AStar(m_System,m_bakedMap);
                CreateGridVisualization();
                Debug.Log("地图加载成功: " + filePath);
            }
        }
    }
}
