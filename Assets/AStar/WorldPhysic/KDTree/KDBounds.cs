/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	KDNode
作    者:	HappLI
描    述:	Kd树节包围盒
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using Bounds = UnityEngine.Bounds;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
namespace Framework.Physic.RVO
{
    public struct KDBounds
    {
        public FVector3 min;
        public FVector3 max;

        public FVector3 size 
        {
            get
            {
                return max - min;
            }
        }

        // returns unity bounds
        public Bounds Bounds 
        {
            get
            {
                return new Bounds( (min + max) / 2, (max - min));
            }
        }
        public FVector3 ClosestPoint(FVector3 point)
        {
            // X axis
            if (point.x < min.x) point.x = min.x;
            else 
            if(point.x > max.x) point.x = max.x;
            
            // Y axis
            if(point.y < min.y) point.y = min.y;
            else 
            if(point.y > max.y) point.y = max.y;
            
            // Z axis
            if(point.z < min.z) point.z = min.z;
            else 
            if(point.z > max.z) point.z = max.z;
            
            return point;
        }
        //-----------------------------------------------------
        public bool Culling(FMatrix4x4 clipMatrix)
        {
            if (PositionInView(clipMatrix, new FVector3(max.x, max.y, max.z))) return false;
            if (PositionInView(clipMatrix, new FVector3(max.x, min.y, max.z))) return false;
            if (PositionInView(clipMatrix, new FVector3(max.x, min.y, min.z))) return false;
            if (PositionInView(clipMatrix, new FVector3(max.x, max.y, min.z))) return false;
            if (PositionInView(clipMatrix, new FVector3(min.x, min.y, min.z))) return false;
            if (PositionInView(clipMatrix, new FVector3(min.x, min.y, max.z))) return false;
            if (PositionInView(clipMatrix, new FVector3(min.x, max.y, max.z))) return false;
            if (PositionInView(clipMatrix, new FVector3(min.x, max.y, min.z))) return false;
            return true;
        }
        //-----------------------------------------------------
        public bool PositionInView(FMatrix4x4 clipMatrix, FVector3 worldPos, float factor = 1f)
        {
            worldPos = clipMatrix.MultiplyPoint(worldPos);

#if USE_FIXEDMATH
            if (FMath.Abs(worldPos.x) < factor
             && FMath.Abs(worldPos.y) < factor
             && worldPos.z <= factor)
            {
                return true;
            }
#else
            if (System.Math.Abs(worldPos.x) < factor
             && System.Math.Abs(worldPos.y) < factor
             && worldPos.z <= factor)
            {
                return true;
            }
#endif
            return false;
        }
    }
}