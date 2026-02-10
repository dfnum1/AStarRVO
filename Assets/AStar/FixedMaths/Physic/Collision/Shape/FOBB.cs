#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  13:57
类    名: 	FOBB
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public class FOBB : FAABB
    {
        public override int TypeId => (int) EShape2D.OBB;
        public FFloat deg;
        public FVector2 up;

        public FOBB(FVector2 size, FFloat deg) : base(size)
        {
            this.deg = deg;
            SetDeg(deg);
        }

        public FOBB(FVector2 size, FVector2 up) : base(size)
        {
            SetUp(up);
        }

        //CCW 旋转角度
        public void Rotate(FFloat rdeg)
        {
            deg += rdeg;
            if (deg > 360 || deg < -360) {
                deg = deg - (deg / 360 * 360);
            }

            SetDeg(deg);
        }

        public void SetUp(FVector2 up)
        {
            this.up = up;
            this.deg = FMath.Atan2(-up.x, up.y);
        }

        public void SetDeg(FFloat rdeg)
        {
            deg = rdeg;
            var rad = FMath.Deg2Rad * deg;
            var c = FMath.Cos(rad);
            var s = FMath.Sin(rad);
            up = new FVector2(c,s);
        }
        public override string ToString()
        {
            return $"(radius:{radius} up:{size} deg:{radius} up:{up} )";
        }
    }
}
#endif