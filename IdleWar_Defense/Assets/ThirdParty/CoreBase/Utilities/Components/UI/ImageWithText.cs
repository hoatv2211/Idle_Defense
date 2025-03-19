#pragma warning disable 0649

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Inspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Components
{
    [AddComponentMenu("Utitlies/UI/ImageWithText")]
    public class ImageWithText : MonoBehaviour
    {
        public enum AutoResizeType
        {
            None,
            FixedWith,
            FixedHeight,
        }

        [SerializeField] protected Image mImg;
        [SerializeField] protected TextMeshProUGUI mTxt;

        [Separator("Custom")]
        [SerializeField] protected bool mAutoReize;
        [SerializeField] protected AutoResizeType mAutoReizeType;
        [SerializeField] protected float mFixedSize;

        public Image image
        {
            get
            {
                if (mImg == null)
                    mImg = GetComponentInChildren<Image>();
                return mImg;
            }
        }
        public TextMeshProUGUI label
        {
            get
            {
                if (mTxt == null)
                    mTxt = GetComponentInChildren<TextMeshProUGUI>();
                return mTxt;
            }
        }
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
        public Sprite sprite
        {
            get { return image.sprite; }
            set
            {
                if (mImg.sprite != value)
                    SetSprite(value);
            }
        }

        public void SetSprite(Sprite pSprite)
        {
            image.sprite = pSprite;

            if (mAutoReize)
            {
                if (pSprite == null)
                    return;

                Vector2 nativeSize = pSprite.NativeSize();
                switch (mAutoReizeType)
                {
                    case AutoResizeType.FixedHeight:
                        if (mFixedSize == 0) mFixedSize = mImg.rectTransform.rect.height;
                        mImg.SketchByFixedHeight(mFixedSize);
                        break;

                    case AutoResizeType.FixedWith:
                        if (mFixedSize == 0) mFixedSize = mImg.rectTransform.rect.width;
                        mImg.SketchByFixedWidth(mFixedSize);
                        break;
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Validate")]
        private void OnValidate()
        {
            Validate();
        }

        protected virtual void Validate()
        {
            if (mImg == null)
                mImg = GetComponentInChildren<Image>();
            if (mTxt == null)
                mTxt = GetComponentInChildren<TextMeshProUGUI>();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ImageWithText), true)]
    public class ImageWithTextEditor : Editor
    {
        private ImageWithText mScript;

        private void OnEnable()
        {
            mScript = (ImageWithText)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorHelper.Button("Auto Reize"))
                mScript.SetSprite(mScript.image.sprite);
        }
    }
#endif
}
