#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:56
类    名: 	FVector3Int
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace ExternEngine
{
    public struct FVector3Int : IEquatable<FVector3Int> 
    {
        public class Mathf 
        {
            public static int Min(int a, int b)
            {
                return a >= b ? b : a;
            }
            public static int Max(int a, int b)
            {
                return a <= b ? b : a;
            }  
            public static FFloat Sqrt(FFloat val)
            {
                return Mathf.Sqrt(val);
            }
        }

        private static readonly FVector3Int s_Zero = new FVector3Int(0, 0, 0);
        private static readonly FVector3Int s_One = new FVector3Int(1, 1, 1);
        private static readonly FVector3Int s_Up = new FVector3Int(0, 1, 0);
        private static readonly FVector3Int s_Down = new FVector3Int(0, -1, 0);
        private static readonly FVector3Int s_Left = new FVector3Int(-1, 0, 0);
        private static readonly FVector3Int s_Right = new FVector3Int(1, 0, 0);
        private int m_X;
        private int m_Y;
        private int m_Z;

        //-------------------------------------------------
        public FVector3Int(int x, int y, int z)
        {
            this.m_X = x;
            this.m_Y = y;
            this.m_Z = z;
        }
        //-------------------------------------------------
        public int x 
        {
            get { return this.m_X; }
            set { this.m_X = value; }
        }
        //-------------------------------------------------
        public int y
        {
            get { return this.m_Y; }
            set { this.m_Y = value; }
        }
        //-------------------------------------------------
        public int z 
        {
            get { return this.m_Z; }
            set { this.m_Z = value; }
        }
        //-------------------------------------------------
        public void Set(int x, int y, int z)
        {
            this.m_X = x;
            this.m_Y = y;
            this.m_Z = z;
        }
        //-------------------------------------------------
        public int this[int index]
        {
            get
            {
                switch (index) 
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Invalid FVector3Int index addressed: {0}!",
                            (object) index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(
                            string.Format("Invalid FVector3Int index addressed: {0}!", (object) index));
                }
            }
        }
        //-------------------------------------------------
        public FFloat magnitude 
        {
            get { return Mathf.Sqrt(new FFloat(this.x * this.x + this.y * this.y + this.z * this.z)); }
        }
        //-------------------------------------------------
        public int sqrMagnitude
        {
            get { return this.x * this.x + this.y * this.y + this.z * this.z; }
        }
        //-------------------------------------------------
        public static FFloat Distance(FVector3Int a, FVector3Int b)
        {
            return (a - b).magnitude;
        }
        //-------------------------------------------------
        public static FVector3Int Min(FVector3Int lhs, FVector3Int rhs)
        {
            return new FVector3Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
        }
        //-------------------------------------------------
        public static FVector3Int Max(FVector3Int lhs, FVector3Int rhs)
        {
            return new FVector3Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
        }
        //-------------------------------------------------
        public static FVector3Int Scale(FVector3Int a, FVector3Int b)
        {
            return new FVector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        //-------------------------------------------------
        public void Scale(FVector3Int scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }
        //-------------------------------------------------
        public void Clamp(FVector3Int min, FVector3Int max)
        {
            this.x = Mathf.Max(min.x, this.x);
            this.x = Mathf.Min(max.x, this.x);
            this.y = Mathf.Max(min.y, this.y);
            this.y = Mathf.Min(max.y, this.y);
            this.z = Mathf.Max(min.z, this.z);
            this.z = Mathf.Min(max.z, this.z);
        }
        //-------------------------------------------------
        public static explicit operator FVector2Int(FVector3Int v)
        {
            return new FVector2Int(v.x, v.y);
        }
        //-------------------------------------------------
        public static FVector3Int operator +(FVector3Int a, FVector3Int b)
        {
            return new FVector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        //-------------------------------------------------
        public static FVector3Int operator -(FVector3Int a, FVector3Int b)
        {
            return new FVector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        //-------------------------------------------------
        public static FVector3Int operator *(FVector3Int a, FVector3Int b)
        {
            return new FVector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        //-------------------------------------------------
        public static FVector3Int operator *(FVector3Int a, int b)
        {
            return new FVector3Int(a.x * b, a.y * b, a.z * b);
        }
        //-------------------------------------------------
        public static bool operator ==(FVector3Int lhs, FVector3Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }
        //-------------------------------------------------
        public static bool operator !=(FVector3Int lhs, FVector3Int rhs)
        {
            return !(lhs == rhs);
        }
        //-------------------------------------------------
        public override bool Equals(object other)
        {
            if (!(other is FVector3Int))
                return false;
            return this.Equals((FVector3Int) other);
        }
        //-------------------------------------------------
        public bool Equals(FVector3Int other)
        {
            return this == other;
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            int hashCode1 = this.y.GetHashCode();
            int hashCode2 = this.z.GetHashCode();
            return this.x.GetHashCode() ^ hashCode1 << 4 ^ hashCode1 >> 28 ^ hashCode2 >> 4 ^ hashCode2 << 28;
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", this.x, this.y, this.z);
        }
        //-------------------------------------------------
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", this.x.ToString(format),
                this.y.ToString(format), this.z.ToString(format));
        }
        //-------------------------------------------------
        public static FVector3Int zero 
        {
            get { return FVector3Int.s_Zero; }
        }
        //-------------------------------------------------
        public static FVector3Int one 
        {
            get { return FVector3Int.s_One; }
        }
        //-------------------------------------------------
        public static FVector3Int up 
        {
            get { return FVector3Int.s_Up; }
        }
        //-------------------------------------------------
        public static FVector3Int down
        {
            get { return FVector3Int.s_Down; }
        }
        //-------------------------------------------------
        public static FVector3Int left {
            get { return FVector3Int.s_Left; }
        }
        //-------------------------------------------------
        public static FVector3Int right
        {
            get { return FVector3Int.s_Right; }
        }
    }
}
#endif