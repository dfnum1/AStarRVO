/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WorldKDTree
作    者:	HappLI
描    述:	KDTree
*********************************************************************/
#define KDTREE_DUPLICATES

using System.Collections.Generic;
using System;
using UnityEngine;

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif

namespace Framework.Physic.RVO
{
    public class WorldKDTree
    {
        private struct FloatPair
        {
            private FFloat a_;
            private FFloat b_;

            /**
             * <summary>Constructs and initializes a pair of scalar
             * values.</summary>
             *
             * <param name="a">The first scalar value.</returns>
             * <param name="b">The second scalar value.</returns>
             */
            internal FloatPair(FFloat a, FFloat b)
            {
                a_ = a;
                b_ = b;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is less
             * than the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is less than the
             * second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator <(FloatPair pair1, FloatPair pair2)
            {
                return pair1.a_ < pair2.a_ || !(pair2.a_ < pair1.a_) && pair1.b_ < pair2.b_;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is less
             * than or equal to the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is less than or
             * equal to the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator <=(FloatPair pair1, FloatPair pair2)
            {
                return (pair1.a_ == pair2.a_ && pair1.b_ == pair2.b_) || pair1 < pair2;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is
             * greater than the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is greater than
             * the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator >(FloatPair pair1, FloatPair pair2)
            {
                return !(pair1 <= pair2);
            }

            /**
             * <summary>Returns true if the first pair of scalar values is
             * greater than or equal to the second pair of scalar values.
             * </summary>
             *
             * <returns>True if the first pair of scalar values is greater than
             * or equal to the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator >=(FloatPair pair1, FloatPair pair2)
            {
                return !(pair1 < pair2);
            }
        }
        List<int> m_QueryResult = new List<int>(64);
        public KDNode RootNode { get; private set; }
        internal RVONode[] Points { get { return points; } } // points on which kd-tree will build on. This array will stay unchanged when re/building kdtree!
        private RVONode[] points;

        public int[] Permutation { get { return permutation; } } // index aray, that will be permuted
        private int[] permutation;

        KDQuery m_query = null;

        private Stack<List<RVOObstacle>> m_vObsListPool = null;

        public int Count { get; private set; }

        private int maxPointsPerLeafNode = 32;

        private KDNode[] kdNodesStack;
        private int kdNodesCount = 0;

        class ObstacleTreeNode
        {
            public ObstacleTreeNode left;
            public RVO.RVOObstacle obstacle;
            public ObstacleTreeNode right;
        };
        ObstacleTreeNode m_obstacleTree;
        //------------------------------------------------------
        public WorldKDTree( int maxPointsPerLeafNode = 32)
        {
            Count = 0;
            points = new RVONode[128];
            permutation = new int[128];

            kdNodesStack = new KDNode[64];

            this.maxPointsPerLeafNode = maxPointsPerLeafNode;
        }
        //------------------------------------------------------
        internal void Set(RVONode point, int index, bool bRebuild = false)
        {
            SetCount(index + 1);
            points[index] = point;
            if (bRebuild) Rebuild();
        }
        //------------------------------------------------------
        public void Rebuild(int maxPointsPerLeafNode = -1)
        {
            for (int i = 0; i < Count; i++)
            {
                permutation[i] = i;
            }

            if (maxPointsPerLeafNode > 0)
            {
                this.maxPointsPerLeafNode = maxPointsPerLeafNode;
            }

            BuildTree();
        }
        //------------------------------------------------------
        public void SetCount(int newSize)
        {
            Count = newSize;
            // upsize internal arrays
            if (Count > points.Length)
            {
                Array.Resize(ref points, Count);
                Array.Resize(ref permutation, Count);
            }
        }
        //------------------------------------------------------
        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(points, 0, Count);
                Array.Clear(permutation, 0, Count);
            }
            Count = 0;
        }
        //------------------------------------------------------
        List<RVOObstacle> MallocList(int cap)
        {
            if (m_vObsListPool != null && m_vObsListPool.Count > 0)
                return m_vObsListPool.Pop();
            return new List<RVOObstacle>(cap);
        }
        //------------------------------------------------------
        void FreeMallocList(List<RVOObstacle> list)
        {
            if (m_vObsListPool == null) m_vObsListPool = new Stack<List<RVOObstacle>>(4);
            if (m_vObsListPool.Count >= 4)
                return;

            m_vObsListPool.Push(list);
        }
        //------------------------------------------------------
        internal void BuildObstacleTree(WorldPhysic physic, List<RVO.RVOObstacle> vObs)
        {
            m_obstacleTree = buildObstacleTreeRecursive(physic, vObs, vObs, vObs.Count);
        }
        //------------------------------------------------------
        void BuildTree()
        {
            ResetKDNodeStack();

            RootNode = GetKDNode();
            RootNode.bounds = MakeBounds();
            RootNode.start = 0;
            RootNode.end = Count;

            SplitNode(RootNode);
        }
        //------------------------------------------------------
        KDNode GetKDNode()
        {
            KDNode node = null;

            if (kdNodesCount < kdNodesStack.Length)
            {
                if (kdNodesStack[kdNodesCount] == null)
                {
                    kdNodesStack[kdNodesCount] = node = new KDNode();
                }
                else
                {
                    node = kdNodesStack[kdNodesCount];
                    node.partitionAxis = -1;
                }
            }
            else
            {
                // automatic resize of KDNode pool array
                Array.Resize(ref kdNodesStack, kdNodesStack.Length * 2);
                node = kdNodesStack[kdNodesCount] = new KDNode();
            }

            kdNodesCount++;

            return node;
        }
        //------------------------------------------------------
        void ResetKDNodeStack()
        {
            kdNodesCount = 0;
        }
        //------------------------------------------------------
        /// <summary>
        /// For calculating root node bounds
        /// </summary>
        /// <returns>Boundary of all FVector3 points</returns>
        KDBounds MakeBounds()
        {
#if USE_FIXEDMATH
            FVector3 max = FVector3.min;
            FVector3 min = FVector3.max;
#else
            FVector3 max = Vector3.one*float.MinValue;
            FVector3 min = Vector3.one * float.MaxValue;
#endif
            int even = Count & ~1; // calculate even Length

            FVector3 pi0, pi1, pevn;
            // min, max calculations
            // 3n/2 calculations instead of 2n
            for (int i0 = 0; i0 < even; i0 += 2)
            {

                int i1 = i0 + 1;
                pi0 = points[i0].GetPosition();
                pi1 = points[i1].GetPosition();
                // X Coords
                if (pi0.x > pi1.x)
                {
                    // i0 is bigger, i1 is smaller
                    if (pi1.x < min.x)
                        min.x = pi1.x;

                    if (pi0.x > max.x)
                        max.x = pi0.x;
                }
                else
                {
                    // i1 is smaller, i0 is bigger
                    if (pi0.x < min.x)
                        min.x = pi0.x;

                    if (pi1.x > max.x)
                        max.x = pi1.x;
                }

                // Y Coords
                if (pi0.y > pi1.y)
                {
                    // i0 is bigger, i1 is smaller
                    if (pi1.y < min.y)
                        min.y = pi1.y;

                    if (pi0.y > max.y)
                        max.y = pi0.y;
                }
                else
                {
                    // i1 is smaller, i0 is bigger
                    if (pi0.y < min.y)
                        min.y = pi0.y;

                    if (pi1.y > max.y)
                        max.y = pi1.y;
                }

                // Z Coords
                if (pi0.z > pi1.z)
                {
                    // i0 is bigger, i1 is smaller
                    if (pi1.z < min.z)
                        min.z = pi1.z;

                    if (pi0.z > max.z)
                        max.z = pi0.z;
                }
                else
                {
                    // i1 is smaller, i0 is bigger
                    if (pi0.z < min.z)
                        min.z = pi0.z;

                    if (pi1.z > max.z)
                        max.z = pi1.z;
                }
            }

            // if array was odd, calculate also min/max for the last element
            if (even != Count)
            {
                pevn = points[even].GetPosition();
                // X
                if (min.x > pevn.x)
                    min.x = pevn.x;

                if (max.x < pevn.x)
                    max.x = pevn.x;
                // Y
                if (min.y > pevn.y)
                    min.y = pevn.y;

                if (max.y < pevn.y)
                    max.y = pevn.y;
                // Z
                if (min.z > pevn.z)
                    min.z = pevn.z;

                if (max.z < pevn.z)
                    max.z = pevn.z;
            }

            KDBounds b = new KDBounds();
            b.min = min;
            b.max = max;

            return b;
        }
        //------------------------------------------------------
        /// <summary>
        /// Recursive splitting procedure
        /// </summary>
        /// <param name="parent">This is where root node goes</param>
        /// <param name="depth"></param>
        ///
        void SplitNode(KDNode parent)
        {
            // center of bounding box
            KDBounds parentBounds = parent.bounds;
            FVector3 parentBoundsSize = parentBounds.size;

            // Find axis where bounds are largest
            int splitAxis = 0;
            FFloat axisSize = parentBoundsSize.x;

            if (axisSize < parentBoundsSize.y)
            {
                splitAxis = 1;
                axisSize = parentBoundsSize.y;
            }

            if (axisSize < parentBoundsSize.z)
            {
                splitAxis = 2;
            }

            // Our axis min-max bounds
            FFloat boundsStart = parentBounds.min[splitAxis];
            FFloat boundsEnd = parentBounds.max[splitAxis];

            // Calculate the spliting coords
            FFloat splitPivot = CalculatePivot(parent.start, parent.end, boundsStart, boundsEnd, splitAxis);

            parent.partitionAxis = splitAxis;
            parent.partitionCoordinate = splitPivot;

            // 'Spliting' array to two subarrays
            int splittingIndex = Partition(parent.start, parent.end, splitPivot, splitAxis);

            // Negative / Left node
            FVector3 negMax = parentBounds.max;
            negMax[splitAxis] = splitPivot;

            KDNode negNode = GetKDNode();
            negNode.bounds = parentBounds;
            negNode.bounds.max = negMax;
            negNode.start = parent.start;
            negNode.end = splittingIndex;
            parent.negativeChild = negNode;

            // Positive / Right node
            FVector3 posMin = parentBounds.min;
            posMin[splitAxis] = splitPivot;

            KDNode posNode = GetKDNode();
            posNode.bounds = parentBounds;
            posNode.bounds.min = posMin;
            posNode.start = splittingIndex;
            posNode.end = parent.end;
            parent.positiveChild = posNode;

            // check if we are actually splitting it anything
            // this if check enables duplicate coordinates, but makes construction a bit slower
#if KDTREE_DUPLICATES
            if (negNode.Count != 0 && posNode.Count != 0)
            {
#endif
                // Constraint function deciding if split should be continued
                if (ContinueSplit(negNode))
                    SplitNode(negNode);


                if (ContinueSplit(posNode))
                    SplitNode(posNode);

#if KDTREE_DUPLICATES
            }
#endif
        }
        //------------------------------------------------------
        /// <summary>
        /// Sliding midpoint splitting pivot calculation
        /// 1. First splits node to two equal parts (midPoint)
        /// 2. Checks if elements are in both sides of splitted bounds
        /// 3a. If they are, just return midPoint
        /// 3b. If they are not, then points are only on left or right bound.
        /// 4. Move the splitting pivot so that it shrinks part with points completely (calculate min or max dependent) and return.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="boundsStart"></param>
        /// <param name="boundsEnd"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        FFloat CalculatePivot(int start, int end, FFloat boundsStart, FFloat boundsEnd, int axis)
        {
            //! sliding midpoint rule
            FFloat midPoint = (boundsStart + boundsEnd) / 2f;

            bool negative = false;
            bool positive = false;

            FFloat negMax = FFloat.MinValue;
            FFloat posMin = FFloat.MaxValue;

            FVector3 perp;
            // this for loop section is used both for sorted and unsorted data
            for (int i = start; i < end; i++)
            {
                perp = points[permutation[i]].GetPosition();
                if (perp[axis] < midPoint)
                    negative = true;
                else
                    positive = true;

                if (negative == true && positive == true)
                    return midPoint;
            }

            if (negative)
            {
                for (int i = start; i < end; i++)
                {
                    perp = points[permutation[i]].GetPosition();
                    if (negMax < perp[axis])
                        negMax = perp[axis];
                }
                return negMax;
            }
            else
            {
                for (int i = start; i < end; i++)
                {
                    perp = points[permutation[i]].GetPosition();
                    if (posMin > perp[axis])
                        posMin = perp[axis];
                }
                return posMin;
            }
        }
        //------------------------------------------------------
        /// <summary>
        /// Similar to Hoare partitioning algorithm (used in Quick Sort)
        /// Modification: pivot is not left-most element but is instead argument of function
        /// Calculates splitting index and partially sorts elements (swaps them until they are on correct side - depending on pivot)
        /// Complexity: O(n)
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        /// <param name="partitionPivot">Pivot that decides boundary between left and right</param>
        /// <param name="axis">Axis of this pivoting</param>
        /// <returns>
        /// Returns splitting index that subdivides array into 2 smaller arrays
        /// left = [start, pivot),
        /// right = [pivot, end)
        /// </returns>
        int Partition(int start, int end, FFloat partitionPivot, int axis)
        {
            // note: increasing right pointer is actually decreasing!
            int LP = start - 1; // left pointer (negative side)
            int RP = end;       // right pointer (positive side)

            int temp;           // temporary var for swapping permutation indexes

            while (true)
            {
                do
                {
                    // move from left to the right until "out of bounds" value is found
                    LP++;
                }
                while (LP < RP && points[permutation[LP]].GetPosition()[axis] < partitionPivot);

                do
                {
                    // move from right to the left until "out of bounds" value found
                    RP--;
                }
                while (LP < RP && points[permutation[RP]].GetPosition()[axis] >= partitionPivot);

                if (LP < RP)
                {
                    // swap
                    temp = permutation[LP];
                    permutation[LP] = permutation[RP];
                    permutation[RP] = temp;
                }
                else
                {
                    return LP;
                }
            }
        }
        //------------------------------------------------------
        /// <summary>
        /// Constraint function. You can add custom constraints here - if you have some other data/classes binded to Vector3 points
        /// Can hardcode it into
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool ContinueSplit(KDNode node)
        {
            return (node.Count > maxPointsPerLeafNode);
        }
        //------------------------------------------------------
        private ObstacleTreeNode buildObstacleTreeRecursive(WorldPhysic physic, List<RVO.RVOObstacle> simObstacles, List<RVO.RVOObstacle> obstacles, int cnt)
        {
            if (obstacles.Count == 0)
            {
                return null;
            }

            ObstacleTreeNode node = new ObstacleTreeNode();
            if (cnt <= 0) cnt = obstacles.Count;
            int optimalSplit = 0;
            int minLeft = cnt;
            int minRight = cnt;

            for (int i = 0; i < cnt; ++i)
            {
                int leftSize = 0;
                int rightSize = 0;

                RVO.RVOObstacle obstacleI1 = obstacles[i];
                RVO.RVOObstacle obstacleI2 = obstacleI1.next;

                /* Compute optimal split node. */
                for (int j = 0; j < cnt; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    RVO.RVOObstacle obstacleJ1 = obstacles[j];
                    RVO.RVOObstacle obstacleJ2 = obstacleJ1.next;

                    FFloat j1LeftOfI = RVO.RVOMath.leftOf(obstacleI1.point, obstacleI2.point, obstacleJ1.point);
                    FFloat j2LeftOfI = RVO.RVOMath.leftOf(obstacleI1.point, obstacleI2.point, obstacleJ2.point);

                    if (j1LeftOfI >= -RVO.RVOMath.RVOEPSILON && j2LeftOfI >= -RVO.RVOMath.RVOEPSILON)
                    {
                        ++leftSize;
                    }
                    else if (j1LeftOfI <= RVO.RVOMath.RVOEPSILON && j2LeftOfI <= RVO.RVOMath.RVOEPSILON)
                    {
                        ++rightSize;
                    }
                    else
                    {
                        ++leftSize;
                        ++rightSize;
                    }

                    if (new FloatPair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) >= new FloatPair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight)))
                    {
                        break;
                    }
                }

                if (new FloatPair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) < new FloatPair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight)))
                {
                    minLeft = leftSize;
                    minRight = rightSize;
                    optimalSplit = i;
                }
            }

            {
                /* Build split node. */
                List<RVO.RVOObstacle> leftObstacles = MallocList(minRight);
                leftObstacles.Clear();
                for (int n = 0; n < minLeft; ++n)
                {
                    leftObstacles.Add(null);
                }

                List<RVO.RVOObstacle> rightObstacles = MallocList(minRight);
                rightObstacles.Clear();
                for (int n = 0; n < minRight; ++n)
                {
                    rightObstacles.Add(null);
                }

                int leftCounter = 0;
                int rightCounter = 0;
                int i = optimalSplit;

                RVO.RVOObstacle obstacleI1 = obstacles[i];
                RVO.RVOObstacle obstacleI2 = obstacleI1.next;

                for (int j = 0; j < cnt; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    RVO.RVOObstacle obstacleJ1 = obstacles[j];
                    RVO.RVOObstacle obstacleJ2 = obstacleJ1.next;

                    FFloat j1LeftOfI = RVO.RVOMath.leftOf(obstacleI1.point, obstacleI2.point, obstacleJ1.point);
                    FFloat j2LeftOfI = RVO.RVOMath.leftOf(obstacleI1.point, obstacleI2.point, obstacleJ2.point);

                    if (j1LeftOfI >= -RVO.RVOMath.RVOEPSILON && j2LeftOfI >= -RVO.RVOMath.RVOEPSILON)
                    {
                        leftObstacles[leftCounter++] = obstacles[j];
                    }
                    else if (j1LeftOfI <= RVO.RVOMath.RVOEPSILON && j2LeftOfI <= RVO.RVOMath.RVOEPSILON)
                    {
                        rightObstacles[rightCounter++] = obstacles[j];
                    }
                    else
                    {
                        /* Split obstacle j. */
                        FFloat t = RVO.RVOMath.det(obstacleI2.point - obstacleI1.point, obstacleJ1.point - obstacleI1.point) / RVO.RVOMath.det(obstacleI2.point - obstacleI1.point, obstacleJ1.point - obstacleJ2.point);

                        FVector3 splitPoint = obstacleJ1.point + t * (obstacleJ2.point - obstacleJ1.point);

                        RVO.RVOObstacle newObstacle = physic.MallocObstacle();
                        newObstacle.point = splitPoint;
                        newObstacle.prev = obstacleJ1;
                        newObstacle.next = obstacleJ2;
                        newObstacle.IsConvex = true;
                        newObstacle.unitDir = obstacleJ1.unitDir;

                        newObstacle.group = obstacleJ1.group; // Copy group from J1 to new obstacle
                        newObstacle.id = obstacleJ1.id; // Copy ID from J1 to new obstacle

                        simObstacles.Add(newObstacle);

                        obstacleJ1.next = newObstacle;
                        obstacleJ2.prev = newObstacle;

                        if (j1LeftOfI > 0.0f)
                        {
                            leftObstacles[leftCounter++] = obstacleJ1;
                            rightObstacles[rightCounter++] = newObstacle;
                        }
                        else
                        {
                            rightObstacles[rightCounter++] = obstacleJ1;
                            leftObstacles[leftCounter++] = newObstacle;
                        }
                    }
                }

                node.obstacle = obstacleI1;
                node.left = buildObstacleTreeRecursive(physic, simObstacles, leftObstacles, -1);
                FreeMallocList(leftObstacles);
                node.right = buildObstacleTreeRecursive(physic, simObstacles, rightObstacles, -1);
                FreeMallocList(rightObstacles);
                return node;
            }
        }
        //------------------------------------------------------
        internal void computeObstacleNeighbors(WorldPhysic.ScanerObser agent, FFloat rangeSq)
        {
            queryObstacleTreeRecursive(agent, rangeSq, m_obstacleTree);
        }
        //------------------------------------------------------
        void queryObstacleTreeRecursive(WorldPhysic.ScanerObser agent, FFloat rangeSq, ObstacleTreeNode node)
        {
            if (node != null)
            {
                RVO.RVOObstacle obstacle1 = node.obstacle;
                if (agent.obstacleInsideSets.Contains(obstacle1.group))
                    return;

                RVO.RVOObstacle obstacle2 = obstacle1.next;

                FFloat agentLeftOfLine = RVO.RVOMath.leftOf(obstacle1.point, obstacle2.point, agent.GetPosition());

                queryObstacleTreeRecursive(agent, rangeSq, agentLeftOfLine >= 0.0f ? node.left : node.right);

                FFloat distSqLine = RVO.RVOMath.sqr(agentLeftOfLine) / RVO.RVOMath.absSq(obstacle2.point - obstacle1.point);

                if (distSqLine < rangeSq)
                {
                    if (agentLeftOfLine < 0.0f)
                    {
                        /*
                         * Try obstacle at this node only if agent is on right side of
                         * obstacle (and can see obstacle).
                         */
                        agent.insertObstacleNeighbor(node.obstacle, rangeSq);
                    }
                    else
                    {
                        if (node.obstacle.IsPointInPolygon(agent.GetPosition()))
                        {
                            agent.obstacleInsideSets.Add(node.obstacle.id);
                            agent.insertObstacleNeighbor(node.obstacle, rangeSq);
                        }
                    }

                    /* Try other side of line. */
                    queryObstacleTreeRecursive(agent, rangeSq, agentLeftOfLine >= 0.0f ? node.right : node.left);
                }
            }
        }
        //------------------------------------------------------
        public void QueryClosestPoint(FVector3 queryPosition, List<int> resultIndices, List<FFloat> resultDistances = null)
        {
            if (m_query == null) m_query = new KDQuery();
            m_query.ClosestPoint(this, queryPosition, resultIndices, resultDistances);
        }
        //------------------------------------------------------
        public void QueryInterval(FVector3 min, FVector3 max, List<int> resultIndices)
        {
            if (m_query == null) m_query = new KDQuery();
            m_query.Interval(this, min, max, resultIndices);
        }
        //------------------------------------------------------
        public void QueryKNearest(FVector3 queryPosition, int k, List<int> resultIndices, List<FFloat> resultDistances = null)
        {
            if (m_query == null) m_query = new KDQuery();
            m_query.KNearest(this, queryPosition, k, resultIndices, resultDistances);
        }
        //------------------------------------------------------
        public void QueryRadius(FVector3 queryPosition, FFloat queryRadius, List<int> resultIndices, bool bPowerRange = true)
        {
            if (queryRadius <= 0) return;
            if (m_query == null) m_query = new KDQuery();
            m_query.Radius(this, queryPosition, queryRadius, resultIndices, bPowerRange);
        }
        //------------------------------------------------------
        public void QueryCulling(FVector3 queryPosition, Matrix4x4 culling, List<int> resultIndices)
        {
            if (m_query == null) m_query = new KDQuery();
            m_query.Culling(this, queryPosition, culling, resultIndices);
        }
        //------------------------------------------------------
        internal void QueryRadius(FVector3 queryPosition, FFloat queryRadius, List<RVONode> resultIndices, RVONode ingoreNode = null)
        {
            if (queryRadius <= 0) return;
            if (m_query == null) m_query = new KDQuery();
            m_QueryResult.Clear();
            m_query.Radius(this, queryPosition, queryRadius, m_QueryResult);
            for (int i = 0; i < m_QueryResult.Count; ++i)
            {
                if (points[m_QueryResult[i]] == ingoreNode) continue;
                resultIndices.Add(points[m_QueryResult[i]]);
            }
            m_QueryResult.Clear();
        }
        //------------------------------------------------------
        public void DrawDebug()
        {
            if (m_query == null) return;
            m_query.DrawLastQuery();
        }
    }
}