/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	RVONode
作    者:	HappLI
描    述:	RVO节点
*********************************************************************/
#define KDTREE_DUPLICATES

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif

namespace Framework.Physic.RVO
{
    internal class RVONode
    {
        private int m_nID = 0;
        private int m_nDummyID = 0;
        private FVector3 m_vPosition = FVector3.zero;
        private FVector3 m_vPrefSpeed = FVector3.zero;
        private FVector3 m_vAdvSpeed = FVector3.zero;
        private FFloat m_fPhysicRadius = 0.0f;
        private FFloat m_fWeight = 1.0f;
        private FVector3 m_Velocity = FVector3.zero;
        private RVONode m_pNext = null;
        private RVONode m_pPrev = null;
#if UNITY_EDITOR
        internal System.Collections.Generic.List<WorldPhysic.Line> vDebugOrca = new System.Collections.Generic.List<WorldPhysic.Line>(8);
#endif
        internal RVONode()
        {
            m_nID = 0;
            m_nDummyID = 0;
        }
        //------------------------------------------------------
        internal void SetId(int id)
        {
            m_nID = id;
        }
        //------------------------------------------------------
        public RVONode GetNext()
        {
            return m_pNext;
        }
        //------------------------------------------------------
        public void SetNext(RVONode pNode)
        {
            m_pNext = pNode;
        }
        //------------------------------------------------------
        public RVONode GetPrev()
        {
            return m_pPrev;
        }
        //------------------------------------------------------
        public void SetPrev(RVONode pNode)
        {
            m_pPrev = pNode;
        }
        //------------------------------------------------------
        public int GetID()
        {
            return m_nID;
        }
        //------------------------------------------------------
        public int GetDummyID()
        {
            return m_nDummyID;
        }
        //------------------------------------------------------
        public void SetDummyID(int nDummyID)
        {
            m_nDummyID = nDummyID;
        }
        //------------------------------------------------------
        public FVector3 GetPosition()
        {
            return m_vPosition;
        }
        //------------------------------------------------------
        public void SetPosition(FVector3 vPosition)
        {
            m_vPosition = vPosition;
        }
        //------------------------------------------------------
        public FVector3 GetPrefSpeed()
        {
            return m_vPrefSpeed;
        }
        //------------------------------------------------------
        public void SetPrefSpeed(FVector3 vSpeed)
        {
            m_vPrefSpeed = vSpeed;
        }
        //------------------------------------------------------
        public void SetNodeTargetPositon(FVector3 targetPos, FFloat moveSpeed)
        {
            FVector3 dir = targetPos - m_vPosition;
            dir.y = 0;
            m_vPrefSpeed = dir.normalized* moveSpeed;
        }
        //------------------------------------------------------
        public FVector3 GetAdvSpeed()
        {
            return m_vAdvSpeed;
        }
        //------------------------------------------------------
        public void SetAdvSpeed(FVector3 vSpeed)
        {
            m_vAdvSpeed = vSpeed;
        }
        //------------------------------------------------------
        public FVector3 GetVelocity()
        {
            return m_Velocity;
        }
        //------------------------------------------------------
        public void SetVelocity(FVector3 vSpeed)
        {
            m_Velocity = vSpeed;
        }
        //------------------------------------------------------
        public FFloat GetWeight()
        {
            return m_fWeight;
        }
        //------------------------------------------------------
        public void SetWeight(FFloat fWeight)
        {
            m_fWeight = fWeight;
        }
        //------------------------------------------------------
        public FFloat GetPhysicRadius()
        {
            return m_fPhysicRadius;
        }
        //------------------------------------------------------
        public void SetPhysicRadius(FFloat fPhysicRadius)
        {
            m_fPhysicRadius = fPhysicRadius;
        }
        //------------------------------------------------------
        public BoundBox GetBound()
        {
            return new BoundBox(m_vPosition, FVector3.one*m_fPhysicRadius);
        }
        //------------------------------------------------------
        public void Destroy()
        {       
            m_nID = 0;
            m_nDummyID = 0;
            m_vPosition = FVector3.zero;
            m_vPrefSpeed = FVector3.zero;
            m_Velocity = FVector3.zero;
            m_vAdvSpeed = FVector3.zero;
            m_fPhysicRadius = 0.0f;
            m_pNext = null;
            m_pPrev = null;
        }
        //------------------------------------------------------
        public bool IsDestroy()
        {
            return m_nID <= 0;
        }
    }
}