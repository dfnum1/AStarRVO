#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:53
类    名: 	FRect
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public struct FRect 
    {
        public FFloat x;
        public FFloat y;
        public FFloat xMax;
        public FFloat yMax;
        //-------------------------------------------------
        public FRect(FFloat x, FFloat y, FFloat width, FFloat heigh)
        {
            this.x = x;
            this.y = y;
            this.xMax = x + width;
            this.yMax = y + heigh;
        }
        //-------------------------------------------------
        public FRect(FVector2 position, FVector2 size)
        {
            this.x = position.x;
            this.y = position.y;
            this.xMax = x + size.x;
            this.yMax = y + size.y;
        }
        //-------------------------------------------------
        public static FRect CreateRect(FVector2 center, FVector2 halfSize)
        {
            return new FRect(center - halfSize, halfSize * 2);
        }
        //-------------------------------------------------
        public FFloat width
        {
            get => xMax - x;
            set => xMax = x + width;
        }
        //-------------------------------------------------
        public FFloat height
        {
            get => yMax - y;
            set => yMax = y + width;
        }
        //-------------------------------------------------
        public static FRect zero
        {
            get { return new FRect(FFloat.zero, FFloat.zero, FFloat.zero, FFloat.zero); }
        }
        //-------------------------------------------------
        public static FRect MinMaxRect(FFloat xmin, FFloat ymin, FFloat xmax, FFloat ymax)
        {
            return new FRect(xmin, ymin, xmax - xmin, ymax - ymin);
        }
        //-------------------------------------------------
        public void Set(FFloat x, FFloat y, FFloat width, FFloat height)
        {
            this.x = x;
            this.y = y;
            this.xMax = x + width;
            this.yMax = y + height;
        }
        //-------------------------------------------------
        public FVector2 position
        {
            get { return new FVector2(this.x, this.y); }
            set {
                this.x = value.x;
                this.y = value.y;
            }
        }
        //-------------------------------------------------
        public FVector2 center 
        {
            get { return new FVector2((x + xMax) / 2, (y + yMax) / 2); }
            set {
                var wid = width;
                var high = height;
                this.x = value.x - width / 2;
                this.y = value.y - height / 2;
                xMax = x + wid;
                yMax = y + high;
            }
        }
        //-------------------------------------------------
        public FVector2 min 
        {
            get { return new FVector2(this.x, this.y); }
            set {
                this.x = value.x;
                this.y = value.y;
            }
        }
        //-------------------------------------------------
        public FVector2 max
        {
            get { return new FVector2(this.xMax, this.yMax); }
            set {
                this.xMax = value.x;
                this.yMax = value.y;
            }
        }
        //-------------------------------------------------
        public FVector2 size
        {
            get { return new FVector2(xMax - x, yMax - y); }
            set {
                this.xMax = value.x + x;
                this.yMax = value.y + y;
            }
        }
        //-------------------------------------------------
        public FVector2 halfSize => new FVector2(xMax - x, yMax - y)/2;
        //-------------------------------------------------
        public bool Contains(FVector2 point)
        {
            return point.x >= this.x && point.x < this.xMax &&
                   point.y >= this.y && point.y < this.yMax;
        }
        //-------------------------------------------------
        public bool Contains(FVector3 point)
        {
            return point.x >= this.x && point.x < this.xMax &&
                   point.y >= this.y && point.y < this.yMax;
        }
        //-------------------------------------------------
        private static FRect OrderMinMax(FRect rect)
        {
            if (rect.x > rect.xMax)
            {
                FFloat xMin = rect.x;
                rect.x = rect.xMax;
                rect.xMax = xMin;
            }

            if (rect.y > rect.yMax) 
            {
                FFloat yMin = rect.y;
                rect.y = rect.yMax;
                rect.yMax = yMin;
            }

            return rect;
        }
        //-------------------------------------------------
        public bool Overlaps(FRect other)
        {
            return
                other.xMax > this.x
                && other.x < this.xMax
                && other.yMax > this.y
                && other.y < this.yMax;
        }
        //-------------------------------------------------
        public bool IntersectRay(FRay2D other,out FFloat distance)
        {
            return CollisionUtils.TestRayAABB(other.origin, other.direction, min, max,out  distance);
        }
        //-------------------------------------------------
        public bool Overlaps(FRect other, bool allowInverse)
        {
            var rect = this;
            if (allowInverse)
            {
                rect = FRect.OrderMinMax(rect);
                other = FRect.OrderMinMax(other);
            }

            return rect.Overlaps(other);
        }
        //-------------------------------------------------
        public static FVector2 NormalizedToPoint
            (
            FRect rectangle,
            FVector2 normalizedRectCoordinates)
        {
            return new FVector2(FMath.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x),
                FMath.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
        }
        //-------------------------------------------------
        public static bool operator !=(FRect lhs, FRect rhs)
        {
            return !(lhs == rhs);
        }
        //-------------------------------------------------
        public static bool operator ==(FRect lhs, FRect rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y &&
                   lhs.xMax == rhs.xMax && lhs.yMax == rhs.yMax;
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.xMax.GetHashCode() << 2 ^ this.y.GetHashCode() >> 2 ^
                   this.yMax.GetHashCode() >> 1;
        }
        //-------------------------------------------------
        public override bool Equals(object other)
        {
            if (!(other is FRect))
                return false;
            return this.Equals((FRect) other);
        }
        //-------------------------------------------------
        public bool Equals(FRect other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y) && this.xMax.Equals(other.xMax) &&
                   this.yMax.Equals(other.yMax);
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return
                $"(x:{(object) this.x:F2}, y:{(object) this.y:F2}, width:{(object) this.xMax:F2}, height:{(object) this.yMax:F2})";
        }
    }
}
#endif