#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  13:56
类    名: 	BaseShape
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public class BaseShape 
    {
        public virtual int TypeId => (int) EShape2D.EnumCount;
        public int id;
        public FFloat high;
    }
}
#endif