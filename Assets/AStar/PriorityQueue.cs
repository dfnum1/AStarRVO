/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	PriorityQueue
作    者:	HappLI
描    述:	大小堆（优先队列）类，用于AStar算法的高效路径搜索
*********************************************************************/
using System.Collections.Generic;

namespace Framework.Pathfinding.Runtime
{
    public class PriorityQueue
    {
        private List<Node> m_heap;

        public PriorityQueue()
        {
            m_heap = new List<Node>();
        }

        //-------------------------------------------

        public int Count { get { return m_heap.Count; } }

        //-------------------------------------------

        public void Clear()
        {
            m_heap.Clear();
        }

        //-------------------------------------------

        public void Enqueue(Node node)
        {
            m_heap.Add(node);
            int index = m_heap.Count - 1;
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (m_heap[index].CompareTo(m_heap[parentIndex]) >= 0)
                    break;
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        //-------------------------------------------

        public Node Dequeue()
        {
            Node first = m_heap[0];
            int lastIndex = m_heap.Count - 1;
            m_heap[0] = m_heap[lastIndex];
            m_heap.RemoveAt(lastIndex);

            int index = 0;
            while (true)
            {
                int leftChildIndex = index * 2 + 1;
                int rightChildIndex = index * 2 + 2;
                int smallestIndex = index;

                if (leftChildIndex < m_heap.Count && m_heap[leftChildIndex].CompareTo(m_heap[smallestIndex]) < 0)
                    smallestIndex = leftChildIndex;
                if (rightChildIndex < m_heap.Count && m_heap[rightChildIndex].CompareTo(m_heap[smallestIndex]) < 0)
                    smallestIndex = rightChildIndex;

                if (smallestIndex == index)
                    break;
                Swap(index, smallestIndex);
                index = smallestIndex;
            }

            return first;
        }

        //-------------------------------------------

        public bool Contains(Node node)
        {
            return m_heap.Contains(node);
        }

        //-------------------------------------------

        public void UpdateNode(Node node)
        {
            int index = m_heap.IndexOf(node);
            if (index == -1)
                return;

            // 向上调整
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (m_heap[index].CompareTo(m_heap[parentIndex]) >= 0)
                    break;
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        //-------------------------------------------

        private void Swap(int a, int b)
        {
            Node temp = m_heap[a];
            m_heap[a] = m_heap[b];
            m_heap[b] = temp;
        }
    }
}
