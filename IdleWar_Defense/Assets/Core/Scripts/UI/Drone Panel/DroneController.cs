using DG.Tweening;
using FoodZombie.UI;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.UI.Drone_Panel
{
	public class DroneController : MonoBehaviour
	{
		public bool isClaimed = false;
		public enum DroneType
		{
			Ads = 0, IAP = 1
		}

		[SerializeField] private DroneType droneType;
		[SerializeField] private SkeletonGraphic skeletonGraphic;
		[SerializeField] private Button button;
		public void Init(SkeletonDataAsset skeletonDataAsset, DroneType type)
		{
			skeletonGraphic.skeletonDataAsset = skeletonDataAsset;
			skeletonGraphic.Initialize(true);
			droneType = type;
			transform.localPosition = new Vector3(0, 1500, 0);
			transform.DOLocalMove(Vector3.zero, 5f).SetEase(Ease.OutQuart);
			button.enabled = true;
		}

		public void OnClick()
		{
			//GameplayController.Instance.PauseGame();
			if (droneType == DroneType.Ads)
			{
				MainGamePanel.instance.ShowDroneWatchAdPopup(PurchaseSuccess);
			}
			else
			{
				MainGamePanel.instance.ShowDronePayGemPopup(PurchaseSuccess);
			}
		}

		private void PurchaseSuccess()
		{
			button.enabled = false;
			isClaimed = true;
			var tween = transform.DOLocalMove(new Vector3(0, -1500, 0), 5f).SetEase(Ease.InQuart).SetDelay(1f);
			tween.OnComplete(() => { transform.localPosition = new Vector3(0, 1500, 0); });
		}
	}
}
