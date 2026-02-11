/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	Map
作    者:	HappLI
描    述:	地图类，管理所有格子
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
    public class Map
    {
        private Grid[,] m_grids;
        private int m_width;
        private int m_height;
        private FFloat m_cellSize;

        public Map(int width, int height, float cellSize = 1f)
        {
            m_width = width;
            m_height = height;
            m_cellSize = cellSize;
            m_grids = new Grid[width, height];

            // 初始化所有格子
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    m_grids[x, z] = new Grid(x, z);
                }
            }
        }
        //-------------------------------------------
        public int Width { get { return m_width; } }
        public int Height { get { return m_height; } }
        public FFloat CellSize { get { return m_cellSize; } }
        //-------------------------------------------
        public Grid GetGrid(int x, int z)
        {
            if (x < 0 || x >= m_width || z < 0 || z >= m_height)
                return null;
            return m_grids[x, z];
        }
        //-------------------------------------------
        public void SetGrid(int x, int z, FFloat y, FFloat cost, int blockType)
        {
            if (x < 0 || x >= m_width || z < 0 || z >= m_height)
                return;

            Grid grid = m_grids[x, z];
            grid.Y = y;
            grid.Cost = cost;
            grid.BlockType = blockType;
        }
    }
}
