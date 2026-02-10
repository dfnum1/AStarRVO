
/********************************************************************
生成日期:	3:10:2022  11:57
类    名: 	FFloat
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Runtime.CompilerServices;

namespace ExternEngine
{
#if USE_FIXEDMATH
    [Serializable]
    public struct FFloat : IEquatable<FFloat>, IComparable<FFloat>
    {
        public const long Precision = 10000;
        public const long HalfPrecision = Precision / 2;
        public const float PrecisionFactor = 0.0001f;
        public const float PrecisionFactorRound = 0.00005f;

        internal static long DoubleToLong(double f)
        {
            return (long)((f + PrecisionFactorRound) * Precision);
        }
        internal static long FloatToLong(float f)
        {
            return (long)((f + PrecisionFactorRound) * Precision);
        }

        public long _val;

        public static readonly FFloat zero = new FFloat( true,0L);
        public static readonly FFloat one = new FFloat(true, FFloat.Precision);
        public static readonly FFloat negOne = new FFloat(true, -FFloat.Precision);
        public static readonly FFloat half = new FFloat(true, FFloat.Precision / 2L);
        public static readonly FFloat FLT_MAX = new FFloat(true, long.MaxValue);
        public static readonly FFloat FLT_MIN = new FFloat(true, long.MinValue);
        public static readonly FFloat EPSILON = new FFloat(true, 1L);
        public static readonly FFloat INTERVAL_EPSI_LON = new FFloat(true, 1L);
        public static readonly FFloat FRAMEFPS30 = new FFloat(0.033333f);
        public static readonly FFloat FRAMEFPS45 = new FFloat(0.022222f);
        public static readonly FFloat FRAMEFPS60 = new FFloat(0.016666f);

        public static readonly FFloat MaxValue = new FFloat(true, long.MaxValue);
        public static readonly FFloat MinValue = new FFloat(true, long.MinValue);

        //-------------------------------------------------
        public FFloat(bool isUseRawVal,long rawVal)
        {
            this._val =  rawVal;
        }
        //-------------------------------------------------
        public FFloat(int val)
        {
            this._val = val * FFloat.Precision;
        }
        //-------------------------------------------------
        public FFloat(float val)
        {
            this._val = FloatToLong(val);
        }
        //-------------------------------------------------
        public FFloat(long val)
        {
            this._val = val * FFloat.Precision;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 直接使用浮点型 进行构造 警告!!! 仅应该在Editor模式下使用，不应该在正式代码中使用,避免出现引入浮点的不确定性
        /// </summary>
        public FFloat(bool shouldOnlyUseInEditor,float val)
        {
            this._val = FloatToLong(val);
        }
#endif

    #region override operator 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FFloat a, FFloat b)
        {
            return a._val < b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FFloat a, FFloat b)
        {
            return a._val > b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FFloat a, FFloat b)
        {
            return a._val <= b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FFloat a, FFloat b)
        {
            return a._val >= b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FFloat a, FFloat b)
        {
            return a._val == b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FFloat a, FFloat b)
        {
            return a._val != b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator +(FFloat a, FFloat b)
        {
            return new FFloat(true, a._val + b._val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator -(FFloat a, FFloat b)
        {
            return new FFloat(true, a._val - b._val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator *(FFloat a, FFloat b)
        {
            long val = (long) (a._val) * b._val;
            return new FFloat(true, val/FFloat.Precision);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator /(FFloat a, FFloat b)
        {
            if (b._val == 0) return a;
            long val = (long) (a._val * FFloat.Precision) / b._val;
            return new FFloat( true,val);
        }
        public static FFloat operator %(FFloat a, FFloat b)
        {
            if (b._val == 0) return a;
            long val = (long)a._val % b._val;
            return new FFloat(true, val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator -(FFloat a)
        {
            return new FFloat(true, -a._val);
        }

    #region adapt for int
        public static FFloat operator +(FFloat a, int b)
        {
            return new FFloat(true, a._val + b * Precision);
        }
        //-------------------------------------------------
        public static FFloat operator -(FFloat a, int b)
        {
            return new FFloat(true, a._val - b * Precision);
        }
        //-------------------------------------------------
        public static FFloat operator *(FFloat a, int b)
        {
            return new FFloat(true, (a._val * b));
        }
        //-------------------------------------------------
        public static FFloat operator /(FFloat a, int b)
        {
            return new FFloat(true, (a._val) / b);
        }
        //-------------------------------------------------
        public static FFloat operator +(int a, FFloat b)
        {
            return new FFloat(true, b._val + a * Precision);
        }
        //-------------------------------------------------
        public static FFloat operator -(int a, FFloat b)
        {
            return new FFloat(true, a * Precision - b._val);
        }
        //-------------------------------------------------
        public static FFloat operator *(int a, FFloat b)
        {
            return new FFloat(true, (b._val * a));
        }
        //-------------------------------------------------
        public static FFloat operator /(int a, FFloat b)
        {
            return new FFloat(true, ((long) (a * Precision * Precision) / b._val));
        }
        //-------------------------------------------------
        public static bool operator <(FFloat a, int b)
        {
            return a._val < (b * Precision);
        }

        public static bool operator >(FFloat a, int b)
        {
            return a._val > (b * Precision);
        }

        public static bool operator <=(FFloat a, int b){
            return a._val <= (b * Precision);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FFloat a, int b){
            return a._val >= (b * Precision);
        }

        public static bool operator ==(FFloat a, int b){
            return a._val == (b * Precision);
        }

        public static bool operator !=(FFloat a, int b){
            return a._val != (b * Precision);
        }


        public static bool operator <(int a, FFloat b){
            return (a * Precision) < (b._val);
        }

        public static bool operator >(int a, FFloat b){
            return (a * Precision) > (b._val);
        }

        public static bool operator <=(int a, FFloat b){
            return (a * Precision) <= (b._val);
        }

        public static bool operator >=(int a, FFloat b){
            return (a * Precision) >= (b._val);
        }

        public static bool operator ==(int a, FFloat b){
            return (a * Precision) == (b._val);
        }

        public static bool operator !=(int a, FFloat b){
            return (a * Precision) != (b._val);
        }

    #endregion

    #region adapt for float
        public static FFloat operator +(FFloat a, float b)
        {
            return new FFloat(true, a._val + FloatToLong(b));
        }
        //-------------------------------------------------
        public static FFloat operator -(FFloat a, float b)
        {
            return new FFloat(true, a._val - FloatToLong(b));
        }
        //-------------------------------------------------
        public static FFloat operator *(FFloat a, float b)
        {
            return new FFloat(true, (a._val * FloatToLong(b) / Precision));
        }
        //-------------------------------------------------
        public static FFloat operator /(FFloat a, float b)
        {
            long temp = FloatToLong(b);
            if (temp == 0) return FFloat.zero;
            return new FFloat(true, (long)((a._val) / temp * Precision));
        }
        //-------------------------------------------------
        public static FFloat operator +(float a, FFloat b)
        {
            return new FFloat(true, b._val + FloatToLong(a));
        }
        //-------------------------------------------------
        public static FFloat operator -(float a, FFloat b)
        {
            return new FFloat(true, FloatToLong(a) - b._val);
        }
        //-------------------------------------------------
        public static FFloat operator *(float a, FFloat b)
        {
            return new FFloat(true, (long)(b._val * FloatToLong(a) / Precision));
        }
        //-------------------------------------------------
        public static FFloat operator /(float a, FFloat b)
        {
            return new FFloat(true, ((long)(FloatToLong(a) * Precision) / b._val));
        }
        //-------------------------------------------------
        public static bool operator <(FFloat a, float b)
        {
            return a._val < FloatToLong(b);
        }

        public static bool operator >(FFloat a, float b)
        {
            return a._val > FloatToLong(b);
        }

        public static bool operator <=(FFloat a, float b)
        {
            return a._val <= FloatToLong(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FFloat a, float b)
        {
            return a._val >= FloatToLong(b);
        }

        public static bool operator ==(FFloat a, float b)
        {
            return a._val == FloatToLong(b);
        }

        public static bool operator !=(FFloat a, float b)
        {
            return a._val != FloatToLong(b);
        }


        public static bool operator <(float a, FFloat b)
        {
            return FloatToLong(a) < (b._val);
        }

        public static bool operator >(float a, FFloat b)
        {
            return FloatToLong(a) > (b._val);
        }

        public static bool operator <=(float a, FFloat b)
        {
            return FloatToLong(a) <= (b._val);
        }

        public static bool operator >=(float a, FFloat b)
        {
            return FloatToLong(a) >= (b._val);
        }

        public static bool operator ==(float a, FFloat b)
        {
            return FloatToLong(a) == (b._val);
        }

        public static bool operator !=(float a, FFloat b)
        {
            return FloatToLong(a) != (b._val);
        }

    #endregion

    #region adapt for long
        public static FFloat operator +(FFloat a, long b)
        {
            return new FFloat(true, a._val + b * Precision);
        }
        //-------------------------------------------------
        public static FFloat operator -(FFloat a, long b)
        {
            return new FFloat(true, a._val - b * Precision);
        }
        //-------------------------------------------------
        public static FFloat operator *(FFloat a, long b)
        {
            return new FFloat(true, (a._val * b));
        }
        //-------------------------------------------------
        public static FFloat operator /(FFloat a, long b)
        {
            return new FFloat(true, (a._val) / b);
        }
        //-------------------------------------------------
        public static FFloat operator +(long a, FFloat b)
        {
            return new FFloat(true, b._val + a * Precision);
        }
        //-------------------------------------------------
        public static FFloat operator -(long a, FFloat b)
        {
            return new FFloat(true, a * Precision - b._val);
        }
        //-------------------------------------------------
        public static FFloat operator *(long a, FFloat b)
        {
            return new FFloat(true, (b._val * a));
        }
        //-------------------------------------------------
        public static FFloat operator /(long a, FFloat b)
        {
            return new FFloat(true, ((long) (a * Precision * Precision) / b._val));
        }
        //-------------------------------------------------
        public static bool operator <(FFloat a, long b)
        {
            return a._val < (b * Precision);
        }
        //-------------------------------------------------
        public static bool operator >(FFloat a, long b)
        {
            return a._val > (b * Precision);
        }
        //-------------------------------------------------
        public static bool operator <=(FFloat a, long b)
        {
            return a._val <= (b * Precision);
        }
        //-------------------------------------------------
        public static bool operator >=(FFloat a, long b)
        {
            return a._val >= (b * Precision);
        }
        //-------------------------------------------------
        public static bool operator ==(FFloat a, long b)
        {
            return a._val == (b * Precision);
        }
        //-------------------------------------------------
        public static bool operator !=(FFloat a, long b)
        {
            return a._val != (b * Precision);
        }
        //-------------------------------------------------
        public static bool operator <(long a, FFloat b)
        {
            return (a * Precision) < (b._val);
        }
        //-------------------------------------------------
        public static bool operator >(long a, FFloat b)
        {
            return (a * Precision) > (b._val);
        }
        //-------------------------------------------------
        public static bool operator <=(long a, FFloat b)
        {
            return (a * Precision) <= (b._val);
        }
        //-------------------------------------------------
        public static bool operator >=(long a, FFloat b)
        {
            return (a * Precision) >= (b._val);
        }
        //-------------------------------------------------
        public static bool operator ==(long a, FFloat b)
        {
            return (a * Precision) == (b._val);
        }
        //-------------------------------------------------
        public static bool operator !=(long a, FFloat b)
        {
            return (a * Precision) != (b._val);
        }
    #endregion

    #endregion

    #region override object func 
        public override bool Equals(object obj)
        {
            return obj is FFloat && ((FFloat) obj)._val == _val;
        }
        //-------------------------------------------------
        public bool Equals(FFloat other)
        {
            return _val == other._val;
        }
        //-------------------------------------------------
        public int CompareTo(FFloat other)
        {
            return _val.CompareTo(other._val);
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return (int)_val;
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return (_val * FFloat.PrecisionFactor).ToString("F3");
        }
        public static bool TryParse(string intPut, out FFloat outV)
        {
            outV._val = 0;
            float temp;
            if (float.TryParse(intPut, out temp))
            {
                outV._val = FloatToLong(temp);
                return true;
            }
            return false;
        }

    #endregion

    #region override type convert 
        public static implicit operator FFloat(short value){
            return new FFloat(true, value * Precision);
        }

        public static explicit operator short(FFloat value){
            return (short)(value._val / Precision);
        }
        
        public static implicit operator FFloat(int value){
            return new FFloat(true, value * Precision);
        }

        public static implicit operator int(FFloat value){
            return (int)(value._val / Precision);
        }

//         public static explicit operator FFloat(long value){
//             return new FFloat(true, value * Precision);
//         }

//         public static implicit operator long(FFloat value){
//             return value._val / Precision;
//         }


        public static implicit operator FFloat(float value){
            return new FFloat(true, FloatToLong(value));
        }

        public static implicit operator float(FFloat value){
            return (float) value._val *FFloat.PrecisionFactor;
        }

        public static implicit operator FFloat(double value){
            return new FFloat(true, DoubleToLong(value));
        }

        public static implicit operator double(FFloat value){
            return (double) value._val *FFloat.PrecisionFactor;
        }

    #endregion

        //-------------------------------------------------
        public int ToInt()
        {
            return (int)(_val / FFloat.Precision);
        }
        //-------------------------------------------------
        public long ToLong()
        {
            return _val / FFloat.Precision;
        }
        //-------------------------------------------------
        public float ToFloat()
        {
            return _val *FFloat.PrecisionFactor;
        }
        //-------------------------------------------------
        public double ToDouble()
        {
            return _val *FFloat.PrecisionFactor;
        }
        //-------------------------------------------------
        public int Floor()
        {
            var x = this._val;
            if (x > 0)
            {
                x /= FFloat.Precision;
            }
            else 
            {
                if (x % FFloat.Precision == 0) 
                {
                    x /= FFloat.Precision;
                }
                else
                {
                    x = x / FFloat.Precision - 1;
                }
            }

            return (int)x;
        }
        //-------------------------------------------------
        public int Ceil()
        {
            var x = this._val;
            if (x < 0) 
            {
                x /= FFloat.Precision;
            }
            else 
            {
                if (x % FFloat.Precision == 0) 
                {
                    x /= FFloat.Precision;
                }
                else 
                {
                    x = x / FFloat.Precision + 1;
                }
            }

            return (int)x;
        }
    }
#else
    [Serializable]
    public struct FFloat : IEquatable<FFloat>, IComparable<FFloat>
    {
        public float _val;

        public static readonly FFloat zero = new FFloat(0.0f);
        public static readonly FFloat one = new FFloat(1.0f);
        public static readonly FFloat negOne = new FFloat(-1.0f);
        public static readonly FFloat half = new FFloat(0.5f);
        public static readonly FFloat FLT_MAX = new FFloat(float.MaxValue);
        public static readonly FFloat FLT_MIN = new FFloat(float.MinValue);
        public static readonly FFloat EPSILON = new FFloat(float.Epsilon);
        public static readonly FFloat FRAMEFPS30 = new FFloat(0.033333f);
        public static readonly FFloat FRAMEFPS45 = new FFloat(0.022222f);
        public static readonly FFloat FRAMEFPS60 = new FFloat(0.016666f);

        public static readonly FFloat MaxValue = new FFloat(float.MaxValue);
        public static readonly FFloat MinValue = new FFloat(float.MinValue);
        //-------------------------------------------------
        public FFloat(int val)
        {
            this._val = val;
        }
        //-------------------------------------------------
        public FFloat(float val)
        {
            this._val = val;
        }
        //-------------------------------------------------
        public FFloat(double val)
        {
            this._val = (float)val;
        }
        //-------------------------------------------------
        public FFloat(long val)
        {
            this._val = val;
        }
        #region override operator 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FFloat a, FFloat b)
        {
            return a._val < b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FFloat a, FFloat b)
        {
            return a._val > b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FFloat a, FFloat b)
        {
            return a._val <= b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FFloat a, FFloat b)
        {
            return a._val >= b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FFloat a, FFloat b)
        {
            return a._val == b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FFloat a, FFloat b)
        {
            return a._val != b._val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator +(FFloat a, FFloat b)
        {
            return new FFloat(a._val + b._val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator -(FFloat a, FFloat b)
        {
            return new FFloat(a._val - b._val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator *(FFloat a, FFloat b)
        {
            return new FFloat(a._val*b._val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator /(FFloat a, FFloat b)
        {
            if (b._val == 0) return a;
            return new FFloat(a._val/b._val);
        }
        public static FFloat operator %(FFloat a, FFloat b)
        {
            if (b._val == 0) return a;
            long aF = (long)(a._val * 100000);
            long bF = (long)(b._val * 1000000);
            long val = aF % bF;
            return new FFloat(val*0.000001f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FFloat operator -(FFloat a)
        {
            return new FFloat(-a._val);
        }

        #region adapt for int
        public static FFloat operator +(FFloat a, int b)
        {
            return new FFloat(a._val + b);
        }
        //-------------------------------------------------
        public static FFloat operator -(FFloat a, int b)
        {
            return new FFloat(a._val - b);
        }
        //-------------------------------------------------
        public static FFloat operator *(FFloat a, int b)
        {
            return new FFloat(a._val * b);
        }
        //-------------------------------------------------
        public static FFloat operator /(FFloat a, int b)
        {
            return new FFloat((a._val) / b);
        }
        //-------------------------------------------------
        public static FFloat operator +(int a, FFloat b)
        {
            return new FFloat(b._val + a);
        }
        //-------------------------------------------------
        public static FFloat operator -(int a, FFloat b)
        {
            return new FFloat(a - b._val);
        }
        //-------------------------------------------------
        public static FFloat operator *(int a, FFloat b)
        {
            return new FFloat(b._val * a);
        }
        //-------------------------------------------------
        public static FFloat operator /(int a, FFloat b)
        {
            return new FFloat(a / b._val);
        }
        //-------------------------------------------------
        public static bool operator <(FFloat a, int b)
        {
            return a._val < b;
        }

        public static bool operator >(FFloat a, int b)
        {
            return a._val > b;
        }

        public static bool operator <=(FFloat a, int b)
        {
            return a._val <= b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FFloat a, int b)
        {
            return a._val >= b;
        }

        public static bool operator ==(FFloat a, int b)
        {
            return a._val == b;
        }

        public static bool operator !=(FFloat a, int b)
        {
            return a._val != b;
        }


        public static bool operator <(int a, FFloat b)
        {
            return a < (b._val);
        }

        public static bool operator >(int a, FFloat b)
        {
            return a > (b._val);
        }

        public static bool operator <=(int a, FFloat b)
        {
            return a <= (b._val);
        }

        public static bool operator >=(int a, FFloat b)
        {
            return a >= (b._val);
        }

        public static bool operator ==(int a, FFloat b)
        {
            return a == (b._val);
        }

        public static bool operator !=(int a, FFloat b)
        {
            return a != (b._val);
        }

        #endregion

        #region adapt for float
        public static FFloat operator +(FFloat a, float b)
        {
            return new FFloat(a._val + b);
        }
        //-------------------------------------------------
        public static FFloat operator -(FFloat a, float b)
        {
            return new FFloat(a._val - b);
        }
        //-------------------------------------------------
        public static FFloat operator *(FFloat a, float b)
        {
            return new FFloat(a._val * b);
        }
        //-------------------------------------------------
        public static FFloat operator /(FFloat a, float b)
        {
            if (b == 0) return FFloat.zero;
            return new FFloat(a._val / b);
        }
        //-------------------------------------------------
        public static FFloat operator +(float a, FFloat b)
        {
            return new FFloat(b._val + a);
        }
        //-------------------------------------------------
        public static FFloat operator -(float a, FFloat b)
        {
            return new FFloat(a - b._val);
        }
        //-------------------------------------------------
        public static FFloat operator *(float a, FFloat b)
        {
            return new FFloat(b._val * a);
        }
        //-------------------------------------------------
        public static FFloat operator /(float a, FFloat b)
        {
            return new FFloat(a / b._val);
        }
        //-------------------------------------------------
        public static bool operator <(FFloat a, float b)
        {
            return a._val < b;
        }

        public static bool operator >(FFloat a, float b)
        {
            return a._val > b;
        }

        public static bool operator <=(FFloat a, float b)
        {
            return a._val <= b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FFloat a, float b)
        {
            return a._val >= b;
        }

        public static bool operator ==(FFloat a, float b)
        {
            return System.Math.Abs(a._val - b) <= 0.00001f;
        }

        public static bool operator !=(FFloat a, float b)
        {
            return System.Math.Abs(a._val - b) > 0.00001f;
        }


        public static bool operator <(float a, FFloat b)
        {
            return a < (b._val);
        }

        public static bool operator >(float a, FFloat b)
        {
            return a > (b._val);
        }

        public static bool operator <=(float a, FFloat b)
        {
            return a <= (b._val);
        }

        public static bool operator >=(float a, FFloat b)
        {
            return a >= (b._val);
        }

        public static bool operator ==(float a, FFloat b)
        {
            return System.Math.Abs(b._val - a) <= 0.00001f;
        }

        public static bool operator !=(float a, FFloat b)
        {
            return System.Math.Abs(b._val - a) > 0.00001f;
        }

        #endregion

        #region adapt for long
        public static FFloat operator +(FFloat a, long b)
        {
            return new FFloat(a._val + b);
        }
        //-------------------------------------------------
        public static FFloat operator -(FFloat a, long b)
        {
            return new FFloat(a._val - b );
        }
        //-------------------------------------------------
        public static FFloat operator *(FFloat a, long b)
        {
            return new FFloat(a._val * b);
        }
        //-------------------------------------------------
        public static FFloat operator /(FFloat a, long b)
        {
            return new FFloat(a._val / b);
        }
        //-------------------------------------------------
        public static FFloat operator +(long a, FFloat b)
        {
            return new FFloat(b._val + a);
        }
        //-------------------------------------------------
        public static FFloat operator -(long a, FFloat b)
        {
            return new FFloat(a - b._val);
        }
        //-------------------------------------------------
        public static FFloat operator *(long a, FFloat b)
        {
            return new FFloat(b._val * a);
        }
        //-------------------------------------------------
        public static FFloat operator /(long a, FFloat b)
        {
            return new FFloat(a  / b._val);
        }
        //-------------------------------------------------
        public static bool operator <(FFloat a, long b)
        {
            return a._val < b;
        }
        //-------------------------------------------------
        public static bool operator >(FFloat a, long b)
        {
            return a._val > b;
        }
        //-------------------------------------------------
        public static bool operator <=(FFloat a, long b)
        {
            return a._val <= b;
        }
        //-------------------------------------------------
        public static bool operator >=(FFloat a, long b)
        {
            return a._val >= b;
        }
        //-------------------------------------------------
        public static bool operator ==(FFloat a, long b)
        {
            return System.Math.Abs(a._val - b) <= 0.00001f;
        }
        //-------------------------------------------------
        public static bool operator !=(FFloat a, long b)
        {
            return System.Math.Abs(a._val - b) > 0.00001f;
        }
        //-------------------------------------------------
        public static bool operator <(long a, FFloat b)
        {
            return a < (b._val);
        }
        //-------------------------------------------------
        public static bool operator >(long a, FFloat b)
        {
            return a  > (b._val);
        }
        //-------------------------------------------------
        public static bool operator <=(long a, FFloat b)
        {
            return a  <= (b._val);
        }
        //-------------------------------------------------
        public static bool operator >=(long a, FFloat b)
        {
            return a >= (b._val);
        }
        //-------------------------------------------------
        public static bool operator ==(long a, FFloat b)
        {
            return System.Math.Abs(b._val - a) <= 0.00001f;
        }
        //-------------------------------------------------
        public static bool operator !=(long a, FFloat b)
        {
            return System.Math.Abs(b._val - a) > 0.00001f;
        }
        #endregion

        #endregion

        #region override object func 
        public override bool Equals(object obj)
        {
            return obj is FFloat && ((FFloat)obj)._val == _val;
        }
        //-------------------------------------------------
        public bool Equals(FFloat other)
        {
            return _val == other._val;
        }
        //-------------------------------------------------
        public int CompareTo(FFloat other)
        {
            return _val.CompareTo(other._val);
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return (int)_val;
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return _val.ToString("F3");
        }
        public static bool TryParse(string intPut, out FFloat outV)
        {
            outV._val = 0;
            float temp;
            if (float.TryParse(intPut, out temp))
            {
                outV._val = temp;
                return true;
            }
            return false;
        }

        #endregion

        #region override type convert 
        public static implicit operator FFloat(short value)
        {
            return new FFloat(value);
        }

        public static explicit operator short(FFloat value)
        {
            return (short)(value._val);
        }

        public static implicit operator FFloat(int value)
        {
            return new FFloat(value);
        }

        public static implicit operator int(FFloat value)
        {
            return (int)(value._val);
        }

        //         public static explicit operator FFloat(long value){
        //             return new FFloat(true, value * Precision);
        //         }

        //         public static implicit operator long(FFloat value){
        //             return value._val / Precision;
        //         }


        public static implicit operator FFloat(float value)
        {
            return new FFloat(value);
        }

        public static implicit operator float(FFloat value)
        {
            return value._val;
        }

        public static implicit operator FFloat(double value)
        {
            return new FFloat((float)value);
        }

        public static implicit operator double(FFloat value)
        {
            return (double)value._val;
        }

        #endregion

        //-------------------------------------------------
        public int ToInt()
        {
            return (int)_val;
        }
        //-------------------------------------------------
        public long ToLong()
        {
            return (long)_val;
        }
        //-------------------------------------------------
        public float ToFloat()
        {
            return _val;
        }
        //-------------------------------------------------
        public double ToDouble()
        {
            return (double)_val;
        }
        //-------------------------------------------------
        public int Floor()
        {
            return (int)Math.Floor((double)_val);
        }
        //-------------------------------------------------
        public int Ceil()
        {
            return (int)Math.Ceiling(_val);
        }
    }
#endif
}