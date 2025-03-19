using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodZombie
{
    public class UICamera : MonoBehaviour
    {
        private static UICamera mInstance;
        public static UICamera Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<UICamera>();
                return mInstance;
            }
        }

        private Camera mCamera;
        public Camera Camera
        {
            get
            {
                if (mCamera == null)
                    mCamera = GetComponent<Camera>();
                return mCamera;
            }
        }
    }
}