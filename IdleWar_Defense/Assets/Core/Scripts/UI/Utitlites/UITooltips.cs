using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;

namespace FoodZombie.UI
{
    public enum Alignment
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BotLeft,
        Bot,
        BotRight,
    }

    public class UITooltips : MonoBehaviour
    {
        #region Internal Class

        public class Message
        {
            public string message;
            public Vector2 size;
            public bool blockInput = false;
            public Message() { }
            public Message(string pMessage, Vector2 pSize, bool pBlockInput = false)
            {
                message = pMessage;
                size = pSize;
                blockInput = pBlockInput;
            }
        }

        public class Option
        {
            public Sprite imgIcon;
            public int value;
            public string content;
            public UnityAction onSelect;
            internal bool enableWordWrapping = true;
        }

        public class QuickTrade
        {
            public string description;
            public string labelButton = "SELL";
            public int cost;
            public int currencyId;
            public UnityAction onTrade;
            public bool enabled = true;
        }

        public class Notification
        {
            public Notification(int pId) { id = pId; }
            public int id;
            public RectTransform target;
            public string message;
            public Alignment alignment;
            public Vector2 pos;
            public bool notUpdatePoint;
            public bool displayArrow;
            public bool displayMessage;
            public bool displaySkip;
        }

        #endregion

        //=============================================

        #region Members

        [SerializeField] private Button mBtnBlock;

        [Separator("Auto Hide Widget")]
        [Header("Message only")]
        [SerializeField] private ImageWithText mIwtMessage;
        [Header("Message and a button")]
        [SerializeField] private RectTransform mTradeRect;
        [SerializeField] private PriceTMPButton mBtnTrade;
        [Header("Grid of multi choices")]
        [SerializeField] private OptimizedVerticalScrollView mOptionsGrid;
        [SerializeField] private JustButton mBtnCancelOption;
        [Separator("Notifications")] //This type of message used for notification, which mean it should not auto disapear when player tap else where
        [SerializeField] private List<MessageWithPointer> mNotifications;

        private Option[] mOptions;
        private bool mInitialized;
        private bool mLockOptionsGrid;

        public GameObject OptionsGrid => mOptionsGrid.gameObject;

        public static Message LockedFeatureMessage
        {
            get { return new Message("THIS FEATURE IS NOT UNLOCKED", new Vector2(600, 100)); }
        }

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Start()
        {
            mBtnBlock.onClick.AddListener(BtnBlock_Pressed);
        }

        private void Update()
        {
            CheckTouch();

            if (!mOptionsGrid.gameObject.activeSelf
                && !mIwtMessage.gameObject.activeSelf
                && !mTradeRect.gameObject.activeSelf)
            //&& !IsShowingNotification())
            {
                enabled = false;
                gameObject.SetActive(false);
            }
        }

        #endregion

        //=============================================

        #region Public

        public void ShowSimpleMessage(RectTransform pTarget, Message pMessage)
        {
            if (!mInitialized) Init();
            mIwtMessage.SetActive(true);

            mIwtMessage.rectTransform.sizeDelta = pMessage.size;
            mIwtMessage.rectTransform.position = pTarget.position;
            var anchoredPos = mIwtMessage.rectTransform.anchoredPosition;
            anchoredPos.y += pTarget.rect.height / 2f + mIwtMessage.rectTransform.rect.height / 2f;
            mIwtMessage.rectTransform.anchoredPosition = anchoredPos;

            mIwtMessage.label.text = pMessage.message;
            mIwtMessage.label.color = Color.black;
            mBtnBlock.SetActive(pMessage.blockInput);
            enabled = true;

            mIwtMessage.transform.localScale = Vector3.one;
            SimpleLeanFX.instance.Bubble(mIwtMessage.transform, 0.25f);
        }

        public void ShowWarning(RectTransform pTarget, Message pMessage)
        {
            ShowSimpleMessage(pTarget, pMessage);
            mIwtMessage.label.color = Color.red;
        }

        public void ShowOptions(RectTransform pTarget, Option[] pOptions, float pFixedWidth, Action pCancelAction = null)
        {
            if (!mInitialized)
                Init();
            mOptionsGrid.SetActive(true);
            mBtnBlock.SetActive(false);

            mOptions = pOptions;
            var gridRect = (mOptionsGrid.transform as RectTransform);
            if (pFixedWidth > 0)
            {
                var gridSize = gridRect.sizeDelta;
                gridSize.x = pFixedWidth;
                gridRect.sizeDelta = gridSize;

                var contentSize = mOptionsGrid.container.sizeDelta;
                contentSize.x = pFixedWidth;
                mOptionsGrid.container.sizeDelta = contentSize;

                mOptionsGrid.itemFixedWidth = pFixedWidth;
            }
            mOptionsGrid.Init(pOptions.Length, true);
            gridRect.position = pTarget.position;
            var anchoredPos = gridRect.anchoredPosition;
            anchoredPos.y += pTarget.rect.height / 4f + gridRect.rect.height / 2f;

            //Calculate top screen to ensure all options are visible
            var screenTop = (transform as RectTransform).TopLeft();
            var boxTop = anchoredPos.y + mOptionsGrid.scrollHeight / 2;
            float offsetTop = 0;
            if (boxTop > screenTop.y)
                offsetTop = screenTop.y - boxTop;
            anchoredPos.y += offsetTop;

            //Assign final position
            gridRect.anchoredPosition = anchoredPos;

            enabled = true;

            mBtnCancelOption.SetActive(pCancelAction != null);
            if (pCancelAction != null)
            {
                mBtnCancelOption.onClick.RemoveAllListeners();
                mBtnCancelOption.onClick.AddListener(() =>
                {
                    pCancelAction();
                    HideOptionsToolTip();
                });
            }

            gridRect.transform.localScale = Vector3.one;
            SimpleLeanFX.instance.Bubble(gridRect, 0.25f);
        }

        public void HideOptionsToolTip()
        {
            mOptionsGrid.SetActive(false);
        }

        public void ShowTradeOption(RectTransform pTarget, QuickTrade pOption)
        {
            if (!mInitialized) Init();
            mTradeRect.SetActive(true);
            mBtnBlock.SetActive(false);

            mTradeRect.position = pTarget.position;
            var anchoredPos = mTradeRect.anchoredPosition;
            anchoredPos.y += mTradeRect.rect.height / 2f;
            mTradeRect.anchoredPosition = anchoredPos;

            mBtnTrade.labelTMP.text = pOption.description;
            mBtnTrade.labelTMPCost.text = pOption.cost.ToString();
            mBtnTrade.onClick.RemoveAllListeners();
            mBtnTrade.onClick.AddListener(() =>
            {
                pOption.onTrade.Raise();
                mTradeRect.SetActive(false);
            });
            mBtnTrade.imgCurrency.sprite = AssetsCollection.instance.GetCurrencyIcon(pOption.currencyId);
            mBtnTrade.SetEnable(pOption.enabled);
            enabled = true;

            mTradeRect.transform.localScale = Vector3.one;
            SimpleLeanFX.instance.Bubble(mTradeRect, 0.25f);
        }

        public Option GetOption(int pIndex)
        {
            if (pIndex > mOptions.Length)
                return mOptions[mOptions.Length - 1];
            else if (pIndex < 0)
                return mOptions[0];
            return mOptions[pIndex];
        }

        public void LockOptionsGrid(bool pValue)
        {
            mLockOptionsGrid = pValue;
        }

        //==== Notifications

        public MessageWithPointer ShowNotificationBoard(Notification pNotification)
        {
            if (!mInitialized) Init();

            MessageWithPointer board = null;
            board = GetActiveNotification(pNotification.id);
            if (board == null)
                board = mNotifications.Obtain(transform);
            if (!pNotification.displayArrow)
                board.RectPointer.SetActive(false);
            board.RectMessage.SetActive(pNotification.displayMessage && pNotification.message.Trim().Length > 0);
            board.ButtonSkip.gameObject.SetActive(true);
            //  board.ButtonSkip.gameObject.SetActive(pNotification.displaySkip && (pNotification.displayArrow || pNotification.displayMessage));

            if (pNotification.displayArrow && !pNotification.notUpdatePoint)
                board.PointToTarget(pNotification.target, pNotification.alignment);
            //if (board.RectPointer.rectTransform().position.x < 0)
            //{
            //    board.ButtonSkip.rectTransform().anchoredPosition = new Vector2(353.0f, 97.0f);
            //}
            //else
            //{
            //    board.ButtonSkip.rectTransform().anchoredPosition = new Vector2(-353.0f, 97.0f);
            //}
            if (pNotification.notUpdatePoint)
            {

                if (board.RectPointer.position.y < 0)
                {
                    pNotification.pos = new Vector2(144f, 450.0f);
                }
                else
                {
                    pNotification.pos = new Vector2(144f, -620f);
                }

            }
            if (pNotification.displayMessage && pNotification.message.Trim().Length > 0)
                board.MessageToTarget(pNotification.message, pNotification.alignment, pNotification.pos);
            board.id = pNotification.id;
            board.SetActive(true);
            //   board.transform.SetParent(pNotification.target);

            mBtnBlock.SetActive(false);
            gameObject.SetActive(true);
            enabled = true;

            Debug.Log("Notification " + board.RectPointer.anchoredPosition + "_" + board.ButtonSkip.rectTransform().anchoredPosition);
            //SimpleLeanFX.instance.Bubble(board.transform, 0.25f);

            return board;
        }

        internal void HideNotificationBoard(int pId)
        {
            var board = GetActiveNotification(pId);
            if (board != null)
                board.SetActive(false);

        }

        #endregion

        //==============================================

        #region Private

        private void Init()
        {
            mIwtMessage.SetActive(false);
            mNotifications.Free();
            mTradeRect.SetActive(false);
            mOptionsGrid.SetActive(false);
            mInitialized = true;
        }

        private void BtnBlock_Pressed()
        {
            mIwtMessage.SetActive(false);
        }

        private void CheckTouch()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (mIwtMessage.gameObject.activeSelf)
                    CheckClickOutBoard(mIwtMessage.rectTransform);
                if (mTradeRect.gameObject.activeSelf)
                    CheckClickOutBoard(mTradeRect.transform as RectTransform);
                if (mOptionsGrid.gameObject.activeSelf && !mLockOptionsGrid)
                    CheckClickOutBoard(mOptionsGrid.transform as RectTransform, mBtnCancelOption.transform as RectTransform);
            }
        }

        private void CheckClickOutBoard(params RectTransform[] boards)
        {
            bool valid = false;
            for (int i = 0; i < boards.Length; i++)
            {
                var castPoint = UICamera.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);
                var inBoard = RectTransformUtility.RectangleContainsScreenPoint(boards[i], castPoint);
                if (inBoard && !valid)
                    valid = true;
            }
            if (!valid)
                for (int i = 0; i < boards.Length; i++)
                    boards[i].SetActive(false);
        }

        private bool IsShowingNotification()
        {
            for (int i = 0; i < mNotifications.Count; i++)
                if (mNotifications[i].gameObject.activeSelf)
                    return true;
            return false;
        }

        private MessageWithPointer GetActiveNotification(int pId)
        {
            for (int i = 0; i < mNotifications.Count; i++)
            {
                if (mNotifications[i].gameObject.activeSelf && mNotifications[i].id == pId)
                    return mNotifications[i];
            }
            return null;
        }

        #endregion
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(UITooltips))]
    public class UIToolTipsEditor : UnityEditor.Editor
    {
        private UITooltips mScript;

        private void OnEnable()
        {
            mScript = (UITooltips)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Focus to Laboratory"))
                MainPanel.instance.ShowNotificationBoard(new UITooltips.Notification(0)
                {
                    target = MainPanel.instance.MainMenuPanel.btnLaboratory.rectTransform,
                    message = "test message",
                    alignment = Alignment.TopRight,
                    pos = new Vector2(0f, 0f)
                });
            if (GUILayout.Button("Focus to Laboratory"))
                MainPanel.instance.ShowNotificationBoard(new UITooltips.Notification(0)
                {
                    target = MainPanel.instance.MainMenuPanel.btnLaboratory.rectTransform,
                    message = "test message",
                    alignment = Alignment.TopLeft,
                    pos = new Vector2(0f, 0f)
                });
        }
    }

#endif
}