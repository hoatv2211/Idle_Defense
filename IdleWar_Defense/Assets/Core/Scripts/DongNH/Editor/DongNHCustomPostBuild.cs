#if UNITY_IOS
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;

using UnityEditor.iOS.Xcode;

using System;
using System.IO;
using UnityEngine;

public class DongNHCustomPostBuild
{
	/// <summary>
	/// Description for IDFA request notification 
	/// [sets NSUserTrackingUsageDescription]
	/// </summary>
	const string TrackingDescription =
		"This identifier will be used to deliver personalized ads to you";
	const string NSCalendarsUsageDescription =
	 "Calender is required to set match reminders";

	[PostProcessBuild(1)]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToXcode)
	{
		if (buildTarget == BuildTarget.iOS)
		{
			AddPListValues(pathToXcode);

			string projectPath = pathToXcode + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject pbxProject = new PBXProject();
			pbxProject.ReadFromFile(projectPath);

			//    pbxProject.Set
			string target = pbxProject.GetUnityMainTargetGuid();
			pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", DongNHEditorSetting.Instance.IOS_Build_enableBitcode ? "YES" : "NO");
			Debug.Log("Bitcode " + DongNHEditorSetting.Instance.IOS_Build_enableBitcode);
			if (DongNHEditorSetting.Instance.CapabilityTypeList.Length > 0)
			{
				foreach (var item in DongNHEditorSetting.Instance.CapabilityTypeList)
				{
					string itemString = "";
					switch (item)
					{
						case CapabilityTypeEnum.ApplePay:
							itemString = "com.apple.ApplePay";
							break;
						case CapabilityTypeEnum.SignInWithApple:
							itemString = "com.apple.SignInWithApple";
							break;
						case CapabilityTypeEnum.AccessWiFiInformation:
							itemString = "com.apple.AccessWiFiInformation";
							break;
						case CapabilityTypeEnum.WirelessAccessoryConfiguration:
							itemString = "com.apple.WAC";
							break;
						case CapabilityTypeEnum.Wallet:
							itemString = "com.apple.Wallet";
							break;
						case CapabilityTypeEnum.Siri:
							itemString = "com.apple.Siri";
							break;
						case CapabilityTypeEnum.PushNotifications:
							itemString = "com.apple.Push";
							break;
						case CapabilityTypeEnum.PersonalVPN:
							itemString = "com.apple.VPNLite";
							break;
						case CapabilityTypeEnum.KeychainSharing:
							itemString = "com.apple.KeychainSharing";
							break;
						case CapabilityTypeEnum.InterAppAudio:
							itemString = "com.apple.InterAppAudio";
							break;
						case CapabilityTypeEnum.Maps:
							itemString = "com.apple.Maps.iOS";
							break;
						case CapabilityTypeEnum.iCloud:
							itemString = "com.apple.iCloud";
							break;
						case CapabilityTypeEnum.HomeKit:
							itemString = "com.apple.HomeKit";
							break;
						case CapabilityTypeEnum.HealthKit:
							itemString = "com.apple.HealthKit";
							break;
						case CapabilityTypeEnum.GameCenter:
							itemString = "com.apple.GameCenter";
							break;
						case CapabilityTypeEnum.DataProtection:
							itemString = "com.apple.DataProtection";
							break;
						case CapabilityTypeEnum.BackgroundModes:
							itemString = "com.apple.BackgroundModes";
							break;
						case CapabilityTypeEnum.AssociatedDomains:
							itemString = "com.apple.SafariKeychain";
							break;
						case CapabilityTypeEnum.AppGroups:
							itemString = "com.apple.ApplicationGroups.iOS";
							break;
						case CapabilityTypeEnum.InAppPurchase:
							itemString = "com.apple.InAppPurchase";
							break;
						default:
							break;
					}
					pbxProject.AddCapability(target, PBXCapabilityType.StringToPBXCapabilityType(itemString));
					Debug.Log("AddCapability " + item.ToString());
				}

			}
			//  target.set

			pbxProject.WriteToFile(projectPath);
		}
	}

	static void AddPListValues(string pathToXcode)
	{
		// Get Plist from Xcode project 
		string plistPath = pathToXcode + "/Info.plist";

		// Read in Plist 
		PlistDocument plistObj = new PlistDocument();
		plistObj.ReadFromString(File.ReadAllText(plistPath));

		// set values from the root obj
		PlistElementDict plistRoot = plistObj.root;

		// Set NSUserTrackingUsageDescription:
		if (DongNHEditorSetting.Instance.IOS_Build_NSUserTrackingUsageDescriptions)
		{
			Debug.Log("NSUserTrackingUsageDescriptions on");
			plistRoot.SetString("NSUserTrackingUsageDescription", TrackingDescription);
		}
		if (DongNHEditorSetting.Instance.IOS_Build_NSCalendarsUsageDescription)
		{
			Debug.Log("NSCalendarsUsageDescription on");
			plistRoot.SetString("NSCalendarsUsageDescription", NSCalendarsUsageDescription);
		}
		//Set SKAdNetworkItems:
		PlistElementArray SKAdNetworkItems = null;
		if (DongNHEditorSetting.Instance.IOS_Build_SKAdNetworkItems)
		{
			Debug.Log("IOS_Build_SKAdNetworkItems on");
			if (plistRoot.values.ContainsKey("SKAdNetworkItems"))
			{
				try
				{
					SKAdNetworkItems = plistRoot.values["SKAdNetworkItems"] as PlistElementArray;
				}
				catch (System.Exception ex)
				{
					Debug.LogWarning(string.Format("Could not obtain SKAdNetworkItems PlistElementArray: {0}", ex.Message));
					//   throw;
				}

				if (SKAdNetworkItems == null)
				{
					SKAdNetworkItems = plistRoot.CreateArray("SKAdNetworkItems");
				}

				string plistContent = File.ReadAllText(plistPath);
				//if (!plistContent.Contains(IronSourceConstants.IRONSOURCE_SKAN_ID_KEY))
				//{
				//    PlistElementDict SKAdNetworkIdentifierDict = SKAdNetworkItems.AddDict();
				//    SKAdNetworkIdentifierDict.SetString("SKAdNetworkIdentifier", IronSourceConstants.IRONSOURCE_SKAN_ID_KEY);
				//}

				string[] ids = NetIDs.IDs;
				foreach (var id in ids)
				{
					CustomAddNetworkID(plistContent, id, SKAdNetworkItems);
				}
			}
		}
		// save
		File.WriteAllText(plistPath, plistObj.WriteToString());
	}

	private string[] NetworkIDs = new string[] { };
	private static void CustomAddNetworkID(string plistContent, string idToAdd, PlistElementArray SKAdNetworkItems)
	{
		//  string plistContent = File.ReadAllText(plistPath);
		if (!plistContent.Contains(idToAdd))
		{
			PlistElementDict SKAdNetworkIdentifierDict = SKAdNetworkItems.AddDict();
			SKAdNetworkIdentifierDict.SetString("SKAdNetworkIdentifier", idToAdd);
		}
	}
}



#endif