#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:47
类    名: 	FMath
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public static partial class FMath 
    {
        public const long LPIQuad  = (long)(0.7853981*FFloat.Precision);
        public const long LPIHalf  = (long)(1.5707963 * FFloat.Precision);
        public const long LPI      = (long)(3.1415926 * FFloat.Precision);
        public const long LPI2     = (long)(6.2831853 * FFloat.Precision);
        public const long LRad2Deg = (long)(57.2957795 * FFloat.Precision);
        public const long LDeg2Rad = (long)(0.0174532 * FFloat.Precision);

        public static readonly FFloat PIQuad =  new FFloat(true, LPIHalf); 
        public static readonly FFloat PIHalf =  new FFloat(true, LPIHalf);  
        public static readonly FFloat PI =      new FFloat(true, LPI);          
        public static readonly FFloat PI2 =     new FFloat(true, LPI2);        
        public static readonly FFloat Rad2Deg = new FFloat(true, LRad2Deg);
        public static readonly FFloat Deg2Rad = new FFloat(true, LDeg2Rad);
        public static FFloat Pi => PI;

#region Atan2
        public static long _Atan2(long y, long x)
        {
            //特殊情况处理
            if (y == 0) 
            {
                if (x == 0) 
                {
                    return 0;
                }

                return x < 0 ? FMath.LPI : 0;
            }

            if (x == 0) 
            {
                return y > 0 ? FMath.LPIHalf : -FMath.LPIHalf;
            }

            //决定象限
            int idxV = 0;
            if (x < 0)
            {
                x = -x;
                idxV += 4;
            }

            if (y < 0) 
            {
                y = -y;
                idxV += 2;
            }

            FFloat factor = 0;
            if (y > x) 
            {
                idxV += 1;
                factor = new FFloat(y) / x;
            }
            else 
            {
                factor = new FFloat(x) / y;
            }

            //逆时针 idx 为 0 1 5 4 6 7 3 2
            var info = idx2LutInfo[idxV];
            if (x == y) 
            {
                return info.offset;
            }
            var deg = _LutATan(factor) - FMath.LPIQuad;
            return info.sign * deg + info.offset;
        }
        //-------------------------------------------------
        private static LutAtan2Helper[] idx2LutInfo = new LutAtan2Helper[] 
        {
            new LutAtan2Helper(-1, FMath.LPIQuad),
            new LutAtan2Helper(1, FMath.LPIQuad),
            new LutAtan2Helper(1, -FMath.LPIQuad),
            new LutAtan2Helper(-1, -FMath.LPIQuad),

            new LutAtan2Helper(1, FMath.LPIQuad * 3),
            new LutAtan2Helper(-1, FMath.LPIQuad * 3),
            new LutAtan2Helper(-1, -FMath.LPIQuad * 3),
            new LutAtan2Helper(1, -FMath.LPIQuad * 3),
        };
        //-------------------------------------------------
        public struct LutAtan2Helper 
        {
            public long sign;
            public long offset;

            public LutAtan2Helper(long sign, long offset)
            {
                this.sign = sign;
                this.offset = offset;
            }
        }
        //-------------------------------------------------
        public static long _LutATan(FFloat ydx)
        {
            if (ydx < 1) return FMath.LPIHalf;
            //Plugin.Logger.Asset(ydx >= 1);
            if (ydx >= LUTAtan2.MaxQueryIdx) return FMath.LPIHalf;
            var iydx = (int) ydx;
            var startIdx = LUTAtan2._startIdx[iydx - 1];
            var size = LUTAtan2._arySize[iydx - 1];
            var remaind = ydx - iydx;
            var idx = startIdx + (int) (remaind * size);
            return LUTAtan2._tblTbl[idx];
        }
#endregion
        //-------------------------------------------------
        public static FFloat Atan2(FFloat y, FFloat x)
        {
            return Atan2(y._val, x._val);
        }
        //-------------------------------------------------
        public static FFloat Atan2(long y, long x)
        {
            return new FFloat(true,_Atan2(y, x));
        }
        //-------------------------------------------------
        public static FFloat Acos(FFloat val)
        {
            int idx = (int) (val._val *  LUTAcos.HALF_COUNT / FFloat.Precision) +
                      LUTAcos.HALF_COUNT;
            idx = Clamp(idx, 0, LUTAcos.COUNT);
            return new FFloat(true,  LUTAcos.table[idx]);
        }
        //-------------------------------------------------
        public static FFloat Asin(FFloat val)
        {
            int idx = (int) (val._val *  LUTAsin.HALF_COUNT / FFloat.Precision) +
                      LUTAsin.HALF_COUNT;
            idx = Clamp(idx, 0, LUTAsin.COUNT);
            return new FFloat(true,  LUTAsin.table[idx]);
        }
        //-------------------------------------------------
        //ccw
        public static FFloat Sin(FFloat radians)
        {
            return new FFloat(true, LUTSin.table[_GetIdx(radians)]);
        }
        //-------------------------------------------------
        //ccw
        public static FFloat Cos(FFloat radians)
        {
            return new FFloat(true, LUTCos.table[_GetIdx(radians)]);
        }
        //-------------------------------------------------
        private static int _GetIdx(FFloat radians)
        {
            var rawVal = radians._val % FMath.LPI2;
            if (rawVal < 0) rawVal += FMath.LPI2;
            var val = new FFloat(true,rawVal) / FMath.PI2;
            var idx = (int)(val * LUTCos.COUNT);
            idx = Clamp(idx, 0, LUTCos.COUNT);
            return idx;
        }
        //-------------------------------------------------
        //ccw
        public static void SinCos(out FFloat s, out FFloat c, FFloat radians)
        {
            int idx = _GetIdx(radians);
            s = new FFloat(true, LUTSin.table[idx]);
            c = new FFloat(true, LUTCos.table[idx]);
        }
        //-------------------------------------------------
        public static uint Sqrt32(uint a)
        {
            ulong rem = 0;  
            ulong root = 0;  
            ulong divisor = 0;  
            for(int i=0; i<16; i++)
            {  
                root <<= 1;  
                rem = ((rem << 2) + (a >> 30));  
                a <<= 2;  
                divisor = (root<<1) + 1;  
                if(divisor <= rem)
                {  
                    rem -= divisor;  
                    root++;  
                }  
            }  
            return (uint)root;  
        }
        //x = 2*p + q  
        //x^2 = 4*p^2 + 4pq + q^2
        //q = (x^2 - 4*p^2)/(4*p+q)  
        //https://www.cnblogs.com/10cm/p/3922398.html
        //-------------------------------------------------
        public static uint Sqrt64(ulong a)
        {
            ulong rem = 0;  
            ulong root = 0;  
            ulong divisor = 0;  
            for(int i=0; i<32; i++)
            {  
                root <<= 1;  
                rem = ((rem << 2) + (a >> 62));//(x^2 - 4*p^2)  
                a <<= 2;  
                divisor = (root<<1) + 1; //(4*p+q) 
                if(divisor <= rem)
                {  
                    rem -= divisor;  
                    root++;  
                }  
            }  
            return (uint)root;  
        }
        //-------------------------------------------------
        public static int Sqrt(int a)
        {
            if (a <= 0)
            {
                return 0;
            }

            return (int) FMath.Sqrt32((uint) a);
        }
        //-------------------------------------------------
        public static long Sqrt(long a)
        {
            if (a <= 0L)
            {
                return 0;
            }

            if (a <= (long) (0xffffffffu))
            {
                return (long) FMath.Sqrt32((uint) a);
            }

            return (long) FMath.Sqrt64((ulong) a);
        }
        //-------------------------------------------------
        public static FFloat Sqrt(FFloat a)
        {
            if (a._val <= 0)
            {
                return FFloat.zero;
            }

            return new FFloat(true, Sqrt((long) a._val * FFloat.Precision));
        }
        //-------------------------------------------------
        public static FFloat Sqr(FFloat a)
        {
            return a * a;
        }
        //-------------------------------------------------
        public static uint RoundPowOfTwo(uint x)
        {
            uint val = 1;
            while (val < x)
            {
                val = val << 1;
            }
            return val;
        }
        //-------------------------------------------------
        public static ulong RoundPowOfTwo(ulong x)
        {
            ulong val = 1;
            while (val < x)
            {
                val = val << 1;
            }
            return val;
        }
        //-------------------------------------------------
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }
        //-------------------------------------------------
        public static long Clamp(long a, long min, long max)
        {
            if (a < min)
            {
                return min;
            }

            if (a > max)
            {
                return max;
            }

            return a;
        }
        //-------------------------------------------------
        public static FFloat Clamp(FFloat a, FFloat min, FFloat max)
        {
            if (a < min)
            {
                return min;
            }

            if (a > max)
            {
                return max;
            }

            return a;
        }
        //-------------------------------------------------
        public static FFloat Clamp01(FFloat a)
        {
            if (a < FFloat.zero)
            {
                return FFloat.zero;
            }

            if (a > FFloat.one)
            {
                return FFloat.one;
            }

            return a;
        }
        //-------------------------------------------------
        public static bool SameSign(FFloat a, FFloat b)
        {
            return (long) a._val * b._val > 0L;
        }
        //-------------------------------------------------
        public static int Abs(int val)
        {
            if (val < 0)
            {
                return -val;
            }

            return val;
        }
        //-------------------------------------------------
        public static long Abs(long val)
        {
            if (val < 0L)
            {
                return -val;
            }

            return val;
        }
        //-------------------------------------------------
        public static FFloat Abs(FFloat val)
        {
            if (val._val < 0)
            {
                return new FFloat(true, -val._val);
            }

            return val;
        }
        //-------------------------------------------------
        public static int Sign(FFloat val)
        {
            return System.Math.Sign(val._val);
        }
        //-------------------------------------------------
        public static FFloat Round(FFloat val)
        {
            if (val <= 0) 
            {
                var remainder = (-val._val) % FFloat.Precision;
                if (remainder > FFloat.HalfPrecision) 
                {
                    return new FFloat(true, val._val + remainder - FFloat.Precision);
                }
                else
                {
                    return new FFloat(true, val._val + remainder);
                }
            }
            else
            {
                var remainder = (val._val) % FFloat.Precision;
                if (remainder > FFloat.HalfPrecision)
                {
                    return new FFloat(true, val._val - remainder + FFloat.Precision);
                }
                else 
                {
                    return new FFloat(true, val._val - remainder);
                }
            }
        }

        //-------------------------------------------------
        public static void Swap(ref long a, ref long b)
        {
            a ^= b;
            b ^= a;
            a ^= b;
        }
        //-------------------------------------------------
        public static void Swap(ref int a, ref int b)
        {
            a ^= b;
            b ^= a;
            a ^= b;
        }
        //-------------------------------------------------
        public static long Max(long a, long b)
        {
            return (a <= b) ? b : a;
        }
        //-------------------------------------------------
        public static int Max(int a, int b)
        {
            return (a <= b) ? b : a;
        }
        //-------------------------------------------------
        public static long Min(long a, long b)
        {
            return (a > b) ? b : a;
        }
        //-------------------------------------------------
        public static int Min(int a, int b)
        {
            return (a > b) ? b : a;
        }
        //-------------------------------------------------
        public static FFloat Min(float a, FFloat b)
        {
            return Min(new FFloat(a), b);
        }
        //-------------------------------------------------
        public static FFloat Min(FFloat a,float b)
        {
            return Min(a,new FFloat(b));
        }
        //-------------------------------------------------
        public static FFloat Max(float a, FFloat b)
        {
            return Max(new FFloat(a), b);
        }
        //-------------------------------------------------
        public static FFloat Max(FFloat a, float b)
        {
            return Max(a, new FFloat(b));
        }
        //-------------------------------------------------
        public static int Min(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }
            return num;
        }
        //-------------------------------------------------
        public static FFloat Min(params FFloat[] values)
        {
            int length = values.Length;
            if (length == 0)
                return FFloat.zero;
            FFloat num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }
            return num;
        }
        //-------------------------------------------------
        public static int Max(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] > num)
                    num = values[index];
            }
            return num;
        }
        //-------------------------------------------------
        public static FFloat Max(params FFloat[] values)
        {
            int length = values.Length;
            if (length == 0)
                return FFloat.zero;
            var num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] > num)
                    num = values[index];
            }
            return num;
        }
        //-------------------------------------------------
        public static int FloorToInt(FFloat a)
        {
            var val = a._val;
            if (val < 0) {
                val = val - FFloat.Precision + 1;
            }
            return (int)(val / FFloat.Precision) ;
        }
        //-------------------------------------------------
        public static long ToRawFloat(float a)
        {
            return FFloat.FloatToLong(a);
        }
        //-------------------------------------------------
        public static FFloat ToFloat(float a)
        {
            return  new FFloat(true, FFloat.FloatToLong(a));
        }
        //-------------------------------------------------
        public static FFloat ToFloat(int a)
        {
            return  new FFloat(true, (long)(a * FFloat.Precision));
        }
        //-------------------------------------------------
        public static FFloat ToFloat(long a)
        {
            return  new FFloat(true, (long)(a * FFloat.Precision));
        }
        //-------------------------------------------------
        public static FFloat Min(FFloat a, FFloat b)
        {
            return new FFloat(true, Min(a._val, b._val));
        }
        //-------------------------------------------------
        public static FFloat Max(FFloat a, FFloat b)
        {
            return new FFloat(true, Max(a._val, b._val));
        }
        //-------------------------------------------------
        public static FFloat Lerp(FFloat a, FFloat b, FFloat f)
        {
            return new FFloat(true, (int) (((long) (b._val - a._val) * f._val) / FFloat.Precision) + a._val);
        }
        //-------------------------------------------------
        public static FFloat InverseLerp(FFloat a, FFloat b, FFloat value)
        {
            if ( a !=  b)
                return Clamp01( (( value -  a) / ( b -  a)));
            return FFloat.zero;
        }
        //-------------------------------------------------
        public static FVector2 Lerp(FVector2 a, FVector2 b, FFloat f)
        {
            return new FVector2(true,
                (int) (((long) (b._x - a._x) * f._val) / FFloat.Precision) + a._x,
                (int) (((long) (b._y - a._y) * f._val) / FFloat.Precision) + a._y);
        }
        //-------------------------------------------------
        public static FVector3 Lerp(FVector3 a, FVector3 b, FFloat f)
        {
            return new FVector3(true,
                (int) (((long) (b._x - a._x) * f._val) / FFloat.Precision) + a._x,
                (int) (((long) (b._y - a._y) * f._val) / FFloat.Precision) + a._y,
                (int) (((long) (b._z - a._z) * f._val) / FFloat.Precision) + a._z);
        }
        //-------------------------------------------------
        public static bool IsPowerOfTwo(int x)
        {
            return (x & x - 1) == 0;
        }
        //-------------------------------------------------
        public static int CeilPowerOfTwo(int x)
        {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;
            return x;
        }
        //-------------------------------------------------
        public static FFloat Dot(FVector2 u, FVector2 v)
        {
            return new FFloat(true, ((long) u._x * v._x + (long) u._y * v._y) / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FFloat Dot(FVector3 lhs, FVector3 rhs)
        {
            var val = ((long) lhs._x) * rhs._x + ((long) lhs._y) * rhs._y + ((long) lhs._z) * rhs._z;
            return new FFloat(true, val / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector3 Cross(FVector3 lhs, FVector3 rhs)
        {
            return new FVector3(true,
                ((long) lhs._y * rhs._z - (long) lhs._z * rhs._y) / FFloat.Precision,
                ((long) lhs._z * rhs._x - (long) lhs._x * rhs._z) / FFloat.Precision,
                ((long) lhs._x * rhs._y - (long) lhs._y * rhs._x) / FFloat.Precision
            );
        }
        //-------------------------------------------------
        public static FFloat Cross2D(FVector2 u, FVector2 v)
        {
            return new FFloat(true, ((long)u._x * v._y - (long)u._y * v._x) / FFloat.Precision);
        }
        //-------------------------------------------------
        public static FFloat Dot2D(FVector2 u, FVector2 v)
        {
            return new FFloat(true, ((long) u._x * v._x + (long) u._y * v._y) / FFloat.Precision);
        }

    }
}
#endif