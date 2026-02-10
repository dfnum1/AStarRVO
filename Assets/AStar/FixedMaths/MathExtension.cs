#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:50
类    名: 	LMathExtension
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public static partial class FMathExtension
    {
        //-------------------------------------------------
        public static FVector2 ToVector2(this UnityEngine.Vector2Int vec)
        {
            return new FVector2(true, vec.x * FFloat.Precision, vec.y * FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector3 ToVector3(this UnityEngine.Vector3Int vec)
        {
            return new FVector3(true, vec.x * FFloat.Precision, vec.y * FFloat.Precision, vec.z * FFloat.Precision);
        }
        //-------------------------------------------------
        public static FVector2Int ToVector2Int(this UnityEngine.Vector2Int vec)
        {
            return new FVector2Int(vec.x, vec.y);
        }
        //-------------------------------------------------
        public static FVector3Int ToVector3Int(this UnityEngine.Vector3Int vec)
        {
            return new FVector3Int(vec.x, vec.y, vec.z);
        }
        //-------------------------------------------------
        public static UnityEngine.Vector2Int ToVector2Int(this FVector2Int vec)
        {
            return new UnityEngine.Vector2Int(vec.x, vec.y);
        }
        //-------------------------------------------------
        public static UnityEngine.Vector3Int ToVector3Int(this FVector3Int vec)
        {
            return new UnityEngine.Vector3Int(vec.x, vec.y, vec.z);
        }
        //-------------------------------------------------
        public static FVector2 ToVector2(this UnityEngine.Vector2 vec)
        {
            return new FVector2(
                FMath.ToFloat(vec.x),
                FMath.ToFloat(vec.y));
        }
        //-------------------------------------------------
        public static FVector3 ToVector3(this UnityEngine.Vector2 vec)
        {
            return new FVector3(
                FMath.ToFloat(vec.x),
                FMath.ToFloat(vec.y), FFloat.zero);
        }
        //-------------------------------------------------
        public static FVector3 ToVector3(this UnityEngine.Vector3 vec)
        {
            return new FVector3(
                FMath.ToFloat(vec.x),
                FMath.ToFloat(vec.y),
                FMath.ToFloat(vec.z));
        }
        //-------------------------------------------------
        public static FVector2 ToVector2XZ(this UnityEngine.Vector3 vec)
        {
            return new FVector2(
                FMath.ToFloat(vec.x),
                FMath.ToFloat(vec.z));
        }
    }
}
#endif