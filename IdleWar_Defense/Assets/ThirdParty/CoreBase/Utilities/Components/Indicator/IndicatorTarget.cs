////
/// Credit https://unitylist.com/p/6je/Unity-Asset-Easy-Indicators
////

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Utilities.Components
{
    public class IndicatorTarget : MonoBehaviour
    {
        //User-assigned variables
        [Header("User-Assigned Variables")]
        [Tooltip("The custom indicator Panel shown only on this target. (Overrides viewer default) If left empty, the viewer default will be used instead.")]
        public GameObject customIndicatorPanel;

        //Settings & options
        [Header("Settings")]
        [Tooltip("Offset position of the on-screen indicator from the target. If left to 0, the indicator will be positioned at the center of the target.")]
        public Vector2 onScreenOffset;
        [Tooltip("Should indicators track target when it is On-Screen regardless of viewer default?")]
        public bool forceOnScreen = false;
        [Tooltip("Should indicators track target when it is Off-Screen regardless of viewer default?")]
        public bool forceOffScreen = false;

        private IndicatorViewer mViewer;
        private IndicatorPanel mPanel;
        private bool mIsVisable; //Check if the viewer is on-screen or off-screen
        private bool mIsOnScreenEnabled; //Check if onscreen is enabled
        private bool mIsOffScreenEnabled;//Check if offscreen is enabled.
        private float mDistanceFromViewer; //Distance from the viewer
                                           //private float previousZposition;//The last Z position value of the scaling axis. (used for scaling the indicator)
                                           //private Vector3 previousScale;//The last local scale value of the indicator. (used for scaling the indicator) 

        private void Awake()
        {
            mViewer = FindObjectOfType<IndicatorViewer>();
        }

        private void Start()
        {
            InitializeIndicator();
        }

        private void OnEnable()
        {
            //Add the target the the viewer's target list so viewer can track this target.
            if (!IndicatorViewer.Targets.Contains(this) && mPanel != null)
                IndicatorViewer.Targets.Add(this);

            //Disable the panel so viewer does not see it ingame
            if (mPanel != null)
                mPanel.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            //Remove the target from the viewer's list so viewer no longer tracks this target.
            IndicatorViewer.Targets.Remove(this);

            //Enable the panel so viewer can see it ingame
            if (mPanel != null)
                mPanel.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            //if the viewer is currently tracking...
            if (mPanel != null && IndicatorViewer.IsTracking)
                mPanel.gameObject.SetActive(true);

            //else if the viewer is not tracking...
            else if (mPanel != null)
                mPanel.gameObject.SetActive(false);
        }

        #region Initial set-up of indicator

        /// <summary>
        /// Create & set-up the indicator panel
        /// </summary>
        private void InitializeIndicator()
        {
            //If no custom indicator panel is assigned to this gameobject, the viewer default indicator panel will be used instead
            if (customIndicatorPanel == null)
                customIndicatorPanel = mViewer.defaultIndicatorPanel;

            //If customIndicatorPanel is still null because there is no viewer default, throw error 
            try
            {
                //Create the runtime indicator object and assign it under the canvas. 
                GameObject panel = Instantiate(customIndicatorPanel, Vector2.zero, Quaternion.identity) as GameObject;
                panel.name = name + " indicator";
                panel.transform.SetParent(mViewer.IndicatorCanvas.transform);
#if UNITY_2019_2_OR_NEWER
                panel.TryGetComponent(out mPanel);
#else
                mPanel = panel.GetComponent<IndicatorPanel>();
#endif

                //Set the offset position of the onscreen image.
                if (mPanel.onScreen != null)
                    mPanel.onScreen.transform.position += new Vector3(onScreenOffset.x, onScreenOffset.y, 0);

                //Assign the initial z position && scale value
                //previousZposition = IPanel.transform.position.z;
                //previousScale = IPanel.transform.localScale;

                //Add this target to the list of targets if not already
                if (!IndicatorViewer.Targets.Contains(this))
                    IndicatorViewer.Targets.Add(this);

                //initial indicator toggle
                if (OnScreen(mViewer.viewerCamera.WorldToScreenPoint(transform.position), mViewer.viewerCamera))
                {
                    if (mPanel.onScreen != null)
                        ToggleOnScreen(true);
                    if (mPanel.offScreen != null)
                        ToggleOffScreen(false);
                }
                else
                {
                    if (mPanel.onScreen != null)
                        ToggleOnScreen(false);
                    if (mPanel.offScreen != null)
                        ToggleOffScreen(true);
                }

            }
            catch
            { Debug.LogError("The 'DefaultIndicatorPanel' requires a UnityUI Panel object!", mViewer); }
        }

        #endregion

        #region Update the position & rotation of the indicator panel of this target.

        /// <summary>
        /// Called from 'IndicatorViewer' script.
        /// </summary>
        public void UpdateIndicator()
        {
            //Get the target's world position in screen coordinate position
            Vector3 targetPosOnScreen = mViewer.viewerCamera.WorldToScreenPoint(transform.position);

            //if the target is visable on screen...
            if (OnScreen(targetPosOnScreen, mViewer.viewerCamera))
            {

                //Disable the off-screen indicator if it exist
                if (mPanel.offScreen != null && mIsOffScreenEnabled)
                    ToggleOffScreen(false);

                //if the viewer allows indicators to show when the target is on-screen
                if ((forceOnScreen || IndicatorViewer.TrackOnScreen) && mPanel.onScreen != null)
                {
                    //Set target to visable.
                    mIsVisable = true;

                    //Get distance from this target and viewer
                    mDistanceFromViewer = GetDistance(transform.position, mViewer.viewerObject.transform.position);

                    //If the on-screen indicator is within distance threshold, then do stuff...
                    if (CheckDisableOnDistance(mDistanceFromViewer) == false)
                    {
                        //Enable the on-screen indicator
                        if (!mIsOnScreenEnabled)
                            ToggleOnScreen(true);

                        //set the UI panel position to the target's position
                        mPanel.transform.position = targetPosOnScreen;
                        //IPanel.transform.position = transform.position + new Vector3(onScreenOffset.x, onScreenOffset.y, transform.position.z);

                        //If OnScreen exist && scaling is enabled && if the target's indicator axis towards the camera has changed...
                        if (mViewer.autoScale) //(IPanel.transform.position.z != previousZposition || IPanel.transform.localScale != previousScale))
                        {
                            UpdateScale(mDistanceFromViewer);

                            //Record the new axis position & local scale of the indicator panel
                            //previousZposition = IPanel.transform.position.z;
                            //previousScale = IPanel.transform.localScale;
                        }
                    }
                    else if (mIsOnScreenEnabled)
                    {
                        ToggleOnScreen(false);
                    }
                }
                else if (mPanel.onScreen != null && mIsOnScreenEnabled)
                {
                    ToggleOnScreen(false);
                }
            }
            //Else, target is Off-Screen...
            else
            {
                // Set target as invisable.
                mIsVisable = false;

                //Disables the on screen indicator if it exists
                if (mPanel.onScreen != null && mIsOnScreenEnabled)
                    ToggleOnScreen(false);

                if ((forceOffScreen || IndicatorViewer.TrackOffScreen) && mPanel.offScreen != null)
                {
                    //Get distance from this target and viewer
                    mDistanceFromViewer = GetDistance(transform.position, mViewer.viewerObject.transform.position);

                    //If the off-screen indicator is within distance threshold, then do stuff...
                    if (CheckDisableOnDistance(mDistanceFromViewer) == false)
                    {
                        //Enables the off screen indicator
                        if (!mIsOffScreenEnabled)
                            ToggleOffScreen(true);

                        //Update the indicator positon and rotation based on the target's position on the screen
                        UpdateOffScreen(targetPosOnScreen);

                        //If scaling is enabled... then perform the update indicator method
                        if (mViewer.autoScale)
                            UpdateScale(mDistanceFromViewer);
                    }
                    else if (mIsOffScreenEnabled)
                    {
                        ToggleOffScreen(false);
                    }
                }
                else if (mPanel.offScreen != null && mIsOffScreenEnabled)
                {
                    ToggleOffScreen(false);
                }
            }
        }

        #endregion

        #region Method that returns true if target is within the camera screen boundaries and it's near clipping plane.

        private bool OnScreen(Vector3 pos, Camera camera)
        {
            if (pos.x < (Screen.width * mViewer.targetEdgeDistance) && pos.x > (Screen.width - Screen.width * mViewer.targetEdgeDistance) &&
            pos.y < (Screen.height * mViewer.targetEdgeDistance) && pos.y > (Screen.height - Screen.height * mViewer.targetEdgeDistance) &&
            pos.z > camera.nearClipPlane && pos.z < camera.farClipPlane)
                return true;
            return false;
        }

        #endregion

        #region Method that returns false if indicator does not need to be disabled because either DisableOnDistance is false or distance is within threshold. 

        private bool CheckDisableOnDistance(float distance)
        {
            //Check if indicator will disable on distance and check if distance is over the distance threshold
            if (mViewer.disableOnDistance && distance > mViewer.disableDistance)
                return true;
            return false;
        }

        #endregion

        #region Method that updates scale

        /// <summary>
        /// Updates the scaling of the UI based on distance provided
        /// </summary>
        private void UpdateScale(float distance)
        {
            //Create a scaling factor based on distance
            float newScaleSize = mViewer.scalingFactor / distance;

            //Set the scale based on the scaling factor
            mPanel.transform.localScale = new Vector2(newScaleSize, newScaleSize);

            //If the indicator scale is lower then or equal to the minimum size... Then, set the scale to the minimum size.
            //Else if the indicator scale is greater or equal to the maximum size... Then, set the scale to the maximum size.
            if (mPanel.transform.localScale.x <= mViewer.minScaleSize || mPanel.transform.localScale.y <= mViewer.minScaleSize)
                mPanel.transform.localScale = new Vector2(mViewer.minScaleSize, mViewer.minScaleSize);
            else if (mPanel.transform.localScale.x >= mViewer.maxScaleSize || mPanel.transform.localScale.y >= mViewer.maxScaleSize)
                mPanel.transform.localScale = new Vector2(mViewer.maxScaleSize, mViewer.maxScaleSize);

            //Debug.Log("Scaling...");
        }

        #endregion

        #region Calculate OffScreen Position, Rotation, and Size

        /// <summary>
        /// Updates the indicator based on the off-screen target
        /// </summary>
        private void UpdateOffScreen(Vector3 targetPosOnScreen)
        {
            //Create a variable for the center position of the screen.
            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

            //Set newIndicatorPos anchor to the center of the screen instead of bottom left
            Vector3 newIndicatorPos = targetPosOnScreen - screenCenter;

            //Flip the newIndicatorPos to correct the calculations for indicators behind the camera.
            if (newIndicatorPos.z < 0)
                newIndicatorPos *= -1;

            //Get angle from center of screen to target position
            float angle = Mathf.Atan2(newIndicatorPos.y, newIndicatorPos.x);
            angle -= 90 * Mathf.Deg2Rad;

            //y = mx + b (intercept forumla)
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            float m = cos / sin;

            //Create the screen boundaries that the indicators reside in.
            Vector3 screenBounds = new Vector3(screenCenter.x * mViewer.indicatorEdgeDistance, screenCenter.y * mViewer.indicatorEdgeDistance);

            //Check which screen side the target is currently in.
            //Check top & bottom
            if (cos > 0)
                newIndicatorPos = new Vector2(-screenBounds.y / m, screenBounds.y);
            else
                newIndicatorPos = new Vector2(screenBounds.y / m, -screenBounds.y);

            //Check left & right
            if (newIndicatorPos.x > screenBounds.x)
                newIndicatorPos = new Vector2(screenBounds.x, -screenBounds.x * m);
            else if (newIndicatorPos.x < -screenBounds.x)
                newIndicatorPos = new Vector2(-screenBounds.x, screenBounds.x * m);

            //Reset the newIndicatorPos anchor back to bottom left corner.
            newIndicatorPos += screenCenter;

            //Assign new position
            mPanel.transform.position = new Vector3(newIndicatorPos.x, newIndicatorPos.y, targetPosOnScreen.z);

            //Assign new rotation
            if (mViewer.offScreenRotates)
                mPanel.offScreen.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            else
                mPanel.offScreen.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        #endregion

        #region that returns the distance magnitude between two vector3 positions. (Faster then built-in Vector3.Distance function)

        private float GetDistance(Vector3 PosA, Vector3 PosB)
        {
            Vector3 heading;

            //Subtracting from both vectors returns the magnitude
            heading.x = PosA.x - PosB.x;
            heading.y = PosA.y - PosB.y;
            heading.z = PosA.z - PosB.z;

            //Return the sqaure root of the sum of each sqaured vector axises.
            return Mathf.Sqrt((heading.x * heading.x) + (heading.y * heading.y) + (heading.z * heading.z));
        }

        #endregion

        #region OnScreen Tranisitions

        /// <summary>
        /// /Toggle On-Screen indicator
        /// </summary>
        private void ToggleOnScreen(bool enable)
        {
            //Set its enabled state, enable/disable its gameobject, then determine which animation to use based on viewer settings
            if (enable)
            {
                mIsOnScreenEnabled = true;
                mPanel.onScreen.SetActive(true);

                switch (mViewer.onScreenEnableTransition)
                {
                    case IndicatorViewer.Transitions.Slide:
                        mPanel.SlideTransition(mPanel.onScreen.transform, 2 * onScreenOffset, onScreenOffset, mViewer.oransitionDuration, false);
                        break;
                    case IndicatorViewer.Transitions.Fade:
                        mPanel.FadeTransition(mPanel.onScreen.transform, 1, mViewer.oransitionDuration, false);
                        break;
                    case IndicatorViewer.Transitions.Rotate:
                        mPanel.RotateTransition(mPanel.onScreen.transform, Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, 0), mViewer.oransitionDuration, false);
                        break;
                    case IndicatorViewer.Transitions.RotateReverse:
                        mPanel.RotateTransition(mPanel.onScreen.transform, Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 0), mViewer.oransitionDuration, false);
                        break;
                    case IndicatorViewer.Transitions.Scale:
                        mPanel.ScaleTransition(mPanel.onScreen.transform, Vector3.zero, Vector3.one, mViewer.oransitionDuration, false);
                        break;
                }
            }
            else
            {
                mIsOnScreenEnabled = false;

                switch (mViewer.onScreenDisableTransition)
                {
                    case IndicatorViewer.Transitions.None:
                        mPanel.onScreen.SetActive(false);
                        break;
                    case IndicatorViewer.Transitions.Slide:
                        mPanel.SlideTransition(mPanel.onScreen.transform, onScreenOffset, 2 * onScreenOffset, mViewer.oransitionDuration, true);
                        break;
                    case IndicatorViewer.Transitions.Fade:
                        mPanel.FadeTransition(mPanel.onScreen.transform, 0, mViewer.oransitionDuration, true);
                        break;
                    case IndicatorViewer.Transitions.Rotate:
                        mPanel.RotateTransition(mPanel.onScreen.transform, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -90), mViewer.oransitionDuration, true);
                        break;
                    case IndicatorViewer.Transitions.RotateReverse:
                        mPanel.RotateTransition(mPanel.onScreen.transform, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 90), mViewer.oransitionDuration, true);
                        break;
                    case IndicatorViewer.Transitions.Scale:
                        mPanel.ScaleTransition(mPanel.onScreen.transform, Vector3.one, Vector3.zero, mViewer.oransitionDuration, true);
                        break;
                }
            }
        }

        #endregion

        #region OffScreen Transitions

        /// <summary>
        /// Toggle Off-Screen indicator
        /// </summary>
        private void ToggleOffScreen(bool enable)
        {
            // Set its enabled state, enable/ disable its gameobject, then determine which animation to use based on viewer settings
            if (enable)
            {
                mIsOffScreenEnabled = true;
                mPanel.offScreen.SetActive(true);

                switch (mViewer.offScreenEnableTransition)
                {
                    case IndicatorViewer.Transitions.Slide:
                        mPanel.SlideTransition(mPanel.offScreen.transform, new Vector3(0, 50, 0), Vector3.zero, mViewer.oransitionDuration, false);
                        break;
                    case IndicatorViewer.Transitions.Fade:
                        mPanel.FadeTransition(mPanel.offScreen.transform, 1, mViewer.oransitionDuration, false);
                        break;
                    case IndicatorViewer.Transitions.Scale:
                        mPanel.ScaleTransition(mPanel.offScreen.transform, Vector3.zero, Vector3.one, mViewer.oransitionDuration, false);
                        break;
                }
            }
            else
            {
                mIsOffScreenEnabled = false;

                switch (mViewer.offScreenDisableTransition)
                {
                    case IndicatorViewer.Transitions.None:
                        mPanel.offScreen.SetActive(false);
                        break;
                    case IndicatorViewer.Transitions.Slide:
                        mPanel.SlideTransition(mPanel.offScreen.transform, Vector3.zero, new Vector3(0, 50, 0), mViewer.oransitionDuration, true);
                        break;
                    case IndicatorViewer.Transitions.Fade:
                        mPanel.FadeTransition(mPanel.offScreen.transform, 0, mViewer.oransitionDuration, true);
                        break;
                    case IndicatorViewer.Transitions.Scale:
                        mPanel.ScaleTransition(mPanel.offScreen.transform, Vector3.one, Vector3.zero, mViewer.oransitionDuration, true);
                        break;
                }
            }
        }

        #endregion

        #region OnDestory()

        void OnDestroy()
        {
            DestroyIndicator();
            //print("object was destroyed");
        }

        #endregion

        #region Method that handles destroying of indicator

        public void DestroyIndicator()
        {
            if (mPanel != null)
                Destroy(mPanel.gameObject);
#if UNITY_2019_2_OR_NEWER
            TryGetComponent(out IndicatorTargetCamera indicator);
#else
            var indicator = GetComponent<IndicatorTargetCamera>();
#endif
            if (indicator != null)
                Destroy(indicator);
            Destroy(this);
        }

        #endregion

        #region Getters/Setters

        public IndicatorPanel IndicatorPanel
        {
            get { return mPanel; }
        }
        public bool IsVisable
        {
            get { return mIsVisable; }
        }
        public float DistanceFromViewer
        {
            get { return mDistanceFromViewer; }
        }

        #endregion
    }
}