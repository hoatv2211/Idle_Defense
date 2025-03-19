using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class RewardInfoView : ImageWithText
    {
        #region Members

        [SerializeField] private SimpleTMPButton mBtnInfo;

        private RewardInfo mRewardInfo;

        #endregion

        #region Public

        private void Start()
        {
            mBtnInfo.onClick.AddListener(() =>
            {
                BtnInfo_Pressed((mBtnInfo.transform as RectTransform));
            });
        }

        public void Init(RewardInfo pRewardInfo)
        {
            mRewardInfo = pRewardInfo;

            if (label != null) {
                SetSprite(mRewardInfo.GetIcon());
                label.text = "x" + mRewardInfo.Value;
            }
            else SetSprite(mRewardInfo.GetIcon());
        }

        #endregion

        //==============================================

        #region Private

        private void BtnInfo_Pressed(RectTransform pTarget)
        {
            var options = new UITooltips.Option[1];

            options[0] = new UITooltips.Option();
            options[0].imgIcon = mRewardInfo.GetIcon();
            options[0].content = mRewardInfo.GetDescription();
            options[0].value = mRewardInfo.Value;
            MainPanel.instance.ShowOptionsToolTip(pTarget, options, 650f);
        }

        #endregion
    }
}
