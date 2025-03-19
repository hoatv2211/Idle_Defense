using UnityEngine;

namespace Utilities.Components
{
    public class OptimizedScrollItem : MonoBehaviour
    {
        protected int mIndex = -1;
        public int Index => mIndex;
        public virtual void UpdateContent(int pIndex)
        {
            mIndex = pIndex;
        }
    }
}
