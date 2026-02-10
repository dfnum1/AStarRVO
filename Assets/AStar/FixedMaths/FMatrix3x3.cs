#if USE_FIXEDMATH
/********************************************************************
生成日期:	3:10:2022  11:51
类    名: 	FMatrix3x3
作    者:	HappLI
描    述:	
*********************************************************************/
using System;

namespace ExternEngine
{
    public struct FMatrix3x3 : IEquatable<FMatrix3x3>
    {
        public static readonly FMatrix3x3
            zero = new FMatrix3x3(FVector3.zero, FVector3.zero, FVector3.zero);

        public static readonly FMatrix3x3 identity = new FMatrix3x3(new FVector3(true,FFloat.Precision, 0, 0),
            new FVector3(true,0, FFloat.Precision, 0), new FVector3(true,0, 0, FFloat.Precision));

        // mRowCol  列优先存储
        public long m00;
        public long m10;
        public long m20;
        public long m01;
        public long m11;
        public long m21;
        public long m02;
        public long m12;
        public long m22;

        //-------------------------------------------------
        public FMatrix3x3(FVector3 column0, FVector3 column1, FVector3 column2)
        {
            this.m00 = column0._x;
            this.m01 = column1._x;
            this.m02 = column2._x;
            this.m10 = column0._y;
            this.m11 = column1._y;
            this.m12 = column2._y;
            this.m20 = column0._z;
            this.m21 = column1._z;
            this.m22 = column2._z;
        }
        //-------------------------------------------------
        internal FMatrix3x3(long m00, long m01, long m02, long m10, long m11, long m12, long m20, long m21, long m22)
        {
            this.m00 = m00; this.m10 = m10; this.m20 = m20;
            this.m01 = m01; this.m11 = m11; this.m21 = m21;
            this.m02 = m02; this.m12 = m12; this.m22 = m22; 
        }
        //-------------------------------------------------

        public FFloat this[int row, int column]
        {
            get { return this[row + column * 3]; }
            set { this[row + column * 3] = value; }
        }
        //-------------------------------------------------
        public FFloat this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return new FFloat(true, this.m00);
                    case 1:
                        return new FFloat(true, this.m10);
                    case 2:
                        return new FFloat(true, this.m20);
                    case 3:
                        return new FFloat(true, this.m01);
                    case 4:
                        return new FFloat(true, this.m11);
                    case 5:
                        return new FFloat(true, this.m21);
                    case 6:
                        return new FFloat(true, this.m02);
                    case 7:
                        return new FFloat(true, this.m12);
                    case 8:
                        return new FFloat(true, this.m22);
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
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
                        this.m01 = value._val;
                        break;
                    case 4:
                        this.m11 = value._val;
                        break;
                    case 5:
                        this.m21 = value._val;
                        break;
                    case 6:
                        this.m02 = value._val;
                        break;
                    case 7:
                        this.m12 = value._val;
                        break;
                    case 8:
                        this.m22 = value._val;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }
        //-------------------------------------------------
        public override int GetHashCode()
        {
            return this.GetColumn(0).GetHashCode() ^ this.GetColumn(1).GetHashCode() << 2 ^
                   this.GetColumn(2).GetHashCode() >> 2;
        }
        //-------------------------------------------------
        public override bool Equals(object other)
        {
            if (!(other is FMatrix3x3))
                return false;
            return this.Equals((FMatrix3x3) other);
        }
        //-------------------------------------------------
        public bool Equals(FMatrix3x3 other)
        {
            return this.GetColumn(0).Equals(other.GetColumn(0))
                   && this.GetColumn(1).Equals(other.GetColumn(1))
                   && this.GetColumn(2).Equals(other.GetColumn(2));
        }
        //-------------------------------------------------
        public static FMatrix3x3 operator *(FMatrix3x3 lhs, FMatrix3x3 rhs)
        {
            FMatrix3x3 mat;
            mat.m00 =  (( lhs.m00 *  rhs.m00 +  lhs.m01 *  rhs.m10 +
                            lhs.m02 *  rhs.m20) / FFloat.Precision);
            mat.m01 =  (( lhs.m00 *  rhs.m01 +  lhs.m01 *  rhs.m11 +
                            lhs.m02 *  rhs.m21) / FFloat.Precision);
            mat.m02 =  (( lhs.m00 *  rhs.m02 +  lhs.m01 *  rhs.m12 +
                            lhs.m02 *  rhs.m22) / FFloat.Precision);
            mat.m10 =  (( lhs.m10 *  rhs.m00 +  lhs.m11 *  rhs.m10 +
                            lhs.m12 *  rhs.m20) / FFloat.Precision);
            mat.m11 =  (( lhs.m10 *  rhs.m01 +  lhs.m11 *  rhs.m11 +
                            lhs.m12 *  rhs.m21) / FFloat.Precision);
            mat.m12 =  (( lhs.m10 *  rhs.m02 +  lhs.m11 *  rhs.m12 +
                            lhs.m12 *  rhs.m22) / FFloat.Precision);
            mat.m20 =  (( lhs.m20 *  rhs.m00 +  lhs.m21 *  rhs.m10 +
                            lhs.m22 *  rhs.m20) / FFloat.Precision);
            mat.m21 =  (( lhs.m20 *  rhs.m01 +  lhs.m21 *  rhs.m11 +
                            lhs.m22 *  rhs.m21) / FFloat.Precision);
            mat.m22 =  (( lhs.m20 *  rhs.m02 +  lhs.m21 *  rhs.m12 +
                               lhs.m22 *  rhs.m22) / FFloat.Precision);
            return mat;
        }
        //-------------------------------------------------
        public static FVector3 operator *(FMatrix3x3 lhs, FVector3 vector3)
        {
            FVector3 vec;
            vec._x =  (( lhs.m00 * vector3.x._val +  lhs.m01 *  vector3.y._val +
                              lhs.m02 *  vector3.z._val) / FFloat.Precision);
            vec._y =  (( lhs.m10 *  vector3.x._val +  lhs.m11 *  vector3.y._val +
                              lhs.m12 *  vector3.z._val) / FFloat.Precision);
            vec._z =  (( lhs.m20 *  vector3.x._val +  lhs.m21 *  vector3.y._val +
                              lhs.m22 *  vector3.z._val) / FFloat.Precision);
            return vec;
        }
        //-------------------------------------------------
        public static bool operator ==(FMatrix3x3 lhs, FMatrix3x3 rhs)
        {
            return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) &&
                   lhs.GetColumn(2) == rhs.GetColumn(2);
        }
        //-------------------------------------------------
        public static bool operator !=(FMatrix3x3 lhs, FMatrix3x3 rhs)
        {
            return !(lhs == rhs);
        }
        //-------------------------------------------------
        public FVector3 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return new FVector3(true,this.m00, this.m10, this.m20);
                case 1:
                    return new FVector3(true,this.m01, this.m11, this.m21);
                case 2:
                    return new FVector3(true,this.m02, this.m12, this.m22);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }
        //-------------------------------------------------
        public FVector3 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return new FVector3(true,this.m00, this.m01, this.m02);
                case 1:
                    return new FVector3(true,this.m10, this.m11, this.m12);
                case 2:
                    return new FVector3(true,this.m20, this.m21, this.m22);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }
        //-------------------------------------------------
        public void SetColumn(int index, FVector3 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
        }
        //-------------------------------------------------
        public void SetRow(int index, FVector3 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
        }
    }
}
#endif