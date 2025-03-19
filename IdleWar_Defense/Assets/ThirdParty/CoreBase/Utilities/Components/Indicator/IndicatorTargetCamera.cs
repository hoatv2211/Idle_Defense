////
/// Credit https://unitylist.com/p/6je/Unity-Asset-Easy-Indicators
////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Utilities.Components
{
    [RequireComponent(typeof(IndicatorTarget))]
    public class IndicatorTargetCamera : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Offset position for the camera to see the target.")]
        public Vector3 cameraPositionOffset = new Vector3(0, 0, -2);
        [Tooltip("Offset rotation for the camera to see the target.")]
        public Vector3 cameraRotationOffset;
        [Tooltip("Farclipping plane of the camera. Adjust for long meshed objects.")]
        public int cameraFarclipDistance = 4;
        [Tooltip("Viewing size of the camera. Adjust to properly size target.")]
        public float cameraViewSize = 1.5f;

        [Space(10)]
        [Tooltip("The texture size of the target. Lower = better performance")]
        public int targetResolution = 128;
        [Tooltip("The layer that the camera will render. Only objects with this layer can be seen by the TargetCamera.")]
        public LayerMask targetLayer = 1 << 0;

        private IndicatorTarget mTarget;
        private IndicatorPanel mPanel;
        private GameObject mTargetCamGO;
        private GameObject mTargetCameraIndicator;

        private void Awake()
        {
            mTarget = GetComponent<IndicatorTarget>();
        }

        private void Start()
        {
            StartCoroutine(CoCreateTargetCamera());
        }

        private void OnEnable()
        {
            if (mTargetCameraIndicator != null)
                mTargetCameraIndicator.SetActive(true);
            if (mTargetCamGO != null)
                mTargetCamGO.SetActive(true);
        }
   
        private void OnDisable()
        {
            if (mTargetCameraIndicator != null)
                mTargetCameraIndicator.SetActive(false);
            if (mTargetCamGO != null)
                mTargetCamGO.SetActive(false);
        }

        private void LateUpdate()
        {
            if (mPanel != null && mTargetCamGO != null)
            {
                if (mTarget.IsVisable && mPanel.onScreen != null)
                {
                    //  Disable Camera
                    //targetCameraIndicator.SetActive(false);
                    mTargetCamGO.SetActive(false);
                }
                else if (!mTarget.IsVisable && mPanel.offScreen != null)
                {
                    //  Enable Camera
                    //targetCameraIndicator.SetActive(true);
                    mTargetCamGO.SetActive(true);

                    //  Update camera position/rotation to target
                    mTargetCamGO.transform.position = cameraPositionOffset + transform.position;
                    mTargetCamGO.transform.rotation = Quaternion.Euler(cameraRotationOffset.x, cameraRotationOffset.y, cameraRotationOffset.z);
                    mTargetCameraIndicator.transform.rotation = Quaternion.identity;
                }
            }
        }

        #region IEnumerator that checks for a indicator panel that may not have been created yet thus we need to keep checking till it exist.
      
        private IEnumerator CoCreateTargetCamera()
        {
            mPanel = mTarget.IndicatorPanel;

            while (mPanel == null)
            {
                mPanel = mTarget.IndicatorPanel;
                yield return null;
            }

            //  Now that the indicator panel exist, create the camera
            CreateTargetCamera();
        }

        #endregion

        #region Create the target camera and indicator camera

        /// <summary>
        /// Creates the target camera, target's RenderTexture, and set-up UI stuff 
        /// </summary>
        private void CreateTargetCamera()
        {
            //  1. Create empty gameobject to hold the camera
            mTargetCamGO = new GameObject("Indicator_TargetCam");
            mTargetCamGO.transform.SetParent(transform);
            mTargetCamGO.transform.position = cameraPositionOffset + transform.position;
            mTargetCamGO.transform.rotation = Quaternion.Euler(cameraRotationOffset.x, cameraRotationOffset.y, cameraRotationOffset.z);

            //  2. Create the target camera raw image for the panel
            mTargetCameraIndicator = new GameObject("TargetCameraImage");
            mTargetCameraIndicator.layer = 1 << 4;
            mTargetCameraIndicator.transform.SetParent(mTarget.IndicatorPanel.offScreen.transform);
            mTargetCameraIndicator.transform.localPosition = Vector3.zero;
            mTargetCameraIndicator.transform.localScale = Vector3.one;
            //_indicatorPanel.TargetCam = targetCameraIndicator;
            RawImage rawImage = mTargetCameraIndicator.AddComponent<RawImage>();
            rawImage.raycastTarget = false;

            //  3. Create the render texture & set parameters
            //renderTexture = new RenderTexture(TargetResolution, TargetResolution, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            //renderTexture.antiAliasing = 1;
            RenderTexture renderTexture = RenderTexture.GetTemporary(targetResolution, targetResolution, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);
            renderTexture.name = "TargetCamRenderTexture";
            mTargetCameraIndicator.GetComponent<RawImage>().texture = renderTexture;

            //  4. Create Camera and set up parameters
            Camera targetCamera = mTargetCamGO.AddComponent<Camera>();
            targetCamera.cullingMask = targetLayer;
            targetCamera.orthographic = true;
            targetCamera.orthographicSize = cameraViewSize;
            targetCamera.farClipPlane = cameraFarclipDistance;
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
            targetCamera.backgroundColor = Color.clear;
            targetCamera.targetTexture = renderTexture;
        }
      
        #endregion
    }
}