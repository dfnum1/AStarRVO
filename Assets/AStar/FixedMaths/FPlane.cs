#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:15:2022  16:22
类    名: 	FPlane
作    者:	HappLI
描    述:	
*********************************************************************/

namespace ExternEngine
{
    public struct FPlane
    {
        internal const int size = 16;

        private FVector3 m_Normal;

        private FFloat m_Distance;

        /// <summary>
        ///   <para>Normal vector of the plane.</para>
        /// </summary>
        public FVector3 normal
        {
            get
            {
                return this.m_Normal;
            }
            set
            {
                this.m_Normal = value;
            }
        }

        /// <summary>
        ///   <para>The distance measured from the FPlane to the origin, along the FPlane's normal.</para>
        /// </summary>
        public FFloat distance
        {
            get
            {
                return this.m_Distance;
            }
            set
            {
                this.m_Distance = value;
            }
        }

        /// <summary>
        ///   <para>Returns a copy of the plane that faces in the opposite direction.</para>
        /// </summary>
        public FPlane flipped
        {
            get
            {
                return new FPlane(-this.m_Normal, -this.m_Distance);
            }
        }

        /// <summary>
        ///   <para>Creates a plane.</para>
        /// </summary>
        /// <param name="inNormal"></param>
        /// <param name="inPoint"></param>
        public FPlane(FVector3 inNormal, FVector3 inPoint)
        {
            this.m_Normal = FVector3.Normalize(inNormal);
            this.m_Distance = -FVector3.Dot(this.m_Normal, inPoint);
        }

        /// <summary>
        ///   <para>Creates a plane.</para>
        /// </summary>
        /// <param name="inNormal"></param>
        /// <param name="d"></param>
        public FPlane(FVector3 inNormal, FFloat d)
        {
            this.m_Normal = inNormal.Normalize();
            this.m_Distance = d;
        }

        /// <summary>
        ///   <para>Creates a plane.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public FPlane(FVector3 a, FVector3 b, FVector3 c)
        {
            this.m_Normal = FVector3.Normalize(FVector3.Cross(b - a, c - a));
            this.m_Distance = -FVector3.Dot(this.m_Normal, a);
        }

        /// <summary>
        ///   <para>Sets a plane using a point that lies within it along with a normal to orient it.</para>
        /// </summary>
        /// <param name="inNormal">The plane's normal vector.</param>
        /// <param name="inPoint">A point that lies on the plane.</param>
        public void SetNormalAndPosition(FVector3 inNormal, FVector3 inPoint)
        {
            this.m_Normal = FVector3.Normalize(inNormal);
            this.m_Distance = -FVector3.Dot(inNormal, inPoint);
        }

        /// <summary>
        ///   <para>Sets a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.</para>
        /// </summary>
        /// <param name="a">First point in clockwise order.</param>
        /// <param name="b">Second point in clockwise order.</param>
        /// <param name="c">Third point in clockwise order.</param>
        public void Set3Points(FVector3 a, FVector3 b, FVector3 c)
        {
            this.m_Normal = FVector3.Normalize(FVector3.Cross(b - a, c - a));
            this.m_Distance = -FVector3.Dot(this.m_Normal, a);
        }

        /// <summary>
        ///   <para>Makes the plane face in the opposite direction.</para>
        /// </summary>
        public void Flip()
        {
            this.m_Normal = -this.m_Normal;
            this.m_Distance = -this.m_Distance;
        }

        /// <summary>
        ///   <para>Moves the plane in space by the translation vector.</para>
        /// </summary>
        /// <param name="translation">The offset in space to move the plane with.</param>
        public void Translate(FVector3 translation)
        {
            this.m_Distance += FVector3.Dot(this.m_Normal, translation);
        }

        /// <summary>
        ///   <para>Returns a copy of the given plane that is moved in space by the given translation.</para>
        /// </summary>
        /// <param name="plane">The plane to move in space.</param>
        /// <param name="translation">The offset in space to move the plane with.</param>
        /// <returns>
        ///   <para>The translated plane.</para>
        /// </returns>
        public static FPlane Translate(FPlane plane, FVector3 translation)
        {
            return new FPlane(plane.m_Normal, plane.m_Distance += FVector3.Dot(plane.m_Normal, translation));
        }

        /// <summary>
        ///   <para>For a given point returns the closest point on the plane.</para>
        /// </summary>
        /// <param name="point">The point to project onto the plane.</param>
        /// <returns>
        ///   <para>A point on the plane that is closest to point.</para>
        /// </returns>
        public FVector3 ClosestPointOnPlane(FVector3 point)
        {
            FFloat d = FVector3.Dot(this.m_Normal, point) + this.m_Distance;
            return point - this.m_Normal * d;
        }

        /// <summary>
        ///   <para>Returns a signed distance from plane to point.</para>
        /// </summary>
        /// <param name="point"></param>
        public float GetDistanceToPoint(FVector3 point)
        {
            return FVector3.Dot(this.m_Normal, point) + this.m_Distance;
        }

        /// <summary>
        ///   <para>Is a point on the positive side of the plane?</para>
        /// </summary>
        /// <param name="point"></param>
        public bool GetSide(FVector3 point)
        {
            return FVector3.Dot(this.m_Normal, point) + this.m_Distance > 0f;
        }

        /// <summary>
        ///   <para>Are two points on the same side of the plane?</para>
        /// </summary>
        /// <param name="inPt0"></param>
        /// <param name="inPt1"></param>
        public bool SameSide(FVector3 inPt0, FVector3 inPt1)
        {
            float distanceToPoint = this.GetDistanceToPoint(inPt0);
            float distanceToPoint2 = this.GetDistanceToPoint(inPt1);
            return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
        }

        public bool Raycast(FRay ray, out FFloat enter)
        {
            FFloat num = FVector3.Dot(ray.direction, this.m_Normal);
            FFloat num2 = -FVector3.Dot(ray.origin, this.m_Normal) - this.m_Distance;
            bool result;
            if (num == 0)
            {
                enter = 0;
                result = false;
            }
            else
            {
                enter = num2 / num;
                result = (enter > 0f);
            }
            return result;
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return string.Format("(normal:({0:F1}, {1:F1}, {2:F1}), distance:{3:F1})",this.m_Normal.x,
                this.m_Normal.y,
                this.m_Normal.z,
                this.m_Distance);
        }
    }
}
#endif