#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  14:00
类    名: 	FRay2D
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Runtime.InteropServices;

namespace ExternEngine
{

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct FRay2D
    {
        public int TypeId => (int)EShape2D.Ray;
        public FVector2 origin;
        public FVector2 direction;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FRaycastHit2D
    {
        public FVector2 point;
        public FFloat distance;
        public int colliderId;
    }
}
#endif