#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  14:00
类    名: 	FPolygon
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public class FPolygon : FCircle 
    {
        public override int TypeId => (int) EShape2D.Polygon;
        public int vertexCount;
        public FFloat deg;
        public FVector2[] vertexes;
    }
}
#endif