#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:15:2022  16:26
类    名: 	Bounds
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace ExternEngine
{
    public struct Bounds : IEquatable<Bounds>
    {
        private FVector3 m_Center;

        private FVector3 m_Extents;
        public FVector3 center
        {
            get
            {
                return this.m_Center;
            }
            set
            {
                this.m_Center = value;
            }
        }

        public FVector3 size
        {
            get
            {
                return this.m_Extents * 2.0f;
            }
            set
            {
                this.m_Extents = value/2;
            }
        }

        /// <summary>
        ///   <para>The extents of the Bounding Box. This is always half of the size of the Bounds.</para>
        /// </summary>
        public FVector3 extents
        {
            get
            {
                return this.m_Extents;
            }
            set
            {
                this.m_Extents = value;
            }
        }

        /// <summary>
        ///   <para>The minimal point of the box. This is always equal to center-extents.</para>
        /// </summary>
        public FVector3 min
        {
            get
            {
                return this.center - this.extents;
            }
            set
            {
                this.SetMinMax(value, this.max);
            }
        }

        /// <summary>
        ///   <para>The maximal point of the box. This is always equal to center+extents.</para>
        /// </summary>
        public FVector3 max
        {
            get
            {
                return this.center + this.extents;
            }
            set
            {
                this.SetMinMax(this.min, value);
            }
        }

        /// <summary>
        ///   <para>Creates a new Bounds.</para>
        /// </summary>
        /// <param name="center">The location of the origin of the Bounds.</param>
        /// <param name="size">The dimensions of the Bounds.</param>
        public Bounds(FVector3 center, FVector3 size)
        {
            this.m_Center = center;
            this.m_Extents = size /2;
        }

        public override int GetHashCode()
        {
            return this.center.GetHashCode() ^ this.extents.GetHashCode() << 2;
        }

        public override bool Equals(object other)
        {
            bool flag = !(other is Bounds);
            return !flag && this.Equals((Bounds)other);
        }

        public bool Equals(Bounds other)
        {
            return this.center.Equals(other.center) && this.extents.Equals(other.extents);
        }

        public static bool operator ==(Bounds lhs, Bounds rhs)
        {
            return lhs.center == rhs.center && lhs.extents == rhs.extents;
        }

        public static bool operator !=(Bounds lhs, Bounds rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        ///   <para>Sets the bounds to the min and max value of the box.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetMinMax(FVector3 min, FVector3 max)
        {
            this.extents = (max - min) /2;
            this.center = min + this.extents;
        }

        /// <summary>
        ///   <para>Grows the Bounds to include the point.</para>
        /// </summary>
        /// <param name="point"></param>
        public void Encapsulate(FVector3 point)
        {
            this.SetMinMax(FVector3.Min(this.min, point), FVector3.Max(this.max, point));
        }

        /// <summary>
        ///   <para>Grow the bounds to encapsulate the bounds.</para>
        /// </summary>
        /// <param name="bounds"></param>
        public void Encapsulate(Bounds bounds)
        {
            this.Encapsulate(bounds.center - bounds.extents);
            this.Encapsulate(bounds.center + bounds.extents);
        }

        /// <summary>
        ///   <para>Expand the bounds by increasing its size by amount along each side.</para>
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(FFloat amount)
        {
            amount /=2;
            this.extents += new FVector3(amount, amount, amount);
        }

        /// <summary>
        ///   <para>Expand the bounds by increasing its size by amount along each side.</para>
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(FVector3 amount)
        {
            this.extents += amount/2;
        }

        /// <summary>
        ///   <para>Does another bounding box intersect with this bounding box?</para>
        /// </summary>
        /// <param name="bounds"></param>
        public bool Intersects(Bounds bounds)
        {
            return this.min.x <= bounds.max.x && this.max.x >= bounds.min.x && this.min.y <= bounds.max.y && this.max.y >= bounds.min.y && this.min.z <= bounds.max.z && this.max.z >= bounds.min.z;
        }

        /// <summary>
        ///   <para>Does ray intersect this bounding box?</para>
        /// </summary>
        /// <param name="ray"></param>
        public bool IntersectRay(FRay ray)
        {
            FFloat num;
            return Bounds.IntersectRayAABB(ray, this, out num);
        }

        public bool IntersectRay(FRay ray, out FFloat distance)
        {
            return Bounds.IntersectRayAABB(ray, this, out distance);
        }

        /// <summary>
        ///   <para>Returns a nicely formatted string for the bounds.</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString()
        {
            return string.Format("Center: {0}, Extents: {1}", 
                this.m_Center,
                this.m_Extents);
        }
        public bool Contains(FVector3 point)
        {
            return Bounds.Contains_Injected(ref this, ref point);
        }
        public float SqrDistance(FVector3 point)
        {
            return Bounds.SqrDistance_Injected(ref this, ref point);
        }

        private static bool IntersectRayAABB(FRay ray, Bounds bounds, out FFloat dist)
        {
            return Bounds.IntersectRayAABB_Injected(ref ray, ref bounds, out dist);
        }

        public FVector3 ClosestPoint(FVector3 point)
        {
            FVector3 result;
            Bounds.ClosestPoint_Injected(ref this, ref point, out result);
            return result;
        }

        private static bool Contains_Injected(ref Bounds _unity_self, ref FVector3 inPoint)
        {
            if (inPoint[0] < _unity_self.m_Center[0] - _unity_self.m_Extents[0])
                return false;
            if (inPoint[0] > _unity_self.m_Center[0] + _unity_self.m_Extents[0])
                return false;

            if (inPoint[1] < _unity_self.m_Center[1] - _unity_self.m_Extents[1])
                return false;
            if (inPoint[1] > _unity_self.m_Center[1] + _unity_self.m_Extents[1])
                return false;

            if (inPoint[2] < _unity_self.m_Center[2] - _unity_self.m_Extents[2])
                return false;
            if (inPoint[2] > _unity_self.m_Center[2] + _unity_self.m_Extents[2])
                return false;
            return true;
        }

        private static FFloat SqrDistance_Injected(ref Bounds _unity_self, ref FVector3 point)
        {
            FVector3 closest = point - _unity_self.center;
            FFloat sqrDistance = 0;

            for (int i = 0; i < 3; ++i)
            {
                FFloat clos = closest[i];
                FFloat ext = _unity_self.m_Extents[i];
                if (clos < -ext)
                {
                    FFloat delta = clos + ext;
                    sqrDistance += delta * delta;
                    closest[i] = -ext;
                }
                else if (clos > ext)
                {
                    FFloat delta = clos - ext;
                    sqrDistance += delta * delta;
                    closest[i] = ext;
                }
            }

            return sqrDistance;
        }

        private static bool IntersectRayAABB_Injected(ref FRay ray, ref Bounds inAABB, out FFloat dist)
        {
            dist = 0;
            FFloat tmin = FFloat.MinValue;
            FFloat tmax = FFloat.MaxValue;

            FFloat t0, t1, f;

            FVector3 p = inAABB.center - ray.origin;
            FVector3 extent = inAABB.extents;
            int i;
            for (i = 0; i < 3; i++)
            {
                // ray and plane are paralell so no valid intersection can be found
                {
                    f = 1 / ray.direction[i];
                    t0 = (p[i] + extent[i]) * f;
                    t1 = (p[i] - extent[i]) * f;
                    // FRay leaves on Right, Top, Back Side
                    if (t0 < t1)
                    {
                        if (t0 > tmin)
                            tmin = t0;

                        if (t1 < tmax)
                            tmax = t1;

                        if (tmin > tmax)
                            return false;

                        if (tmax < 0)
                            return false;
                    }
                    // FRay leaves on Left, Bottom, Front Side
                    else
                    {
                        if (t1 > tmin)
                            tmin = t1;

                        if (t0 < tmax)
                            tmax = t0;

                        if (tmin > tmax)
                            return false;

                        if (tmax < 0)
                            return false;
                    }
                }
            }

            return true;
        }

        private static void ClosestPoint_Injected(ref Bounds rkBox, ref FVector3 rkPoint, out FVector3 outPoint)
        {
            FFloat outSqrDistance = 0;
            outPoint = FVector3.zero;
            FVector3 kClosest = rkPoint - rkBox.m_Center;

            // project test point onto box
            FFloat fSqrDistance = 0;
            FFloat fDelta;

            for (int i = 0; i < 3; i++)
            {
                if (kClosest[i] < -rkBox.m_Extents[i])
                {
                    fDelta = kClosest[i] + rkBox.m_Extents[i];
                    fSqrDistance += fDelta * fDelta;
                    kClosest[i] = -rkBox.m_Extents[i];
                }
                else if (kClosest[i] > rkBox.m_Extents[i])
                {
                    fDelta = kClosest[i] - rkBox.m_Extents[i];
                    fSqrDistance += fDelta * fDelta;
                    kClosest[i] = rkBox.m_Extents[i];
                }
            }

            // Inside
            if (fSqrDistance == 0.0F)
            {
                outPoint = rkPoint;
                outSqrDistance = 0;
            }
            // Outside
            else
            {
                outPoint = kClosest + rkBox.m_Center;
                outSqrDistance = fSqrDistance;
            }
        }
    }
}
#endif