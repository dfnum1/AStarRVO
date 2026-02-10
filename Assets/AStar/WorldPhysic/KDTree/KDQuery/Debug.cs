
#define KDTREE_VISUAL_DEBUG
using UnityEngine;

namespace Framework.Physic.RVO
{
    public partial class KDQuery 
    {
        // uses gizmos
        public void DrawLastQuery() 
        {
            Color start = Color.blue;
            Color end   = Color.cyan;

            start.a = 0.25f;
            end.a = 0.25f;

            for(int i = 0; i < queryIndex; i++) 
            {
                float val = i / (float)queryIndex;

                Gizmos.color = Color.Lerp(end, start, val);

#if USE_FIXEDMATH
                ExternEngine.Bounds b = queueArray[i].node.bounds.Bounds;
#else
                Bounds b = queueArray[i].node.bounds.Bounds;
#endif
                Gizmos.DrawWireCube(b.center, b.size);
            }
        }
    }

}