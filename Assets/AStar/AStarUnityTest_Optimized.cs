using UnityEngine;
using System.Collections.Generic;
using Framework.Physic.RVO;
using ExternEngine;
using Random = UnityEngine.Random;

namespace AStarPathfinding
{
    public class AStarUnityTest_Optimized : MonoBehaviour
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
        
        // RVO参数配置
        [Header("RVO避障参数")]
        [Tooltip("时间视距：预测未来多少秒的碰撞")]
        public float rvoTimeHorizon = 2.0f;
        [Tooltip("障碍物时间视距：预测未来多少秒与障碍物的碰撞")]
        public float rvoTimeHorizonObs = 2.0f;
        [Tooltip("避障半径倍数：用于扩大避障检测范围")]
        public float rvoRadiusPower = 1.5f;
        [Tooltip("侧向避障强度：当正面受阻时，侧向避障的力度")]
        public float lateralAvoidanceStrength = 0.5f;
        [Tooltip("重新寻路阈值：当速度低于此值时，考虑重新寻路")]
        public float replanSpeedThreshold = 0.1f;
        [Tooltip("重新寻路等待时间：速度过低后等待多少秒才重新寻路")]
        public float replanWaitTime = 1.0f;
        [Tooltip("是否启用侧向避障")]
        public bool enableLateralAvoidance = true;
        [Tooltip("是否启用重新寻路")]
        public bool enableReplanPath = true;
        [Tooltip("单位权重：用于决定避让优先级")]
        public float unitWeight = 1.0f;
        
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
            public int rvoId;
            public Unit unit;
            public Color color;
            public UnitState state;
            public List<Grid> path;
            public int currentPathIndex;
            public float waitTime;
            public float elapsedWaitTime;
            public float lowSpeedTime;
            public Vector3 lastPosition;
        }

        private Map m_map;
        private UnitManager m_unitManager;
        WorldPhysic m_WorldRVO;

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

            // 初始化WorldPhysics
            m_WorldRVO = new WorldPhysic();

            // 重新设置障碍物，确保添加到WorldPhysics
            RecreateObstacles();

            // 初始化单位管理器
            m_unitManager = AStarPathfinding.CreateUnitManager(m_map);

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

                // 创建单位显示对象
                float size = UnityEngine.Random.Range(1, 3);
                GameObject unitObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                unitObj.transform.position = startPos + new Vector3(0, 0.5f, 0);
                unitObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * size;
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
                    elapsedWaitTime = 0f,
                    lowSpeedTime = 0f,
                    lastPosition = startPos
                };
                info.unit.MoveSpeed = UnityEngine.Random.Range(3, 10);
                // 初始化AStar
                info.astar = AStarPathfinding.CreateAStar(m_map);
                info.astar.SetUseMultiThreading(useMultiThreading);
                info.astar.SetUnitSize((int)size, unitHeight);
                info.color = color;
                info.rvoId = m_WorldRVO.AddNode(unit.Position, FVector3.zero, unitWidth, unitHeight);
                // 设置单位权重
                m_WorldRVO.SetNodeWeight(info.rvoId, unitWeight);
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
            if (useRVO && m_WorldRVO != null)
            {
                m_WorldRVO.Update(Time.deltaTime);
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

            // R键切换RVO模式
            if (Input.GetKeyDown(KeyCode.R))
            {
                useRVO = !useRVO;
                Debug.Log("RVO模式: " + useRVO);
            }
        }

        private void UpdateUnits()
        {
            for (int i = 0; i < m_unitInfos.Count; i++)
            {
                UnitInfo info = m_unitInfos[i];
                
                // 更新RVO节点位置和半径
                if (m_WorldRVO != null)
                {
                    m_WorldRVO.SetNodePhysicRadius(info.rvoId, info.unit.Radius);
                    m_WorldRVO.SetNodePosition(info.rvoId, info.unit.Position);
                }
                
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
                            info.lowSpeedTime = 0f;
                            info.lastPosition = info.unit.Position;
                            m_unitInfos[i] = info;
                        }
                        break;

                    case UnitState.Moving:
                        if (!useRVO)
                        {
                            // 未启用RVO时，使用原有的移动逻辑
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
                                    info.unit.Position = waypoint;
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
                        }
                        else
                        {
                            // 启用RVO时，由RVO系统控制移动
                            if (m_WorldRVO != null)
                            {
                                if (info.path != null && info.currentPathIndex < info.path.Count)
                                {
                                    Grid currentGrid = info.path[info.currentPathIndex];
                                    Vector3 waypoint = new Vector3(currentGrid.X, 0, currentGrid.Z);
                                    
                                    // 设置目标位置
                                    m_WorldRVO.SetNodeTargetPositon(info.rvoId, waypoint);
                                    
                                    // 计算新的速度，使用优化的参数
                                    var newVelocity = m_WorldRVO.ComputerNewVelocity(
                                        info.rvoId, 
                                        out var isCollisoned, 
                                        info.unit.MoveSpeed,
                                        rvoTimeHorizon,
                                        rvoTimeHorizonObs,
                                        rvoRadiusPower
                                    );
                                    
                                    // 计算新位置
                                    Vector3 newPosition = info.unit.Position + newVelocity * Time.deltaTime;
                                    
                                    // 检查新位置是否可行走
                                    if (IsWalkable(newPosition))
                                    {
                                        // 如果可行走，更新位置
                                        info.unit.Position = newPosition;
                                        
                                        // 检查速度是否过低
                                        float speed = Vector3.Distance(info.unit.Position, info.lastPosition) / Time.deltaTime;
                                        if (speed < replanSpeedThreshold)
                                        {
                                            info.lowSpeedTime += Time.deltaTime;
                                            
                                            // 如果速度过低时间过长，尝试重新寻路
                                            if (enableReplanPath && info.lowSpeedTime >= replanWaitTime)
                                            {
                                                // 尝试重新计算路径
                                                if (TryReplanPath(info))
                                                {
                                                    info.lowSpeedTime = 0f;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            info.lowSpeedTime = 0f;
                                        }
                                        
                                        info.lastPosition = info.unit.Position;
                                    }
                                    else
                                    {
                                        // 如果不可行走，尝试侧向避障
                                        if (enableLateralAvoidance)
                                        {
                                            Vector3 lateralVelocity = TryLateralAvoidance(info, newVelocity);
                                            if (lateralVelocity != Vector3.zero)
                                            {
                                                Vector3 lateralPosition = info.unit.Position + lateralVelocity * Time.deltaTime;
                                                if (IsWalkable(lateralPosition))
                                                {
                                                    info.unit.Position = lateralPosition;
                                                    info.lastPosition = info.unit.Position;
                                                }
                                            }
                                        }
                                        
                                        // 如果仍然不可行走，停止移动
                                        m_WorldRVO.SetNodeVelocity(info.rvoId, FVector3.zero);
                                        m_WorldRVO.SetNodePrefSpeed(info.rvoId, FVector3.zero);
                                    }

                                    // 检查是否到达当前路径点
                                    if (Vector3.Distance(info.unit.Position, waypoint) < 0.5f)
                                    {
                                        info.currentPathIndex++;
                                        info.lowSpeedTime = 0f;

                                        if(info.currentPathIndex>= info.path.Count)
                                        {
                                            // 到达目标，开始等待
                                            info.state = UnitState.Waiting;
                                            info.waitTime = Random.Range(1f, 3f);
                                            info.elapsedWaitTime = 0f;
                                            m_WorldRVO.SetNodePrefSpeed(info.rvoId, FVector3.zero);
                                            info.path.Clear();
                                        }
                                    }
                                }
                                m_unitInfos[i] = info;
                            }
                            
                            // 更新显示对象位置
                            if (m_unitGameObjects.ContainsKey(info.unit))
                            {
                                m_unitGameObjects[info.unit].transform.position = info.unit.Position + new Vector3(0, 0.5f, 0);
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

        // 尝试侧向避障
        private Vector3 TryLateralAvoidance(UnitInfo info, Vector3 originalVelocity)
        {
            if (info.path == null || info.currentPathIndex >= info.path.Count)
                return Vector3.zero;
            
            // 获取目标方向
            Grid currentGrid = info.path[info.currentPathIndex];
            Vector3 targetDir = (new Vector3(currentGrid.X, 0, currentGrid.Z) - info.unit.Position).normalized;
            
            // 计算侧向方向（垂直于目标方向）
            Vector3 lateralDir = new Vector3(-targetDir.z, 0, targetDir.x);
            
            // 尝试左右两个侧向方向
            Vector3[] testDirections = new Vector3[] { lateralDir, -lateralDir };
            
            foreach (var dir in testDirections)
            {
                Vector3 testVelocity = originalVelocity + dir * info.unit.MoveSpeed * lateralAvoidanceStrength;
                Vector3 testPosition = info.unit.Position + testVelocity * Time.deltaTime;
                
                if (IsWalkable(testPosition))
                {
                    return testVelocity;
                }
            }
            
            return Vector3.zero;
        }

        // 尝试重新寻路
        private bool TryReplanPath(UnitInfo info)
        {
            if (info.path == null || info.path.Count == 0)
                return false;
            
            // 获取当前目标
            Grid finalGrid = info.path[info.path.Count - 1];
            Vector3 targetPos = new Vector3(finalGrid.X, 0, finalGrid.Z);
            
            // 重新计算路径
            int startX = Mathf.FloorToInt(info.unit.Position.x);
            int startZ = Mathf.FloorToInt(info.unit.Position.z);
            int endX = Mathf.FloorToInt(targetPos.x);
            int endZ = Mathf.FloorToInt(targetPos.z);
            
            List<Grid> newPath = info.astar.FindPath(startX, startZ, endX, endZ);
            
            if (newPath != null && newPath.Count > 0)
            {
                info.path = newPath;
                info.currentPathIndex = 0;
                return true;
            }
            
            return false;
        }

        private void CalculatePath(UnitInfo info)
        {
            if (info.unit.TargetPosition == Vector3.zero)
                return;

            int startX = Mathf.FloorToInt(info.unit.Position.x);
            int startZ = Mathf.FloorToInt(info.unit.Position.z);
            int endX = Mathf.FloorToInt(info.unit.TargetPosition.x);
            int endZ = Mathf.FloorToInt(info.unit.TargetPosition.z);

            List<Grid> path = info.astar.FindPath(startX, startZ, endX, endZ);

            if (path != null && path.Count > 0)
            {
                info.path = path;
                info.currentPathIndex = 0;
                info.state = UnitState.Moving;
                info.lowSpeedTime = 0f;
                info.lastPosition = info.unit.Position;
            }
        }

        private void ShowUnitPaths()
        {
            if (!showPaths)
                return;

            foreach (var info in m_unitInfos)
            {
                if (info.path != null && info.path.Count > 1)
                {
                    for (int i = 0; i < info.path.Count - 1; i++)
                    {
                        Vector3 start = new Vector3(info.path[i].X, 0.1f, info.path[i].Z);
                        Vector3 end = new Vector3(info.path[i + 1].X, 0.1f, info.path[i + 1].Z);
                        Debug.DrawLine(start, end, info.color);
                    }
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
        }

        private void SetObstacle(int x, int z)
        {
            if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
            {
                m_map.SetGrid(x, z, 0, 1, 1);
                SetGridColor(x, z, m_obstacleColor);
                
                // 向WorldPhysics添加静态障碍物
                if (m_WorldRVO != null)
                {
                    // 为每个障碍物创建一个正方形区域
                    List<Vector3> vertices = new List<Vector3>();
                    float size = cellSize * 0.9f; // 稍小于格子大小，避免重叠
                    vertices.Add(new Vector3(-size/2, 0, -size/2));
                    vertices.Add(new Vector3(size/2, 0, -size/2));
                    vertices.Add(new Vector3(size/2, 0, size/2));
                    vertices.Add(new Vector3(-size/2, 0, size/2));
                    
                    Vector3 obstaclePosition = new Vector3(x * cellSize + cellSize/2, 0, z * cellSize + cellSize/2);
                    m_WorldRVO.AddStaticObstacle(obstaclePosition, vertices);
                }
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

        private void RecreateObstacles()
        {
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
    }
}
