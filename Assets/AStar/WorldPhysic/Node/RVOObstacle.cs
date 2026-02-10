/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	RVOObstacle
作    者:	HappLI
描    述:	障碍节点
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using System.Collections.Generic;
using FVector3 = UnityEngine.Vector3;
using FFloat = System.Single;
#endif

namespace Framework.Physic.RVO
{
    internal class RVOObstacle
    {
        public bool IsCircle;
        public bool IsConvex;
        public FVector3 point;
        public FVector3 unitDir;
        public int id;
        public int group;

        public RVOObstacle next;
        public RVOObstacle prev;

        //------------------------------------------------------
        public bool IsPointInPolygon(FVector3 point)
        {
            bool inside = false;
            RVOObstacle start = this;
            RVOObstacle curr = this;
            do
            {
                RVOObstacle next = curr.next;
                if (next == null) break;
                FVector3 a = curr.point;
                FVector3 b = next.point;

                // 只考虑 x/z 平面
                if (((a.z > point.z) != (b.z > point.z)) &&
                    (point.x < (b.x - a.x) * (point.z - a.z) / (b.z - a.z + 1e-8f) + a.x))
                {
                    inside = !inside;
                }
                curr = next;
            } while (curr != start);
            return inside;
        }
        //------------------------------------------------------
        public FVector3 GetNearestPointOnPolygon(FVector3 point, FFloat radius)
        {
            if(IsCircle)
            {
                FVector3 target = this.point + (point - this.point).normalized * (this.unitDir.x + radius*1.5f);
                return target;
            }
            RVOObstacle start = this;
            RVOObstacle curr = this;
            FVector3 nearest = curr.point;
            float minDist = float.MaxValue;
            do
            {
                RVOObstacle next = curr.next;
                if (next == null) break;
                FVector3 a = curr.point;
                FVector3 b = next.point;
                FVector3 proj = RVOMath.ProjectPointToSegment(point, a, b);
                float dist = (proj - point).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = proj + (proj - point).normalized*(radius*2);
                }
                curr = next;
            } while (curr != start);
            return nearest;
        }
        //------------------------------------------------------
        public void Destroy()
        {
            IsCircle = false;
            IsConvex = false;
            point = FVector3.zero;
            unitDir = FVector3.zero;
            id = 0;
            group = 0;
            prev = null;
            next = null;
        }

    }
}