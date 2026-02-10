#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
using UnityEngine;

namespace Framework.Physic.RVO
{
    public class KDQueryNode
    {
        public KDNode node;
        public FVector3 tempClosestPoint;
        public FFloat distance;

        public KDQueryNode() 
        {

        }
        public KDQueryNode(KDNode node, FVector3 tempClosestPoint) 
        {
            this.node = node;
            this.tempClosestPoint = tempClosestPoint;
        }

        public void Destroy()
        {
        }
    }
}