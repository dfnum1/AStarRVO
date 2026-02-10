namespace AStarPathfinding
{
    // 地图类，管理所有格子
    public class Map
    {
        private Grid[,] m_grids;
        private int m_width;
        private int m_height;
        private float m_cellSize;

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
        public float CellSize { get { return m_cellSize; } }

        //-------------------------------------------

        public Grid GetGrid(int x, int z)
        {
            if (x < 0 || x >= m_width || z < 0 || z >= m_height)
                return null;
            return m_grids[x, z];
        }

        //-------------------------------------------

        public void SetGrid(int x, int z, float y, float cost, int blockType)
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
