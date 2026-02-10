using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
    // 整数向量类，用于RVO算法中的整数运算
    public struct IntVector2
    {
        public int x;
        public int z;

        public IntVector2(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static IntVector2 Zero { get { return new IntVector2(0, 0); } }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.z + b.z);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.z - b.z);
        }

        public static IntVector2 operator *(IntVector2 a, int scalar)
        {
            return new IntVector2(a.x * scalar, a.z * scalar);
        }

        public static IntVector2 operator /(IntVector2 a, int scalar)
        {
            return new IntVector2(a.x / scalar, a.z / scalar);
        }

        public int SqrMagnitude()
        {
            return x * x + z * z;
        }

        public int Magnitude()
        {
            return (int)Mathf.Sqrt(x * x + z * z);
        }

        public IntVector2 Normalized()
        {
            int mag = Magnitude();
            if (mag == 0)
                return Zero;
            return this / mag;
        }

        public static int Dot(IntVector2 a, IntVector2 b)
        {
            return a.x * b.x + a.z * b.z;
        }

        public override string ToString()
        {
            return $"({x}, {z})";
        }
    }

    // RVO智能体类
    public class RVOAgent
    {
        public int AgentId { get; set; }
        public IntVector2 Position { get; set; } // 位置（厘米）
        public IntVector2 Velocity { get; set; } // 速度（厘米/秒）
        public IntVector2 PreferredVelocity { get; set; } // 期望速度（厘米/秒）
        public int Radius { get; set; } // 半径（厘米）
        public int MaxSpeed { get; set; } // 最大速度（厘米/秒）
        public int TimeHorizon { get; set; } // 时间范围（100倍秒，即厘秒）

        public RVOAgent(int agentId, IntVector2 position, int radius = 50, int maxSpeed = 100)
        {
            AgentId = agentId;
            Position = position;
            Velocity = IntVector2.Zero;
            PreferredVelocity = IntVector2.Zero;
            Radius = radius;
            MaxSpeed = maxSpeed;
            TimeHorizon = 300; // 3秒
        }

        // 更新智能体状态
        public void Update(int deltaTimeMs)
        {
            // 根据速度更新位置
            // deltaTimeMs 是毫秒，转换为秒需要除以1000
            // 所以速度（厘米/秒） * (deltaTimeMs / 1000) = 厘米
            IntVector2 deltaPosition = Velocity * deltaTimeMs / 1000;
            Position += deltaPosition;
        }

        // 设置目标位置，计算期望速度
        public void SetTarget(IntVector2 targetPosition, int speed = -1)
        {
            IntVector2 direction = targetPosition - Position;
            int desiredSpeed = (speed == -1) ? MaxSpeed : speed;

            if (direction.SqrMagnitude() > 0)
            {
                PreferredVelocity = direction.Normalized() * desiredSpeed;
            }
            else
            {
                PreferredVelocity = IntVector2.Zero;
            }
        }
    }

    // RVO算法核心类
    public class RVOAlgorithm
    {
        private List<RVOAgent> m_agents;
        private int m_timeStep; // 时间步长（毫秒）
        private int m_neighborDist; // 邻居搜索距离（厘米）
        private int m_maxNeighbors; // 最大邻居数量
        private List<RVOAgent> m_tempNeighbors; // 临时邻居列表，用于优化GC
        private List<IntVector2> m_tempVelocities; // 临时速度列表，用于优化GC

        public RVOAlgorithm(int timeStep = 16, int neighborDist = 500, int maxNeighbors = 10)
        {
            m_agents = new List<RVOAgent>();
            m_timeStep = timeStep;
            m_neighborDist = neighborDist;
            m_maxNeighbors = maxNeighbors;

            // 预分配临时列表，减少GC
            m_tempNeighbors = new List<RVOAgent>(maxNeighbors * 2);
            m_tempVelocities = new List<IntVector2>(20); // 预分配足够的空间
        }

        // 添加智能体
        public void AddAgent(RVOAgent agent)
        {
            m_agents.Add(agent);
        }

        // 移除智能体
        public void RemoveAgent(int agentId)
        {
            m_agents.RemoveAll(a => a.AgentId == agentId);
        }

        // 执行一步RVO算法（优化版本）
        public void DoStep()
        {
            // 先计算所有智能体的新速度
            foreach (RVOAgent agent in m_agents)
            {
                // 使用优化的邻居搜索，减少GC
                List<RVOAgent> neighbors = FindNeighbors(agent, m_tempNeighbors);

                // 计算最优速度
                IntVector2 newVelocity = ComputeOptimalVelocity(agent, neighbors);

                // 更新速度
                agent.Velocity = newVelocity;
            }

            // 再更新所有智能体的位置
            foreach (RVOAgent agent in m_agents)
            {
                agent.Update(m_timeStep);
            }
        }

        // 优化的邻居搜索，使用预分配的列表
        private List<RVOAgent> FindNeighbors(RVOAgent agent, List<RVOAgent> tempNeighbors)
        {
            tempNeighbors.Clear();

            foreach (RVOAgent other in m_agents)
            {
                if (other.AgentId != agent.AgentId)
                {
                    IntVector2 distance = other.Position - agent.Position;
                    int sqrDist = distance.SqrMagnitude();
                    int maxDistSqr = m_neighborDist * m_neighborDist;

                    if (sqrDist < maxDistSqr)
                    {
                        tempNeighbors.Add(other);
                    }
                }
            }

            // 按距离排序（使用插入排序，对于小列表更高效）
            for (int i = 1; i < tempNeighbors.Count; i++)
            {
                RVOAgent current = tempNeighbors[i];
                int j = i - 1;
                int currentDistSqr = (current.Position - agent.Position).SqrMagnitude();

                while (j >= 0)
                {
                    int jDistSqr = (tempNeighbors[j].Position - agent.Position).SqrMagnitude();
                    if (jDistSqr > currentDistSqr)
                    {
                        tempNeighbors[j + 1] = tempNeighbors[j];
                        j--;
                    }
                    else
                    {
                        break;
                    }
                }
                tempNeighbors[j + 1] = current;
            }

            // 限制邻居数量
            if (tempNeighbors.Count > m_maxNeighbors)
            {
                tempNeighbors.RemoveRange(m_maxNeighbors, tempNeighbors.Count - m_maxNeighbors);
            }

            return tempNeighbors;
        }

        // 计算最优速度
        private IntVector2 ComputeOptimalVelocity(RVOAgent agent, List<RVOAgent> neighbors)
        {
            IntVector2 optimalVelocity = agent.PreferredVelocity;
            float optimalCost = CalculateCost(agent, optimalVelocity, neighbors);

            // 生成候选速度
            List<IntVector2> candidateVelocities = GenerateCandidateVelocities(agent);

            // 评估每个候选速度
            foreach (IntVector2 candidate in candidateVelocities)
            {
                float cost = CalculateCost(agent, candidate, neighbors);
                if (cost < optimalCost)
                {
                    optimalVelocity = candidate;
                    optimalCost = cost;
                }
            }

            return optimalVelocity;
        }

        // 生成候选速度（优化版本，使用预分配的列表）
        private List<IntVector2> GenerateCandidateVelocities(RVOAgent agent)
        {
            m_tempVelocities.Clear();

            // 添加期望速度
            m_tempVelocities.Add(agent.PreferredVelocity);

            // 添加当前速度
            m_tempVelocities.Add(agent.Velocity);

            // 添加零速度
            m_tempVelocities.Add(IntVector2.Zero);

            // 生成一些其他候选速度（减少采样数量，提高性能）
            int numSamples = 6;
            for (int i = 0; i < numSamples; i++)
            {
                float angle = 2 * Mathf.PI * i / numSamples;
                int x = (int)(Mathf.Cos(angle) * agent.MaxSpeed);
                int z = (int)(Mathf.Sin(angle) * agent.MaxSpeed);
                m_tempVelocities.Add(new IntVector2(x, z));
            }

            // 确保速度不超过最大速度
            int maxSpeedSqr = agent.MaxSpeed * agent.MaxSpeed;
            for (int i = 0; i < m_tempVelocities.Count; i++)
            {
                IntVector2 candidate = m_tempVelocities[i];
                int speedSqr = candidate.SqrMagnitude();

                if (speedSqr > maxSpeedSqr)
                {
                    m_tempVelocities[i] = candidate.Normalized() * agent.MaxSpeed;
                }
            }

            return m_tempVelocities;
        }

        // 计算速度的代价
        private float CalculateCost(RVOAgent agent, IntVector2 velocity, List<RVOAgent> neighbors)
        {
            float cost = 0;

            // 与期望速度的差距
            IntVector2 velocityDiff = velocity - agent.PreferredVelocity;
            cost += velocityDiff.SqrMagnitude() / 10000.0f; // 归一化

            // 与其他智能体的碰撞风险
            foreach (RVOAgent other in neighbors)
            {
                float collisionRisk = CalculateCollisionRisk(agent, other, velocity);
                cost += collisionRisk;
            }

            return cost;
        }

        // 计算碰撞风险
        private float CalculateCollisionRisk(RVOAgent agent, RVOAgent other, IntVector2 velocity)
        {
            IntVector2 relativePosition = other.Position - agent.Position;
            IntVector2 relativeVelocity = other.Velocity - velocity;

            int distanceSqr = relativePosition.SqrMagnitude();
            int combinedRadius = agent.Radius + other.Radius;
            int combinedRadiusSqr = combinedRadius * combinedRadius;

            // 如果已经重叠，风险很高
            if (distanceSqr < combinedRadiusSqr)
            {
                return 1000.0f;
            }

            // 计算相对速度与相对位置的点积
            int dotProduct = IntVector2.Dot(relativeVelocity, relativePosition);

            // 如果相对速度是远离的，风险较低
            if (dotProduct > 0)
            {
                return 0.0f;
            }

            // 计算碰撞时间
            int velocitySqr = relativeVelocity.SqrMagnitude();
            if (velocitySqr == 0)
            {
                return 0.0f;
            }

            // 碰撞时间的计算
            // t = -(v · r) / |v|²
            int tNumerator = -dotProduct;
            int tDenominator = velocitySqr;
            int t = tNumerator * 1000 / tDenominator; // 乘以1000，转换为厘秒

            // 如果碰撞时间大于时间范围，风险较低
            if (t > agent.TimeHorizon)
            {
                return 0.0f;
            }

            // 计算碰撞风险
            float risk = 1.0f - (float)t / agent.TimeHorizon;
            return risk * 10.0f; // 权重
        }

        // 检查位置是否可行走（考虑静态障碍物）
        private bool IsPositionWalkable(IntVector2 position, int radius)
        {
            // 这里需要实现静态障碍物的检查
            // 由于RVOAlgorithm类没有直接访问地图的权限，
            // 我们需要在RVOAStarIntegrator中实现这个功能
            // 并将结果传递给RVOAlgorithm

            // 暂时返回true，后续会在集成器中完善
            return true;
        }

        // 获取所有智能体
        public List<RVOAgent> GetAgents()
        {
            return m_agents;
        }

        // 清除所有智能体
        public void ClearAgents()
        {
            m_agents.Clear();
        }
    }

    // RVO与A*的集成类
    public class RVOAStarIntegrator
    {
        private RVOAlgorithm m_rvo;
        private UnitManager m_unitManager;
        private Dictionary<int, RVOAgent> m_unitToAgentMap;
        private float m_cellSize; // 地图格子大小（米）
        private Map m_map; // 地图引用，用于检查静态障碍物

        public RVOAStarIntegrator(Map map, UnitManager unitManager, int timeStep = 16)
        {
            m_unitManager = unitManager;
            m_map = map;
            m_cellSize = map.CellSize;
            m_rvo = new RVOAlgorithm(timeStep);
            m_unitToAgentMap = new Dictionary<int, RVOAgent>();
        }

        // 添加单位到RVO系统
        public void AddUnit(Unit unit)
        {
            // 将单位的世界坐标转换为RVO的整数坐标（厘米）
            IntVector2 position = new IntVector2(
                (int)(unit.Position.x * 100),
                (int)(unit.Position.z * 100)
            );

            // 单位半径（厘米）
            int radius = (int)(Mathf.Max(unit.Width, unit.Height) * m_cellSize * 100 * 0.5f);

            // 最大速度（厘米/秒）
            int maxSpeed = 100; // 1米/秒

            // 创建RVO智能体
            RVOAgent agent = new RVOAgent(unit.UnitId, position, radius, maxSpeed);

            // 添加到RVO系统
            m_rvo.AddAgent(agent);
            m_unitToAgentMap[unit.UnitId] = agent;
        }

        // 移除单位
        public void RemoveUnit(int unitId)
        {
            if (m_unitToAgentMap.ContainsKey(unitId))
            {
                m_rvo.RemoveAgent(unitId);
                m_unitToAgentMap.Remove(unitId);
            }
        }

        // 更新单位目标
        public void UpdateUnitTarget(Unit unit, Vector3 targetPosition)
        {
            if (m_unitToAgentMap.TryGetValue(unit.UnitId, out RVOAgent agent))
            {
                // 将世界坐标转换为RVO的整数坐标（厘米）
                IntVector2 target = new IntVector2(
                    (int)(targetPosition.x * 100),
                    (int)(targetPosition.z * 100)
                );

                // 更新RVO智能体的目标
                agent.SetTarget(target);
            }
        }

        // 执行一步集成算法
        public void DoStep()
        {
            // 执行RVO算法
            m_rvo.DoStep();

            // 更新单位位置和速度
            foreach (var kvp in m_unitToAgentMap)
            {
                int unitId = kvp.Key;
                RVOAgent agent = kvp.Value;

                // 将RVO的整数坐标转换回世界坐标（米）
                Vector3 position = new Vector3(
                    agent.Position.x / 100.0f,
                    0,
                    agent.Position.z / 100.0f
                );

                // 检查位置是否可行走（考虑静态障碍物）
                if (IsPositionWalkable(position))
                {
                    // 更新单位位置
                    m_unitManager.UpdateUnitPosition(unitId, position);
                }
                else
                {
                    // 如果位置不可行走，尝试寻找附近的可行走位置
                    Vector3 newPosition = FindNearestWalkablePosition(position);
                    if (newPosition != Vector3.zero)
                    {
                        m_unitManager.UpdateUnitPosition(unitId, newPosition);
                        // 更新RVO智能体的位置
                        agent.Position = new IntVector2(
                            (int)(newPosition.x * 100),
                            (int)(newPosition.z * 100)
                        );
                    }
                }
            }
        }

        // 检查位置是否可行走（考虑静态障碍物）
        private bool IsPositionWalkable(Vector3 position)
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

        // 寻找最近的可行走位置
        private Vector3 FindNearestWalkablePosition(Vector3 position)
        {
            int radius = 3; // 搜索半径

            for (int i = 1; i <= radius; i++)
            {
                // 搜索当前半径的所有点
                for (int xOffset = -i; xOffset <= i; xOffset++)
                {
                    for (int zOffset = -i; zOffset <= i; zOffset++)
                    {
                        // 只搜索半径的边缘
                        if (Mathf.Abs(xOffset) == i || Mathf.Abs(zOffset) == i)
                        {
                            Vector3 testPosition = position + new Vector3(xOffset * m_cellSize, 0, zOffset * m_cellSize);
                            if (IsPositionWalkable(testPosition))
                            {
                                return testPosition;
                            }
                        }
                    }
                }
            }

            return Vector3.zero; // 没有找到可行走位置
        }

        // 获取RVO算法实例
        public RVOAlgorithm RVO { get { return m_rvo; } }
    }
}
