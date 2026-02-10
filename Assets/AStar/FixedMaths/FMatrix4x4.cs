#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:15:2022  13:41
类    名: 	FMatrix4x4
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace ExternEngine
{
    public struct FMatrix4x4 : IEquatable<FMatrix4x4>
    {
        public static readonly FMatrix4x4
            zero = new FMatrix4x4(FVector4.zero, FVector4.zero, FVector4.zero, FVector4.zero);

        public static readonly FMatrix4x4 identity = new FMatrix4x4(new FVector4(true, FFloat.Precision, 0, 0,0),
            new FVector4(true, 0, FFloat.Precision, 0, 0), new FVector4(true, 0, 0, FFloat.Precision, 0), new FVector4(true, 0, 0, 0, FFloat.Precision));

        // mRowCol  列优先存储
        public long m00;
        public long m10;
        public long m20;
        public long m30;
        public long m01;
        public long m11;
        public long m21;
        public long m31;
        public long m02;
        public long m12;
        public long m22;
        public long m32;
        public long m03;
        public long m13;
        public long m23;
        public long m33;

        public FQuaternion rotation
        {
            get
            {
                return this.GetRotation();
            }
        }
        //-------------------------------------------------
        public bool isIdentity
        {
            get
            {
                return this.IsIdentity();
            }
        }
        //-------------------------------------------------
        public FMatrix4x4 inverse
        {
            get
            {
                return FMatrix4x4.Inverse(this);
            }
        }
        //-------------------------------------------------
        public FMatrix4x4 transpose
        {
            get
            {
                return FMatrix4x4.Transpose(this);
            }
        }
        //-------------------------------------------------
        public FFloat this[int row, int column]
        {
            get
            {
                return this[row + column * 4];
            }
            set
            {
                this[row + column * 4] = value;
            }
        }
        //-------------------------------------------------
        public FFloat this[int index]
        {
            get
            {
                FFloat result;
                switch (index)
                {
                    case 0:
                        result = new FFloat(true, this.m00);
                        break;
                    case 1:
                        result = new FFloat(true, this.m10);
                        break;
                    case 2:
                        result = new FFloat(true, this.m20);
                        break;
                    case 3:
                        result = new FFloat(true, this.m30);
                        break;
                    case 4:
                        result = new FFloat(true, this.m01);
                        break;
                    case 5:
                        result = new FFloat(true, this.m11);
                        break;
                    case 6:
                        result = new FFloat(true, this.m21);
                        break;
                    case 7:
                        result = new FFloat(true, this.m31);
                        break;
                    case 8:
                        result = new FFloat(true, this.m02);
                        break;
                    case 9:
                        result = new FFloat(true, this.m12);
                        break;
                    case 10:
                        result = new FFloat(true, this.m22);
                        break;
                    case 11:
                        result = new FFloat(true, this.m32);
                        break;
                    case 12:
                        result = new FFloat(true, this.m03);
                        break;
                    case 13:
                        result = new FFloat(true, this.m13);
                        break;
                    case 14:
                        result = new FFloat(true, this.m23);
                        break;
                    case 15:
                        result = new FFloat(true, this.m33);
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
                return result;
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.m00 = value._val;
                        break;
                    case 1:
                        this.m10 = value._val;
                        break;
                    case 2:
                        this.m20 = value._val;
                        break;
                    case 3:
                        this.m30 = value._val;
                        break;
                    case 4:
                        this.m01 = value._val;
                        break;
                    case 5:
                        this.m11 = value._val;
                        break;
                    case 6:
                        this.m21 = value._val;
                        break;
                    case 7:
                        this.m31 = value._val;
                        break;
                    case 8:
                        this.m02 = value._val;
                        break;
                    case 9:
                        this.m12 = value._val;
                        break;
                    case 10:
                        this.m22 = value._val;
                        break;
                    case 11:
                        this.m32 = value._val;
                        break;
                    case 12:
                        this.m03 = value._val;
                        break;
                    case 13:
                        this.m13 = value._val;
                        break;
                    case 14:
                        this.m23 = value._val;
                        break;
                    case 15:
                        this.m33 = value._val;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }
        //-------------------------------------------------
        private FQuaternion GetRotation()
        {
            return FQuaternion.MatrixToQuaternion(this);
        }
        //-------------------------------------------------
        private bool IsIdentity()
        {
            return FMatrix4x4.IsIdentity_Injected(ref this);
        }
        //-------------------------------------------------
        public static FMatrix4x4 TRS(FVector3 pos, FQuaternion q, FVector3 s)
        {
            FMatrix4x4 result;
            FMatrix4x4.TRS_Injected(ref pos, ref q, ref s, out result);
            return result;
        }
        //-------------------------------------------------
        public void SetTRS(FVector3 pos, FQuaternion q, FVector3 s)
        {
            this = FMatrix4x4.TRS(pos, q, s);
        }
        //-------------------------------------------------
        public static FMatrix4x4 Inverse(FMatrix4x4 m)
        {
            FMatrix4x4 result;
            FMatrix4x4.Inverse_Injected(ref m, out result);
            return result;
        }
        //-------------------------------------------------
        public static FMatrix4x4 Transpose(FMatrix4x4 m)
        {
            FMatrix4x4 result = m;
            FMatrix4x4.Transpose_Injected(ref result);
            return result;
        }
        //-------------------------------------------------
        public static FMatrix4x4 Ortho(FFloat left, FFloat right, FFloat bottom, FFloat top, FFloat zNear, FFloat zFar)
        {
            FMatrix4x4 result;
            FMatrix4x4.Ortho_Injected(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }
        //-------------------------------------------------
        public static FMatrix4x4 Perspective(FFloat fov, FFloat aspect, FFloat zNear, FFloat zFar)
        {
            FMatrix4x4 result;
            FMatrix4x4.Perspective_Injected(fov, aspect, zNear, zFar, out result);
            return result;
        }
        //-------------------------------------------------
        public static FMatrix4x4 LookAt(FVector3 from, FVector3 to, FVector3 up)
        {
            FMatrix4x4 result;
            FMatrix4x4.LookAt_Injected(ref from, ref to, ref up, out result);
            return result;
        }
        //-------------------------------------------------
        public static FMatrix4x4 Frustum(FFloat left, FFloat right, FFloat bottom, FFloat top, FFloat zNear, FFloat zFar)
        {
            FMatrix4x4 result;
            FMatrix4x4.Frustum_Injected(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }
        //-------------------------------------------------
        public FMatrix4x4(FVector4 column0, FVector4 column1, FVector4 column2, FVector4 column3)
        {
            this.m00 = column0.x;
            this.m01 = column1.x;
            this.m02 = column2.x;
            this.m03 = column3.x;
            this.m10 = column0.y;
            this.m11 = column1.y;
            this.m12 = column2.y;
            this.m13 = column3.y;
            this.m20 = column0.z;
            this.m21 = column1.z;
            this.m22 = column2.z;
            this.m23 = column3.z;
            this.m30 = column0.w;
            this.m31 = column1.w;
            this.m32 = column2.w;
            this.m33 = column3.w;
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return this.GetColumn(0).GetHashCode() ^ this.GetColumn(1).GetHashCode() << 2 ^ this.GetColumn(2).GetHashCode() >> 2 ^ this.GetColumn(3).GetHashCode() >> 1;
        }
        //-------------------------------------------------
        public override bool Equals(object other)
        {
            bool flag = !(other is FMatrix4x4);
            return !flag && this.Equals((FMatrix4x4)other);
        }
        //-------------------------------------------------
        public bool Equals(FMatrix4x4 other)
        {
            return this.GetColumn(0).Equals(other.GetColumn(0)) && this.GetColumn(1).Equals(other.GetColumn(1)) && this.GetColumn(2).Equals(other.GetColumn(2)) && this.GetColumn(3).Equals(other.GetColumn(3));
        }
        //-------------------------------------------------
        public static FMatrix4x4 operator *(FMatrix4x4 lhs, FMatrix4x4 rhs)
        {
            FMatrix4x4 result;
            result.m00 = (lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30)/FFloat.Precision;
            result.m01 = (lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31)/FFloat.Precision;
            result.m02 = (lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32)/FFloat.Precision;
            result.m03 = (lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33)/FFloat.Precision;
            result.m10 = (lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30)/FFloat.Precision;
            result.m11 = (lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31)/FFloat.Precision;
            result.m12 = (lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32)/FFloat.Precision;
            result.m13 = (lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33)/FFloat.Precision;
            result.m20 = (lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30)/FFloat.Precision;
            result.m21 = (lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31)/FFloat.Precision;
            result.m22 = (lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32)/FFloat.Precision;
            result.m23 = (lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33)/FFloat.Precision;
            result.m30 = (lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30)/FFloat.Precision;
            result.m31 = (lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31)/FFloat.Precision;
            result.m32 = (lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32)/FFloat.Precision;
            result.m33 = (lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33)/FFloat.Precision;
            return result;
        }
        //-------------------------------------------------
        public static implicit operator FMatrix4x4(UnityEngine.Matrix4x4 value)
        {
            FMatrix4x4 ret = new FMatrix4x4();
            ret.m00 = FFloat.FloatToLong(value.m00);
            ret.m01 = FFloat.FloatToLong(value.m01);
            ret.m02 = FFloat.FloatToLong(value.m02);
            ret.m03 = FFloat.FloatToLong(value.m03);
            ret.m10 = FFloat.FloatToLong(value.m10);
            ret.m11 = FFloat.FloatToLong(value.m11);
            ret.m12 = FFloat.FloatToLong(value.m12);
            ret.m13 = FFloat.FloatToLong(value.m13);
            ret.m20 = FFloat.FloatToLong(value.m20);
            ret.m21 = FFloat.FloatToLong(value.m21);
            ret.m22 = FFloat.FloatToLong(value.m22);
            ret.m23 = FFloat.FloatToLong(value.m23);
            ret.m30 = FFloat.FloatToLong(value.m30);
            ret.m31 = FFloat.FloatToLong(value.m31);
            ret.m32 = FFloat.FloatToLong(value.m32);
            ret.m33 = FFloat.FloatToLong(value.m33);
            return ret;
        }
        //-------------------------------------------------
        public static implicit operator UnityEngine.Matrix4x4(FMatrix4x4 value)
        {
            UnityEngine.Matrix4x4 ret = new UnityEngine.Matrix4x4();
            ret.m00 = value.m00*FFloat.PrecisionFactor;
            ret.m01 = value.m01*FFloat.PrecisionFactor;
            ret.m02 = value.m02*FFloat.PrecisionFactor;
            ret.m03 = value.m03*FFloat.PrecisionFactor;
            ret.m10 = value.m10*FFloat.PrecisionFactor;
            ret.m11 = value.m11*FFloat.PrecisionFactor;
            ret.m12 = value.m12*FFloat.PrecisionFactor;
            ret.m13 = value.m13*FFloat.PrecisionFactor;
            ret.m20 = value.m20*FFloat.PrecisionFactor;
            ret.m21 = value.m21*FFloat.PrecisionFactor;
            ret.m22 = value.m22*FFloat.PrecisionFactor;
            ret.m23 = value.m23*FFloat.PrecisionFactor;
            ret.m30 = value.m30*FFloat.PrecisionFactor;
            ret.m31 = value.m31*FFloat.PrecisionFactor;
            ret.m32 = value.m32*FFloat.PrecisionFactor;
            ret.m33 = value.m33 * FFloat.PrecisionFactor;
            return ret;
        }
        //-------------------------------------------------
        public static FVector4 operator *(FMatrix4x4 lhs, FVector4 vector)
        {
            FVector4 result;
            result._x = (lhs.m00 * vector._x + lhs.m01 * vector._y + lhs.m02 * vector._z + lhs.m03 * vector._w) / FFloat.Precision;
            result._y = (lhs.m10 * vector._x + lhs.m11 * vector._y + lhs.m12 * vector._z + lhs.m13 * vector._w) / FFloat.Precision;
            result._z = (lhs.m20 * vector._x + lhs.m21 * vector._y + lhs.m22 * vector._z + lhs.m23 * vector._w) / FFloat.Precision;
            result._w = (lhs.m30 * vector._x + lhs.m31 * vector._y + lhs.m32 * vector._z + lhs.m33 * vector._w) / FFloat.Precision;
            return result;
        }
        //-------------------------------------------------
        public static bool operator ==(FMatrix4x4 lhs, FMatrix4x4 rhs)
        {
            return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);
        }
        //-------------------------------------------------
        public static bool operator !=(FMatrix4x4 lhs, FMatrix4x4 rhs)
        {
            return !(lhs == rhs);
        }
        //-------------------------------------------------
        public FVector4 GetColumn(int index)
        {
            FVector4 result;
            switch (index)
            {
                case 0:
                    result = new FVector4(true, this.m00, this.m10, this.m20, this.m30);
                    break;
                case 1:
                    result = new FVector4(true, this.m01, this.m11, this.m21, this.m31);
                    break;
                case 2:
                    result = new FVector4(true, this.m02, this.m12, this.m22, this.m32);
                    break;
                case 3:
                    result = new FVector4(true, this.m03, this.m13, this.m23, this.m33);
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
            return result;
        }
        //-------------------------------------------------
        public FVector4 GetRow(int index)
        {
            FVector4 result;
            switch (index)
            {
                case 0:
                    result = new FVector4(true, this.m00, this.m01, this.m02, this.m03);
                    break;
                case 1:
                    result = new FVector4(true, this.m10, this.m11, this.m12, this.m13);
                    break;
                case 2:
                    result = new FVector4(true, this.m20, this.m21, this.m22, this.m23);
                    break;
                case 3:
                    result = new FVector4(true, this.m30, this.m31, this.m32, this.m33);
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
            return result;
        }
        //-------------------------------------------------
        public void SetColumn(int index, FVector4 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
            this[3, index] = column.w;
        }
        //-------------------------------------------------
        public void SetRow(int index, FVector4 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
        }
        //-------------------------------------------------
        public FVector3 MultiplyPoint(FVector3 point)
        {
            FVector3 result;
            result._x = (this.m00 * point._x + this.m01 * point._y + this.m02 * point._z) / FFloat.Precision + this.m03;
            result._y = (this.m10 * point._x + this.m11 * point._y + this.m12 * point._z) / FFloat.Precision + this.m13;
            result._z = (this.m20 * point._x + this.m21 * point._y + this.m22 * point._z) / FFloat.Precision + this.m23;
            long num = (this.m30 * point._x + this.m31 * point._y + this.m32 * point._z) / FFloat.Precision + this.m33;
            if(num!=0)
            {
                result._x = (long)(result._x* FFloat.Precision) / num;
                result._y = (long)(result._y* FFloat.Precision) / num ;
                result._z = (long)(result._z* FFloat.Precision) / num ;
            }

            return result;
        }
        //-------------------------------------------------
        public FVector3 MultiplyPoint3x4(FVector3 point)
        {
            FVector3 vec;
            vec._x = (long)((m00 * point._x + m01 * point._y + m02 * point._z) / FFloat.Precision) + this.m03;
            vec._y = (long)((m10 * point._x + m11 * point._y + m12 * point._z) / FFloat.Precision) + this.m13;
            vec._z = (long)((m20 * point._x + m21 * point._y + m22 * point._z) / FFloat.Precision) + this.m23;
            return vec;
        }
        //-------------------------------------------------
        public FVector3 MultiplyVector(FVector3 vector)
        {
            FVector3 vec;
            vec._x = (long)((m00 * vector._x + m01 * vector._y + m02 * vector._z) / FFloat.Precision);
            vec._y = (long)((m10 * vector._x + m11 * vector._y + m12 * vector._z) / FFloat.Precision);
            vec._z = (long)((m20 * vector._x + m21 * vector._y + m22 * vector._z) / FFloat.Precision);
            return vec;
        }
        //-------------------------------------------------
        public static FMatrix4x4 Scale(FVector3 vector)
        {
            FMatrix4x4 result;
            result.m00 = vector._x;
            result.m01 = 0;
            result.m02 = 0;
            result.m03 = 0;
            result.m10 = 0;
            result.m11 = vector._y;
            result.m12 = 0;
            result.m13 = 0;
            result.m20 = 0;
            result.m21 = 0;
            result.m22 = vector._z;
            result.m23 = 0;
            result.m30 = 0;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = FFloat.one;
            return result;
        }
        //-------------------------------------------------
        public static FMatrix4x4 Translate(FVector3 vector)
        {
            FMatrix4x4 result;
            result.m00 = FFloat.Precision;
            result.m01 = 0;
            result.m02 = 0;
            result.m03 = vector._x;
            result.m10 = 0;
            result.m11 = FFloat.Precision;
            result.m12 = 0;
            result.m13 = vector._y;
            result.m20 = 0;
            result.m21 = 0;
            result.m22 = FFloat.Precision;
            result.m23 = vector._z;
            result.m30 = 0;
            result.m31 = 0;
            result.m32 = 0;
            result.m33 = FFloat.Precision;
            return result;
        }
        //-------------------------------------------------
        public static FMatrix4x4 Rotate(FQuaternion q)
        {
            long num = q.x._val * 2;
            long num2 = q.y._val * 2;
            long num3 = q.z._val * 2;
            long num4 = q.x._val * num/FFloat.Precision;
            long num5 = q.y._val * num2 / FFloat.Precision;
            long num6 = q.z._val * num3 / FFloat.Precision;
            long num7 = q.x._val * num2 / FFloat.Precision;
            long num8 = q.x._val * num3 / FFloat.Precision;
            long num9 = q.y._val * num3 / FFloat.Precision;
            long num10 = q.w._val * num / FFloat.Precision;
            long num11 = q.w._val * num2 / FFloat.Precision;
            long num12 = q.w._val * num3 / FFloat.Precision;
            FMatrix4x4 result;
            result.m00 = FFloat.one - (num5 + num6);
            result.m10 = num7 + num12;
            result.m20 = num8 - num11;
            result.m30 = 0;
            result.m01 = num7 - num12;
            result.m11 = FFloat.one - (num4 + num6);
            result.m21 = num9 + num10;
            result.m31 = 0;
            result.m02 = num8 + num11;
            result.m12 = num9 - num10;
            result.m22 = FFloat.one - (num4 + num5);
            result.m32 = 0;
            result.m03 = 0;
            result.m13 = 0;
            result.m23 = 0;
            result.m33 = FFloat.one;
            return result;
        }
        //-------------------------------------------------
        public override string ToString()
        {
            return string.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n", this.m00,
                this.m01,
                this.m02,
                this.m03,
                this.m10,
                this.m11,
                this.m12,
                this.m13,
                this.m20,
                this.m21,
                this.m22,
                this.m23,
                this.m30,
                this.m31,
                this.m32,
                this.m33);
        }
        //-------------------------------------------------
        private static bool IsIdentity_Injected(ref FMatrix4x4 m)
        {
            if ( m[0, 0] == FFloat.Precision && m[0, 1]== 0                  && m[0, 2] == 0                 && m[0, 3] == 0 &&
                 m[1, 0] == 0               && m[1, 1] == FFloat.Precision   && m[1, 2] == 0                 && m[1, 3] == 0 &&
                 m[2, 0] == 0               && m[2, 1] == 0                 && m[2, 2] == FFloat.Precision   && m[2, 3] == FFloat.Precision &&
                 m[3, 0] == 0               && m[3, 1] == 0                 && m[3, 2] == 0                 && m[3, 3] == FFloat.Precision)
                return true;
            return false;
        }
        //-------------------------------------------------
        private static void TRS_Injected(ref FVector3 pos, ref FQuaternion q, ref FVector3 s, out FMatrix4x4 ret)
        {
            ret = FQuaternion.QuaternionToMatrix4x4(q);

            ret[0] *= s[0];
            ret[1] *= s[0];
            ret[2] *= s[0];

            ret[4] *= s[1];
            ret[5] *= s[1];
            ret[6] *= s[1];

            ret[8] *= s[2];
            ret[9] *= s[2];
            ret[10] *= s[2];

            ret[12] = pos[0];
            ret[13] = pos[1];
            ret[14] = pos[2];
        }
        //-------------------------------------------------
        private static void Inverse_Injected(ref FMatrix4x4 m, out FMatrix4x4 ret)
        {
            ret = FMatrix4x4.identity;
            FFloat det = FFloat.zero;
            det = m[0,0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]);
            det -= m[0,1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]);
            det += m[0,2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
            FFloat idet = FFloat.one / det;

            ret[0,0] = (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) * idet;
            ret[1,0] = -(m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) * idet;
            ret[2,0] = (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]) * idet;
            ret[3,0] = 0;
            ret[0,1] = -(m.m01 * m[2, 2] - m.m02 * m[2, 1]) * idet;
            ret[1,1] = (m[0,0] * m[2, 2] - m.m02 * m[2, 0]) * idet;
            ret[2,1] = -(m[0,0] * m[2, 1] - m.m01 * m[2, 0]) * idet;
            ret[3,1] = 0;
            ret[0,2] = (m.m01 * m[1, 2] - m.m02 * m[1, 1]) * idet;
            ret[1,2] = -(m[0,0] * m[1, 2] - m.m02 * m[1, 0]) * idet;
            ret[2,2] = (m[0,0] * m[1, 1] - m.m01 * m[1, 0]) * idet;
            ret[3,2] = 0;
            ret[0,3] = -(ret[0,0] * m[0,3] + ret[0,1] * m[1,3] + ret[0,2] * m[2,3]);
            ret[1,3] = -(ret.m10 * m[0, 3] + ret[1,1] * m[1,3] + ret[1,2] * m[2,3]);
            ret[2,3] = -(ret.m20 * m[0, 3] + ret[2,1] * m[1,3] + ret[2,2] * m[2,3]);
            ret[3,3] = FFloat.one;
        }
        //-------------------------------------------------
        private static void Transpose_Injected(ref FMatrix4x4 m)
        {
            FMath.Swap(ref m.m01, ref m.m10);
            FMath.Swap(ref m.m02, ref m.m20);
            FMath.Swap(ref m.m03, ref m.m30);
            FMath.Swap(ref m.m12, ref m.m21);
            FMath.Swap(ref m.m13, ref m.m31);
            FMath.Swap(ref m.m23, ref m.m32);
        }
        //-------------------------------------------------
        private static void Ortho_Injected(FFloat left, FFloat right, FFloat bottom, FFloat top, FFloat zNear, FFloat zFar, out FMatrix4x4 ret)
        {
            ret = FMatrix4x4.identity;

            FFloat deltax = right - left;
            FFloat deltay = top - bottom;
            FFloat deltaz = zFar - zNear;

            ret[0, 0] = 2 / deltax;
            ret[0, 3] = -(right + left) / deltax;
            ret[1, 1] = 2 / deltay;
            ret[1, 3] = -(top + bottom) / deltay;
            ret[2, 2] = -2 / deltaz;
            ret[2, 3] = -(zFar + zNear) / deltaz;
        }
        //-------------------------------------------------
        private static void Perspective_Injected(FFloat fov, FFloat aspect, FFloat zNear, FFloat zFar, out FMatrix4x4 ret)
        {
            ret = FMatrix4x4.identity;
            FFloat cotangent, deltaZ;
            FFloat radians = FMath.Deg2Rad*(fov / 2);
            cotangent = FMath.Cos(radians) / FMath.Sin(radians);
            deltaZ = zNear - zFar;

            ret[0, 0] = cotangent / aspect; ret[0, 1] = 0;          ret[0, 2] = 0;                          ret[0, 3] = 0;
            ret[1, 0] = 0;                  ret[1, 1] = cotangent;  ret[1, 2] = 0;                          ret[1, 3] = 0;
            ret[2, 0] = 0;                  ret[2, 1] = 0;          ret[2, 2] = (zFar + zNear) / deltaZ;    ret[2, 3] = 2 * zNear * zFar / deltaZ;
            ret[3, 0] = 0;                  ret[3, 1] = 0;          ret[3, 2] = -1;                         ret[3, 3] = 0;
        }
        //-------------------------------------------------
        private static void LookAt_Injected(ref FVector3 from, ref FVector3 to, ref FVector3 up, out FMatrix4x4 ret)
        {
            FMatrix4x4 m0 = FMatrix4x4.identity;
            ret = FMatrix4x4.identity;
            FVector3 z = (to - from).normalized;
            FVector3 x = FMath.Cross(up, z).normalized;
            FVector3 y = FMath.Cross(z, x).normalized;
            m0.m00 = x._x; m0.m01 = x._y; m0.m02 = x._z; m0.m03 = 0;
            m0.m10 = y._x; m0.m11 = y._y; m0.m12 = y._z; m0.m13 = 0;
            m0.m20 = z._x; m0.m21 = z._y; m0.m22 = z._z; m0.m23 = 0;
            m0.m30 = 0; m0.m31 = 0; m0.m32 = 0; m0.m33 = FFloat.Precision;
            ret = m0*FMatrix4x4.Translate(from);
        }
        //-------------------------------------------------
        private static void Frustum_Injected(FFloat left, FFloat right, FFloat bottom, FFloat top, FFloat nearval, FFloat farval, out FMatrix4x4 ret)
        {
            ret = FMatrix4x4.identity;
            FFloat x, y, a, b, c, d, e;

            x = (2 * nearval) / (right - left);
            y = (2 * nearval) / (top - bottom);
            a = (right + left) / (right - left);
            b = (top + bottom) / (top - bottom);
            c = -(farval + nearval) / (farval - nearval);
            d = -(2 * farval * nearval) / (farval - nearval);
            e = -1;

            ret[0, 0] = x;   ret[0, 1] = 0; ret[0, 2] = a; ret[0, 3] = 0;
            ret[1, 0] = 0;   ret[1, 1] = y; ret[1, 2] = b; ret[1, 3] = 0;
            ret[2, 0] = 0;   ret[2, 1] = 0; ret[2, 2] = c; ret[2, 3] = d;
            ret[3, 0] = 0;   ret[3, 1] = 0; ret[3, 2] = e; ret[3, 3] = 0;
        }
    }
}
#endif