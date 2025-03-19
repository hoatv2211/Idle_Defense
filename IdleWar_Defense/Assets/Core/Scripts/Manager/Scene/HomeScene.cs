using UnityEngine;
using FoodZombie.UI;

namespace FoodZombie
{
    public class HomeScene : MonoBehaviour
    {
        private void Start()
        {
            GameInitializer.Instance.Init();
            MainPanel.instance.Init();
            TutorialsManager.Instance.Init();
            //--
            TutorialsManager.Instance.Ready();
            
            GameData.Instance.SaveGame();
            
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}