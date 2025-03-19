////
/// Credit https://unitylist.com/p/6je/Unity-Asset-Easy-Indicators
////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//This script is used to assign a random color and it's indicator UI.
//Simply attach this to any target that has the 'IndicatorTarget' component on it.

namespace Utilities.Components
{
    public class IndicatorColor : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The color of the indicator.")]
        public Color color = Color.red;
        [Tooltip("Use a random color. Will override 'NewColor'")]
        public bool randomColor = false;
        [Tooltip("Should this gameobject be set to the new color?")]
        public bool changeGameobjectColor = false;
        [Tooltip("Should this gameobject's children be set to the new color?")]
        public bool changeChildrenColor = false;

        private void Start()
        {
            ChangeColor(color, randomColor, changeGameobjectColor, changeChildrenColor);
        }

        private void ChangeColor(Color newColor, bool random, bool changeGO, bool changeChildren)
        {
            //Get a new random color if enabled
            if (random)
                newColor = new Color(UnityEngine.Random.Range(0f, 1f), Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            //Change color of this gameobject.
            if (changeGO)
            {
                //Gameobject
#if UNITY_2019_2_OR_NEWER
                if (TryGetComponent(out Renderer render))
                    render.material.color = newColor;
#else
                var render = GetComponent<Renderer>();
                if (render != null)
                    render.material.color = newColor;
#endif
            }

            //Change color of this gameobject's children
            if (changeChildren)
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renders.Length; i++)
                    renders[i].material.color = newColor;
            }

            //Change the indicator color if it exsist
#if UNITY_2019_2_OR_NEWER
            if (TryGetComponent(out IndicatorTarget indicator))
                StartCoroutine(CoChangeColor(newColor));
#else
            if (GetComponent<IndicatorTarget>() != null)
                StartCoroutine(CoChangeColor(newColor));
#endif
        }

        #region IEnumerator that checks for a indicator panel that may not have been created yet thus we need to keep checking till it exist.

        private IEnumerator CoChangeColor(Color newColor)
        {
            //Change color of all the indicator panel items
#if UNITY_2019_2_OR_NEWER
            TryGetComponent(out IndicatorTarget target);
            IndicatorPanel IPanel = target.IndicatorPanel;
#else
            IndicatorPanel IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
#endif
            while (IPanel == null)
            {
                IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
                yield return null;
            }

            //Changes all graphic colors
            Graphic[] graphics = IPanel.GetComponentsInChildren<Graphic>(true);
            if (graphics.Length > 0)
                for (int i = 0; i < graphics.Length; i++)
                    graphics[i].color = newColor;
        }

        #endregion
    }
}