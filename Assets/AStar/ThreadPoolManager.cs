/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	ThreadPoolManager
作    者:	HappLI
描    述:	线程池管理器，根据设备性能自动调整线程数，提供任务执行接口，确保主线程性能稳定。
*********************************************************************/
using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework.Pathfinding.Runtime
{
    // 线程池管理器
    internal class ThreadPoolManager
    {
        private int m_maxThreads;
        private int m_availableThreads;
        private object m_lockObject;
        
        public ThreadPoolManager()
        {
            // 根据设备性能自动调整线程数
            int processorCount = SystemInfo.processorCount;
            m_maxThreads = Mathf.Max(1, processorCount - 1); // 保留一个核心给主线程
            m_availableThreads = m_maxThreads;
            m_lockObject = new object();
            
            Debug.Log($"ThreadPoolManager initialized with {m_maxThreads} threads");
        }
        
        // 获取最大线程数
        public int MaxThreads { get { return m_maxThreads; } }
        
        // 获取可用线程数
        public int AvailableThreads { get { return m_availableThreads; } }
        
        // 执行任务
        public Task<T> ExecuteTask<T>(Func<T> function)
        {
            lock (m_lockObject)
            {
                // 如果没有可用线程，在主线程执行
                if (m_availableThreads <= 0)
                {
                    return Task.FromResult(function());
                }
                
                m_availableThreads--;
            }
            
            return Task.Run(() =>
            {
                try
                {
                    return function();
                }
                finally
                {
                    lock (m_lockObject)
                    {
                        m_availableThreads++;
                    }
                }
            });
        }
        
        // 重置线程池
        public void Reset()
        {
            lock (m_lockObject)
            {
                m_availableThreads = m_maxThreads;
            }
        }
    }
}
