#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  13:55
类    名: 	FAABB
作    者:	HappLI
描    述:	
*********************************************************************/

namespace ExternEngine
{
    public class FAABB : FCircle 
    {
        public override int TypeId => (int) EShape2D.AABB;
        /// <summary> Half size of BoundBox</summary>
        public FVector2 size;

        public FAABB() : base(){ }

        public FAABB(FVector2 size)
        {
            this.size = size;
            radius = size.magnitude;
        }
        public override string ToString()
        {
            return $"(radius:{radius} deg:{radius} hSize:{size})";
        }
    }
}
#endif