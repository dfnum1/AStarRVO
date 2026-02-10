#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  12:00
类    名: 	FRandom
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
namespace ExternEngine 
{
    internal partial struct Random 
    {
        public ulong randSeed ;
        //-------------------------------------------------
        public Random(uint seed = 17)
        {
            randSeed = seed;
        }
        public FFloat value => new FFloat(true, Range(0, (int)FFloat.Precision));
        //-------------------------------------------------
        public uint Next()
        {
            randSeed = randSeed * 1103515245 + 36153;
            return (uint) (randSeed / 65536);
        }
        //-------------------------------------------------
        // range:[0 ~(max-1)]
        public uint Next(uint max)
        {
            return Next() % max;
        }
        //-------------------------------------------------
        public FVector2 NextVector2()
        {
            return new FVector2(true, Next((uint)FFloat.Precision),Next((uint)FFloat.Precision));
        }
        //-------------------------------------------------
        public FVector3 NextVector3()
        {
            return new FVector3(true, Next((uint)FFloat.Precision),Next((uint)FFloat.Precision),Next((uint)FFloat.Precision));
        }
        //-------------------------------------------------
        public int Next(int max)
        {
            return (int) (Next() % max);
        }
        //-------------------------------------------------
        // range:[min~(max-1)]
        public uint Range(uint min, uint max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = max - min;
            return this.Next(num) + min;
        }
        //-------------------------------------------------
        public int Range(int min, int max)
        {
            if (min >= max - 1)
                return min;
            int num = max - min;

            return this.Next(num) + min;
        }
        //-------------------------------------------------
        public FFloat Range(FFloat min, FFloat max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            var num =  (max._val - min._val);
            return new FFloat(true, Next((uint)num) + min._val);
        }
    }

    public class FixedRandom 
    {
        private static Random _i = new Random(3274);
        public static FFloat Range01 => _i.value;
        public static void Seed(ulong seed) { _i.randSeed = seed; }
        public static uint Next(){return _i.Next();}
        public static uint Next(uint max){return _i.Next(max);}
        public static int Next(int max){return _i.Next(max);}
        public static uint Range(uint min, uint max){return _i.Range(min, max);}
        public static int Range(int min, int max){return _i.Range(min, max);}
        public static FFloat Range(FFloat min, FFloat max){return _i.Range(min, max);}
    }
}
#endif