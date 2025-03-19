using UnityEngine;
using FoodZombie.UI;

namespace FoodZombie
{
    public class GameplayScene : MonoBehaviour
    {
        private void Start()
        {
            MainGamePanel.instance.Init();
            TutorialsManager.Instance.Init();
            //--
            TutorialsManager.Instance.Ready();
            
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}