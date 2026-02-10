/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	KDQuery
作    者:	HappLI
描    述:	包围盒查询
*********************************************************************/
using System.Collections.Generic;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif

namespace Framework.Physic.RVO
{
    public partial class KDQuery 
    {
        /// <summary>
        /// Search by radius method.
        /// </summary>
        /// <param name="tree">Tree to do search on</param>
        /// <param name="queryPosition">Position</param>
        /// <param name="queryRadius">Radius</param>
        /// <param name="resultIndices">Initialized list, cleared.</param>
        public void Culling(WorldKDTree tree, FVector3 queryPosition, FMatrix4x4 culling, List<int> resultIndices) 
        {
            Reset();

            RVONode[] points = tree.Points;
            int[] permutation = tree.Permutation;

            var rootNode = tree.RootNode;

            PushToQueue(rootNode, rootNode.bounds.ClosestPoint(queryPosition));

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

                        FFloat sqrDist = FVector3.SqrMagnitude(tempClosestPoint - queryPosition);

                        // testing other side
                        if(node.positiveChild.Count != 0
                        && !node.positiveChild.bounds.Culling(culling)) 
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

                        FFloat sqrDist = FVector3.SqrMagnitude(tempClosestPoint - queryPosition);

                        // testing other side
                        if(node.negativeChild.Count != 0
                        && !node.negativeChild.bounds.Culling(culling)) 
                        {
                            PushToQueue(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else
                {
                    // LEAF
                    for(int i = node.start; i < node.end; i++) 
                    {
                        int index = permutation[i];
                        if (points[index] == null) continue;
                        if (points[index].GetBound().IsInView(culling) )
                        {
                            resultIndices.Add(index);
                        }
                    }
                }
            }
        }

    }
}