using UnityEngine;
using FoodZombie.UI;
using System.Collections;
using Utilities.Service.RFirebase;

namespace FoodZombie
{
    public class SplashScene : MonoBehaviour
    {
        [SerializeField] private IntroPanel introPanel;

        private IEnumerator Start()
        {
            yield return null;
            GameInitializer.Instance.Init();

            yield return null;
            yield return new WaitUntil(() => RFirebaseManager.initialized);
            yield return null;
            introPanel.Init();
            
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}