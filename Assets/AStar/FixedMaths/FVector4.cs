#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:15:2022  13:45
类    名: 	FVector4
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Runtime.CompilerServices;

namespace ExternEngine
{
    public struct FVector4
    {
        public const float kEpsilon = 1E-05f;

        //-------------------------------------------------
        public FFloat x
        {
            get { return new FFloat(true, _x); }
            set { _x = value._val; }
        }
        //-------------------------------------------------
        public FFloat y
        {
            get { return new FFloat(true, _y); }
            set { _y = value._val; }
        }
        //-------------------------------------------------
        public FFloat z
        {
            get { return new FFloat(true, _z); }
            set { _z = value._val; }
        }
        //-------------------------------------------------
        public FFloat w
        {
            get { return new FFloat(true, _w); }
            set { _w = value._val; }
        }


        public long _x;
        public long _y;
        public long _z;
        public long _w;

        public static readonly FVector4 zero = new FVector4(true, 0, 0, 0, 0);
        public static readonly FVector4 one = new FVector4(true, FFloat.Precision, FFloat.Precision, FFloat.Precision, FFloat.Precision);
        public static readonly FVector4 half = new FVector4(true, FFloat.Precision / 2, FFloat.Precision / 2, FFloat.Precision / 2, FFloat.Precision / 2);
        public static readonly FVector4 positiveInfinity = new FVector4(FFloat.MaxValue, FFloat.MaxValue, FFloat.MaxValue, FFloat.MaxValue);
        public static readonly FVector4 negativeInfinity = new FVector4(FFloat.MinValue, FFloat.MinValue, FFloat.MinValue, FFloat.MinValue);
        //-------------------------------------------------
        public FFloat this[int index]
        {
            get
            {
                FFloat result;
                switch (index)
                {
                    case 0:
                        result = this.x;
                        break;
                    case 1:
                        result = this.y;
                        break;
                    case 2:
                        result = this.z;
                        break;
                    case 3:
                        result = this.w;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector4 index!");
                }
                return result;
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
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FVector4 index!");
                }
            }
        }
        //-------------------------------------------------
        public FVector4 normalized
        {
            get
            {
                long sqr = _x * _x + _y * _y + _z * _z + _w * _w;
                if (sqr == 0L)
                {
                    return FVector4.zero;
                }

                var ret = new FVector4();
                long b = FMath.Sqrt(sqr);
                ret._x = (_x * FFloat.Precision / b);
                ret._y = (_y * FFloat.Precision / b);
                ret._z = (_z * FFloat.Precision / b);
                ret._w = (_w * FFloat.Precision / b);
                return ret;
            }
        }
        //-------------------------------------------------
        public FFloat magnitude
        {
            get
            {
                return new FFloat(true, FMath.Sqrt(_x * _x + _y * _y + _z * _z + _w * _w));
            }
        }
        //-------------------------------------------------
        public FFloat sqrMagnitude
        {
            get
            {
                return new FFloat(true, (_x * _x + _y * _y + _z * _z + _w * _w) / FFloat.Precision);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long RawSqrMagnitude() => (_x * _x + _y * _y + _z * _z);

        //-------------------------------------------------
        internal FVector4(bool isUseRawVal, long _x, long _y, long _z, long _w)
        {
            this._x = _x;
            this._y = _y;
            this._z = _z;
            this._w = _w;
        }
//         //-------------------------------------------------
//         public FVector4(bool shouldOnlyUseInEditor, float x, float y, float z, float w)
//         {
//             this._x = (int)(x * FFloat.Precision);
//             this._y = (int)(y * FFloat.Precision);
//             this._z = (int)(z * FFloat.Precision);
//             this._w = (int)(w * FFloat.Precision);
//         }
        //-------------------------------------------------
        public FVector4(FFloat x, FFloat y, FFloat z, FFloat w)
        {
            this._x = x._val;
            this._y = y._val;
            this._z = z._val;
            this._w = w._val;
        }
        //-------------------------------------------------
        public FVector4(FFloat x, FFloat y, FFloat z)
        {
            this._x = x._val;
            this._y = y._val;
            this._z = z._val;
            this._w = 0;
        }
        //-------------------------------------------------
        public FVector4(FFloat x, FFloat y)
        {
            this._x = x._val;
            this._y = y._val;
            this._z = 0;
            this._w = 0;
        }
        //-------------------------------------------------
        public void Set(FFloat newX, FFloat newY, FFloat newZ, FFloat newW)
        {
            this._x = x._val;
            this._y = y._val;
            this._z = z._val;
            this._w = w._val;
        }
        //-------------------------------------------------
        public static FVector4 Lerp(FVector4 a, FVector4 b, FFloat f)
        {
            f = FMath.Clamp01(f);
            return new FVector4(true,
                (int)(((long)(b._x - a._x) * f._val) / FFloat.Precision) + a._x,
                (int)(((long)(b._y - a._y) * f._val) / FFloat.Precision) + a._y,
                (int)(((long)(b._z - a._z) * f._val) / FFloat.Precision) + a._z,
                (int)(((long)(b._w - a._w) * f._val) / FFloat.Precision) + a._w);
        }
        //-------------------------------------------------
        public static FVector4 LerpUnclamped(FVector4 a, FVector4 b, FFloat f)
        {
            return new FVector4(true,
                (int)(((long)(b._x - a._x) * f._val) / FFloat.Precision) + a._x,
                (int)(((long)(b._y - a._y) * f._val) / FFloat.Precision) + a._y,
                (int)(((long)(b._z - a._z) * f._val) / FFloat.Precision) + a._z,
                (int)(((long)(b._w - a._w) * f._val) / FFloat.Precision) + a._w);
        }
        //-------------------------------------------------
        public static FVector4 MoveTowards(FVector4 current, FVector4 target, FFloat dt)
        {
            if ((target - current).sqrMagnitude <= (dt * dt))
            {
                return target;
            }

            return current + (target - current).Normalize(dt);
        }
        //-------------------------------------------------
        public static FVector4 Scale(FVector4 a, FVector4 b)
        {
            return new FVector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }
        //-------------------------------------------------
        public void Scale(FVector4 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
        }
        //-------------------------------------------------
        public override bool Equals(object other)
        {
            bool flag = !(other is FVector4);
            return !flag && this.Equals((FVector4)other);
        }
        //-------------------------------------------------
        public bool Equals(FVector4 other)
        {
            return this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w;
        }
        //-------------------------------------------------
        public FVector4 Normalize()
        {
            return Normalize((FFloat)1);
        }
        //-------------------------------------------------
        public FVector4 Normalize(FFloat newMagn)
        {
            long sqr = _x * _x + _y * _y + _z * _z + _w * _w;
            if (sqr == 0L)
            {
                return FVector3.zero;
            }
            long b = FMath.Sqrt(sqr);
            _x = (_x * FFloat.Precision / b);
            _y = (_y * FFloat.Precision / b);
            _z = (_z * FFloat.Precision / b);
            _w = (_w * FFloat.Precision / b);
            return this;
        }
        //-------------------------------------------------
        public static FFloat Dot(FVector4 lhs, FVector4 rhs)
        {
            var val = ((long)lhs._x) * rhs._x + ((long)lhs._y) * rhs._y + ((long)lhs._z) * rhs._z + ((long)lhs._w) * rhs._w;
            return new FFloat(true, val / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector4 Project(FVector4 a, FVector4 b)
        {
            //??????
            a = b * (FVector4.Dot(a, b) / FVector4.Dot(b, b));
            a._x = (a._x /FFloat.Precision);
            a._y = (a._y /FFloat.Precision);
            a._z = (a._z /FFloat.Precision);
            a._w = (a._w / FFloat.Precision);
            return a;
        }
        //-------------------------------------------------
        public static FFloat Distance(FVector4 a, FVector4 b)
        {
            return (a - b).magnitude;
        }
        //-------------------------------------------------
        public static FVector4 Min(FVector4 lhs, FVector4 rhs)
        {
            return new FVector4(FMath.Min(lhs.x, rhs.x), FMath.Min(lhs.y, rhs.y), FMath.Min(lhs.z, rhs.z), FMath.Min(lhs.w, rhs.w));
        }
        //-------------------------------------------------
        public static FVector4 Max(FVector4 lhs, FVector4 rhs)
        {
            return new FVector4(FMath.Max(lhs.x, rhs.x), FMath.Max(lhs.y, rhs.y), FMath.Max(lhs.z, rhs.z), FMath.Max(lhs.w, rhs.w));
        }
        //-------------------------------------------------
        public static FVector4 operator +(FVector4 a, FVector4 b)
        {
            return new FVector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }
        //-------------------------------------------------
        public static FVector4 operator -(FVector4 a, FVector4 b)
        {
            return new FVector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }
        //-------------------------------------------------
        public static FVector4 operator -(FVector4 a)
        {
            return new FVector4(-a.x, -a.y, -a.z, -a.w);
        }
        //-------------------------------------------------
        public static FVector4 operator *(FVector4 lhs, FFloat rhs)
        {
            lhs._x = (int)(((long)(lhs._x * rhs._val)) / FFloat.Precision);
            lhs._y = (int)(((long)(lhs._y * rhs._val)) / FFloat.Precision);
            lhs._z = (int)(((long)(lhs._z * rhs._val)) / FFloat.Precision);
            lhs._w = (int)(((long)(lhs._w * rhs._val)) / FFloat.Precision);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector4 operator *(FFloat d, FVector4 a)
        {
            a._x = (int)(((long)(a._x * d._val)) / FFloat.Precision);
            a._y = (int)(((long)(a._y * d._val)) / FFloat.Precision);
            a._z = (int)(((long)(a._z * d._val)) / FFloat.Precision);
            a._w = (int)(((long)(a._w * d._val)) / FFloat.Precision);
            return a;
        }
        //-------------------------------------------------
        public static FVector4 operator /(FVector4 a, FFloat d)
        {
            a._x = (int)(((long)a._x * FFloat.Precision) / d._val);
            a._y = (int)(((long)a._y * FFloat.Precision) / d._val);
            a._z = (int)(((long)a._z * FFloat.Precision) / d._val);
            a._w = (int)(((long)a._w * FFloat.Precision) / d._val);
            return a;
        }
        //-------------------------------------------------
        public static bool operator ==(FVector4 lhs, FVector4 rhs)
        {
            return lhs._x == rhs._x&& lhs._y == rhs._y&& lhs._z== rhs._z&& lhs._w == rhs._w;
        }
        //-------------------------------------------------
        public static bool operator !=(FVector4 lhs, FVector4 rhs)
        {
            return lhs._x != rhs._x || lhs._y != rhs._y || lhs._z != rhs._z || lhs._w != rhs._w;
        }

        //-------------------------------------------------
        public static implicit operator FVector4(FVector3 v)
        {
            return new FVector4(v.x, v.y, v.z, FFloat.zero);
        }

        public static implicit operator FVector3(FVector4 v)
        {
            return new FVector3(v.x, v.y, v.z);
        }

        public static implicit operator FVector4(FVector2 v)
        {
            return new FVector4(v.x, v.y, FFloat.zero, FFloat.zero);
        }

        public static implicit operator FVector2(FVector4 v)
        {
            return new FVector2(v.x, v.y);
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})",
                this.x* FFloat.PrecisionFactor,
                this.y* FFloat.PrecisionFactor,
                this.z* FFloat.PrecisionFactor,
                this.w* FFloat.PrecisionFactor);
        }
        //-------------------------------------------------
        public static float SqrMagnitude(FVector4 a)
        {
            return FVector4.Dot(a, a);
        }
        //-------------------------------------------------
        public float SqrMagnitude()
        {
            return new FFloat(true, (_x * _x + _y * _y + _z * _z + _w * _w) / FFloat.Precision);
        }
    }
}
#endif