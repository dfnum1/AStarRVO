#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  13:56
类    名: 	FCircle
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public class FCircle : BaseShape
    {
        public override int TypeId => (int) EShape2D.Circle;
        public FFloat radius;

        public FCircle() : this(FFloat.zero){ }

        public FCircle(FFloat radius)
        {
            this.radius = radius;
        }

        public override string ToString()
        {
            return $"radius:{radius}";
        }
    }
}
#endif