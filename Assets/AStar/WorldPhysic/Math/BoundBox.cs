/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	BoundBox
作    者:	HappLI
描    述:	BoundBox
*********************************************************************/
using UnityEngine;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
namespace Framework.Physic.RVO
{
    //    1-----0
    //   /|    /|
    //  3-----2 |
    //  | 5-----4
    //  |/    |/
    //  7-----6   
    public struct BoundBox
    {
        private FVector3 m_Min;
        private FVector3 m_Max;
        private FMatrix4x4 m_Transform;

        public BoundBox(FVector3 min, FVector3 max)
        {
            m_Transform = FMatrix4x4.identity;
            m_Min = FVector3.Min(min, max);
            m_Max = FVector3.Max(min, max);
        }
        //-------------------------------------------------
        public BoundBox(BoundBox box)
        {
            m_Transform = FMatrix4x4.identity;
            m_Min = box.m_Min;
            m_Max = box.m_Max;
        }
        //-------------------------------------------------
        public void SetTransform(FMatrix4x4 mtWorld)
        {
            m_Transform = mtWorld;
        }
        //-------------------------------------------------
        public FMatrix4x4 GetTransform()
        {
            return m_Transform;
        }
        //-------------------------------------------------
        public void Clear()
        {
            m_Transform = FMatrix4x4.identity;
            m_Min = FVector3.zero;
            m_Max = FVector3.zero;
        }
        //-------------------------------------------------
        public FVector3 GetCenter(bool bWorld = false)
        {
            if (bWorld) return (GetMax(bWorld)+GetMin(bWorld))*0.5f;
            return (m_Max + m_Min) * 0.5f;
        }
        //-------------------------------------------------
        public FVector3 GetHalf()
        {
            return (m_Max - m_Min)* 0.5f;
        }
        //-------------------------------------------------
        public FVector3 GetSize()
        {
            return m_Max - m_Min;
        }
        //-------------------------------------------------
        public FVector3 GetMin(bool bWorld = false)
        {
            if (bWorld) return m_Transform.MultiplyPoint(m_Min);
            return m_Min;
        }
        //-------------------------------------------------
        public FVector3 GetMax(bool bWorld = false)
        {
            if (bWorld) return m_Transform.MultiplyPoint(m_Max);
            return m_Max;
        }
        //-------------------------------------------------
        public FFloat GetBoundSize()
        {
            return (m_Max - m_Min).magnitude;
        }
        //-------------------------------------------------
        public FFloat GetBoundSizeSqr()
        {
            return (m_Max - m_Min).sqrMagnitude;
        }
        //-------------------------------------------------
        public void Set(FVector3 min, FVector3 max)
        {
            m_Min = FVector3.Min(min, max);
            m_Max = FVector3.Max(min, max);
        }
        //-------------------------------------------------
        public void Set(BoundBox box)
        {
            m_Min = FVector3.Min(box.m_Min, box.m_Max);
            m_Max = FVector3.Max(box.m_Min, box.m_Max);
        }
        //-------------------------------------------------
        public bool RayHit( Ray ray)
        {
            Vector3 min = GetMin(true);
            Vector3 max = GetMax(true);
#if !USE_SERVER
            UnityEngine.Bounds bounds = new UnityEngine.Bounds();
            bounds.SetMinMax(min, max);
            if(bounds.IntersectRay(ray)) return true;
#endif
            Vector3 direction = ray.direction;
            Vector3 point = ray.origin;
            float tmin, tmax;
            float idirectionx = (Mathf.Abs(ray.direction.x) > 0.00001f)?(1/ray.direction.x):0;
            float idirectiony = (Mathf.Abs(ray.direction.y) > 0.00001f) ? (1 /ray.direction.y):0;
            if (ray.direction.x >= 0.0f)
            {
                tmin = (min.x - point.x) * idirectionx;
                tmax = (max.x - point.x) * idirectionx;
            }
            else
            {
                tmin = (max.x - point.x) * idirectionx;
                tmax = (min.x - point.x) * idirectionx;
            }
            float tymin, tymax;
            if (direction.y >= 0.0f)
            {
                tymin = (min.y - point.y) * idirectiony;
                tymax = (max.y - point.y) * idirectiony;
            }
            else
            {
                tymin = (max.y - point.y) * idirectiony;
                tymax = (min.y - point.y) * idirectiony;
            }
            if ((tmin > tymax) || (tmax < tymin)) return false;
            if (tmin < tymin) tmin = tymin;
            if (tmax > tymax) tmax = tymax;
            float tzmin, tzmax;
            float idirectionz = (Mathf.Abs(ray.direction.z) > 0.00001f)?(1 /direction.z):0;
            if (direction.z >= 0.0f)
            {
                tzmin = (min.z - point.z) * idirectionz;
                tzmax = (max.z - point.z) * idirectionz;
            }
            else
            {
                tzmin = (max.z - point.z) * idirectionz;
                tzmax = (min.z - point.z) * idirectionz;
            }
            if ((tmin > tzmax) || (tmax < tzmin)) return false;
            if (tmin < tzmin) tmin = tzmin;
            if (tmax > tzmax) tmax = tzmax;
            return (tmax > 0.0f && tmin < 1.0f);
            //  return Base.IntersetionUtil.CU_LineOBBIntersection(intersetion,ray.origin, ray.origin + ray.direction * 1000, GetCenter(), GetHalf(), m_Transform);
        }
        //-------------------------------------------------
        public bool IsInView(FMatrix4x4 culling)
        {
            FVector3 center = (m_Min + m_Max) * 0.5f;
            FVector3 half = m_Max - center;
            FVector3 vTransCenter = m_Transform.MultiplyPoint(center);

            FVector3 dir = m_Transform.GetColumn(2); //dir
            FVector3 up = m_Transform.GetColumn(1); // up
            FVector3 right = m_Transform.GetColumn(0); //right
            if (RVOMath.PositionInView(culling, vTransCenter + right * half.x + up * half.y + dir * half.z)) return true;
            if (RVOMath.PositionInView(culling, vTransCenter - right * half.x + up * half.y + dir * half.z)) return true;
            if (RVOMath.PositionInView(culling, vTransCenter + right * half.x + up * half.y - dir * half.z)) return true;
            if (RVOMath.PositionInView(culling, vTransCenter - right * half.x + up * half.y - dir * half.z)) return true;
            if (RVOMath.PositionInView(culling, vTransCenter + right * half.x - up * half.y + dir * half.z)) return true;
            if (RVOMath.PositionInView(culling, vTransCenter - right * half.x - up * half.y + dir * half.z)) return true;
            if (RVOMath.PositionInView(culling, vTransCenter + right * half.x - up * half.y - dir * half.z)) return true;
            if (RVOMath.PositionInView(culling, vTransCenter - right * half.x - up * half.y - dir * half.z)) return true;
            return false;
        }
    }
}
