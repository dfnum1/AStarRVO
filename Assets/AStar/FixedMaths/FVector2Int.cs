#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:55
类    名: 	FVector2Int
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace ExternEngine 
{
    public struct FVector2Int : IEquatable<FVector2Int>
    {
        public class Mathf
        {
            public static int Min(int a, int b)
            {
                return a >= b ? b : a;
            }

            /// <summary>
            ///   <para>Returns the largest of two or more values.</para>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="values"></param>
            public static int Max(int a, int b)
            {
                return a <= b ? b : a;
            }

            public static FFloat Sqrt(FFloat val)
            {
                return Mathf.Sqrt(val);
            }
        }

        private static readonly FVector2Int s_Zero = new FVector2Int(0, 0);
        private static readonly FVector2Int s_One = new FVector2Int(1, 1);
        private static readonly FVector2Int s_Up = new FVector2Int(0, 1);
        private static readonly FVector2Int s_Down = new FVector2Int(0, -1);
        private static readonly FVector2Int s_Left = new FVector2Int(-1, 0);
        private static readonly FVector2Int s_Right = new FVector2Int(1, 0);
        private int m_X;
        private int m_Y;

        public FVector2Int(int x, int y)
        {
            this.m_X = x;
            this.m_Y = y;
        }

        /// <summary>
        ///   <para>X component of the vector.</para>
        /// </summary>
        public int x 
        {
            get { return this.m_X; }
            set { this.m_X = value; }
        }

        /// <summary>
        ///   <para>Y component of the vector.</para>
        /// </summary>
        public int y
        {
            get { return this.m_Y; }
            set { this.m_Y = value; }
        }

        //-------------------------------------------------
        public void Set(int x, int y)
        {
            this.m_X = x;
            this.m_Y = y;
        }
        //-------------------------------------------------
        public int this[int index]
        {
            get
            {
                if (index == 0)
                    return this.x;
                if (index == 1)
                    return this.y;
                throw new IndexOutOfRangeException(string.Format("Invalid FVector2Int index addressed: {0}!",
                    (object) index));
            }
            set 
            {
                if (index != 0) 
                {
                    if (index != 1)
                        throw new IndexOutOfRangeException(string.Format("Invalid FVector2Int index addressed: {0}!",
                            (object) index));
                    this.y = value;
                }
                else
                    this.x = value;
            }
        }
        //-------------------------------------------------
        public FFloat magnitude 
        {
            get { return Mathf.Sqrt(new FFloat(this.x * this.x + this.y * this.y)); }
        }
        //-------------------------------------------------
        public int sqrMagnitude 
        {
            get { return this.x * this.x + this.y * this.y; }
        }
        //-------------------------------------------------
        public static FFloat Distance(FVector2Int a, FVector2Int b)
        {
            var num1 = (a.x - b.x);
            var num2 = (a.y - b.y);
            return Mathf.Sqrt(new FFloat(num1 * num1 + num2 * num2));
        }
        //-------------------------------------------------
        public static FVector2Int Min(FVector2Int lhs, FVector2Int rhs)
        {
            return new FVector2Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
        }

        //-------------------------------------------------
        public static FVector2Int Max(FVector2Int lhs, FVector2Int rhs)
        {
            return new FVector2Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
        }

        //-------------------------------------------------
        public static FVector2Int Scale(FVector2Int a, FVector2Int b)
        {
            return new FVector2Int(a.x * b.x, a.y * b.y);
        }
        //-------------------------------------------------
        public void Scale(FVector2Int scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }
        //-------------------------------------------------
        public void Clamp(FVector2Int min, FVector2Int max)
        {
            this.x = Mathf.Max(min.x, this.x);
            this.x = Mathf.Min(max.x, this.x);
            this.y = Mathf.Max(min.y, this.y);
            this.y = Mathf.Min(max.y, this.y);
        }
        //-------------------------------------------------

        public static explicit operator FVector3Int(FVector2Int v)
        {
            return new FVector3Int(v.x, v.y, 0);
        }
        //-------------------------------------------------
        public static FVector2Int operator +(FVector2Int a, FVector2Int b)
        {
            return new FVector2Int(a.x + b.x, a.y + b.y);
        }
        //-------------------------------------------------
        public static FVector2Int operator -(FVector2Int a, FVector2Int b)
        {
            return new FVector2Int(a.x - b.x, a.y - b.y);
        }
        //-------------------------------------------------
        public static FVector2Int operator *(FVector2Int a, FVector2Int b)
        {
            return new FVector2Int(a.x * b.x, a.y * b.y);
        }
        //-------------------------------------------------
        public static FVector2Int operator *(FVector2Int a, int b)
        {
            return new FVector2Int(a.x * b, a.y * b);
        }
        //-------------------------------------------------
        public static bool operator ==(FVector2Int lhs, FVector2Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }
        //-------------------------------------------------
        public static bool operator !=(FVector2Int lhs, FVector2Int rhs)
        {
            return !(lhs == rhs);
        }
        //-------------------------------------------------
        public override bool Equals(object other)
        {
            if (!(other is FVector2Int))
                return false;
            return this.Equals((FVector2Int) other);
        }
        //-------------------------------------------------
        public bool Equals(FVector2Int other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y);
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
        }
        //-------------------------------------------------
        public override string ToString(){
            return string.Format("({0}, {1})", (object) this.x, (object) this.y);
        }
        //-------------------------------------------------
        public static FVector2Int zero
        {
            get { return FVector2Int.s_Zero; }
        }
        //-------------------------------------------------
        public static FVector2Int one 
        {
            get { return FVector2Int.s_One; }
        }
        //-------------------------------------------------
        public static FVector2Int up 
        {
            get { return FVector2Int.s_Up; }
        }
        //-------------------------------------------------
        public static FVector2Int down 
        {
            get { return FVector2Int.s_Down; }
        }
        //-------------------------------------------------
        public static FVector2Int left 
        {
            get { return FVector2Int.s_Left; }
        }
        //-------------------------------------------------
        public static FVector2Int right 
        {
            get { return FVector2Int.s_Right; }
        }
    }
}
#endif