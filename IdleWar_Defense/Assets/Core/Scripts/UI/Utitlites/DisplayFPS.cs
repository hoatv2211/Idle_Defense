using TMPro;
using UnityEngine;

namespace FoodZombie.UI
{
    public class DisplayFPS : MonoBehaviour
    {
        public bool showFps;
        public TextMeshProUGUI txtFps;

        private float timeEslap;
        private int countFrame;

        private void Update()
        {
            if (!showFps)
                return;

            timeEslap += Time.deltaTime;
            countFrame++;

            if (timeEslap >= 1)
            {
                float fps = countFrame / timeEslap;

                timeEslap = 0;
                countFrame = 0;

                txtFps.text = string.Format("FPS: {0}", Mathf.Round(fps));
            }
        }
    }
}
