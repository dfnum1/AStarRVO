using UnityEngine;
using System.Collections.Generic;

namespace AStarPathfinding
{
    public class AStarUnityTest : MonoBehaviour
    {
        public int mapWidth = 20;
        public int mapHeight = 20;
        public float cellSize = 1f;
        public bool useMultiThreading = false;
        public bool useRVO = false;
        public int unitWidth = 1;
        public int unitHeight = 1;

        private Map m_map;
        private AStar m_astar;
        private UnitManager m_unitManager;
        private RVOAlgorithm m_rvo;
        private RVOAStarIntegrator m_rvoIntegrator;
        private List<Grid> m_currentPath;
        private GameObject[,] m_gridObjects;
        private Vector3 m_startPos;
        private Vector3 m_endPos;
        private bool m_isCalculatingPath = false;

        // 颜色设置
        private Color m_emptyColor = Color.white;
        private Color m_obstacleColor = Color.black;
        private Color m_pathColor = Color.green;
        private Color m_startColor = Color.blue;
        private Color m_endColor = Color.red;

        private void Start()
        {
            // 初始化地图
            InitializeMap();

            // 初始化AStar
            m_astar = AStarPathfinding.CreateAStar(m_map);
            m_astar.SetUseMultiThreading(useMultiThreading);
            m_astar.SetUnitSize(unitWidth, unitHeight);

            // 初始化单位管理器
            m_unitManager = AStarPathfinding.CreateUnitManager(m_map);

            // 初始化RVO
            m_rvoIntegrator = AStarPathfinding.CreateRVOAStarIntegrator(m_astar, m_unitManager);

            // 设置起点和终点
            m_startPos = new Vector3(1, 0, 1);
            m_endPos = new Vector3(mapWidth - 2, 0, mapHeight - 2);

            // 计算路径
            CalculatePath();
        }

        private void Update()
        {
            // RVO更新
            if (useRVO && m_rvoIntegrator != null)
            {
                m_rvoIntegrator.DoStep();
            }

            // 鼠标点击设置起点
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    int x = Mathf.FloorToInt(hit.point.x / cellSize);
                    int z = Mathf.FloorToInt(hit.point.z / cellSize);
                    if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
                    {
                        m_startPos = new Vector3(x, 0, z);
                        CalculatePath();
                    }
                }
            }

            // 鼠标右键设置终点
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    int x = Mathf.FloorToInt(hit.point.x / cellSize);
                    int z = Mathf.FloorToInt(hit.point.z / cellSize);
                    if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
                    {
                        m_endPos = new Vector3(x, 0, z);
                        CalculatePath();
                    }
                }
            }

            // 空格键切换多线程模式
            if (Input.GetKeyDown(KeyCode.Space))
            {
                useMultiThreading = !useMultiThreading;
                m_astar.SetUseMultiThreading(useMultiThreading);
                Debug.Log("多线程模式: " + useMultiThreading);
                CalculatePath();
            }

            // R键切换RVO模式
            if (Input.GetKeyDown(KeyCode.R))
            {
                useRVO = !useRVO;
                Debug.Log("RVO模式: " + useRVO);
                CalculatePath();
            }

            // 数字键1-4调整单位大小
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                unitWidth = 1;
                unitHeight = 1;
                m_astar.SetUnitSize(unitWidth, unitHeight);
                Debug.Log("单位大小: 1x1");
                CalculatePath();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                unitWidth = 2;
                unitHeight = 2;
                m_astar.SetUnitSize(unitWidth, unitHeight);
                Debug.Log("单位大小: 2x2");
                CalculatePath();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                unitWidth = 3;
                unitHeight = 3;
                m_astar.SetUnitSize(unitWidth, unitHeight);
                Debug.Log("单位大小: 3x3");
                CalculatePath();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                unitWidth = 4;
                unitHeight = 4;
                m_astar.SetUnitSize(unitWidth, unitHeight);
                Debug.Log("单位大小: 4x4");
                CalculatePath();
            }
        }

        private void InitializeMap()
        {
            // 创建地图
            m_map = AStarPathfinding.CreateMap(mapWidth, mapHeight, cellSize);

            // 创建网格对象
            m_gridObjects = new GameObject[mapWidth, mapHeight];

            // 初始化网格
            for (int x = 0; x < mapWidth; x++)
            {
                for (int z = 0; z < mapHeight; z++)
                {
                    // 创建网格对象
                    GameObject gridObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gridObj.transform.position = new Vector3(x * cellSize, 0, z * cellSize);
                    gridObj.transform.localScale = new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f);
                    gridObj.name = "Grid_" + x + "_" + z;
                    gridObj.AddComponent<BoxCollider>();

                    // 保存网格对象
                    m_gridObjects[x, z] = gridObj;

                    // 设置颜色
                    SetGridColor(x, z, m_emptyColor);
                }
            }

            // 设置边界
            for (int x = 0; x < mapWidth; x++)
            {
                SetObstacle(x, 0);
                SetObstacle(x, mapHeight - 1);
            }
            for (int z = 0; z < mapHeight; z++)
            {
                SetObstacle(0, z);
                SetObstacle(mapWidth - 1, z);
            }

            // 设置中间障碍物
            for (int x = 5; x < mapWidth - 5; x++)
            {
                SetObstacle(x, mapHeight / 2);
            }
            // 设置一个缺口
            SetEmpty(mapWidth / 2, mapHeight / 2);

            // 设置其他障碍物
            SetObstacle(3, 3);
            SetObstacle(4, 3);
            SetObstacle(5, 3);
        }

        private void SetObstacle(int x, int z)
        {
            if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
            {
                m_map.SetGrid(x, z, 0, 1, 1);
                SetGridColor(x, z, m_obstacleColor);
            }
        }

        private void SetEmpty(int x, int z)
        {
            if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
            {
                m_map.SetGrid(x, z, 0, 1, 0);
                SetGridColor(x, z, m_emptyColor);
            }
        }

        private void SetGridColor(int x, int z, Color color)
        {
            if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
            {
                m_gridObjects[x, z].GetComponent<Renderer>().material.color = color;
            }
        }

        private void CalculatePath()
        {
            if (m_isCalculatingPath)
                return;

            m_isCalculatingPath = true;

            // 重置网格颜色
            ResetGridColors();

            // 转换坐标
            int startX = Mathf.FloorToInt(m_startPos.x);
            int startZ = Mathf.FloorToInt(m_startPos.z);
            int endX = Mathf.FloorToInt(m_endPos.x);
            int endZ = Mathf.FloorToInt(m_endPos.z);

            // 计时开始
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // 计算路径
            m_currentPath = m_astar.FindPath(startX, startZ, endX, endZ);

            // 计时结束
            stopwatch.Stop();

            // 显示路径
            if (m_currentPath != null)
            {
                Debug.Log("路径计算完成，长度: " + m_currentPath.Count + ", 时间: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
                ShowPath();

                // 如果开启RVO，创建单位并添加到RVO系统
                if (useRVO && m_rvoIntegrator != null && m_unitManager != null)
                {
                    // 清除旧单位
                    m_unitManager.ClearAllUnits();

                    // 创建新单位
                    Unit unit = new Unit(1, m_startPos, unitWidth, unitHeight);
                    m_unitManager.AddUnit(unit);
                    
                    // 添加到RVO系统
                    m_rvoIntegrator.AddUnit(unit);

                    // 设置目标
                    m_rvoIntegrator.UpdateUnitTarget(unit, m_endPos);
                }
            }
            else
            {
                Debug.Log("无法找到路径，时间: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
            }

            // 标记起点和终点
            SetGridColor(startX, startZ, m_startColor);
            SetGridColor(endX, endZ, m_endColor);

            m_isCalculatingPath = false;
        }

        private void ResetGridColors()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int z = 0; z < mapHeight; z++)
                {
                    Grid grid = m_map.GetGrid(x, z);
                    if (grid.IsWalkable)
                    {
                        SetGridColor(x, z, m_emptyColor);
                    }
                    else
                    {
                        SetGridColor(x, z, m_obstacleColor);
                    }
                }
            }
        }

        private void ShowPath()
        {
            if (m_currentPath == null)
                return;

            foreach (Grid grid in m_currentPath)
            {
                SetGridColor(grid.X, grid.Z, m_pathColor);
            }
        }

        private void OnGUI()
        {
            // 显示信息
            GUI.Box(new Rect(10, 10, 300, 180), "AStar寻路测试");
            GUI.Label(new Rect(20, 40, 280, 20), "多线程模式: " + (useMultiThreading ? "开启" : "关闭"));
            GUI.Label(new Rect(20, 60, 280, 20), "RVO模式: " + (useRVO ? "开启" : "关闭"));
            GUI.Label(new Rect(20, 80, 280, 20), "单位大小: " + unitWidth + "x" + unitHeight);
            GUI.Label(new Rect(20, 100, 280, 20), "左键: 设置起点");
            GUI.Label(new Rect(20, 120, 280, 20), "右键: 设置终点");
            GUI.Label(new Rect(20, 140, 280, 20), "空格键: 切换多线程模式");
            GUI.Label(new Rect(20, 160, 280, 20), "R键: 切换RVO模式");

            // 显示路径信息
            if (m_currentPath != null)
            {
                GUI.Box(new Rect(10, 200, 300, 50), "路径信息");
                GUI.Label(new Rect(20, 220, 280, 20), "路径长度: " + m_currentPath.Count);
            }
        }
    }
}
