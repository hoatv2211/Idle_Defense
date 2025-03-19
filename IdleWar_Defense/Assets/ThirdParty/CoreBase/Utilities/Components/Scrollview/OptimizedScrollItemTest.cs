using UnityEngine.UI;

namespace Utilities.Components
{
    public class OptimizedScrollItemTest : OptimizedScrollItem
    {
        public Text mTxtIndex;

        public override void UpdateContent(int pIndex)
        {
            base.UpdateContent(pIndex);

            name = pIndex.ToString();
            mTxtIndex.text = pIndex.ToString();
        }
    }
}