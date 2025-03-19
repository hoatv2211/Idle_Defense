using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Pattern.UI;
using TMPro;
using Utilities.Inspector;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class MessagesPopup : MyGamesBasePanel
    {
        public enum Choice
        {
            Ignore,
            Yes,
            No,
        }

        public class Message
        {
            public string title;
            public string content;
            public UnityAction yesAction;
            public UnityAction noAction;
            public UnityAction additionalAction;
            /// <summary>
            /// Text on yest button
            /// </summary>
            public string yesActionLabel;
            /// <summary>
            /// Text on No button
            /// </summary>
            public string noActionLabel;
            /// <summary>
            /// Text on anditional choice button
            /// </summary>
            public string additionalActionLabel;
            /// <summary>
            /// Size of popup
            /// </summary>
            public Vector2 popupSize;
            /// <summary>
            /// Can ignore popup by quit by X button
            /// </summary>
            public bool allowIgnore;
            /// <summary>
            /// Do not allow any external interfere which close popup during show
            /// </summary>
            public bool lockPopup;
            public TextAlignmentOptions contentAignment = TextAlignmentOptions.Center;

            public Message() { }

            public Message(string title, string content, UnityAction pYesAction, UnityAction pNoAction)
            {
                yesAction = pYesAction;
                noAction = pNoAction;
            }

            public Message(string title, UnityAction pYesAction, UnityAction pNoAction)
            {
                yesAction = pYesAction;
                noAction = pNoAction;
            }

            public Message(UnityAction pYesAction, UnityAction pNoAction)
            {
                yesAction = pYesAction;
                noAction = pNoAction;
            }
        }

        #region Members

        [SerializeField] private RectTransform mMainRect;
        [SerializeField] private TextMeshProUGUI mTxtTitle;
        [SerializeField] private TextMeshProUGUI mTxtContent;
        [SerializeField] private SimpleTMPButton mBtnYes;
        [SerializeField] private SimpleTMPButton mBtnNo;
        [SerializeField] private SimpleTMPButton mBtnAdditionalDo;

        private Choice mCurrentChoice;
        private Message mMessage;

        public static Message LockFeatureMessage
        {
            get
            {
                return new Message()
                {
                    yesAction = () => { },
                    noAction = null,
                    additionalAction = null,
                    yesActionLabel = "Ok",
                    allowIgnore = true,
                    title = "",
                    content = "This feature is not unlocked!\nThank you for supporting us!",
                    popupSize = new Vector2(800, 440),
                    contentAignment = TextAlignmentOptions.Top
                };
            }
        }

        #endregion

        //=============================================

        #region MonoBehaviour

        private void OnEnable()
        {
            mCurrentChoice = Choice.Ignore;
        }

        #endregion

        //=============================================

        #region Public

        public void InitMessage(Message pMessage)
        {
            mMessage = pMessage;
            if (!string.IsNullOrEmpty(mMessage.title)) mTxtTitle.text = mMessage.title;
            if (!string.IsNullOrEmpty(mMessage.content)) mTxtContent.text = mMessage.content;
            mTxtContent.alignment = pMessage.contentAignment;
            if (mBtnYes != null)
            {
                mBtnYes.onClick.RemoveAllListeners();
                mBtnYes.SetActive(mMessage.yesAction != null);
                if (mMessage.yesAction != null)
                {
                    mBtnYes.onClick.AddListener(OnBtnYes_Pressed);
                    mBtnYes.labelTMP.text = string.IsNullOrEmpty(mMessage.yesActionLabel) ? "Yes" : mMessage.yesActionLabel;
                }
            }
            if (mBtnNo != null)
            {
                mBtnNo.onClick.RemoveAllListeners();
                mBtnNo.SetActive(mMessage.noAction != null);
                if (mMessage.noAction != null)
                {
                    mBtnNo.onClick.AddListener(OnBtnNo_Pressed);
                    mBtnNo.labelTMP.text = string.IsNullOrEmpty(mMessage.noActionLabel) ? "No" : mMessage.noActionLabel;
                }
            }
            if (mBtnAdditionalDo != null)
            {
                mBtnAdditionalDo.onClick.RemoveAllListeners();
                mBtnAdditionalDo.SetActive(mMessage.additionalAction != null);
                if (mMessage.additionalAction != null)
                {
                    mBtnAdditionalDo.onClick.AddListener(OnBtnAdditionalDo_Pressed);
                    mBtnAdditionalDo.labelTMP.text = string.IsNullOrEmpty(mMessage.additionalActionLabel) ? "Do something" : mMessage.additionalActionLabel;
                }
            }
            if (mMessage.popupSize != Vector2.zero)
                mMainRect.sizeDelta = mMessage.popupSize;
            if (btnBack != null)
                btnBack.SetActive(mMessage.allowIgnore);

            Lock(mMessage.lockPopup);
        }

        #endregion

        //==============================================

        #region Private

        protected override void AfterHiding()
        {
            base.AfterHiding();

            if (mCurrentChoice == Choice.Yes)
                mMessage.yesAction.Raise();
            else if (mCurrentChoice == Choice.No)
                mMessage.noAction.Raise();

            mMessage = new Message();
        }

        private void OnBtnNo_Pressed()
        {
            mCurrentChoice = Choice.No;
            Lock(false);

            base.Back();
        }

        private void OnBtnYes_Pressed()
        {
            mCurrentChoice = Choice.Yes;
            Lock(false);

            base.Back();
        }

        internal override void Back()
        {
            if (mMessage.allowIgnore)
            {
                mCurrentChoice = Choice.Ignore;
                Lock(false);

                base.Back();
            }
        }

        private void OnBtnAdditionalDo_Pressed()
        {
            mMessage.additionalAction();
        }

        #endregion
    }
}