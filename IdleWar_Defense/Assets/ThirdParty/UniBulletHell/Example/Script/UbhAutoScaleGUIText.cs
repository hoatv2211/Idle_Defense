using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UbhAutoScaleGUIText : MonoBehaviour
{
    private Text m_guiText;
    private float m_orgFontSize;

    private void Awake()
    {
        m_guiText = GetComponent<Text>();
        m_orgFontSize = m_guiText.fontSize;
    }

    private void Update()
    {
        float screenScaleX = (float)Screen.width / (float)UbhManager.BASE_SCREEN_WIDTH;
        float screenScaleY = (float)Screen.height / (float)UbhManager.BASE_SCREEN_HEIGHT;
        float screenScale = Screen.height < Screen.width ? screenScaleY : screenScaleX;

        m_guiText.fontSize = (int)(m_orgFontSize * screenScale);
    }
}
