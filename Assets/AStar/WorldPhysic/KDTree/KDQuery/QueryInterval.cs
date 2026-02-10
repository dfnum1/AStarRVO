/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	KDQuery
作    者:	HappLI
描    述:	interval
*********************************************************************/
using System.Collections.Generic;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif

namespace Framework.Physic.RVO
{
    public partial class KDQuery 
    {
        public void Interval(WorldKDTree tree, FVector3 min, FVector3 max, List<int> resultIndices) 
        {
            Reset();

            RVONode[] points = tree.Points;
            int[] permutation = tree.Permutation;

            var rootNode = tree.RootNode;

            PushToQueue(
                rootNode,
                rootNode.bounds.ClosestPoint((min + max) / 2)
            );

            KDQueryNode queryNode = null;
            KDNode node = null;


            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while(LeftToProcess > 0) 
            {
                queryNode = PopFromQueue();
                node = queryNode.node;

                if(!node.Leaf)
                {
                    int partitionAxis = node.partitionAxis;
                    FFloat partitionCoord = node.partitionCoordinate;

                    FVector3 tempClosestPoint = queryNode.tempClosestPoint;

                    if((tempClosestPoint[partitionAxis] - partitionCoord) < 0) 
                    {
                        // we already know we are inside negative bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside negative side
                        // assign it to negativeChild
                        PushToQueue(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.positiveChild.Count != 0
                        && tempClosestPoint[partitionAxis] <= max[partitionAxis])
                        {
                            PushToQueue(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else
                    {
                        // we already know we are inside positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside positive side
                        // assign it to positiveChild
                        PushToQueue(node.positiveChild, tempClosestPoint);

                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.negativeChild.Count != 0
                        && tempClosestPoint[partitionAxis] >= min[partitionAxis]) 
                        {
                            PushToQueue(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else 
                {
                    // LEAF

                    // testing if node bounds are inside the query interval
                    if(node.bounds.min[0] >= min[0]
                    && node.bounds.min[1] >= min[1]
                    && node.bounds.min[2] >= min[2]

                    && node.bounds.max[0] <= max[0]
                    && node.bounds.max[1] <= max[1]
                    && node.bounds.max[2] <= max[2])
                    {
                        for(int i = node.start; i < node.end; i++) 
                        {
                            resultIndices.Add(permutation[i]);
                        }
                    }
                    // node is not inside query interval, need to do test on each point separately
                    else
                    {
                        FVector3 v;
                        for (int i = node.start; i < node.end; i++) 
                        {
                            int index = permutation[i];
                            if (points[index] == null) continue;
                            v = points[index].GetPosition();

                            if(v[0] >= min[0]
                            && v[1] >= min[1]
                            && v[2] >= min[2]

                            && v[0] <= max[0]
                            && v[1] <= max[1]
                            && v[2] <= max[2]) 
                            {
                                resultIndices.Add(index);
                            }
                        }
                    }

                }
            }
        }
    }

}