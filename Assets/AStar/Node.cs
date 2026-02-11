/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	Node
作    者:	HappLI
描    述:	路径节点类，用于AStar算法
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector2 = UnityEngine.Vector2;
using FVector3 = UnityEngine.Vector3;
#endif
namespace Framework.Pathfinding.Runtime
{
    public class Node : System.IComparable<Node>
    {
        private int         m_x;
        private int         m_z;
        private Node        m_parent;
        private FFloat      m_gCost; // 从起点到当前节点的代价
        private FFloat      m_hCost; // 从当前节点到终点的估计代价

        public int X { get { return m_x; } }
        public int Z { get { return m_z; } }
        public Node Parent { get { return m_parent; } set { m_parent = value; } }
        public FFloat GCost { get { return m_gCost; } set { m_gCost = value; } }
        public FFloat HCost { get { return m_hCost; } set { m_hCost = value; } }
        public FFloat FCost { get { return m_gCost + m_hCost; } }

        //-------------------------------------------
        public void Reset(int x, int z, Node parent = null, float gCost = 0f, float hCost = 0f)
        {
            m_x = x;
            m_z = z;
            m_parent = parent;
            m_gCost = gCost;
            m_hCost = hCost;
        }
        //-------------------------------------------
        public int CompareTo(Node other)
        {
            if (FCost < other.FCost)
                return -1;
            if (FCost > other.FCost)
                return 1;
            if (m_hCost < other.m_hCost)
                return -1;
            if (m_hCost > other.m_hCost)
                return 1;
            return 0;
        }
    }
}
