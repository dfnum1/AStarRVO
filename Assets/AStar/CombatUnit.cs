using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
    // 目标接口
    public interface ITarget
    {
        int TargetId { get; }
        Vector3 Position { get; }
        float Radius { get; }
        bool IsAlive { get; }
        int Priority { get; }
    }
    
    // 战斗单位类，继承自Unit
    public class CombatUnit : Unit
    {
        public float AttackRange { get; set; }
        public float AttackDamage { get; set; }
        public float AttackCooldown { get; set; }
        public float LastAttackTime { get; set; }
        public ITarget CurrentTarget { get; set; }
        public bool IsAttacking { get; set; }
        public TargetManager TargetManager { get; set; }
        
        public CombatUnit(int unitId, Vector3 position, float attackRange = 2f, int width = 1, int height = 1)
            : base(unitId, position, width, height)
        {
            AttackRange = attackRange;
            AttackDamage = 10f;
            AttackCooldown = 1f;
            LastAttackTime = 0f;
            IsAttacking = false;
        }
        
        // 设置目标管理器
        public void SetTargetManager(TargetManager targetManager)
        {
            TargetManager = targetManager;
        }
        
        // 更新单位状态
        public void Update(float deltaTime)
        {
            if (IsMoving)
            {
                // 移动逻辑
                if (CurrentTarget != null)
                {
                    // 检查是否在攻击范围内
                    if (IsInAttackRange(CurrentTarget))
                    {
                        IsMoving = false;
                        IsAttacking = true;
                    }
                }
            }
            else if (IsAttacking)
            {
                // 攻击逻辑
                if (CurrentTarget != null && CurrentTarget.IsAlive)
                {
                    if (Time.time - LastAttackTime >= AttackCooldown)
                    {
                        Attack(CurrentTarget);
                        LastAttackTime = Time.time;
                    }
                }
                else
                {
                    // 目标死亡或不存在，寻找新目标
                    FindNewTarget();
                }
            }
            else
            {
                // 闲置状态，寻找目标
                FindNewTarget();
            }
        }
        
        // 检查是否在攻击范围内
        public bool IsInAttackRange(ITarget target)
        {
            float distance = Vector3.Distance(Position, target.Position);
            return distance <= AttackRange + target.Radius;
        }
        
        // 攻击目标
        public void Attack(ITarget target)
        {
            // 攻击逻辑
            Debug.Log($"Unit {UnitId} attacks target {target.TargetId} for {AttackDamage} damage");
            
            // 这里可以添加伤害计算、动画触发等逻辑
        }
        
        // 寻找新目标
        public void FindNewTarget()
        {
            if (TargetManager != null)
            {
                ITarget newTarget = TargetManager.FindClosestTarget(Position, AttackRange);
                if (newTarget != null)
                {
                    CurrentTarget = newTarget;
                    IsAttacking = true;
                    IsMoving = false;
                }
                else
                {
                    // 没有找到目标，保持闲置
                    CurrentTarget = null;
                    IsAttacking = false;
                }
            }
        }
        
        // 设置移动目标
        public void SetMoveTarget(Vector3 targetPosition)
        {
            TargetPosition = targetPosition;
            IsMoving = true;
            IsAttacking = false;
            CurrentTarget = null;
        }
        
        // 设置攻击目标
        public void SetAttackTarget(ITarget target)
        {
            CurrentTarget = target;
            IsAttacking = true;
            IsMoving = false;
        }
    }
    
    // 目标管理器，处理目标切换
    public class TargetManager
    {
        private List<ITarget> m_targets;
        private float m_cellSize;
        
        public TargetManager(float cellSize)
        {
            m_targets = new List<ITarget>();
            m_cellSize = cellSize;
        }
        
        // 添加目标
        public void AddTarget(ITarget target)
        {
            if (!m_targets.Contains(target))
            {
                m_targets.Add(target);
            }
        }
        
        // 移除目标
        public void RemoveTarget(ITarget target)
        {
            m_targets.Remove(target);
        }
        
        // 移除目标（通过ID）
        public void RemoveTarget(int targetId)
        {
            m_targets.RemoveAll(t => t.TargetId == targetId);
        }
        
        // 寻找最近的目标
        public ITarget FindClosestTarget(Vector3 position, float maxRange)
        {
            ITarget closestTarget = null;
            float closestDistance = float.MaxValue;
            
            foreach (var target in m_targets)
            {
                if (target.IsAlive)
                {
                    float distance = Vector3.Distance(position, target.Position);
                    if (distance <= maxRange && distance < closestDistance)
                    {
                        closestTarget = target;
                        closestDistance = distance;
                    }
                }
            }
            
            return closestTarget;
        }
        
        // 寻找优先级最高的目标
        public ITarget FindHighestPriorityTarget(Vector3 position, float maxRange)
        {
            ITarget priorityTarget = null;
            int highestPriority = int.MinValue;
            
            foreach (var target in m_targets)
            {
                if (target.IsAlive && Vector3.Distance(position, target.Position) <= maxRange)
                {
                    if (target.Priority > highestPriority)
                    {
                        priorityTarget = target;
                        highestPriority = target.Priority;
                    }
                }
            }
            
            return priorityTarget;
        }
        
        // 寻找所有在范围内的目标
        public List<ITarget> FindTargetsInRange(Vector3 position, float range)
        {
            List<ITarget> targetsInRange = new List<ITarget>();
            
            foreach (var target in m_targets)
            {
                if (target.IsAlive && Vector3.Distance(position, target.Position) <= range)
                {
                    targetsInRange.Add(target);
                }
            }
            
            return targetsInRange;
        }
        
        // 清除所有目标
        public void ClearAllTargets()
        {
            m_targets.Clear();
        }
        
        // 获取所有目标
        public List<ITarget> GetAllTargets()
        {
            return m_targets;
        }
        
        // 更新目标状态
        public void UpdateTargets()
        {
            // 移除死亡目标
            m_targets.RemoveAll(t => !t.IsAlive);
        }
    }
    
    // 建筑物目标类，实现ITarget接口
    public class BuildingTarget : ITarget
    {
        public int TargetId { get; set; }
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public bool IsAlive { get; set; }
        public int Priority { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        
        public BuildingTarget(int targetId, Vector3 position, float radius = 1f, int priority = 0)
        {
            TargetId = targetId;
            Position = position;
            Radius = radius;
            IsAlive = true;
            Priority = priority;
            MaxHealth = 100;
            Health = MaxHealth;
        }
        
        // 受到伤害
        public void TakeDamage(float damage)
        {
            Health -= Mathf.FloorToInt(damage);
            if (Health <= 0)
            {
                Health = 0;
                IsAlive = false;
            }
        }
    }
}
