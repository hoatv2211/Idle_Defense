using FoodZombie;
using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Service;

public class InappPopup : MyGamesBasePanel
{
    public PackageView packageView;
    public GameObject PanelPack, PanelSubcription;
    public GameObject PanelProfit;
    public Text TextProfit;
    public PremiumPackageView[] premiumPackageViews;
    public Button Btn_Back1, Btn_Back2;


    public const int ID_1stTimeBuy = 1;
    public const int ID_starterPack = 2;
    public const int ID_beginnerPack = 3;
    public const int ID_lv10Pack = 6;
    public const int ID_lv20Pack = 7;
    public const int ID_lv30Pack = 8;
    public const int ID_lv40Pack = 9;
    public const int ID_lv50Pack = 10;
    public const int ID_lv60Pack = 11;
    public const int ID_lv70Pack = 12;
    public const int ID_lv80Pack = 13;
    public const int ID_lv90Pack = 14;
    public const int ID_lv100Pack = 15;
    public const int ID_subcription = 99;
    private static StoreGroup StoreGroup => GameData.Instance.StoreGroup;
    System.Action OnDone;
    protected override void Awake()
    {
        base.Awake();
        Btn_Back1.onClick.AddListener(BtnBack_Pressed);
        Btn_Back2.onClick.AddListener(BtnBack_Pressed);
        packageView.OnBuySuccess = BtnBack_Pressed;
    }
    public void SetAction(System.Action onDone)
    {
        this.OnDone = onDone;
    }
    //bool isInit = false;
    internal override void Init()
    {
        //if (isInit) return;
        base.Init();
        //Refresh();
        //isInit = true;
    }
    internal override void Back()
    {
        base.Back();
        Debug.Log("inapp Popup back");


    }
    protected override void AfterHiding()
    {
        base.AfterHiding();
        if (MainPanel.instance.TopPanel is MainMenuPanel || MainPanel.instance.TopPanel is LevelUpPanel)
            if (this.OnDone != null)
                this.OnDone();
    }
    public void LoadViewPack()
    {
        int target = CheckConditionToShow();
       // Debug.LogError("target " + target);
        if (target <= 0)
        {
            BtnBack_Pressed();
            return;
        }
        PanelProfit.gameObject.SetActive(false);
        if (target == ID_subcription)
        {
            PanelSubcription.SetActive(true);
            PanelPack.SetActive(false);
            LoadSubscription();
        }
        else
        {
            PanelSubcription.SetActive(false);
            PanelPack.SetActive(true);
            LoadPack(target);
        }
    }
    //void Refresh()
    //{
    //	//show
    //	//	int target = GameData.Instance.StoreGroup.InAppPopupTarget;

    //}

    /// <summary>
    /// Check again to show
    /// </summary>
    void CheckSpecialPackToShow()
    {

    }

    /// <summary>
    /// Check Special Pack by ID
    /// Null if cant show,cant buy
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    static PackageData SpecialPackByID(int ID)
    {
        List<PackageData> packageDatas = StoreGroup.GetAllSpecialPackageDatas();
        int count = packageDatas.Count;
        for (int i = 0; i < count; i++)
        {
            var packageData = packageDatas[i];
            if (ID == ID_1stTimeBuy)
            {
                if (packageData.Id == 1)
                {
                    return packageData;
                }
            }
            else
            {
                if (packageData.Id == ID)
                {
                    if (packageData.CanShow && packageData.CanBuy)
                        return packageData;
                    else
                        return null;
                }
            }

        }
        return null;
    }
    void LoadPack(int target)
    {
        //pack
        PackageData packToShow = null;
        List<PackageData> packageDatas = StoreGroup.GetAllSpecialPackageDatas();
        int count = packageDatas.Count;
        for (int i = 0; i < count; i++)
        {
            var packageData = packageDatas[i];

            // Debug.Log(packageData.Id + "_ " + packageData.Name + "_" + packageData.LogName);
            //    if (packageData.CanShow && packageData.CanBuy)
            {
                if (packageData.Id == target)
                {
                    packToShow = packageData;
                    packageView.Init(packToShow);
                    StoreGroup.InAppPopupUpdateShow(target, true);
                    if (target == ID_starterPack || target == ID_beginnerPack)
                    {
                        PanelProfit.SetActive(true);
                        TextProfit.text = "BEST SELLER";
                    }
                    else
                    {
                        if (target != ID_1stTimeBuy)
                        {
                            PanelProfit.SetActive(true);
                            TextProfit.text = "+50% ITEMS";
                        }
                    }
                }
            }
        }
    }
    void LoadSubscription()
    {
        //premium
        var premiumPackageDatas = StoreGroup.GetAllPremiumPackageDatas();
        int count = premiumPackageDatas.Count;
        for (int i = 0; i < count; i++)
        {
            premiumPackageViews[i].Init(premiumPackageDatas[i]);
        }
        StoreGroup.InAppPopupUpdateShow(ID_subcription, true);
        StoreGroup.InappPopupSubcriptionDayShow = 1;
    }

    public static void RequestShow(int idToShow)
    {
        if (idToShow == ID_starterPack)
        {
            if (GameData.Instance.StoreGroup.InAppPopupIsShow(idToShow))
            {
                //some pack only show 1 time
                return;
            }
        }

        GameData.Instance.StoreGroup.InAppPopupRequestShow(idToShow);
    }
    public static int CheckConditionToShow()
    {
        PackageData packData = null;
        int currentMissionID = GameData.Instance.MissionsGroup.CurrentMissionId;

        //Check 1str Purchase:
        if (currentMissionID > 1005)
        {
            packData = SpecialPackByID(ID_1stTimeBuy);
            if (packData != null && !GameData.Instance.StoreGroup.InAppPopupIsShow(ID_1stTimeBuy))
            {
                //can show
                var buyCount = GameData.Instance.StoreGroup.BuyCount;
                if (buyCount == 0)
                    return ID_1stTimeBuy;
            }
        }
        //Check BeginnerPack
        if (Config.isLoseToday)
        {
            packData = SpecialPackByID(ID_beginnerPack);
            if (packData != null && !GameData.Instance.StoreGroup.InAppPopupIsShow(ID_beginnerPack))
            {
                //can show
                return ID_beginnerPack;
            }
        }
        //Check StarterPack
        if (currentMissionID > 1010)
        {
            packData = SpecialPackByID(ID_starterPack);
            if (packData != null && !GameData.Instance.StoreGroup.InAppPopupIsShow(ID_starterPack))
            {
                //can show
                return ID_starterPack;
            }
        }
        //Check Subscription
        if (GameData.Instance.UserGroup.MissGemCount >= 5)
        {
            //  packData = SpecialPackByID(ID_subcription);
            if (!GameData.Instance.StoreGroup.InAppPopupIsShow(ID_subcription))
            {
                //can show
                var premiumPackageDatas = StoreGroup.GetAllPremiumPackageDatas();
                bool isSubscribed = false;
                foreach (var item in premiumPackageDatas)
                {
                    if (PaymentHelper.IsSubscribed(item))
                        isSubscribed = true;
                }
                if (!isSubscribed)
                    return ID_subcription;
            }
        }
        //Check LevelUp
        //Check Level Special Pack:
        if (CheckLevelPack(10, ID_lv10Pack)) return ID_lv10Pack;
        if (CheckLevelPack(20, ID_lv20Pack)) return ID_lv20Pack;
        if (CheckLevelPack(30, ID_lv30Pack)) return ID_lv30Pack;
        if (CheckLevelPack(40, ID_lv40Pack)) return ID_lv40Pack;
        if (CheckLevelPack(50, ID_lv50Pack)) return ID_lv50Pack;
        if (CheckLevelPack(60, ID_lv60Pack)) return ID_lv60Pack;
        if (CheckLevelPack(70, ID_lv70Pack)) return ID_lv70Pack;
        if (CheckLevelPack(80, ID_lv80Pack)) return ID_lv80Pack;
        if (CheckLevelPack(90, ID_lv90Pack)) return ID_lv90Pack;
        if (CheckLevelPack(100, ID_lv100Pack)) return ID_lv100Pack;

        return -1;


    }
    static bool CheckLevelPack(int level, int IDPack)
    {
        PackageData packData = null;
        int userLevel = GameData.Instance.UserGroup.LevelShowup;
        if (userLevel >= level)
        {
            packData = SpecialPackByID(IDPack);
            if (packData != null && !GameData.Instance.StoreGroup.InAppPopupIsShow(IDPack))
            {
                //can show
                return true;
            }
        }
        return false;
    }
}
