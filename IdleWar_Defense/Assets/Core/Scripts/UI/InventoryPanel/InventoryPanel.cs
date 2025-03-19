using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula.Functions;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;

namespace FoodZombie.UI
{
    public class InventoryPanel : MyGamesBasePanel
    {
        private const int ODER_GEAR_BY_RANK = 1;
        private const int ODER_GEAR_BY_POWER = 2;
        private const int ODER_GEAR_BY_STAR = 3;
        private const int ODER_GEAR_BY_TYPE = 4;

        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;

        public Toggle togGear;
        public Toggle togItem;
        public GameObject imgNoticeItem;
        public Toggle togFragment;
        public GameObject imgNoticeFragment;

        public GameObject groupGear;
        public GameObject groupItem;
        public GameObject groupFragment;

        //=========
        public SimpleTMPButton btnGearRank;
        public GameObject imgHighlightGearRank;
        
        public SimpleTMPButton btnGearPower;
        public GameObject imgHighlightGearPower;
        
        public SimpleTMPButton btnGearStar;
        public GameObject imgHighlightGearStar;

        public SimpleTMPButton btnGearType;
        public GameObject imgHighlightGearType;

        public Transform transformGearViewPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<GearView> gearViewsPool;

        public Transform transformItemViewPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<ItemView> itemViewsPool;

        public Transform transformHeroViewPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<HeroFragmentView> heroFragmentViewsPool;

        [Separator("Bot-Center")]
        public SimpleTMPButton btnMain;
        public SimpleTMPButton btnFormation;
        public GameObject imgFormationNoti;
        public SimpleTMPButton btnBase;
        public GameObject imgBaseNoti;
        public Image imgBasePicture;
        public UnityEngine.UI.Text txtBaseUnlock;
        public SimpleTMPButton btnHero;
        public GameObject imgHeroNoti;

        private bool gearAscentRank = false;
        private bool gearAscentPower = false;
        private bool gearAscentStar = false;
        private bool gearAscentType = true;

        private int gearTab = ODER_GEAR_BY_RANK;
        private GearsGroup GearsGroup => GameData.Instance.GearsGroup;
        private BaseGroup BaseGroup => GameData.Instance.BaseGroup;
        private ItemsGroup ItemsGroup => GameData.Instance.ItemsGroup;
        private HeroesGroup HeroesGroup => GameData.Instance.HeroesGroup;

        private void Start()
        {
            togGear.onValueChanged.AddListener(TogGear_Changed);
            togItem.onValueChanged.AddListener(TogItem_Changed);
            togFragment.onValueChanged.AddListener(TogFragment_Changed);

            btnGearRank.onClick.AddListener(BtnGearRank_Pressed);
            btnGearPower.onClick.AddListener(BtnGearPower_Pressed);
            btnGearStar.onClick.AddListener(BtnGearStar_Pressed);
            btnGearType.onClick.AddListener(BtnGearType_Pressed);

            //bot-center
            btnMain.onClick.AddListener(BtnMain_Pressed);
            btnFormation.onClick.AddListener(BtnFormation_Pressed);
            btnHero.onClick.AddListener(BtnHero_Pressed);
            btnBase.onClick.AddListener((BtnBase_Pressed));

            EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
            EventDispatcher.AddListener<ChangeGearEvent>(OnChangeGear);
            EventDispatcher.AddListener<FormationChangeEvent>(OnFormationChange);
            EventDispatcher.AddListener<HeroStarUpEvent>(OnHeroStarUp);
            EventDispatcher.AddListener<HeroLevelUpEvent>(OnHeroLevelUp);
            EventDispatcher.AddListener<HeroEvolutionEvent>(OnHeroEvolution);
            EventDispatcher.AddListener<BaseLevelUpEvent>(OnBaseLevelUp);
        }

        private void OnDestroy()
        {
            EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
            EventDispatcher.RemoveListener<ChangeGearEvent>(OnChangeGear);
            EventDispatcher.RemoveListener<FormationChangeEvent>(OnFormationChange);
            EventDispatcher.RemoveListener<HeroStarUpEvent>(OnHeroStarUp);
            EventDispatcher.RemoveListener<HeroLevelUpEvent>(OnHeroLevelUp);
            EventDispatcher.RemoveListener<HeroEvolutionEvent>(OnHeroEvolution);
            EventDispatcher.RemoveListener<BaseLevelUpEvent>(OnBaseLevelUp);
        }

        internal override void Init()
        {
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            InitBtns();

            if (togGear.isOn) ShowGears();
            else if (togItem.isOn) ShowItems();
            else ShowFragments();
        }

        private void InitBtns()
        {
            ShowHeroBtn();
            ShowFormationBtn();
            ShowBaseBtn();
        }

        //gear
        private void ShowGears()
        {
            groupGear.SetActive(true);
            groupItem.SetActive(false);
            groupFragment.SetActive(false);
            imgNoticeItem.SetActive(GameData.Instance.ItemsGroup.CheckNotiItem());
            imgNoticeFragment.SetActive(GameData.Instance.ItemsGroup.CheckNoticePart());

            if (gearTab == ODER_GEAR_BY_RANK) ShowGearsByRank();
            else if (gearTab == ODER_GEAR_BY_POWER) ShowGearsByPower();
            else if (gearTab == ODER_GEAR_BY_STAR) ShowGearsByStar();
            else if (gearTab == ODER_GEAR_BY_TYPE) ShowGearsByType();
        }

        private void ShowGearBy(List<GearData> gearDatas)
        {
            gearViewsPool.Free();
            var count = gearDatas.Count;

            //show
            for (int i = 0; i < count; i++)
            {
                var gearView = gearViewsPool.Obtain(transformGearViewPool);
                gearView.Init(gearDatas[i], ChoiceGearView);
                gearView.SetActive(true);
            }
        }

        private void ChoiceGearView(GearView gearView)
        {
            MainPanel.instance.ShowGearDetailPanel(gearView.gearData, ShowGears);
        }

        private void BtnGearRank_Pressed()
        {
            if (gearTab == ODER_GEAR_BY_RANK) gearAscentRank = !gearAscentRank;
            gearTab = ODER_GEAR_BY_RANK;
            ShowGearsByRank();
        }

        private void BtnGearPower_Pressed()
        {
            if (gearTab == ODER_GEAR_BY_POWER) gearAscentPower = !gearAscentPower;
            gearTab = ODER_GEAR_BY_POWER;
            ShowGearsByPower();
        }

        private void BtnGearStar_Pressed()
        {
            if (gearTab == ODER_GEAR_BY_STAR) gearAscentStar = !gearAscentStar;
            gearTab = ODER_GEAR_BY_STAR;
            ShowGearsByStar();
        }

        private void BtnGearType_Pressed()
        {
            if (gearTab == ODER_GEAR_BY_TYPE) gearAscentType = !gearAscentType;
            gearTab = ODER_GEAR_BY_TYPE;
            ShowGearsByType();
        }

        private void ShowGearsByRank()
        {
            imgHighlightGearRank.SetActive(true);
            imgHighlightGearPower.SetActive(false);
            imgHighlightGearStar.SetActive(false);
            imgHighlightGearType.SetActive(false);

            var gearDatas = GearsGroup.GetAllGearDatas();
            var count = gearDatas.Count;
            GearData gearDataTemp;
            //oder
            for (int i = 0; i < count - 1; i++)
            {
                var gearData1 = gearDatas[i];
                for (int j = i + 1; j < count; j++)
                {
                    var gearData2 = gearDatas[j];

                    if (gearAscentRank)
                    {
                        if (gearData1.Rank > gearData2.Rank)
                        {
                            gearDataTemp = gearDatas[i];
                            gearDatas[i] = gearDatas[j];
                            gearDatas[j] = gearDataTemp;
                        }
                    }
                    else
                    {
                        if (gearData1.Rank < gearData2.Rank)
                        {
                            gearDataTemp = gearDatas[i];
                            gearDatas[i] = gearDatas[j];
                            gearDatas[j] = gearDataTemp;
                        }
                    }
                }
            }

            ShowGearBy(gearDatas);
        }

        private void ShowGearsByPower()
        {
            imgHighlightGearRank.SetActive(false);
            imgHighlightGearPower.SetActive(true);
            imgHighlightGearStar.SetActive(false);
            imgHighlightGearType.SetActive(false);

            var gearDatas = GearsGroup.GetAllGearDatas();
            if (gearAscentPower)
            {
                gearDatas.Sort(delegate (GearData c1, GearData c2) { return (c1.Power - c2.Power); });
            }
            else
            {
                gearDatas.Sort(delegate (GearData c1, GearData c2) { return (c2.Power - c1.Power); });
            }

            ShowGearBy(gearDatas);
        }

        private void ShowGearsByStar()
        {
            imgHighlightGearRank.SetActive(false);
            imgHighlightGearPower.SetActive(false);
            imgHighlightGearStar.SetActive(true);
            imgHighlightGearType.SetActive(false);

            var gearDatas = GearsGroup.GetAllGearDatas();
            var count = gearDatas.Count;
            GearData gearDataTemp;
            //oder
            for (int i = 0; i < count - 1; i++)
            {
                var gearData1 = gearDatas[i];
                for (int j = i + 1; j < count; j++)
                {
                    var gearData2 = gearDatas[j];

                    if (gearAscentStar)
                    {
                        if (gearData1.star > gearData2.star)
                        {
                            gearDataTemp = gearDatas[i];
                            gearDatas[i] = gearDatas[j];
                            gearDatas[j] = gearDataTemp;
                        }
                    }
                    else
                    {
                        if (gearData1.star < gearData2.star)
                        {
                            gearDataTemp = gearDatas[i];
                            gearDatas[i] = gearDatas[j];
                            gearDatas[j] = gearDataTemp;
                        }
                    }
                }
            }

            ShowGearBy(gearDatas);
        }

        private void ShowGearsByType()
        {
            imgHighlightGearRank.SetActive(false);
            imgHighlightGearPower.SetActive(false);
            imgHighlightGearStar.SetActive(false);
            imgHighlightGearType.SetActive(true);

            var gearDatas = GearsGroup.GetAllGearDatas();
            var count = gearDatas.Count;
            GearData gearDataTemp;
            //oder
            for (int i = 0; i < count - 1; i++)
            {
                var gearData1 = gearDatas[i];
                for (int j = i + 1; j < count; j++)
                {
                    var gearData2 = gearDatas[j];

                    if (gearAscentType)
                    {
                        if (gearData1.Slot > gearData2.Slot)
                        {
                            gearDataTemp = gearDatas[i];
                            gearDatas[i] = gearDatas[j];
                            gearDatas[j] = gearDataTemp;
                        }
                    }
                    else
                    {
                        if (gearData1.Slot < gearData2.Slot)
                        {
                            gearDataTemp = gearDatas[i];
                            gearDatas[i] = gearDatas[j];
                            gearDatas[j] = gearDataTemp;
                        }
                    }
                }
            }

            ShowGearBy(gearDatas);
        }

        //item
        private void ShowItems()
        {
            groupGear.SetActive(false);
            groupItem.SetActive(true);
            groupFragment.SetActive(false);
            imgNoticeItem.SetActive(false);
            imgNoticeFragment.SetActive(GameData.Instance.ItemsGroup.CheckNoticePart());


            itemViewsPool.Free();
            var traps = BaseGroup.GetAllTrapDatas();
            var count = traps.Count;

            //show
            ItemView itemView;
            for (int i = 0; i < count; i++)
            {
                var trap = traps[i];
                if (trap.StockNumber > 0)
                {
                    itemView = itemViewsPool.Obtain(transformItemViewPool);
                    itemView.Init(trap, ChoiceItemView);
                    itemView.SetActive(true);
                }
            }
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_COIN, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_GEM, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_EXP_HERO, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_DUST_ELECTRIC, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_DUST_METALIC, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_DUST_NITROGEN, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_DUST_LAVA, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_MATERIAL, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_TICKET, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_TICKET_PVP, ChoiceItemView, true, true);

            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_HONOR, ChoiceItemView, true, true);

            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_POWER_FRAGMENT, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_POWER_CRYSTAL, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_DEVINE_CRYSTAL, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_BLUE_CHIP, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_GOLDEN_CHIP, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_BLUE_HERO_FRAGMENT, ChoiceItemView, true, true);
            //itemView.SetActive(true);
            itemView = itemViewsPool.Obtain(transformItemViewPool);
            itemView.Init(IDs.CURRENCY_EPIC_HERO_FRAGMENT, ChoiceItemView, true, true);
            //itemView.SetActive(true);
        }

        private void ChoiceItemView(ItemView itemView)
        {
            MainPanel.instance.ShowItemDetailPanel(itemView, ShowItems);
        }

        //fragment
        private void ShowFragments()
        {
            groupGear.SetActive(false);
            groupItem.SetActive(false);
            groupFragment.SetActive(true);

            imgNoticeItem.SetActive(GameData.Instance.ItemsGroup.CheckNotiItem());
            imgNoticeFragment.SetActive(false);

            heroFragmentViewsPool.Free();
            var heroFragments = ItemsGroup.GetHeroFragmentItems();
            var count = heroFragments.Count;

            //show
            for (int i = 0; i < count; i++)
            {
                var heroFragment = heroFragments[i];
                if (heroFragment.StockNumber > 0)
                {
                    var heroFragmentView = heroFragmentViewsPool.Obtain(transformHeroViewPool);
                    heroFragmentView.Init(heroFragment, ChoiceHeroFragmentView);
                    heroFragmentView.SetActive(true);
                }
            }
        }

        private void ChoiceHeroFragmentView(HeroFragmentView heroFragmentView)
        {
            MainPanel.instance.ShowHeroFragmentDetailPanel(heroFragmentView.heroFragmentItemData, ShowFragments);
        }

        //UI
        private void TogGear_Changed(bool value)
        {
            if (value)
            {
                ShowGears();

                Config.LogEvent(TrackingConstants.CLICK_INVENTORY_GEAR);
            }
        }

        private void TogItem_Changed(bool value)
        {
            if (value)
            {
                ShowItems();

                Config.LogEvent(TrackingConstants.CLICK_INVENTORY_ITEM);
            }
        }

        private void TogFragment_Changed(bool value)
        {
            if (value)
            {
                ShowFragments();

                Config.LogEvent(TrackingConstants.CLICK_INVENTORY_PART);
            }
        }

        //bot-center
        private void BtnMain_Pressed()
        {
            Back();
            Config.LogEvent(TrackingConstants.CLICK_MAINMENU);
        }

        private void BtnHero_Pressed()
        {
            Back();
            MainPanel.instance.ShowHeroPanel();
        }

        private void BtnBase_Pressed()
        {
            Back();
            MainPanel.instance.ShowBasePanel();
        }

        private void BtnFormation_Pressed()
        {
            Back();
            MainPanel.instance.ShowFormationPanel();
        }

        //noti
        private void OnCurrencyChanged(CurrencyChangedEvent e)
        {
            if (e.id == IDs.CURRENCY_COIN)
            {
                ShowHeroBtn();
                ShowBaseBtn();
            }
            else if (e.id == IDs.CURRENCY_EXP_HERO)
            {
                ShowHeroBtn();
            }
        }

        private void ShowHeroBtn()
        {
            imgHeroNoti.SetActive(GameData.Instance.HeroesGroup.CheckHeroUpgradeNoti());
        }

        private void OnChangeGear(ChangeGearEvent e)
        {
            ShowHeroBtn();
        }

        private void OnFormationChange(FormationChangeEvent e)
        {
            ShowFormationBtn();
            ShowHeroBtn();
        }

        private void ShowFormationBtn()
        {
            imgFormationNoti.SetActive(GameData.Instance.HeroesGroup.CheckFormationNoti());
        }

        private void OnHeroStarUp(HeroStarUpEvent e)
        {
            ShowHeroBtn();
        }

        private void OnHeroLevelUp(HeroLevelUpEvent e)
        {
            ShowHeroBtn();
        }

        private void OnHeroEvolution(HeroEvolutionEvent e)
        {
            ShowHeroBtn();
        }

        private void ShowBaseBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_BASE;
            if (unlocked)
            {
                btnBase.interactable = true;
                imgBasePicture.color = Color.white;
                txtBaseUnlock.gameObject.SetActive(false);
            }
            else
            {
                btnBase.interactable = false;
                imgBasePicture.color = Color.gray;
                txtBaseUnlock.text = Localization.Get(Localization.ID.CLEAR).ToUpper() + "\n" + GameData.Instance.MissionLevelToString(Constants.UNLOCK_BASE);
                txtBaseUnlock.gameObject.SetActive(true);
            }

            imgBaseNoti.SetActive(unlocked && GameData.Instance.BaseGroup.CheckNoti());
        }

        private void OnBaseLevelUp(BaseLevelUpEvent e)
        {
            ShowBaseBtn();
        }
    }
}