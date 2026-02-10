/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	KDQuery
作    者:	HappLI
描    述:	临近查询
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif

namespace Framework.Physic.RVO
{
    public partial class KDQuery
    {
        public void ClosestPoint(WorldKDTree tree, FVector3 queryPosition, List<int> resultIndices, List<FFloat> resultDistances = null)
        {
            Reset();

            RVONode[] points = tree.Points;
            int[] permutation = tree.Permutation;

            if (points.Length == 0) 
            {
                return;
            }

            int smallestIndex = 0;
            /// Smallest Squared Radius
            FFloat SSR = FFloat.MinValue;


            var rootNode = tree.RootNode;

            FVector3 rootClosestPoint = rootNode.bounds.ClosestPoint(queryPosition);

            PushToHeap(rootNode, rootClosestPoint, queryPosition);

            KDQueryNode queryNode = null;
            KDNode node = null;

            int partitionAxis;
            FFloat partitionCoord;

            FVector3 tempClosestPoint;

            // searching
            while(minHeap.Count > 0)
            {
                queryNode = PopFromHeap();

                if(queryNode.distance > SSR)
                    continue;

                node = queryNode.node;

                if(!node.Leaf)
                {
                    partitionAxis = node.partitionAxis;
                    partitionCoord = node.partitionCoordinate;

                    tempClosestPoint = queryNode.tempClosestPoint;

                    if((tempClosestPoint[partitionAxis] - partitionCoord) < 0)
                    {
                        // we already know we are on the side of negative bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if(node.positiveChild.Count != 0) 
                        {

                            PushToHeap(node.positiveChild, tempClosestPoint, queryPosition);
                        }

                    }
                    else
                    {
                        // we already know we are on the side of positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        PushToHeap(node.positiveChild, tempClosestPoint, queryPosition);
                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if(node.positiveChild.Count != 0)
                        {
                            PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                        }

                    }
                }
                else
                {
                    FFloat sqrDist;
                    // LEAF
                    for(int i = node.start; i < node.end; i++)
                    {
                        int index = permutation[i];
                        if (points[index] == null) continue;
                        sqrDist = (points[index].GetPosition() - queryPosition).sqrMagnitude;

                        if(sqrDist <= SSR)
                        {
                            SSR = sqrDist;
                            smallestIndex = index;
                        }
                    }

                }
            }

            resultIndices.Add(smallestIndex);

            if(resultDistances != null) 
            {
                resultDistances.Add(SSR);
            }
        }

    }

}
