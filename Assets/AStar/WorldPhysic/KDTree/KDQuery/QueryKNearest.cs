/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	KDQuery
作    者:	HappLI
描    述:	KD相邻查询
*********************************************************************/
#define KDTREE_VISUAL_DEBUG

using System.Collections.Generic;
using UnityEngine;
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
        SortedList<int, KSmallestHeap<int>> _heaps = new SortedList<int, KSmallestHeap<int>>();
        /// <summary>
        /// Returns indices to k closest points, and optionaly can return distances
        /// </summary>
        /// <param name="tree">Tree to do search on</param>
        /// <param name="queryPosition">Position</param>
        /// <param name="k">Max number of points</param>
        /// <param name="resultIndices">List where resulting indices will be stored</param>
        /// <param name="resultDistances">Optional list where resulting distances will be stored</param>
        public void KNearest(WorldKDTree tree, Vector3 queryPosition, int k, List<int> resultIndices, List<FFloat> resultDistances = null)
        {
            // pooled heap arrays
            KSmallestHeap<int> kHeap;

            _heaps.TryGetValue(k, out kHeap);

            if(kHeap == null)
            {
                kHeap = new KSmallestHeap<int>(k);
                _heaps.Add(k, kHeap);
            }

            kHeap.Clear();
            Reset();

            RVONode[] points = tree.Points;
            int[] permutation = tree.Permutation;

            ///Biggest Smallest Squared Radius
            FFloat BSSR = FFloat.MinValue;// Single.PositiveInfinity;

            var rootNode = tree.RootNode;

            Vector3 rootClosestPoint = rootNode.bounds.ClosestPoint(queryPosition);

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

                if(queryNode.distance > BSSR)
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
                        sqrDist = FVector3.SqrMagnitude(points[index].GetPosition() - queryPosition);

                        if(sqrDist <= BSSR)
                        {
                            kHeap.PushObj(index, sqrDist);

                            if(kHeap.Full)
                            {
                                BSSR = kHeap.HeadValue;
                            }
                        }
                    }
                }
            }

            kHeap.FlushResult(resultIndices, resultDistances);
        }

    }

}