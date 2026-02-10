#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:45
类    名: 	HashCodeExtension
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public static class HashCodeExtension 
    {
        //-------------------------------------------------
        public static int GetHash(this byte val,ref int idx)
        {
            return val;
        }
        //-------------------------------------------------
        public static int GetHash(this short val,ref int idx)
        {
            return val;
        }
        //-------------------------------------------------
        public static int GetHash(this int val,ref int idx)
        {
            return val;
        }
        //-------------------------------------------------
        public static int GetHash(this long val,ref int idx)
        {
            return (int) val;
        }
        //-------------------------------------------------
        public static int GetHash(this sbyte val,ref int idx)
        {
            return val;
        }
        //-------------------------------------------------
        public static int GetHash(this ushort val,ref int idx)
        {
            return val;
        }
        //-------------------------------------------------
        public static int GetHash(this uint val,ref int idx)
        {
            return (int) val;
        }
        //-------------------------------------------------
        public static int GetHash(this ulong val,ref int idx)
        {
            return (int) val;
        }
        //-------------------------------------------------
        public static int GetHash(this bool val,ref int idx)
        {
            return val ? 1 : 0;
        }
        //-------------------------------------------------
        public static int GetHash(this string val,ref int idx){
            return val?.GetHashCode() ?? 0;
        }
        //-------------------------------------------------
        public static int GetHash(this FFloat val,ref int idx)
        {
            return PrimerLUT.GetPrimer(val._val);
        }
        //-------------------------------------------------
        public static int GetHash(this FVector2 val,ref int idx)
        {
            return PrimerLUT.GetPrimer(val._x) + PrimerLUT.GetPrimer(val._y) * 17;
        }
        //-------------------------------------------------
        public static int GetHash(this FVector3 val,ref int idx)
        {
            return PrimerLUT.GetPrimer(val._x)
                   + PrimerLUT.GetPrimer(val._y) * 31
                   + PrimerLUT.GetPrimer(val._z) * 37;
        }
    }
}
#endif