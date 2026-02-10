using UnityEngine;
using System.Collections.Generic;

namespace AStarPathfinding
{
    public class RVOTest : MonoBehaviour
    {
        public int unitCount = 20;
        public float spawnRadius = 10f;
        public float targetRadius = 20f;
        public float testDuration = 10f;
        
        private Map m_map;
        private AStar m_astar;
        private UnitManager m_unitManager;
        private RVOAlgorithm m_rvo;
        private List<Unit> m_units;
        private List<GameObject> m_unitVisuals;
        private float m_testTime;
        private bool m_testing;
        
        private void Start()
        {
            // 创建地图
            m_map = AStarPathfinding.CreateMap(100, 100, 1f);
            
            // 创建AStar实例
            m_astar = AStarPathfinding.CreateAStar(m_map);
            
            // 创建单位管理器
            m_unitManager = AStarPathfinding.CreateUnitManager(m_map);
            
            // 创建RVO算法
            m_rvo = AStarPathfinding.CreateRVOAlgorithm();
            
            // 初始化单位列表
            m_units = new List<Unit>();
            m_unitVisuals = new List<GameObject>();
            
            // 生成测试单位
            SpawnUnits();
            
            // 开始测试
            m_testing = true;
            m_testTime = 0f;
        }
        
        private void SpawnUnits()
        {
            for (int i = 0; i < unitCount; i++)
            {
                // 随机生成起始位置
                Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
                Vector3 position = new Vector3(randomPos.x, 0, randomPos.y);
                
                // 创建单位
                Unit unit = new Unit(i, position, 1, 1);
                m_units.Add(unit);
                
                // 添加到单位管理器
                m_unitManager.AddUnit(unit);
                m_rvo.AddUnit(unit);
                 
                // 创建可视化对象
                GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                visual.transform.position = position;
                visual.transform.localScale = new Vector3(0.8f, 0.2f, 0.8f);
                visual.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
                m_unitVisuals.Add(visual);
                
                // 随机生成目标位置
                Vector2 randomTarget = Random.insideUnitCircle * targetRadius;
                Vector3 targetPosition = new Vector3(randomTarget.x, 0, randomTarget.y);
            }
        }
        
        private void Update()
        {
            if (m_testing)
            {
                // 更新测试时间
                m_testTime += Time.deltaTime;
                
                // 执行RVO算法
                m_rvo.DoStep();
                
                // 更新可视化
                UpdateVisuals();
                
                // 检查测试是否结束
                if (m_testTime >= testDuration)
                {
                    EndTest();
                }
            }
        }
        
        private void UpdateVisuals()
        {   
            for (int i = 0; i < m_units.Count && i < m_unitVisuals.Count; i++)
            {
                Unit unit = m_units[i];
                GameObject visual = m_unitVisuals[i];
                
                if (visual != null)
                {
                    visual.transform.position = unit.Position;
                }
            }
        }
        
        private void EndTest()
        {
            m_testing = false;
            
            Debug.Log($"RVO测试完成！测试了 {unitCount} 个单位，持续了 {testDuration} 秒。");
            Debug.Log($"平均每帧执行时间: {(Time.timeSinceLevelLoad / Time.frameCount) * 1000} ms");
        }
        
        private void OnGUI()
        {   
            GUI.Box(new Rect(10, 10, 300, 150), "RVO测试");
            GUI.Label(new Rect(20, 40, 280, 20), "单位数量: " + unitCount);
            GUI.Label(new Rect(20, 60, 280, 20), "测试时间: " + m_testTime.ToString("F2") + " / " + testDuration + " 秒");
            GUI.Label(new Rect(20, 80, 280, 20), "测试状态: " + (m_testing ? "运行中" : "已完成"));
            
            if (!m_testing)
            {
                if (GUI.Button(new Rect(20, 110, 260, 30), "重新开始测试"))
                {
                    RestartTest();
                }
            }
        }
        
        private void RestartTest()
        {   
            // 清理旧单位
            foreach (GameObject visual in m_unitVisuals)
            {
                if (visual != null)
                {
                    Destroy(visual);
                }
            }
            
            m_units.Clear();
            m_unitVisuals.Clear();
            m_unitManager.ClearAllUnits();
            
            // 重新生成单位
            SpawnUnits();
            
            // 重新开始测试
            m_testing = true;
            m_testTime = 0f;
        }
        
        private void OnDestroy()
        {   
            // 清理资源
            foreach (GameObject visual in m_unitVisuals)
            {
                if (visual != null)
                {
                    Destroy(visual);
                }
            }
        }
    }
}
