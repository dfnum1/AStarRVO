#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:50
类    名: 	FMathExtension2
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public static partial class FMathExtension 
    {
        //-------------------------------------------------
        public static FVector2 ToVector2(this FVector2Int vec)
        {
            return new FVector2(true, vec.x * FFloat.Precision, vec.y * FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector3 ToVector3(this FVector3Int vec)
        {
            return new FVector3(true, vec.x * FFloat.Precision, vec.y * FFloat.Precision, vec.z * FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector2Int ToVector2Int(this FVector2 vec)
        {
            return new FVector2Int(vec.x.ToInt(), vec.y.ToInt());
        }
        //-------------------------------------------------
        public static FVector3Int ToVector3Int(this FVector3 vec)
        {
            return new FVector3Int(vec.x.ToInt(), vec.y.ToInt(), vec.z.ToInt());
        }
    }

    public static partial class FMathExtension 
    {
        public static FFloat ToFloat(this float v)
        {
            return FMath.ToFloat(v);
        }
        //-------------------------------------------------
        public static FFloat ToFloat(this int v)
        {
            return FMath.ToFloat(v);
        }
//         //-------------------------------------------------
//         public static FFloat ToFloat(this long v)
//         {
//             return FMath.ToFloat(v);
//         }
    }

    public static partial class FMathExtension
    {
        public static FVector2Int Floor(this FVector2 vec)
        {
            return new FVector2Int(FMath.FloorToInt(vec.x), FMath.FloorToInt(vec.y));
        }
        //-------------------------------------------------
        public static FVector3Int Floor(this FVector3 vec)
        {
            return new FVector3Int(
                FMath.FloorToInt(vec.x),
                FMath.FloorToInt(vec.y),
                FMath.FloorToInt(vec.z)
            );
        }
    }

    public static partial class FMathExtension 
    {
        public static FVector2 RightVec(this FVector2 vec)
        {
            return new FVector2(true, vec._y, -vec._x);
        }
        //-------------------------------------------------
        public static FVector2 LeftVec(this FVector2 vec)
        {
            return new FVector2(true, -vec._y, vec._x);
        }
        //-------------------------------------------------
        public static FVector2 BackVec(this FVector2 vec)
        {
            return new FVector2(true, -vec._x, -vec._y);
        }
        //-------------------------------------------------
        public static FVector2 ToVec2(this FVector3 vec)
        {
            return new FVector2(true, vec._x, vec._y);
        }
        //-------------------------------------------------
        public static FFloat ToDeg(this FVector2 vec)
        {
            return FTransform.ToDeg(vec);
        }
        //-------------------------------------------------
        public static FFloat Abs(this FFloat val)
        {
            return FMath.Abs(val);
        }
    }
}
#endif