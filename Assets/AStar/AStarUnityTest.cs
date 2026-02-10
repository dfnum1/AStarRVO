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
        public bool useRVO = true;
        public int unitWidth = 1;
        public int unitHeight = 1;
        public int unitCount = 10;
        public bool unitTest = false;
        public bool showPaths = true;
        // 单位状态枚举
        private enum UnitState
        {
            Idle,       // 空闲
            Moving,     // 移动中
            Waiting     // 等待中
        }

        // 单位信息结构体
        private struct UnitInfo
        {
            public AStar astar;
            public Unit unit;
            public Color color;
            public UnitState state;
            public List<Grid> path;
            public int currentPathIndex;
            public float waitTime;
            public float elapsedWaitTime;
        }

        private Map m_map;
        private UnitManager m_unitManager;
        private RVOAlgorithm m_rvo;
        private RVOAStarIntegrator m_rvoIntegrator;
        private GameObject[,] m_gridObjects;
        private bool m_isCalculatingPath = false;
        private List<UnitInfo> m_unitInfos = new List<UnitInfo>();
        private Dictionary<Unit, GameObject> m_unitGameObjects = new Dictionary<Unit, GameObject>();

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

            // 初始化单位管理器
            m_unitManager = AStarPathfinding.CreateUnitManager(m_map);

            // 初始化RVO
            m_rvoIntegrator = AStarPathfinding.CreateRVOAStarIntegrator(m_map, m_unitManager);

            // 创建10个单位
            CreateUnits();
        }

        private void CreateUnits()
        {
            // 清除旧的单位对象
            foreach (var go in m_unitGameObjects.Values)
            {
                if (go != null)
                {
                    Destroy(go);
                }
            }
            m_unitGameObjects.Clear();

            m_unitInfos.Clear();
            m_unitManager.ClearAllUnits();

            for (int i = 0; i < unitCount; i++)
            {
                // 随机初始位置（避开边界和障碍物）
                Vector3 startPos;
                do
                {
                    int x = Random.Range(1, mapWidth - 1);
                    int z = Random.Range(1, mapHeight - 1);
                    startPos = new Vector3(x, 0, z);
                } while (!IsWalkable(startPos));

                // 创建单位
                Unit unit = new Unit(i, startPos, unitWidth, unitHeight);
                m_unitManager.AddUnit(unit);
                m_rvoIntegrator.AddUnit(unit);

                // 创建单位显示对象
                float size = UnityEngine.Random.Range(1, 3);
                GameObject unitObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                unitObj.transform.position = startPos + new Vector3(0, 0.5f, 0);
                unitObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f)* size;
                unitObj.name = "Unit_" + i;
                Color color = GetRandomColor();
                unitObj.GetComponent<Renderer>().material.color = color;
                m_unitGameObjects[unit] = unitObj;

                // 创建单位信息
                UnitInfo info = new UnitInfo
                {
                    unit = unit,
                    state = UnitState.Idle,
                    path = null,
                    currentPathIndex = 0,
                    waitTime = 0f,
                    elapsedWaitTime = 0f
                };
                // 初始化AStar
                info.astar = AStarPathfinding.CreateAStar(m_map);
                info.astar.SetUseMultiThreading(useMultiThreading);
                info.astar.SetUnitSize((int)size, unitHeight);
                info.color = color;
                m_unitInfos.Add(info);
            }
        }

        private Color GetRandomColor()
        {
            return new Color(
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f)
            );
        }

        private bool IsWalkable(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int z = Mathf.FloorToInt(pos.z);
            if (x < 0 || x >= mapWidth || z < 0 || z >= mapHeight)
                return false;
            Grid grid = m_map.GetGrid(x, z);
            return grid != null && grid.IsWalkable;
        }

        private void Update()
        {
            // RVO更新
            if (useRVO && m_rvoIntegrator != null)
            {
                m_rvoIntegrator.DoStep();
            }

            // 更新单位状态
            UpdateUnits();

            // 显示单位路径
            ShowUnitPaths();

            // 鼠标点击设置起点
            if (unitTest && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    int x = Mathf.FloorToInt(hit.point.x / cellSize);
                    int z = Mathf.FloorToInt(hit.point.z / cellSize);
                    if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
                    {
                        var startPos = new Vector3(x, 0, z);
                        foreach (var db in m_unitInfos)
                        {
                            db.unit.Position = startPos;
                            CalculatePath(db);
                        }
                    }
                }
            }

            // 鼠标右键设置终点
            if (unitTest && Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    int x = Mathf.FloorToInt(hit.point.x / cellSize);
                    int z = Mathf.FloorToInt(hit.point.z / cellSize);
                    if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
                    {
                        var endPos = new Vector3(x, 0, z);
                        foreach (var db in m_unitInfos)
                        {
                            db.unit.TargetPosition = endPos;
                            CalculatePath(db);
                        }
                    }
                }
            }

            // 空格键切换多线程模式
            if (Input.GetKeyDown(KeyCode.Space))
            {
                useMultiThreading = !useMultiThreading;
                foreach (var db in m_unitInfos)
                {
                    db.astar.SetUseMultiThreading(useMultiThreading);
                    CalculatePath(db);
                }
                Debug.Log("多线程模式: " + useMultiThreading);
            }
        }

        private void UpdateUnits()
        {
            for (int i = 0; i < m_unitInfos.Count; i++)
            {
                UnitInfo info = m_unitInfos[i];
                switch (info.state)
                {
                    case UnitState.Idle:
                        // 生成随机目标点
                        Vector3 targetPos;
                        do
                        {
                            int x = Random.Range(1, mapWidth - 1);
                            int z = Random.Range(1, mapHeight - 1);
                            targetPos = new Vector3(x, 0, z);
                        } while (!IsWalkable(targetPos));

                        // 计算路径
                        int startX = Mathf.FloorToInt(info.unit.Position.x);
                        int startZ = Mathf.FloorToInt(info.unit.Position.z);
                        int endX = Mathf.FloorToInt(targetPos.x);
                        int endZ = Mathf.FloorToInt(targetPos.z);
                        List<Grid> path = info.astar.FindPath(startX, startZ, endX, endZ);

                        if (path != null && path.Count > 0)
                        {
                            // 更新单位信息
                            info.unit.TargetPosition = targetPos;
                            info.path = path;
                            info.currentPathIndex = 0;
                            info.state = UnitState.Moving;
                            m_unitInfos[i] = info;
                        }
                        break;

                    case UnitState.Moving:
                        if (info.path != null && info.currentPathIndex < info.path.Count)
                        {
                            // 获取当前目标点
                            Grid currentGrid = info.path[info.currentPathIndex];
                            Vector3 waypoint = new Vector3(currentGrid.X, 0, currentGrid.Z);

                            // 向目标点移动
                            Vector3 currentPos = info.unit.Position;
                            Vector3 direction = (waypoint - currentPos).normalized;
                            Vector3 newPos = currentPos + direction * 2f * Time.deltaTime;

                            // 检查是否到达当前路径点
                            if (Vector3.Distance(newPos, waypoint) < 0.1f)
                            {
                                // 到达路径点，移动到下一个
                                info.unit.Position=waypoint;
                                info.currentPathIndex++;
                                m_unitInfos[i] = info;

                                // 更新显示对象位置
                                if (m_unitGameObjects.ContainsKey(info.unit))
                                {
                                    m_unitGameObjects[info.unit].transform.position = waypoint + new Vector3(0, 0.5f, 0);
                                }
                            }
                            else
                            {
                                // 更新单位位置
                                info.unit.Position = newPos;
                                m_unitInfos[i] = info;

                                // 更新显示对象位置
                                if (m_unitGameObjects.ContainsKey(info.unit))
                                {
                                    m_unitGameObjects[info.unit].transform.position = newPos + new Vector3(0, 0.5f, 0);
                                }
                            }

                            // 检查是否到达最终目标
                            if (info.currentPathIndex >= info.path.Count)
                            {
                                // 到达目标，开始等待
                                info.state = UnitState.Waiting;
                                info.waitTime = Random.Range(1f, 3f);
                                info.elapsedWaitTime = 0f;
                                info.path.Clear();
                                m_unitInfos[i] = info;
                            }
                        }
                        break;

                    case UnitState.Waiting:
                        // 累计等待时间
                        info.elapsedWaitTime += Time.deltaTime;
                        if (info.elapsedWaitTime >= info.waitTime)
                        {
                            // 等待结束，切换到空闲状态
                            info.state = UnitState.Idle;
                        }
                        m_unitInfos[i] = info;
                        break;
                }
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

        private void CalculatePath(UnitInfo info)
        {
            if (m_isCalculatingPath)
                return;

            m_isCalculatingPath = true;

            // 重置网格颜色
            ResetGridColors();

            // 转换坐标
            int startX = Mathf.FloorToInt(info.unit.Position.x);
            int startZ = Mathf.FloorToInt(info.unit.Position.z);
            int endX = Mathf.FloorToInt(info.unit.TargetPosition.x);
            int endZ = Mathf.FloorToInt(info.unit.TargetPosition.z);

            // 计时开始
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // 计算路径
            info.path = info.astar.FindPath(startX, startZ, endX, endZ);

            // 计时结束
            stopwatch.Stop();

            // 显示路径
            if (info.path != null)
            {
                Debug.Log("路径计算完成，长度: " + info.path.Count + ", 时间: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
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

        private void ShowUnitPaths()
        {
            if (!showPaths)
                return;
            // 重置所有网格颜色
            ResetGridColors();

            // 显示每个单位的路径
            foreach (var info in m_unitInfos)
            {
                if (info.path != null && info.path.Count > 0)
                {
                    // 为每个单位的路径设置颜色
                    Color pathColor = info.color;
                    foreach (Grid grid in info.path)
                    {
                        SetGridColor(grid.X, grid.Z, pathColor);
                    }
                }
            }
        }


        private void OnGUI()
        {
            // 显示信息
            GUI.Box(new Rect(10, 10, 300, 220), "AStar寻路测试");
            GUI.Label(new Rect(20, 40, 280, 20), "多线程模式: " + (useMultiThreading ? "开启" : "关闭"));
            GUI.Label(new Rect(20, 60, 280, 20), "RVO模式: " + (useRVO ? "开启" : "关闭"));
            GUI.Label(new Rect(20, 80, 280, 20), "单位大小: " + unitWidth + "x" + unitHeight);
            GUI.Label(new Rect(20, 100, 280, 20), "左键: 设置起点");
            GUI.Label(new Rect(20, 120, 280, 20), "右键: 设置终点");
            GUI.Label(new Rect(20, 140, 280, 20), "空格键: 切换多线程模式");
            GUI.Label(new Rect(20, 160, 280, 20), "R键: 切换RVO模式");

            string count = $"Count: {this.unitCount:N0}";
            string fps = $"FPS: {1.0f / Time.smoothDeltaTime:F1}";
            GUI.Label(new Rect(18, 180, 450, 20), fps);
            GUI.Label(new Rect(18, 200, 450, 20), count);
        }
    }
}
