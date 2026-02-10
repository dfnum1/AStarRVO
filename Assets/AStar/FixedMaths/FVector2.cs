#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:55
类    名: 	FVector2
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Runtime.CompilerServices;

namespace ExternEngine
{
    [Serializable]
    public struct FVector2 
    {
        public FFloat x
        {
            get { return new FFloat(true, _x); }
            set { _x = value._val; }
        }

        public FFloat y 
        {
            get { return new FFloat(true, _y); }
            set { _y = value._val; }
        }

        public long _x;
        public long _y;
        public static readonly FVector2 zero = new FVector2(true, 0, 0);
        public static readonly FVector2 one = new FVector2(true, FFloat.Precision, FFloat.Precision);
        public static readonly FVector2 half = new FVector2(true, FFloat.Precision / 2, FFloat.Precision / 2);
        public static readonly FVector2 up = new FVector2(true, 0, FFloat.Precision);
        public static readonly FVector2 down = new FVector2(true, 0, -FFloat.Precision);
        public static readonly FVector2 right = new FVector2(true, FFloat.Precision, 0);
        public static readonly FVector2 left = new FVector2(true, -FFloat.Precision, 0);

        private static readonly int[] Rotations = new int[] {
            1,
            0,
            0,
            1,
            0,
            1,
            -1,
            0,
            -1,
            0,
            0,
            -1,
            0,
            -1,
            1,
            0
        };

        //-------------------------------------------------
        /// <summary>
        /// 顺时针旋转90Deg 参数
        /// </summary>
        public const int ROTATE_CW_90 = 1;

        public const int ROTATE_CW_180 = 2;
        public const int ROTATE_CW_270 = 3;
        public const int ROTATE_CW_360 = 4;


        //-------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FVector2(bool isUseRawVal, long x, long y)
        {
            this._x = x;
            this._y = y;
        }
        //-------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FVector2(FFloat x, FFloat y)
        {
            this._x = x._val;
            this._y = y._val;
        }
        //-------------------------------------------------
        public FVector2(long x, long y)
        {
            this._x = x * FFloat.Precision;
            this._y = y * FFloat.Precision;
        }

        //-------------------------------------------------
        /// <summary>
        /// clockwise 顺时针旋转  
        /// 1表示顺时针旋转 90 degree
        /// 2表示顺时针旋转 180 degree
        /// </summary>
        public static FVector2 Rotate(FVector2 v, int r)
        {
            r %= 4;
            return new FVector2(true,
                v._x * FVector2.Rotations[r * 4] + v._y * FVector2.Rotations[r * 4 + 1],
                v._x * FVector2.Rotations[r * 4 + 2] + v._y * FVector2.Rotations[r * 4 + 3]);
        }
        //-------------------------------------------------
        public FVector2 Rotate( FFloat deg)
        {
            var rad = FMath.Deg2Rad * deg;
            FFloat cos, sin;
            FMath.SinCos(out sin, out cos, rad);
            return new FVector2(x * cos - y * sin, x * sin + y * cos);
        }
        //-------------------------------------------------
        public static FVector2 Min(FVector2 a, FVector2 b)
        {
            return new FVector2(true, FMath.Min(a._x, b._x), FMath.Min(a._y, b._y));
        }
        //-------------------------------------------------
        public static FVector2 Max(FVector2 a, FVector2 b)
        {
            return new FVector2(true, FMath.Max(a._x, b._x), FMath.Max(a._y, b._y));
        }
        //-------------------------------------------------
        public void Min(ref FVector2 r)
        {
            this._x = FMath.Min(this._x, r._x);
            this._y = FMath.Min(this._y, r._y);
        }
        //-------------------------------------------------
        public void Max(ref FVector2 r)
        {
            this._x = FMath.Max(this._x, r._x);
            this._y = FMath.Max(this._y, r._y);
        }
        //-------------------------------------------------
        public void Normalize()
        {
            long sqr = _x * _x + _y * _y;
            if (sqr == 0L) 
            {
                return;
            }

            long b = (long) FMath.Sqrt(sqr);
            this._x = (_x * FFloat.Precision / b);
            this._y = (_y * FFloat.Precision / b);
        }
        //-------------------------------------------------
        public FFloat sqrMagnitude 
        {
            get 
            {
                return new FFloat(true, (_x * _x + _y * _y) / FFloat.Precision);
            }
        }
        //-------------------------------------------------
        public long rawSqrMagnitude 
        {
            get
            {
                return _x * _x + _y * _y;
            }
        }
        //-------------------------------------------------
        public FFloat magnitude
        {
            get 
            {
                return new FFloat(true, FMath.Sqrt(_x * _x + _y * _y));
            }
        }
        //-------------------------------------------------
        public FVector2 normalized 
        {
            get {
                FVector2 result = new FVector2(true, this._x, this._y);
                result.Normalize();
                return result;
            }
        }
        //-------------------------------------------------
        public static implicit operator FVector2(UnityEngine.Vector2 value)
        {
            return new FVector2(true, (long)(value.x * FFloat.Precision), (long)(value.y * FFloat.Precision));
        }
        //-------------------------------------------------
        public static implicit operator UnityEngine.Vector2(FVector2 value)
        {
            return new UnityEngine.Vector3(value._x * FFloat.PrecisionFactor, value._y * FFloat.PrecisionFactor);
        }
        //-------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 operator +(FVector2 a, FVector2 b)
        {
            return new FVector2(true, a._x + b._x, a._y + b._y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 operator -(FVector2 a, FVector2 b)
        {
            return new FVector2(true, a._x - b._x, a._y - b._y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 operator -(FVector2 lhs)
        {
            lhs._x = -lhs._x;
            lhs._y = -lhs._y;
            return lhs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 operator *(FFloat rhs, FVector2 lhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._val)) / FFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._val)) / FFloat.Precision);
            return lhs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FVector2 operator *(FVector2 lhs, FFloat rhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._val)) / FFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._val)) / FFloat.Precision);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector2 operator *(int rhs, FVector2 lhs)
        {
            lhs._x = lhs._x * rhs;
            lhs._y = lhs._y * rhs;
            return lhs;
        }
        //-------------------------------------------------
        public static FVector2 operator *(FVector2 lhs, int rhs)
        {
            lhs._x = lhs._x * rhs;
            lhs._y = lhs._y * rhs;
            return lhs;
        }
        //-------------------------------------------------
        public static FVector2 operator /(FVector2 lhs, FFloat rhs)
        {
            lhs._x = (int) (((long) lhs._x * FFloat.Precision) / rhs._val);
            lhs._y = (int) (((long) lhs._y * FFloat.Precision) / rhs._val);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector2 operator /(FVector2 lhs, int rhs)
        {
            lhs._x = lhs._x / rhs;
            lhs._y = lhs._y / rhs;
            return lhs;
        }
        //-------------------------------------------------
        public static bool operator ==(FVector2 a, FVector2 b)
        {
            return a._x == b._x && a._y == b._y;
        }
        //-------------------------------------------------
        public static bool operator !=(FVector2 a, FVector2 b)
        {
            return a._x != b._x || a._y != b._y;
        }

        //-------------------------------------------------
        public static implicit operator FVector2(FVector3 v)
        {
            return new FVector2(true, v._x, v._y);
        }

        //-------------------------------------------------
        public static implicit operator FVector3(FVector2 v)
        {
            return new FVector3(true, v._x, v._y, 0);
        }
        //-------------------------------------------------
        public override bool Equals(object o)
        {
            if (o == null) 
            {
                return false;
            }

            FVector2 vInt = (FVector2) o;
            return this._x == vInt._x && this._y == vInt._y;
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return unchecked((int)(this._x * 49157 + this._y * 98317));
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return string.Format("({0},{1})", _x * FFloat.PrecisionFactor, _y * FFloat.PrecisionFactor);
        }
        //-------------------------------------------------
        public FVector3 ToInt3 
        {
            get { return new FVector3(true, _x, 0, _y); }
        }
        //-------------------------------------------------
        public FFloat this[int index]
        {
            get 
            {
                switch (index) 
                {
                    case 0: return x;
                    case 1: return y;
                    default: throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }

            set 
            {
                switch (index) 
                {
                    case 0:
                        _x = value._val;
                        break;
                    case 1:
                        _y = value._val;
                        break;
                    default: throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }

        //-------------------------------------------------
        public static FFloat Dot(FVector2 u, FVector2 v)
        {
            return new FFloat(true, ((long) u._x * v._x + (long) u._y * v._y) / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FFloat Cross(FVector2 a, FVector2 b)
        {
            return new FFloat(true, ((long) a._x * (long) b._y - (long) a._y * (long) b._x) / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector2 Lerp(FVector2 a, FVector2 b, FFloat f)
        {
            f = FMath.Clamp01(f);
            return new FVector2(true,
                (int) (((long) (b._x - a._x) * f._val) / FFloat.Precision) + a._x,
                (int) (((long) (b._y - a._y) * f._val) / FFloat.Precision) + a._y);
        }
    }
}
#endif