////
/// Credit https://unitylist.com/p/6je/Unity-Asset-Easy-Indicators
////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Utilities.Components
{
    [RequireComponent(typeof(IndicatorTarget))]
    public class IndicatorDistanceTracker : MonoBehaviour
    {
        public enum Decial
        {
            None,
            Tenths,
            Hundredths,
            Thousandths,
            TenThousandths
        }

        [Header("User-Assigned Variables")]
        [Tooltip("The custom font used for the distance indicator. If left empty, the default Arial font will be used instead.")]
        public Font customFont;

        [Header("Settings")]
        [Tooltip("Does the distance show on the On-Screen indicator?")]
        public bool showOnScreen = true;
        [Tooltip("Does the distance show on the Off-Screen indicator?")]
        public bool showOffScreen = true;
        [Tooltip("Offset position for the on-screen indicator.")]
        public Vector2 onScreenPosOffset = new Vector2(0, 65);
        [Tooltip("Offset position for the off-screen indicator.")]
        public Vector2 offScreenPosOffset = new Vector2(0, -50);
        [Tooltip("The suffix text that displays after the distance text. Represents distance units such as Meters.")]
        public string distanceSuffix = "m";
        [Tooltip("The farthest decimal point the distance will display.")]
        public Decial decimalPoint;
        [Tooltip("The font size of distancetext.")]
        public int fontSize = 24;

        private IndicatorTarget mTarget;
        private IndicatorPanel mPanel;
        private GameObject mDistanceIndicator;
        private Text mDistanceText;
        private bool mIsOnScreen = true;

        private void Awake()
        {
            mTarget = GetComponent<IndicatorTarget>();
        }

        private void Start()
        {
            StartCoroutine(IECoCreateDistanceTracker());
        }

        private void OnEnable()
        {
            if (mDistanceIndicator != null)
                mDistanceIndicator.SetActive(true);
        }

        private void OnDisable()
        {
            if (mDistanceIndicator != null)
                mDistanceIndicator.SetActive(false);
        }

        #region Update position & scale of the distance tracker

        private void LateUpdate()
        {
            if ((showOnScreen || showOffScreen) && mPanel != null && mDistanceIndicator != null)
            {
                //Runs once during on-screen & off-screen transition
                if (mTarget.IsVisable && mPanel.onScreen != null && !mIsOnScreen)
                {
                    if (showOnScreen)
                    {
                        mDistanceIndicator.SetActive(true);
                        mDistanceIndicator.transform.SetParent(mPanel.onScreen.transform);
                        mDistanceIndicator.transform.localPosition = onScreenPosOffset;
                    }
                    else
                        mDistanceIndicator.SetActive(false);

                    mIsOnScreen = true;
                }
                else if (!mTarget.IsVisable && mPanel.offScreen != null && mIsOnScreen)
                {
                    if (showOffScreen)
                    {
                        mDistanceIndicator.SetActive(true);
                        mDistanceIndicator.transform.SetParent(mPanel.offScreen.transform);
                        mDistanceIndicator.transform.localPosition = offScreenPosOffset;
                    }
                    else
                        mDistanceIndicator.SetActive(false);

                    mIsOnScreen = false;
                }

                //Set the decimal point
                switch (decimalPoint)
                {
                    case Decial.None:
                        mDistanceText.text = mTarget.DistanceFromViewer.ToString("F0") + distanceSuffix;
                        break;
                    case Decial.Tenths:
                        mDistanceText.text = mTarget.DistanceFromViewer.ToString("F1") + distanceSuffix;
                        break;
                    case Decial.Hundredths:
                        mDistanceText.text = mTarget.DistanceFromViewer.ToString("F2") + distanceSuffix;
                        break;
                    case Decial.Thousandths:
                        mDistanceText.text = mTarget.DistanceFromViewer.ToString("F3") + distanceSuffix;
                        break;
                    case Decial.TenThousandths:
                        mDistanceText.text = mTarget.DistanceFromViewer.ToString("F4") + distanceSuffix;
                        break;
                }

                mDistanceIndicator.transform.localScale = Vector3.one;
                mDistanceIndicator.transform.rotation = Quaternion.identity;
            }
        }

        #endregion

        #region IEnumerator that checks for a indicator panel that may not have been created yet thus we need to keep checking till it exist.

        private IEnumerator IECoCreateDistanceTracker()
        {
            mPanel = mTarget.IndicatorPanel;

            while (mPanel == null)
            {
                mPanel = mTarget.IndicatorPanel;
                yield return null;
            }

            //Now that the indicator panel exist, create the tracker
            CreateDistanceTracker();
        }

        #endregion

        #region Create the distance indicator

        private void CreateDistanceTracker()
        {
            mDistanceIndicator = new GameObject("DistanceTrackerIndicator");
            mDistanceIndicator.layer = 1 << 4;

            //Determine initial parent
            if (mTarget.IsVisable && mPanel.onScreen != null)
                mDistanceIndicator.transform.SetParent(mTarget.IndicatorPanel.onScreen.transform);
            else if (!mTarget.IsVisable && mPanel.offScreen != null)
                mDistanceIndicator.transform.SetParent(mTarget.IndicatorPanel.offScreen.transform);
            else
                mDistanceIndicator.transform.SetParent(mTarget.IndicatorPanel.transform);

            //Reset position & scale
            mDistanceIndicator.transform.localPosition = onScreenPosOffset;
            mDistanceIndicator.transform.localScale = Vector3.one;

            //text
            mDistanceText = mDistanceIndicator.AddComponent<Text>();
            mDistanceText.alignment = TextAnchor.MiddleCenter;
            mDistanceText.horizontalOverflow = HorizontalWrapMode.Overflow;
            mDistanceText.verticalOverflow = VerticalWrapMode.Overflow;
            mDistanceText.fontSize = fontSize;

            if (customFont == null)
            {
                customFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                mDistanceText.font = customFont;
            }

            //Add ui effects
            mDistanceIndicator.AddComponent<Shadow>();
            //distanceIndicator.AddComponent<Outline>();

            //distanceIndicator.SetActive(false);
        }

        #endregion
    }
}