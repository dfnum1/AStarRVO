#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  13:59
类    名: 	FSegment
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public class FSegment : BaseShape 
    {
        public override int TypeId => (int) EShape2D.Segment;
        public FVector2 pos1;
        public FVector2 pos2;
    }
}
#endif