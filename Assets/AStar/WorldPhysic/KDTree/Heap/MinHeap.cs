/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	MinHeap
作    者:	HappLI
描    述:	小堆实现
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
#endif
using System.Collections.Generic;

namespace Framework.Physic.RVO
{
    public class MinHeap : BaseHeap 
    {
        public MinHeap(int initialSize = 2048) : base(initialSize) 
        {

        }
        public override void PushValue(FFloat h) 
        {
            // if heap array is full
            if(nodesCount == maxSize) 
            {

                UpsizeHeap();
            }

            nodesCount++;
            heap[nodesCount] = h;
            BubbleUpMin(nodesCount);
        }

        public override FFloat PopValue() 
        {
            if(nodesCount == 0)
                throw new System.ArgumentException("Heap is empty!");

            FFloat result = heap[1];

            heap[1] = heap[nodesCount];

            nodesCount--;

            if(nodesCount != 0)
                BubbleDownMin(1);

            return result;
        }
    }

    // generic version
    public class MinHeap<T> : MinHeap
    {
        T[] objs; // objects
        public MinHeap(int maxNodes = 2048) : base(maxNodes) 
        {
            objs = new T[maxNodes + 1];
        }
        public T     HeadHeapObject { get { return objs[1]; } }

        T tempObjs;
        protected override void Swap(int A, int B) 
        {
            tempHeap = heap[A];
            tempObjs = objs[A];

            heap[A] = heap[B];
            objs[A] = objs[B];

            heap[B] = tempHeap;
            objs[B] = tempObjs;
        }

        public override void PushValue(FFloat h)
        {
            throw new System.ArgumentException("Use Push(T, FFloat)!");
        }

        public override FFloat PopValue()
        {
            throw new System.ArgumentException("Use Push(T, FFloat)!");
        }

        public void PushObj(T obj, FFloat h) 
        {
            // if heap array is full
            if(nodesCount == maxSize)
            {
                UpsizeHeap();
            }

            nodesCount++;
            heap[nodesCount] = h;
            objs[nodesCount] = obj;

            BubbleUpMin(nodesCount);
        }

        public T PopObj()
        {
            if(nodesCount == 0)
                throw new System.ArgumentException("Heap is empty!");

            T result = objs[1];

            heap[1] = heap[nodesCount];
            objs[1] = objs[nodesCount];

            objs[nodesCount] = default(T);

            nodesCount--;

            if(nodesCount != 0)
                BubbleDownMin(1);

            return result;
        }

        public T PopObj(ref FFloat heapValue) 
        {
            if(nodesCount == 0)
                throw new System.ArgumentException("Heap is empty!");

            heapValue = heap[1];
            T result = PopObj();

            return result;
        }

        protected override void UpsizeHeap() 
        {
            maxSize *= 2;
            System.Array.Resize(ref heap, maxSize + 1);
            System.Array.Resize(ref objs, maxSize + 1);
        }

        //flush internal array, returns ordered data
        public void FlushResult(List<T> resultList, List<FFloat> heapList = null)
        {
            int count = nodesCount + 1;

            if(heapList == null)
            {
                for(int i = 1; i < count; i++) 
                {
                    resultList.Add(PopObj());
                }
            }
            else
            {
                FFloat h = 0;

                for(int i = 1; i < count; i++)
                {
                    resultList.Add(PopObj(ref h));
                    heapList.Add(h);
                }
            }
        }
    }
}