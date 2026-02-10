using System.Collections.Generic;
using UnityEngine;
using ExternEngine;

namespace AStarPathfinding
{
    // RVO算法核心实现
    public class RVOAlgorithm
    {
        private struct Agent
        {
            public Unit unit;
            public FVector3 position;
            public FVector3 velocity;
            public FVector3 preferredVelocity;
            public FFloat radius;
            public FFloat maxSpeed;
            public FFloat maxAcceleration;
        }

        private List<Agent> m_agents;
        private int m_timeStep; // 时间步长（毫秒）
        private FFloat m_neighborDist; // 邻居检测距离
        private int m_maxNeighbors; // 最大邻居数量
        private FFloat m_timeHorizon; // 时间视野
        private FFloat m_timeHorizonObst; // 障碍物时间视野

        public RVOAlgorithm(int timeStep = 16, float neighborDist = 200, int maxNeighbors = 10)
        {
            m_agents = new List<Agent>();
            m_timeStep = timeStep;
            m_neighborDist = neighborDist;
            m_maxNeighbors = maxNeighbors;
            m_timeHorizon = new FFloat(1.0f);
            m_timeHorizonObst = new FFloat(1.0f);
        }

        // 添加单位
        public void AddUnit(Unit unit)
        {
            // 计算基于体积的半径
            float actualRadius = Mathf.Max(unit.Width, unit.Height) * 0.5f;
            unit.Radius = actualRadius;

            Agent agent = new Agent
            {
                unit = unit,
                position = unit.Position,
                velocity = FVector3.zero,
                preferredVelocity = FVector3.zero,
                radius = new FFloat(actualRadius),
                maxSpeed = new FFloat(5.0f), // 最大速度5m/s
                maxAcceleration = new FFloat(10.0f) // 最大加速度10m/s²
            };
            m_agents.Add(agent);
        }

        // 移除单位
        public void RemoveUnit(int unitId)
        {
            m_agents.RemoveAll(agent => agent.unit.UnitId == unitId);
        }

        // 更新单位目标
        public void UpdateUnitTarget(Unit unit, Vector3 targetPosition, FFloat speed)
        {
            for (int i = 0; i < m_agents.Count; i++)
            {
                Agent agent = m_agents[i];
                if (agent.unit.UnitId == unit.UnitId)
                {
                    // 计算期望速度
                    Vector3 direction = (targetPosition - unit.Position).normalized;
                    float distance = Vector3.Distance(unit.Position, targetPosition);
                    speed = FMath.Min(speed, distance / (m_timeStep / 1000.0f));
                    Vector3 desiredVelocity = direction * speed;

                    agent.preferredVelocity = desiredVelocity;
                    m_agents[i] = agent;
                    break;
                }
            }
        }

        public void ClearVelocity (Unit unit)
        {
            for (int i = 0; i < m_agents.Count; i++)
            {
                Agent agent = m_agents[i];
                if (agent.unit.UnitId == unit.UnitId)
                {
                    agent.velocity = FVector3.zero;
                    agent.preferredVelocity = FVector3.zero;
                    m_agents[i] = agent;
                    break;
                }
            }
        }

        // 执行一步RVO计算
        public void DoStep()
        {
            // 为每个智能体计算新速度
            for (int i = 0; i < m_agents.Count; i++)
            {
                Agent agent = m_agents[i];

                // 找到邻居
                List<Agent> neighbors = GetNeighbors(agent);

                // 计算最优速度
                FVector3 newVelocity = ComputeNewVelocity(agent, neighbors);

                // 更新速度
                agent.velocity = newVelocity;
                m_agents[i] = agent;
            }

            // 更新所有智能体的位置
            UpdateAgentPositions();

            // 更新单位位置
            for (int i = 0; i < m_agents.Count; i++)
            {
                Agent agent = m_agents[i];
                agent.unit.Position = agent.position;
                agent.unit.IsMoving = (agent.velocity.magnitude > new FFloat(0.1f));
                m_agents[i] = agent;
            }
        }

        // 更新智能体位置
        private void UpdateAgentPositions()
        {
            FFloat deltaTime = new FFloat(m_timeStep / 1000.0f);

            for (int i = 0; i < m_agents.Count; i++)
            {
                Agent agent = m_agents[i];

                // 更新位置：position = position + velocity * deltaTime
                FVector3 displacement = agent.velocity * deltaTime;
                agent.position = agent.position + displacement;
                m_agents[i] = agent;
            }
        }

        // 获取邻居
        private List<Agent> GetNeighbors(Agent agent)
        {
            List<Agent> neighbors = new List<Agent>();
            List<(Agent, FFloat)> neighborDistances = new List<(Agent, FFloat)>();

            foreach (var otherAgent in m_agents)
            {
                if (otherAgent.unit.UnitId == agent.unit.UnitId)
                    continue;

                FFloat distance = (agent.position - otherAgent.position).magnitude;
                if (distance < m_neighborDist)
                {
                    neighborDistances.Add((otherAgent, distance));
                }
            }

            // 按距离排序，取最近的几个
            neighborDistances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            for (int i = 0; i < Mathf.Min(neighborDistances.Count, m_maxNeighbors); i++)
            {
                neighbors.Add(neighborDistances[i].Item1);
            }

            return neighbors;
        }

        // 计算新速度
        private FVector3 ComputeNewVelocity(Agent agent, List<Agent> neighbors)
        {
            FVector3 newVelocity = agent.preferredVelocity;

            // 速度限制
            FFloat speed = newVelocity.magnitude;
            if (speed > agent.maxSpeed)
            {
                // 归一化后乘以最大速度
                FVector3 normalized = newVelocity.normalized;
                newVelocity = normalized * agent.maxSpeed;
            }

            // 避免碰撞
            FFloat minDistance = FFloat.FLT_MAX;
            FVector3 bestVelocity = newVelocity;

            // 采样速度空间
            List<FVector3> candidateVelocities = GenerateCandidateVelocities(agent);
            candidateVelocities.Add(newVelocity); // 添加首选速度

            foreach (var candidate in candidateVelocities)
            {
                FFloat distance = ComputeDistanceToCollision(agent, candidate, neighbors);
                if (distance > minDistance)
                {
                    minDistance = distance;
                    bestVelocity = candidate;
                }
            }

            return bestVelocity;
        }

        // 生成候选速度
        private List<FVector3> GenerateCandidateVelocities(Agent agent)
        {
            List<FVector3> candidates = new List<FVector3>();

            // 生成不同方向的候选速度
            float[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };
            float[] speeds = { 0, 0.5f, 1.0f };

            foreach (float angle in angles)
            {
                foreach (float speedFactor in speeds)
                {
                    float speed = speedFactor * (float)agent.maxSpeed;
                    float radian = angle * Mathf.Deg2Rad;

                    Vector3 velocity = new Vector3(
                        Mathf.Cos(radian) * speed,
                        0,
                        Mathf.Sin(radian) * speed
                    );

                    candidates.Add(velocity);
                }
            }

            return candidates;
        }

        // 计算到碰撞的距离
        private FFloat ComputeDistanceToCollision(Agent agent, FVector3 velocity, List<Agent> neighbors)
        {
            FFloat minDistance = FFloat.FLT_MAX;

            foreach (var neighbor in neighbors)
            {
                // 相对速度
                FVector3 relativeVelocity = velocity - neighbor.velocity;

                // 相对位置
                FVector3 relativePosition = neighbor.position - agent.position;

                // 距离的平方
                FFloat distSq = relativePosition.sqrMagnitude;
                FFloat combinedRadius = agent.radius + neighbor.radius;
                FFloat combinedRadiusSq = combinedRadius * combinedRadius;

                // 如果已经碰撞
                if (distSq < combinedRadiusSq)
                {
                    return FFloat.zero;
                }

                // 计算碰撞时间
                FFloat timeToCollision = ComputeTimeToCollision(relativePosition, relativeVelocity, combinedRadius);
                if (timeToCollision < minDistance)
                {
                    minDistance = timeToCollision;
                }
            }

            return minDistance;
        }

        // 计算碰撞时间
        private FFloat ComputeTimeToCollision(FVector3 relativePosition, FVector3 relativeVelocity, FFloat combinedRadius)
        {
            FFloat a = relativeVelocity.sqrMagnitude;
            if (a == FFloat.zero)
            {
                return FFloat.FLT_MAX; // 相对速度为0，不会碰撞
            }

            FFloat b = 2 * FVector3.Dot(relativePosition, relativeVelocity);
            FFloat c = relativePosition.sqrMagnitude - combinedRadius * combinedRadius;

            // 解二次方程
            FFloat discriminant = b * b - 4 * a * c;
            if (discriminant < FFloat.zero)
            {
                return FFloat.FLT_MAX; // 没有实根，不会碰撞
            }

            FFloat sqrtDiscriminant = FMath.Sqrt(discriminant);
            FFloat t1 = (-b - sqrtDiscriminant) / (2 * a);
            FFloat t2 = (-b + sqrtDiscriminant) / (2 * a);

            // 取最小的正根
            if (t1 > FFloat.zero && t2 > FFloat.zero)
            {
                return FMath.Min(t1, t2);
            }
            else if (t1 > FFloat.zero)
            {
                return t1;
            }
            else if (t2 > FFloat.zero)
            {
                return t2;
            }
            else
            {
                return FFloat.FLT_MAX; // 已经碰撞过了
            }
        }

        // 清除所有智能体
        public void Clear()
        {
            m_agents.Clear();
        }
    }
}