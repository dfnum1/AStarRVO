#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:58
类    名: 	FMath_Vector
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public static partial class FMath
    {
        //-------------------------------------------------
        public static FVector3 Transform(ref FVector3 point, ref FVector3 axis_x, ref FVector3 axis_y, ref FVector3 axis_z,
            ref FVector3 trans)
        {
            return new FVector3(true,
                ((axis_x._x * point._x + axis_y._x * point._y + axis_z._x * point._z) / FFloat.Precision) + trans._x,
                ((axis_x._y * point._x + axis_y._y * point._y + axis_z._y * point._z) / FFloat.Precision) + trans._y,
                ((axis_x._z * point._x + axis_y._z * point._y + axis_z._z * point._z) / FFloat.Precision) + trans._z);
        }
        //-------------------------------------------------
        public static FVector3 Transform(FVector3 point, ref FVector3 axis_x, ref FVector3 axis_y, ref FVector3 axis_z,
            ref FVector3 trans)
        {
            return new FVector3(true,
                ((axis_x._x * point._x + axis_y._x * point._y + axis_z._x * point._z) / FFloat.Precision) + trans._x,
                ((axis_x._y * point._x + axis_y._y * point._y + axis_z._y * point._z) / FFloat.Precision) + trans._y,
                ((axis_x._z * point._x + axis_y._z * point._y + axis_z._z * point._z) / FFloat.Precision) + trans._z);
        }
        //-------------------------------------------------
        public static FVector3 Transform(ref FVector3 point, ref FVector3 axis_x, ref FVector3 axis_y, ref FVector3 axis_z,
            ref FVector3 trans, ref FVector3 scale)
        {
            long num = (long) point._x * (long) scale._x /FFloat.Precision;
            long num2 = (long) point._y * (long) scale._x/FFloat.Precision;
            long num3 = (long) point._z * (long) scale._x/FFloat.Precision;
            return new FVector3(true,
                (int) (((long) axis_x._x * num + (long) axis_y._x * num2 + (long) axis_z._x * num3) /FFloat.Precision) +
                trans._x,
                (int) (((long) axis_x._y * num + (long) axis_y._y * num2 + (long) axis_z._y * num3) /FFloat.Precision) +
                trans._y,
                (int) (((long) axis_x._z * num + (long) axis_y._z * num2 + (long) axis_z._z * num3) /FFloat.Precision) +
                trans._z);
        }
        //-------------------------------------------------
        public static FVector3 Transform(ref FVector3 point, ref FVector3 forward, ref FVector3 trans)
        {
            FVector3 up = FVector3.up;
            FVector3 vInt = Cross(FVector3.up, forward);
            return FMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
        }
        //-------------------------------------------------
        public static FVector3 Transform(FVector3 point, FVector3 forward, FVector3 trans)
        {
            FVector3 up = FVector3.up;
            FVector3 vInt = Cross(FVector3.up, forward);
            return FMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
        }
        //-------------------------------------------------
        public static FVector3 Transform(FVector3 point, FVector3 forward, FVector3 trans, FVector3 scale)
        {
            FVector3 up = FVector3.up;
            FVector3 vInt = Cross(FVector3.up, forward);
            return FMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans, ref scale);
        }
        //-------------------------------------------------
        public static FVector3 MoveTowards(FVector3 from, FVector3 to, FFloat dt)
        {
            if ((to - from).sqrMagnitude <= (dt * dt))
            {
                return to;
            }

            return from + (to - from).Normalize(dt);
        }
        //-------------------------------------------------
        public static FFloat AngleInt(FVector3 lhs, FVector3 rhs)
        {
            return FMath.Acos(Dot(lhs, rhs));
        }
    }
}
#endif