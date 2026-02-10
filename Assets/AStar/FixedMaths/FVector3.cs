#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:55
类    名: 	FVector3
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace ExternEngine
{
    [Serializable]
    public struct FVector3 : IEquatable<FVector3>
    {
        //-------------------------------------------------
        public FFloat x
        {
            get { return new FFloat(true, _x); }
            set { _x = value._val ; }
        }
        //-------------------------------------------------
        public FFloat y
        {
            get { return new FFloat(true, _y); }
            set { _y = value._val ; }
        }
        //-------------------------------------------------
        public FFloat z
        {
            get { return new FFloat(true, _z); }
            set { _z = value._val ; }
        }

        public long _x;
        public long _y;
        public long _z;


        public static readonly FVector3 zero = new FVector3(true,0, 0, 0);
        public static readonly FVector3 one = new FVector3(true,FFloat.Precision, FFloat.Precision, FFloat.Precision);
        public static readonly FVector3 negOne = new FVector3(true, -FFloat.Precision, -FFloat.Precision, -FFloat.Precision);
        public static readonly FVector3 half = new FVector3(true,FFloat.Precision / 2, FFloat.Precision / 2,FFloat.Precision / 2);
        public static readonly FVector3 infinity = new FVector3(true, FFloat.MaxValue, FFloat.MaxValue, FFloat.MaxValue);
        public static readonly FVector3 max = new FVector3(true, FFloat.MaxValue, FFloat.MaxValue, FFloat.MaxValue);
        public static readonly FVector3 min = new FVector3(true, FFloat.MinValue, FFloat.MinValue, FFloat.MinValue);

        public static readonly FVector3 forward = new FVector3(true,0, 0, FFloat.Precision);
        public static readonly FVector3 up = new FVector3(true,0, FFloat.Precision, 0);
        public static readonly FVector3 right = new FVector3(true,FFloat.Precision, 0, 0);
        public static readonly FVector3 back = new FVector3(true,0, 0, -FFloat.Precision);
        public static readonly FVector3 down = new FVector3(true,0, -FFloat.Precision, 0);
        public static readonly FVector3 left = new FVector3(true,-FFloat.Precision, 0, 0);
        public static readonly FVector3 zZero = new FVector3(true, FFloat.Precision, 0, FFloat.Precision);

        public FVector3(bool isUseRawVal,long _x, long _y, long _z)
        {
            this._x =  _x;
            this._y =  _y;
            this._z =  _z;
        }
        //-------------------------------------------------
        public FVector3(long _x, long _y, long _z)
        {
            this._x = _x * FFloat.Precision;
            this._y = _y * FFloat.Precision;
            this._z = _z * FFloat.Precision;
        }
        //-------------------------------------------------
        public FVector3(FFloat x, FFloat y, FFloat z)
        {
            this._x = x._val;
            this._y = y._val;
            this._z = z._val;
        }
        //-------------------------------------------------
        /// <summary>
        /// 直接使用浮点型 进行构造 警告!!! 仅应该在Editor模式下使用，不应该在正式代码中使用,避免出现引入浮点的不确定性
        /// </summary>
        //public FVector3(bool shouldOnlyUseInEditor, float x, float y, float z)
        //{
        //    this._x = (long)(x * FFloat.Precision);
        //    this._y = (long)(y * FFloat.Precision);
        //    this._z = (long)(z * FFloat.Precision);
        //}
        //-------------------------------------------------
        public FFloat magnitude
        {
            get
            {
                return new FFloat(true, FMath.Sqrt(_x * _x + _y * _y + _z * _z));
            }
        }
        //-------------------------------------------------
        public FFloat sqrMagnitude
        {
            get
            {
                return new FFloat(true, (_x * _x + _y * _y + _z * _z) / FFloat.Precision);
            }
        }
        //-------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long RawSqrMagnitude() => (_x * _x + _y * _y + _z * _z);

        //-------------------------------------------------
        public FVector3 abs
        {
            get { return new FVector3(true,FMath.Abs(this._x), FMath.Abs(this._y), FMath.Abs(this._z)); }
        }
        //-------------------------------------------------
        public FVector3 Normalize()
        {
            return Normalize((FFloat) 1);
        }
        //-------------------------------------------------
        public FVector3 Normalize(FFloat newMagn)
        {
            long sqr = _x * _x + _y * _y + _z * _z;
            if (sqr == 0L)
            {
                return FVector3.zero;
            }
            long b = FMath.Sqrt(sqr);
            _x = (_x * FFloat.Precision / b);
            _y = (_y * FFloat.Precision / b);
            _z = (_z * FFloat.Precision / b);
            return this;
        }
        //-------------------------------------------------
        public FVector3 normalized
        {
            get
            {
                long sqr = _x * _x + _y * _y + _z * _z;
                if (sqr == 0L)
                {
                    return FVector3.zero;
                }

                long b = FMath.Sqrt(sqr);
                if (b == 0) return FVector3.zero;
                var ret = new FVector3();
                ret._x = (_x * FFloat.Precision / b);
                ret._y = (_y * FFloat.Precision / b);
                ret._z = (_z * FFloat.Precision / b);
                return ret;
            }
        }
        //-------------------------------------------------
        public static implicit operator FVector2(FVector3 v)
        {
            return new FVector2(true, v._x, v._y);
        }
        //-------------------------------------------------
        public static implicit operator FVector3(FVector2 v)
        {
            return new FVector3(true, v._x, v._y, FFloat.zero);
        }
        //-------------------------------------------------
        public static bool operator ==(FVector3 lhs, FVector3 rhs)
        {
            return lhs._x == rhs._x && lhs._y == rhs._y && lhs._z == rhs._z;
        }
        //-------------------------------------------------
        public static bool operator !=(FVector3 lhs, FVector3 rhs)
        {
            return lhs._x != rhs._x || lhs._y != rhs._y || lhs._z != rhs._z;
        }
        //-------------------------------------------------
        public static FVector3 operator -(FVector3 lhs, FVector3 rhs)
        {
            lhs._x -= rhs._x;
            lhs._y -= rhs._y;
            lhs._z -= rhs._z;
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator -(FVector3 lhs)
        {
            lhs._x = -lhs._x;
            lhs._y = -lhs._y;
            lhs._z = -lhs._z;
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator +(FVector3 lhs, FVector3 rhs)
        {
            lhs._x += rhs._x;
            lhs._y += rhs._y;
            lhs._z += rhs._z;
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator +(FVector3 lhs, UnityEngine.Vector3 rhs)
        {
            lhs._x += (long)(rhs.x* FFloat.Precision);
            lhs._y += (long)(rhs.y * FFloat.Precision);
            lhs._z += (long)(rhs.z * FFloat.Precision);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator -(FVector3 lhs, UnityEngine.Vector3 rhs)
        {
            lhs._x -= (long)(rhs.x * FFloat.Precision);
            lhs._y -= (long)(rhs.y * FFloat.Precision);
            lhs._z -= (long)(rhs.z * FFloat.Precision);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator +(UnityEngine.Vector3 lhs, FVector3 rhs)
        {
            rhs._x += (long)(lhs.x * FFloat.Precision);
            rhs._y += (long)(lhs.y * FFloat.Precision);
            rhs._z += (long)(lhs.z * FFloat.Precision);
            return rhs;
        }
        //-------------------------------------------------
        public static FVector3 operator -(UnityEngine.Vector3 lhs, FVector3 rhs)
        {
            rhs._x = (long)(lhs.x * FFloat.Precision) - rhs._x;
            rhs._y = (long)(lhs.y * FFloat.Precision) - rhs._y;
            rhs._z = (long)(lhs.z * FFloat.Precision) - rhs._z;
            return rhs;
        }
        //-------------------------------------------------
        public static FVector3 operator *(FVector3 lhs, FVector3 rhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._x)) / FFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._y)) / FFloat.Precision);
            lhs._z = (int) (((long) (lhs._z * rhs._z)) / FFloat.Precision);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator *(FVector3 lhs, float rhs)
        {
            FFloat tempRhs = new FFloat(rhs);
            return lhs*tempRhs;
        }
        //-------------------------------------------------
        public static FVector3 operator *(float rhs,FVector3 lhs)
        {
            FFloat tempRhs = new FFloat(rhs);
            return lhs * tempRhs;
        }
        //-------------------------------------------------
        public static FVector3 operator *(FVector3 lhs, FFloat rhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._val)) / FFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._val)) / FFloat.Precision);
            lhs._z = (int) (((long) (lhs._z * rhs._val)) / FFloat.Precision);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator /(FVector3 lhs, FFloat rhs)
        {
            if (rhs._val == 0) return lhs;
            lhs._x = (int) (((long) lhs._x * FFloat.Precision) / rhs._val);
            lhs._y = (int) (((long) lhs._y * FFloat.Precision) / rhs._val);
            lhs._z = (int) (((long) lhs._z * FFloat.Precision) / rhs._val);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 operator *(FFloat rhs,FVector3 lhs)
        {
            lhs._x = (int) (((long) (lhs._x * rhs._val)) / FFloat.Precision);
            lhs._y = (int) (((long) (lhs._y * rhs._val)) / FFloat.Precision);
            lhs._z = (int) (((long) (lhs._z * rhs._val)) / FFloat.Precision);
            return lhs;
        }
        //-------------------------------------------------
        public static implicit operator FVector3(UnityEngine.Vector3 value)
        {
            return new FVector3(true, (long)(value.x* FFloat.Precision), (long)(value.y * FFloat.Precision), (long)(value.z * FFloat.Precision));
        }
        //-------------------------------------------------
        public static implicit operator UnityEngine.Vector3(FVector3 value)
        {
            return new UnityEngine.Vector3(value._x * FFloat.PrecisionFactor, value._y * FFloat.PrecisionFactor, value._z * FFloat.PrecisionFactor);
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return string.Format("({0:F2},{1:F2},{2:F2})", _x * FFloat.PrecisionFactor, _y * FFloat.PrecisionFactor,
                _z * FFloat.PrecisionFactor);
        }
        //-------------------------------------------------
        public static bool TryParse(string intPut, out FVector3 outV)
        {
            outV = FVector3.zero;

            string[] temps = intPut.Split(',');
            if (temps == null || temps.Length != 3) return false;
            float temp = 0;
            if (float.TryParse(temps[0], out temp)) outV.x = new FFloat(temp);
            else return false;
            if (float.TryParse(temps[1], out temp)) outV.y = new FFloat(temp);
            else return false;
            if (float.TryParse(temps[2], out temp)) outV.z = new FFloat(temp);
            else return false;
            return true;
        }
        //-------------------------------------------------
        public static bool TryParse(List<string> vDatas, int offset, out FVector3 outV)
        {
            outV = FVector3.zero;
            if (vDatas.Count-offset >= 3) return false;
            float temp = 0;
            if (float.TryParse(vDatas[offset], out temp)) outV.x = new FFloat(temp);
            else return false;
            if (float.TryParse(vDatas[offset+1], out temp)) outV.y = new FFloat(temp);
            else return false;
            if (float.TryParse(vDatas[offset+2], out temp)) outV.z = new FFloat(temp);
            else return false;
            return true;
        }
        //-------------------------------------------------
        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            FVector3 other = (FVector3) o;
            return this._x == other._x && this._y == other._y && this._z == other._z;
        }
        //-------------------------------------------------
        public bool Equals(FVector3 other)
        {
            return this._x == other._x && this._y == other._y && this._z == other._z;
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return (int)(this._x * 73856093 ^ this._y * 19349663 ^ this._z * 83492791);
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
                    case 2: return z;
                    default: throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
            set
            {
                switch (index)
                {
                    case 0: _x = value._val; break;
                    case 1: _y = value._val;break;
                    case 2: _z = value._val;break;
                    default: throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }
        //-------------------------------------------------
        public static FVector3 Normalize(FVector3 hs)
        {
            return hs.Normalize();
        }
        //-------------------------------------------------
        public static FFloat SqrMagnitude(FVector3 vec)
        {
            return vec.sqrMagnitude;
        }
        //-------------------------------------------------
        public static FFloat Dot(ref FVector3 lhs, ref FVector3 rhs)
        {
            var val = ((long) lhs._x) * rhs._x + ((long) lhs._y) * rhs._y + ((long) lhs._z) * rhs._z;
            return new FFloat(true, val / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FFloat Dot(FVector3 lhs, FVector3 rhs)
        {
            var val = ((long) lhs._x) * rhs._x + ((long) lhs._y) * rhs._y + ((long) lhs._z) * rhs._z;
            return new FFloat(true, val / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector3 Project(FVector3 vector, FVector3 onNormal)
        {
            FFloat num = FVector3.Dot(onNormal, onNormal);
            bool flag = num < FFloat.EPSILON;
            FVector3 result;
            if (flag)
            {
                result = FVector3.zero;
            }
            else
            {
                FFloat num2 = FVector3.Dot(vector, onNormal);
                result = new FVector3(onNormal.x * num2 / num, onNormal.y * num2 / num, onNormal.z * num2 / num);
            }
            return result;
        }
        //-------------------------------------------------
        public static FVector3 Cross(ref FVector3 lhs, ref FVector3 rhs)
        {
            return new FVector3(true,
                ((long) lhs._y * rhs._z - (long) lhs._z * rhs._y) / FFloat.Precision,
                ((long) lhs._z * rhs._x - (long) lhs._x * rhs._z) / FFloat.Precision,
                ((long) lhs._x * rhs._y - (long) lhs._y * rhs._x) / FFloat.Precision
            );
        }
        //-------------------------------------------------
        public static FVector3 Cross(FVector3 lhs, FVector3 rhs)
        {
            return new FVector3(true,
                ((long)lhs._y * rhs._z - (long)lhs._z * rhs._y) / FFloat.Precision,
                ((long)lhs._z * rhs._x - (long)lhs._x * rhs._z) / FFloat.Precision,
                ((long)lhs._x * rhs._y - (long)lhs._y * rhs._x) / FFloat.Precision
            );
        }
        //-------------------------------------------------
        public static FVector3 Lerp(FVector3 a, FVector3 b, FFloat f)
        {
            f = FMath.Clamp01(f);
            return new FVector3(true,
                (int) (((long) (b._x - a._x) * f._val) / FFloat.Precision) + a._x,
                (int) (((long) (b._y - a._y) * f._val) / FFloat.Precision) + a._y,
                (int) (((long) (b._z - a._z) * f._val) / FFloat.Precision) + a._z);
        }
        //-------------------------------------------------
        public static FVector3 Min(FVector3 lhs, FVector3 rhs)
        {
            lhs._x = FMath.Min(lhs._x, rhs._x);
            lhs._y = FMath.Min(lhs._y, rhs._y);
            lhs._z = FMath.Min(lhs._z, rhs._z);
            return lhs;
        }
        //-------------------------------------------------
        public static FVector3 Max(FVector3 lhs, FVector3 rhs)
        {
            lhs._x = FMath.Max(lhs._x, rhs._x);
            lhs._y = FMath.Max(lhs._y, rhs._y);
            lhs._z = FMath.Max(lhs._z, rhs._z);
            return lhs;
        }
    }
}
#endif