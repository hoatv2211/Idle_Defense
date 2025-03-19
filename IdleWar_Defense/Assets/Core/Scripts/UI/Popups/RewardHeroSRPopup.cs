using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FoodZombie;
using Utilities.Components;


///Popup add-on: show hero reward - summon 1 || summon 10 + rank=4
public class RewardHeroSRPopup : MonoBehaviour
{
    [SerializeField] private List<GameObject> listHeroes;
    [SerializeField] private List<Sprite> sprRank;
    [SerializeField] private List<Sprite> sprLabel;
    [SerializeField] private Image imgRank;
    [SerializeField] private Image imgLabel;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private SimpleTMPButton btnClose;

    [SerializeField] private GameObject main;

    public void CallStart(HeroDefinition _data)
    {
        gameObject.SetActive(true);
        main.transform.localPosition = new Vector3(4000, 4000); //quick fix effect layer
        SoundManager.Instance.PlaySFX(IDs.SOUND_VICTORY,1);

        btnClose.SetUpEvent(Action_BtnExit);
        txtName.text = _data.name;
        imgRank.sprite = sprRank[_data.rank-1];
        imgLabel.sprite = sprLabel[_data.rank-1];

        for(int i = 0; i < listHeroes.Count; i++)
        {
            listHeroes[i].SetActive(i== _data.id-1);
        }
    }


    private void Action_BtnExit()
    {
        gameObject.SetActive(false);
        main.transform.localPosition = new Vector3(0, 0);
    }
}
