/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	KDNode
作    者:	HappLI
描    述:	Kd树节点
*********************************************************************/

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
#endif

namespace Framework.Physic.RVO
{
    public class KDNode 
    {
        public FFloat partitionCoordinate;
        public int partitionAxis = -1;

        public KDNode negativeChild;
        public KDNode positiveChild;

        public int start;
        public int end;

        public int Count { get { return end - start; } }

        public bool Leaf { get { return partitionAxis == -1; } }

        public KDBounds bounds;

    };

}
