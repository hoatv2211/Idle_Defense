
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Pattern.UI;

namespace FoodZombie.UI
{
    public class LoadingPanel : MyGamesBasePanel
    {
        [Separator("Panel Prefabs")]
        [SerializeField]
        private EarlyAccessWarningPopup mEarlyAccessWarningPopup;

        [Separator("Element")]
        [SerializeField]
        private Image mImgProcessBg;

        [SerializeField] private Image mImgProcess;
        [SerializeField] private Transform mPointer;
        [SerializeField] private TextMeshProUGUI mTxtVersion;
        [SerializeField] private TextMeshProUGUI mTxtHints;
        [SerializeField] private SimpleTMPButton mBtnTapToPlay;
        [SerializeField] private TextMeshProUGUI mTxtTapToPlay;

        private AsyncOperation operation;

        public void Start()
        {
            Lock(true);

            SoundManager.Instance.StopMusic(true);
            int dataVersion = 0;
            try
            {
                LevelConfigFirebaseObject levelFirebase = Constants.LevelDataFromFirebase;
                if (levelFirebase != null) dataVersion = levelFirebase.Version;
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
                //throw;
            }
         
            mTxtVersion.text = "Ver: " + Application.version + "_"+ dataVersion;

            if (mTxtHints != null)
            {
                // var localizedHint = new LocalizationGetter("HINTS_" + Random.Range(1, 6), "You can throw grenades on oil to burn enemies.");
                // mTxtHints.text = Localization.Get(Localization.HINTS_TITLE) + localizedHint.Get();

                mTxtHints.text = Localization.Get("HINTS_" + Config.EasyRandom(1, 45));
            }

            if (mBtnTapToPlay != null)
            {
                mBtnTapToPlay.onClick.AddListener(BtnTapToPlay_Pressed);
            }
        }

        public void LoadHomeScene()
        {
            float curProgress = 0;

            operation = SceneLoader.LoadScene("Home", false, false, (progress) =>
            {
                if (progress > curProgress) curProgress = progress;
                if (mImgProcess != null) mImgProcess.fillAmount = curProgress;
            }, () =>
            {
                if (mImgProcessBg != null) mImgProcessBg.SetActive(false); //kiểm tra do bị ngu crash
                if (mBtnTapToPlay != null) mBtnTapToPlay.SetActive(true); //kiểm tra do bị ngu crash
                if (mTxtTapToPlay != null)
                {
                    float alpha = 0f;
                    var color = mTxtTapToPlay.color;
                    DOTween.To(tweenVal => alpha = tweenVal, 0f, 1f, 0.5f)
                        .OnUpdate(() =>
                        {
                            color.a = alpha;
                            mTxtTapToPlay.color = color;
                        }).SetLoops(-1, LoopType.Yoyo);
                }
            }, 3f);
        }

        public void ShowEarlyAccessWarningPopup()
        {
            PushPanelToTop(ref mEarlyAccessWarningPopup);
            mEarlyAccessWarningPopup.Init(LoadGamePlayScreen);
        }

        public void LoadGamePlayScreen()
        {
            float curProgress = 0;

            operation = SceneLoader.LoadScene("GamePlay", false, false, (progress) =>
            {
                if (progress > curProgress) curProgress = progress;
                if (mImgProcess != null) mImgProcess.fillAmount = curProgress;
            }, () =>
            {
                if (mImgProcessBg != null) mImgProcessBg.SetActive(false); //kiểm tra do bị ngu crash
                if (mBtnTapToPlay != null) mBtnTapToPlay.SetActive(true); //kiểm tra do bị ngu crash
                if (mTxtTapToPlay != null)
                {
                    float alpha = 0f;
                    var color = mTxtTapToPlay.color;
                    DOTween.To(tweenVal => alpha = tweenVal, 0f, 1f, 0.5f)
                        .OnUpdate(() =>
                        {
                            color.a = alpha;
                            mTxtTapToPlay.color = color;
                        }).SetLoops(-1, LoopType.Yoyo);
                }
            }, 3f);
        }

        public void LoadGamePlayScreenNotTap()
        {
            float curProgress = 0;

            operation = SceneLoader.LoadScene("GamePlay", false, true, (progress) =>
            {
                if (progress > curProgress) curProgress = progress;
                if (mImgProcess != null) mImgProcess.fillAmount = curProgress;
                float w = 0f;
                if (mImgProcess.rectTransform != null) w = mImgProcess.rectTransform.sizeDelta.x;
                if (mPointer != null) mPointer.localPosition = new Vector3(w * curProgress - w / 2f, mPointer.localPosition.y, mPointer.localPosition.z);
            }, null, 2.5f);
        }

        private void BtnTapToPlay_Pressed()
        {
            operation.allowSceneActivation = true;
        }
    }
}