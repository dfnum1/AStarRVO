#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:47
类    名: 	LAxis3D
作    者:	HappLI
描    述:	
*********************************************************************/
namespace ExternEngine
{
    public struct Axis3D
    {
        public FVector3 x;
        public FVector3 y;
        public FVector3 z;
        public static readonly Axis3D identity = new Axis3D(FVector3.right, FVector3.up, FVector3.forward);

        public Axis3D(FVector3 right, FVector3 up, FVector3 forward)
        {
            this.x = right;
            this.y = up;
            this.z = forward;
        }

        public FVector3 WorldToLocal(FVector3 vec)
        {
            var _x =FMath.Dot(x, vec);
            var _y = FMath.Dot(y, vec);
            var _z = FMath.Dot(z, vec);
            return new FVector3(_x, _y, _z);
        }
        public FVector3 LocalToWorld(FVector3 vec)
        {
            return x * vec.x + y * vec.y + z * vec.z;
        }

        public FVector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default: throw new System.IndexOutOfRangeException("vector idx invalid" + index);
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default: throw new System.IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }
    }
}
#endif