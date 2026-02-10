#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  13:48
类    名: 	FTransform
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Runtime.InteropServices;

namespace ExternEngine
{
    [System.Serializable]
    public partial class FTransform
    {
        public FVector2 pos;
        public FFloat y;
        public FFloat deg; //same as Unity CW deg(up) =0

        //-------------------------------------------------
        public FVector2 forward
        { //等同于2D  up
            get
            {
                FFloat s, c;
                var ccwDeg = (-deg + 90);
                FMath.SinCos(out s, out c, FMath.Deg2Rad * ccwDeg);
                return new FVector2(c, s);
            }
            set => deg = ToDeg(value);
        }
        //-------------------------------------------------
        public static FFloat ToDeg(FVector2 value)
        {
            var ccwDeg = FMath.Atan2(value.y, value.x) * FMath.Rad2Deg;
            var deg = 90 - ccwDeg;
            return AbsDeg(deg);
        }
        //-------------------------------------------------
        public static FFloat TurnToward(FVector2 targetPos, FVector2 currentPos, FFloat cursDeg, FFloat turnVal,
            out bool isLessDeg)
        {
            var toTarget = (targetPos - currentPos).normalized;
            var toDeg = FTransform.ToDeg(toTarget);
            return TurnToward(toDeg, cursDeg, turnVal, out isLessDeg);
        }
        //-------------------------------------------------
        public static FFloat TurnToward(FFloat toDeg, FFloat cursDeg, FFloat turnVal,
            out bool isLessDeg)
        {
            var curDeg = FTransform.AbsDeg(cursDeg);
            var diff = toDeg - curDeg;
            var absDiff = FMath.Abs(diff);
            isLessDeg = absDiff < turnVal;
            if (isLessDeg)
            {
                return toDeg;
            }
            else
            {
                if (absDiff > 180)
                {
                    if (diff > 0)
                    {
                        diff -= 360;
                    }
                    else
                    {
                        diff += 360;
                    }
                }

                return curDeg + turnVal * FMath.Sign(diff);
            }
        }
        //-------------------------------------------------
        public static FFloat AbsDeg(FFloat deg)
        {
            var rawVal = deg._val % ((FFloat)360)._val;
            return new FFloat(true, rawVal);
        }
        //-------------------------------------------------
        public FTransform() { }
        public FTransform(FVector2 pos, FFloat y) : this(pos, y, FFloat.zero) { }
        public FTransform(FVector2 pos) : this(pos, FFloat.zero, FFloat.zero) { }

        //-------------------------------------------------
        public FTransform(FVector2 pos, FFloat y, FFloat deg)
        {
            this.pos = pos;
            this.y = y;
            this.deg = deg;
        }
        //-------------------------------------------------
        public void Reset()
        {
            pos = FVector2.zero;
            y = FFloat.zero;
            deg = FFloat.zero;
        }
        //-------------------------------------------------
        public FVector2 TransformPoint(FVector2 point)
        {
            return pos + TransformDirection(point);
        }
        //-------------------------------------------------
        public FVector2 TransformVector(FVector2 vec)
        {
            return TransformDirection(vec);
        }
        //-------------------------------------------------
        public FVector2 TransformDirection(FVector2 dir)
        {
            var y = forward;
            var x = forward.RightVec();
            return dir.x * x + dir.y * y;
        }
        //-------------------------------------------------
        public static STransform operator +(FTransform a, FTransform b)
        {
            return new STransform { pos = a.pos + b.pos, y = a.y + b.y, deg = a.deg + b.deg };
        }
        //-------------------------------------------------
        public FVector3 Pos3
        {
            get => new FVector3(pos.x, y, pos.y);
            set
            {
                pos = new FVector2(value.x, value.z);
                y = value.y;
            }
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return $"(deg:{deg} pos:{pos} y:{y})";
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct STransform
    {
        public FVector2 pos;
        public FFloat y;
        public FFloat deg;
        public FVector3 Pos3
        {
            get => new FVector3(pos.x, y, pos.y);
            set
            {
                pos = new FVector2(value.x, value.z);
                y = value.y;
            }
        }
    }
}
#endif