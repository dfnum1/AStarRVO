#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  13:53
类    名: 	FRigidbody
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
namespace ExternEngine
{
    public delegate void OnFloorResultCallback(bool isOnFloor);

    [Serializable]
    public partial class CRigidbody 
    {
        public FTransform transform { get; private set; }
        public static FFloat G = new FFloat(10);
        public static FFloat MinSleepSpeed = new FFloat(100);
        public static FFloat FloorFriction = new FFloat(20);
        public static FFloat MinYSpd = new FFloat(-10);
        public static FFloat FloorY = FFloat.zero;

        public OnFloorResultCallback OnFloorEvent;

        public FVector3 Speed;
        public FFloat Mass = FFloat.one;
        public bool isEnable = true;
        public bool isSleep = false;
        public bool isOnFloor;

        public void BindRef(FTransform transform2D)
        {
            this.transform = transform2D;
        }

        //private int __id;
        //private static int __idCount;
        public void DoStart()
        {
            //__id = __idCount++;
            FFloat y = FFloat.zero;
            isOnFloor = TestOnFloor(transform.Pos3, ref y);
            Speed = FVector3.zero;
            isSleep = isOnFloor;
            lastPos = transform.Pos3;
            lastDeg = transform.deg;
            
        }

        public FVector3 lastPos;
        public FFloat lastDeg;
        public void DoUpdate(FFloat deltaTime)
        {
            if (!isEnable) return;
            if (!TestOnFloor(transform.Pos3))
            {
                isSleep = false;
            }

            lastPos = transform.Pos3;
            lastDeg = transform.deg;
            if (!isSleep)
            {
                if (!isOnFloor)
                {
                    Speed.y -= G * deltaTime;
                    Speed.y = FMath.Max(MinYSpd, Speed.y);
                }

                var pos = transform.Pos3;
                pos += Speed * deltaTime;
                FFloat y = pos.y;
                //Test floor
                isOnFloor = TestOnFloor(transform.Pos3, ref y);
                if (isOnFloor && Speed.y <= 0) 
                {
                    Speed.y = FFloat.zero;
                }

                if (Speed.y <= 0)
                {
                    pos.y = y;
                }

                //Test walls
                if (TestOnWall(ref pos)) 
                {
                    Speed.x = FFloat.zero;
                    Speed.z = FFloat.zero;
                }

                if (isOnFloor) 
                {
                    var speedVal = Speed.magnitude - FloorFriction * deltaTime;
                    speedVal = FMath.Max(speedVal, FFloat.zero);
                    Speed = Speed.normalized * speedVal;
                    if (speedVal < MinSleepSpeed)
                    {
                        isSleep = true;
                    }
                }

                transform.Pos3 = pos;
            }
        }


        public void AddImpulse(FVector3 force)
        {
            isSleep = false;
            Speed += force / Mass;
            //Debug.Log(__id+ " AddImpulse " + force  +" after " + Speed);
        }

        public void ResetSpeed(FFloat ySpeed)
        {
            Speed = FVector3.zero;
            Speed.y = ySpeed;
        }

        public void ResetSpeed()
        {
            Speed = FVector3.zero;
        }

        private bool TestOnFloor(FVector3 pos, ref FFloat y)
        {
            var onFloor = pos.y <= 0; //TODO check with scene
            if (onFloor)
            {
                y = FFloat.zero;
            }

            return onFloor;
        }

        private bool TestOnFloor(FVector3 pos)
        {
            var onFloor = pos.y <= 0; //TODO check with scene
            return onFloor;
        }

        private bool TestOnWall(ref FVector3 pos)
        {
            return false; //TODO check with scene
        }
    }
}
#endif