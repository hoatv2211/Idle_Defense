////
/// Credit https://unitylist.com/p/6je/Unity-Asset-Easy-Indicators
////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Components
{
    public class IndicatorViewer : MonoBehaviour
    {
        public enum Transitions
        {
            None,
            Slide,
            Fade,
            Rotate,
            RotateReverse,
            Scale
        }

        //User-assigned variables
        [Header("User-Assigned Variables")]
        [Tooltip("The global default panel prefab that hold all the indicator UI for each target. Will automatically create a Canvas to house this panels.")]
        public GameObject defaultIndicatorPanel;
        [Tooltip("The camera that will view the indicators. If left empty, will be assigned to the main camera instead.")]
        public Camera viewerCamera;
        [Tooltip("The gameobject that the indicator's distance calculations will be based from. If left empty, it will be assigned to this gameobject instead. NOTE: ViewerObject should move with the ViewerCamera.")]
        public GameObject viewerObject;

        //Settings & options
        [Header("Settings")]
        [Tooltip("The sorting layer for all the indicators. Lower value = behind UI. Higher value = front of UI")]
        public int canvasSortingOrder = -100;
        [Tooltip("How many seconds before indicators update their tracking. Higher = better performance, but more stuttering")]
        [Range(0, 1)]
        public float updateInterval = 0.03f;
        [Tooltip("The farthest distance indicators will reach from the screen center to the screen edges. Align slider with TargetEdgeDistance for seamless transition.")]
        [Range(0, 1)]
        public float indicatorEdgeDistance = 0.9f;
        [Tooltip("The farthest distance the target needs to reach from the screen center to the screen edges to be considered off-screen. Align slider with IndicatorEdgeDistance for seamless transition.")]
        [Range(0.5f, 1)]
        public float targetEdgeDistance = 0.95f;

        [Space(10)]
        [Tooltip("Should target indicator be disabled when target is too far from the viewer? Enabled = better performance")]
        public bool disableOnDistance = true;
        [Tooltip("The max distance of the target from the viewer for the target's indicator to disable.")]
        public float disableDistance = 100;

        [Space(10)]
        [Tooltip("Does the off-screen indicator rotate towards the target? Set True for arrow-type indicators. Set False for portrait-style indicators.")]
        public bool offScreenRotates = true;
        [Tooltip("Should indicators automatically scale based on the distance from the viewer?")]
        public bool autoScale = true;
        public float scalingFactor = 10;
        [Tooltip("The minimum and maximum X and Y scale size of the indicator.")]
        public float minScaleSize = 0.2f;
        public float maxScaleSize = 100;

        [Space(10)]
        [Tooltip("The duration of the transition in seconds.")]
        public float oransitionDuration = 0.25f;
        public Transitions onScreenEnableTransition = Transitions.Scale;
        public Transitions onScreenDisableTransition = Transitions.Scale;
        public Transitions offScreenEnableTransition = Transitions.Scale;
        public Transitions offScreenDisableTransition = Transitions.Scale;

        private static bool mIsTracking = true;
        private static bool mTrackOnScreen = true;
        private static bool mTrackOffScreen = true;
        private GameObject mIndicatorCanvas;
        private WaitForSeconds mWaitForUpdate;

        public static bool IsTracking
        {
            get { return mIsTracking; }
        }
        public static bool TrackOnScreen
        {
            get { return mTrackOnScreen; }
            set { mTrackOnScreen = value; }
        }
        public static bool TrackOffScreen
        {
            get { return mTrackOffScreen; }
            set { mTrackOffScreen = value; }
        }
        public GameObject IndicatorCanvas
        {
            get { return mIndicatorCanvas; }
        }

        public static List<IndicatorTarget> Targets = new List<IndicatorTarget>();

        private void Awake()
        {
            //If no custom camera is assigned, use main camera
            if (viewerCamera == null)
                viewerCamera = Camera.main;

            //If no viewer is assigned, use this gameobject
            if (viewerObject == null)
                viewerObject = gameObject;

            //Create canvas is if doesnt already exsist
            if (mIndicatorCanvas == null)
                CreateIndicatorCanvas();
        }

        private void OnEnable()
        {
            mWaitForUpdate = new WaitForSeconds(updateInterval);
            StartCoroutine(IEUpdate());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator IEUpdate()
        {
            while (true)
            {
                if (mIsTracking)
                {
                    //Debug.Log("Tracking: " + IndicatorTargets.Count);

                    for (int i = 0; i < Targets.Count; i++)
                        Targets[i].UpdateIndicator();
                }

                yield return mWaitForUpdate;
            }
        }

        /// <summary>
        /// Create a default canvas for the indicator panels and set parameters.
        /// </summary>
        private void CreateIndicatorCanvas()
        {
            //Create gameobject that holds canvas
            mIndicatorCanvas = new GameObject("Indicator_Canvas");
            mIndicatorCanvas.layer = 1 << 4;

            //Create Canvas
            Canvas canvas = mIndicatorCanvas.AddComponent<Canvas>();
            //canvas.renderMode = RenderMode.WorldSpace;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = canvasSortingOrder;

            //Create default components for canvas
            CanvasScaler cs = mIndicatorCanvas.AddComponent<CanvasScaler>();
            cs.scaleFactor = 1;
            cs.dynamicPixelsPerUnit = 10;
            mIndicatorCanvas.AddComponent<GraphicRaycaster>();
        }

        /// <summary>
        /// Tracks target; add a IndicatorTarget component to the target if it doesn't already exist and add to tracking list.
        /// </summary>
        private static void TrackTarget(GameObject target)
        {
#if UNITY_2019_2_OR_NEWER
            target.TryGetComponent(out IndicatorTarget ITarget);
#else
            IndicatorTarget ITarget = target.GetComponent<IndicatorTarget>();
#endif
            //If the target doesn't have a indicator... Add one
            if (ITarget == null)
                ITarget = target.AddComponent<IndicatorTarget>();
            //Else if the target already has an indicator, check if it just needs to be added to the tracking list if not already
            else if (!Targets.Contains(ITarget))
                Targets.Add(ITarget);
            else
                Debug.Log("Target is already being tracked.");
            ITarget.enabled = true;
        }

        /// <summary>
        /// Untracks target; removes target from tracking list
        /// </summary>
        private static void UntrackTarget(GameObject target)
        {
#if UNITY_2019_2_OR_NEWER
            target.TryGetComponent(out IndicatorTarget ITarget);
#else
            IndicatorTarget ITarget = target.GetComponent<IndicatorTarget>();
#endif
            ITarget.enabled = false;
            Targets.Remove(ITarget);
        }

        /// <summary>
        /// Returns the IndicatorTarget component of the target
        /// </summary>
        private static IndicatorTarget GetIndicatorTarget(GameObject target)
        {
            IndicatorTarget ITarget = target.GetComponent<IndicatorTarget>();
            if (ITarget != null)
                for (int i = 0; i < Targets.Count; i++)
                    if (ITarget = Targets[i])
                        return ITarget;
            return null;
        }

        private static void SetTracking(bool trackOnScreen, bool trackOffScreen)
        {
            TrackOnScreen = trackOnScreen;
            TrackOffScreen = trackOffScreen;
        }

        private static void StartTracking()
        { mIsTracking = true; }

        private static void StopTracking()
        { mIsTracking = false; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            mWaitForUpdate = new WaitForSeconds(updateInterval);
        }
#endif
    }
}