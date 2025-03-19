using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UbhTitle : UbhMonoBehaviour
{
    private const string TITLE_PC = "Press X";
    private const string TITLE_MOBILE = "Tap To Start";

    [SerializeField, FormerlySerializedAs("_StartGUIText")]
    private Text m_startGUIText;

    private void Start()
    {
        m_startGUIText.text = UbhUtil.IsMobilePlatform() ? TITLE_MOBILE : TITLE_PC;
    }
}