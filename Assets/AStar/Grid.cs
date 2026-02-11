/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	Grid
作    者:	HappLI
描    述:	格子类，表示地图中的一个格子
*********************************************************************/
using ExternEngine;

namespace Framework.Pathfinding.Runtime
{
    //-------------------------------------------
    //! 阻挡类型枚举
    //-------------------------------------------
    public enum EBlockType : byte
    {
        Walkable = 0,      // 可行走
        Unwalkable = 1,    // 不可走
        Wall = 2           // 墙
    }
    //-------------------------------------------
    //! Grid
    //-------------------------------------------
    public class Grid
    {
        private int     m_x; // x坐标
        private int     m_z; // z坐标
        private float   m_y; // y坐标（高度）
        private FFloat  m_cost; // 寻路权重
        private int     m_blockType; // 阻挡类型

        public int      X { get { return m_x; } }
        public int      Z { get { return m_z; } }
        public float    Y { get { return m_y; } set { m_y = value; } }
        public FFloat    Cost { get { return m_cost; } set { m_cost = value; } }
        public int      BlockType { get { return m_blockType; } set { m_blockType = value; } }
        public bool     IsWalkable { get { return m_blockType == (int)EBlockType.Walkable; } }

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
