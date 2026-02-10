namespace AStarPathfinding
{
    // 阻挡类型枚举
    public enum EBlockType : byte
    {
        Walkable = 0,      // 可行走
        Unwalkable = 1,    // 不可走
        Wall = 2           // 墙
    }

    // 格子类，表示地图中的一个格子
    public class Grid
    {
        private int m_x; // x坐标
        private int m_z; // z坐标
        private float m_y; // y坐标（高度）
        private float m_cost; // 寻路权重
        private int m_blockType; // 阻挡类型

        public int X { get { return m_x; } }
        public int Z { get { return m_z; } }
        public float Y { get { return m_y; } set { m_y = value; } }
        public float Cost { get { return m_cost; } set { m_cost = value; } }
        public int BlockType { get { return m_blockType; } set { m_blockType = value; } }
        public bool IsWalkable { get { return m_blockType == (int)EBlockType.Walkable; } }

        public Grid(int x, int z, float y = 0f, float cost = 1f, int blockType = (int)EBlockType.Walkable)
        {
            m_x = x;
            m_z = z;
            m_y = y;
            m_cost = cost;
            m_blockType = blockType;
        }
    }
}
