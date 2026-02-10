#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:51
类    名: 	FQuaternion
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
namespace ExternEngine
{
    public struct FQuaternion 
    {
#region public members

        public FFloat x;
        public FFloat y;
        public FFloat z;
        public FFloat w;

#endregion

#region constructor

        //-------------------------------------------------
        public FQuaternion(FFloat p_x, FFloat p_y, FFloat p_z, FFloat p_w)
        {
            x = p_x;
            y = p_y;
            z = p_z;
            w = p_w;
        }
        //-------------------------------------------------
        public FQuaternion(int p_x, int p_y, int p_z, int p_w)
        {
            x._val = p_x;
            y._val = p_y;
            z._val = p_z;
            w._val = p_w;
        }

#endregion

#region public properties

        public FFloat this[int index] 
        {
            get {
                switch (index) 
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid FQuaternion index!");
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
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FQuaternion index!");
                }
            }
        }
        //-------------------------------------------------
        public static FQuaternion identity 
        {
            get { return new FQuaternion(0, 0, 0, 1); }
        }
        //-------------------------------------------------
        public FVector3 eulerAngles 
        {
            get 
            {
                FMatrix3x3 m = QuaternionToMatrix3x3(this);
                return (180 / FMath.PI * MatrixToEuler(m));
            }
            set { this = Euler(value); }
        }

#endregion

#region public functions
        public static FFloat Angle(FQuaternion a, FQuaternion b)
        {
            FFloat single = Dot(a, b);
            return FMath.Acos(FMath.Min(FMath.Abs(single), FFloat.one)) * 2 * (180 / FMath.PI);
        }
        //-------------------------------------------------
        public static FQuaternion AngleAxis(FFloat angle, FVector3 axis)
        {
            axis = axis.normalized;
            angle = angle * FMath.Deg2Rad;

            FQuaternion q = new FQuaternion();

            FFloat halfAngle = angle * FFloat.half;
            FFloat s = FMath.Sin(halfAngle);

            q.w = FMath.Cos(halfAngle);
            q.x = s * axis.x;
            q.y = s * axis.y;
            q.z = s * axis.z;

            return q;
        }
        //-------------------------------------------------
        public static FFloat Dot(FQuaternion a, FQuaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
        //-------------------------------------------------
        public static FQuaternion Euler(FVector3 euler)
        {
            return Euler(euler.x, euler.y, euler.z);
        }
        //-------------------------------------------------
        public static FQuaternion Euler(FFloat x, FFloat y, FFloat z)
        {
            FFloat cX = FMath.Cos(x * FMath.PI / 360);
            FFloat sX = FMath.Sin(x * FMath.PI / 360);

            FFloat cY = FMath.Cos(y * FMath.PI / 360);
            FFloat sY = FMath.Sin(y * FMath.PI / 360);

            FFloat cZ = FMath.Cos(z * FMath.PI / 360);
            FFloat sZ = FMath.Sin(z * FMath.PI / 360);

            FQuaternion qX = new FQuaternion(sX, FFloat.zero, FFloat.zero, cX);
            FQuaternion qY = new FQuaternion(FFloat.zero, sY, FFloat.zero, cY);
            FQuaternion qZ = new FQuaternion(FFloat.zero, FFloat.zero, sZ, cZ);

            FQuaternion q = (qY * qX) * qZ;

            return q;
        }
        //-------------------------------------------------
        public static FQuaternion FromToRotation(FVector3 fromDirection, FVector3 toDirection)
        {
            throw new IndexOutOfRangeException("Not Available!");
        }
        //-------------------------------------------------
        public static FQuaternion Inverse(FQuaternion rotation)
        {
            return new FQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        }
        //-------------------------------------------------
        public static FQuaternion Lerp(FQuaternion a, FQuaternion b, FFloat t)
        {
            if (t > 1) 
            {
                t = FFloat.one;
            }

            if (t < 0)
            {
                t = FFloat.zero;
            }

            return LerpUnclamped(a, b, t);
        }
        //-------------------------------------------------
        public static FQuaternion LerpUnclamped(FQuaternion a, FQuaternion b, FFloat t)
        {
            FQuaternion tmpQuat = new FQuaternion();
            if (Dot(a, b) < 0) 
            {
                tmpQuat.Set(a.x + t * (-b.x - a.x),
                    a.y + t * (-b.y - a.y),
                    a.z + t * (-b.z - a.z),
                    a.w + t * (-b.w - a.w));
            }
            else 
            {
                tmpQuat.Set(a.x + t * (b.x - a.x),
                    a.y + t * (b.y - a.y),
                    a.z + t * (b.z - a.z),
                    a.w + t * (b.w - a.w));
            }

            FFloat nor = FMath.Sqrt(Dot(tmpQuat, tmpQuat));
            return new FQuaternion(tmpQuat.x / nor, tmpQuat.y / nor, tmpQuat.z / nor, tmpQuat.w / nor);
        }
        //-------------------------------------------------
        public static FQuaternion LookRotation(FVector3 forward)
        {
            FVector3 up = FVector3.up;
            return LookRotation(forward, up);
        }
        //-------------------------------------------------
        public static FQuaternion LookRotation(FVector3 forward, FVector3 upwards)
        {
            FMatrix3x3 m = LookRotationToMatrix(forward, upwards);
            return MatrixToQuaternion(m);
        }
        //-------------------------------------------------
        public static FQuaternion RotateTowards(FQuaternion from, FQuaternion to, FFloat maxDegreesDelta)
        {
            FFloat num = FQuaternion.Angle(from, to);
            FQuaternion result = new FQuaternion();
            if (num == 0)
            {
                result = to;
            }
            else 
            {
                FFloat t = FMath.Min(FFloat.one, maxDegreesDelta / num);
                result = FQuaternion.SlerpUnclamped(from, to, t);
            }

            return result;
        }
        //-------------------------------------------------
        public static FQuaternion Slerp(FQuaternion a, FQuaternion b, FFloat t)
        {
            if (t > 1) 
            {
                t = FFloat.one;
            }

            if (t < 0) 
            {
                t = FFloat.zero;
            }

            return SlerpUnclamped(a, b, t);
        }
        //-------------------------------------------------
        public static FQuaternion SlerpUnclamped(FQuaternion q1, FQuaternion q2, FFloat t)
        {
            FFloat dot = Dot(q1, q2);

            FQuaternion tmpQuat = new FQuaternion();
            if (dot < 0) 
            {
                dot = -dot;
                tmpQuat.Set(-q2.x, -q2.y, -q2.z, -q2.w);
            }
            else
                tmpQuat = q2;


            if (dot < 1) 
            {
                FFloat angle = FMath.Acos(dot);
                FFloat sinadiv, sinat, sinaomt;
                sinadiv = 1 / FMath.Sin(angle);
                sinat = FMath.Sin(angle * t);
                sinaomt = FMath.Sin(angle * (1 - t));
                tmpQuat.Set((q1.x * sinaomt + tmpQuat.x * sinat) * sinadiv,
                    (q1.y * sinaomt + tmpQuat.y * sinat) * sinadiv,
                    (q1.z * sinaomt + tmpQuat.z * sinat) * sinadiv,
                    (q1.w * sinaomt + tmpQuat.w * sinat) * sinadiv);
                return tmpQuat;
            }
            else
            {
                return Lerp(q1, tmpQuat, t);
            }
        }
        //-------------------------------------------------
        public void Set(FFloat new_x, FFloat new_y, FFloat new_z, FFloat new_w)
        {
            x = new_x;
            y = new_y;
            z = new_z;
            w = new_w;
        }
        //-------------------------------------------------
        public void SetFromToRotation(FVector3 fromDirection, FVector3 toDirection)
        {
            this = FromToRotation(fromDirection, toDirection);
        }
        //-------------------------------------------------
        public void SetLookRotation(FVector3 view)
        {
            this = LookRotation(view);
        }
        //-------------------------------------------------
        public void SetLookRotation(FVector3 view,  FVector3 up)
        {
            this = LookRotation(view, up);
        }
        //-------------------------------------------------
        public void ToAngleAxis(out FFloat angle, out FVector3 axis)
        {
            angle = 2 * FMath.Acos(w);
            if (angle == 0) 
            {
                axis = FVector3.right;
                return;
            }

            FFloat div = 1 / FMath.Sqrt(1 - w * w);
            axis = new FVector3(x * div, y * div, z * div);
            angle = angle * 180 / FMath.PI;
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", x, y, z, w);
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^
                   this.w.GetHashCode() >> 1;
        }
        //-------------------------------------------------
        public override bool Equals(object other)
        {
            return this == (FQuaternion) other;
        }

#endregion

#region private functions

        private FVector3 MatrixToEuler(FMatrix3x3 m)
        {
            FVector3 v = new FVector3();
            if (m[1, 2] < 1) 
            {
                if (m[1, 2] > -1) 
                {
                    v.x = FMath.Asin(-m[1, 2]);
                    v.y = FMath.Atan2(m[0, 2], m[2, 2]);
                    v.z = FMath.Atan2(m[1, 0], m[1, 1]);
                }
                else
                {
                    v.x = FMath.PI * FFloat.half;
                    v.y = FMath.Atan2(m[0, 1], m[0, 0]);
                    v.z = (FFloat) 0;
                }
            }
            else 
            {
                v.x = -FMath.PI * FFloat.half;
                v.y = FMath.Atan2(-m[0, 1], m[0, 0]);
                v.z = (FFloat) 0;
            }

            for (int i = 0; i < 3; i++)
            {
                if (v[i] < 0) 
                {
                    v[i] += FMath.PI2;
                }
                else if (v[i] > FMath.PI2)
                {
                    v[i] -= FMath.PI2;
                }
            }

            return v;
        }
        //-------------------------------------------------
        public static FMatrix3x3 QuaternionToMatrix3x3(FQuaternion quat)
        {
            FMatrix3x3 m = new FMatrix3x3();

            FFloat x = quat.x * 2;
            FFloat y = quat.y * 2;
            FFloat z = quat.z * 2;
            FFloat xx = quat.x * x;
            FFloat yy = quat.y * y;
            FFloat zz = quat.z * z;
            FFloat xy = quat.x * y;
            FFloat xz = quat.x * z;
            FFloat yz = quat.y * z;
            FFloat wx = quat.w * x;
            FFloat wy = quat.w * y;
            FFloat wz = quat.w * z;

            m[0] = 1 - (yy + zz);
            m[1] = xy + wz;
            m[2] = xz - wy;

            m[3] = xy - wz;
            m[4] = 1 - (xx + zz);
            m[5] = yz + wx;

            m[6] = xz + wy;
            m[7] = yz - wx;
            m[8] = 1 - (xx + yy);

            return m;
        }
        //-------------------------------------------------
        internal static FQuaternion MatrixToQuaternion(FMatrix3x3 m)
        {
            FQuaternion quat = new FQuaternion();

            FFloat fTrace = m[0, 0] + m[1, 1] + m[2, 2];
            FFloat root;

            if (fTrace > 0)
            {
                root = FMath.Sqrt(fTrace + 1);
                quat.w = FFloat.half * root;
                root = FFloat.half / root;
                quat.x = (m[2, 1] - m[1, 2]) * root;
                quat.y = (m[0, 2] - m[2, 0]) * root;
                quat.z = (m[1, 0] - m[0, 1]) * root;
            }
            else 
            {
                int[] s_iNext = new int[] {1, 2, 0};
                int i = 0;
                if (m[1, 1] > m[0, 0])
                {
                    i = 1;
                }

                if (m[2, 2] > m[i, i])
                {
                    i = 2;
                }

                int j = s_iNext[i];
                int k = s_iNext[j];

                root = FMath.Sqrt(m[i, i] - m[j, j] - m[k, k] + 1);
                if (root < 0) 
                {
                    throw new IndexOutOfRangeException("error!");
                }

                quat[i] = FFloat.half * root;
                root = FFloat.half / root;
                quat.w = (m[k, j] - m[j, k]) * root;
                quat[j] = (m[j, i] + m[i, j]) * root;
                quat[k] = (m[k, i] + m[i, k]) * root;
            }

            FFloat nor = FMath.Sqrt(Dot(quat, quat));
            quat = new FQuaternion(quat.x / nor, quat.y / nor, quat.z / nor, quat.w / nor);

            return quat;
        }
        //-------------------------------------------------
        public static FMatrix4x4 QuaternionToMatrix4x4(FQuaternion q)
        {
            FMatrix4x4 m = new FMatrix4x4();

            FFloat x = q.x * 2;
            FFloat y = q.y * 2;
            FFloat z = q.z * 2;
            FFloat xx = q.x * x;
            FFloat yy = q.y * y;
            FFloat zz = q.z * z;
            FFloat xy = q.x * y;
            FFloat xz = q.x * z;
            FFloat yz = q.y * z;
            FFloat wx = q.w * x;
            FFloat wy = q.w * y;
            FFloat wz = q.w * z;

            // Calculate 3x3 matrix from orthonormal basis
            m[0] = 1 - (yy + zz);
            m[1] = xy + wz;
            m[2] = xz - wy;
            m[3] = 0;

            m[4] = xy - wz;
            m[5] = 1 - (xx + zz);
            m[6] = yz + wx;
            m[7] = 0;

            m[8] = xz + wy;
            m[9] = yz - wx;
            m[10] = 1 - (xx + yy);
            m[11] = 0;

            m[12] = 0;
            m[13] = 0;
            m[14] = 0;
            m[15] = 1;

            return m;
        }
        //-------------------------------------------------
        public static FQuaternion MatrixToQuaternion(FMatrix4x4 m)
        {
            FMatrix3x3 mat = new FMatrix3x3(m[0, 0], m[0, 1], m[0, 2],
                            m[1, 0], m[1, 1], m[1, 2],
                            m[2, 0], m[2, 1], m[2, 2]);
            return MatrixToQuaternion(mat);
        }
        //-------------------------------------------------
        private static FMatrix3x3 LookRotationToMatrix(FVector3 viewVec, FVector3 upVec)
        {
            FVector3 z = viewVec;
            FMatrix3x3 m = new FMatrix3x3();

            FFloat mag = z.magnitude;
            if (mag <= 0) 
            {
                m = FMatrix3x3.identity;
            }

            z /= mag;

            FVector3 x = FMath.Cross(upVec, z);
            mag = x.magnitude;
            if (mag <= 0)
            {
                m = FMatrix3x3.identity;
            }

            x /= mag;

            FVector3 y = FMath.Cross(z, x);

            m[0, 0] = x.x;
            m[0, 1] = y.x;
            m[0, 2] = z.x;
            m[1, 0] = x.y;
            m[1, 1] = y.y;
            m[1, 2] = z.y;
            m[2, 0] = x.z;
            m[2, 1] = y.z;
            m[2, 2] = z.z;

            return m;
        }

#endregion

#region operator

        //-------------------------------------------------
        public static FQuaternion operator *(FQuaternion lhs, FQuaternion rhs)
        {
            return new FQuaternion(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }
        //-------------------------------------------------
        public static FVector3 operator *(FQuaternion rotation, FVector3 point)
        {
            FFloat _2x = rotation.x * 2;
            FFloat _2y = rotation.y * 2;
            FFloat _2z = rotation.z * 2;
            FFloat _2xx = rotation.x * _2x;
            FFloat _2yy = rotation.y * _2y;
            FFloat _2zz = rotation.z * _2z;
            FFloat _2xy = rotation.x * _2y;
            FFloat _2xz = rotation.x * _2z;
            FFloat _2yz = rotation.y * _2z;
            FFloat _2xw = rotation.w * _2x;
            FFloat _2yw = rotation.w * _2y;
            FFloat _2zw = rotation.w * _2z;
            var x = (1 - (_2yy + _2zz)) * point.x + (_2xy - _2zw) * point.y + (_2xz + _2yw) * point.z;
            var y = (_2xy + _2zw) * point.x + (1 - (_2xx + _2zz)) * point.y + (_2yz - _2xw) * point.z;
            var z = (_2xz - _2yw) * point.x + (_2yz + _2xw) * point.y + (1 - (_2xx + _2yy)) * point.z;
            return new FVector3(x, y, z);
        }
        //-------------------------------------------------
        public static bool operator ==(FQuaternion lhs, FQuaternion rhs)
        {
            var isEqu = lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;

            return isEqu;
        }
        //-------------------------------------------------
        public static bool operator !=(FQuaternion lhs, FQuaternion rhs)
        {
            return !(lhs == rhs);
        }
        //-------------------------------------------------
        public static implicit operator FQuaternion(UnityEngine.Quaternion value)
        {
            return new FQuaternion(new FFloat(value.x), new FFloat(value.y), new FFloat(value.z), new FFloat(value.w));
        }
        //-------------------------------------------------
        public static implicit operator UnityEngine.Quaternion(FQuaternion value)
        {
            UnityEngine.Quaternion ret = new UnityEngine.Quaternion();
            ret.x = value.x.ToFloat();
            ret.y = value.y.ToFloat();
            ret.z = value.z.ToFloat();
            ret.w = value.w.ToFloat();
            return ret;
        }
#endregion
    }
}
/*
*/
#endif