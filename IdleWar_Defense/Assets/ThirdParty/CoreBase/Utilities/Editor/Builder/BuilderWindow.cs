﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Utilities.Common;
using Debug = UnityEngine.Debug;

namespace Utilities.Editor
{
    public class BuilderWindow : EditorWindow
    {
        private BuildSettingsCollection mBuildProfilesCollection;
        private Vector2 scrollPosition;
        private int mRemovingIndex = -1;
        private int mToggleOptions = -1;
        private int mSelectedCount;
        private string mCommandLine;

        private void OnEnable()
        {
            Init();
        }

        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            mSelectedCount = 0;

            EditorHelper.BoxVertical("Simple Builder Util [by NBear]", () =>
            {
                for (int i = 0; i < mBuildProfilesCollection.profiles.Count; i++)
                {
                    var profile = mBuildProfilesCollection.profiles[i];
                    bool show = true;

                    EditorHelper.BoxHorizontal(() =>
                    {
                        profile.selected = EditorGUILayout.Toggle(profile.selected, GUILayout.Height(20), GUILayout.Width(20));
                        mSelectedCount += profile.selected ? 1 : 0;
                        string surfix = string.IsNullOrEmpty(profile.note) ? "" : $" [{profile.note}]";
                        string prefix = "";
                        if (!string.IsNullOrEmpty(profile.bundleVersion))
                            prefix = $"[{profile.bundleVersion}|{profile.bundleVersionCode}] ";
                        show = EditorHelper.HeaderFoldout(prefix + profile.GetBuildName() + surfix, "BuilderWindow" + i);

                        if (mToggleOptions == i)
                        {
                            if (mRemovingIndex == i)
                            {
                                EditorHelper.BoxHorizontal(() =>
                                {
                                    if (EditorHelper.ButtonColor("Yes", Color.green))
                                    {
                                        mBuildProfilesCollection.profiles.Remove(profile);
                                        mRemovingIndex = -1;
                                    }
                                    else if (EditorHelper.ButtonColor("No", Color.red))
                                        mRemovingIndex = -1;
                                });
                            }
                            else if (EditorHelper.ButtonColor("x", Color.red, 24))
                                mRemovingIndex = i;
                            else if (EditorHelper.ButtonColor("copy", Color.yellow, 50))
                            {
                                var copiedProfile = new BuildProfile(profile);
                                mBuildProfilesCollection.profiles.Add(copiedProfile);
                            }
                            else if (EditorHelper.ButtonColor("Hide", Color.white, 50))
                                mToggleOptions = -1;
                        }
                        else if (EditorHelper.ButtonColor("Show", Color.cyan, 50))
                            mToggleOptions = i;
                    });
                    if (show)
                    {
                        EditorHelper.BoxVertical(() =>
                        {
                            EditorHelper.BoxHorizontal(() =>
                            {
                                profile.outputFolder = EditorHelper.TextField(profile.outputFolder, "Output Folder", 120, 230);
                                if (EditorHelper.Button("..."))
                                {
                                    string path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, Application.productName);
                                    if (!string.IsNullOrEmpty(path))
                                        profile.outputFolder = path;
                                }
                                if (EditorHelper.Button("Open Explorer"))
                                    System.Diagnostics.Process.Start(profile.outputFolder);
                            });
                            if (!profile.autoNameBuild)
                                profile.buildName = EditorHelper.TextField(profile.buildName, "Build Name", 120, 230);
                            profile.suffix = EditorHelper.TextField(profile.suffix, "Suffix", 120, 230);
                            profile.note = EditorHelper.TextField(profile.note, "Note", 120, 230);
                            profile.autoNameBuild = EditorHelper.Toggle(profile.autoNameBuild, "Auto Name Build", 120, 20);
                            EditorHelper.TextField(profile.GetBuildName(), "Preview Name", 120, 350);
                            EditorHelper.Seperator();
                            profile.customBuildName = EditorHelper.Toggle(profile.customBuildName, "Custom Build Name", 120, 20);
                            if (!profile.customBuildName)
                            {
                                EditorHelper.TextField(PlayerSettings.companyName, "Company Name", 120, 230, true);
                                EditorHelper.TextField(PlayerSettings.productName, "Product Name", 120, 230, true);
                            }
                            else
                            {
                                profile.companyName = EditorHelper.TextField(profile.companyName, "Company Name", 120, 230);
                                profile.productName = EditorHelper.TextField(profile.productName, "Product Name", 120, 230);
                            }
                            EditorHelper.Seperator();
                            profile.customPackage = EditorHelper.Toggle(profile.customPackage, "Custom Package", 120, 20);
                            if (!profile.customPackage)
                            {
                                EditorHelper.TextField(Application.identifier, "Package Name", 120, 230, true);
                                EditorHelper.TextField(PlayerSettings.bundleVersion, "Version", 120, 230, true);
#if UNITY_ANDROID
                                EditorHelper.IntField(PlayerSettings.Android.bundleVersionCode, "Bundle Code", 120, 230, true);
#elif UNITY_IOS
                                EditorHelper.TextField(PlayerSettings.iOS.buildNumber, "Build Number", 120, 230, true);
#endif
                            }
                            else
                            {
                                profile.bundleIdentifier = EditorHelper.TextField(profile.bundleIdentifier, "Package Name", 120, 230);
                                profile.bundleVersion = EditorHelper.TextField(profile.bundleVersion, "Version", 120, 230);
                                profile.bundleVersionCode = EditorHelper.IntField(profile.bundleVersionCode, "Bundle Code", 120, 230);
                            }
                            EditorHelper.Seperator();
                            profile.scriptBackend = EditorHelper.DropdownListEnum(profile.scriptBackend, "Script Backend", 120, 230);
                            if (profile.scriptBackend == ScriptingImplementation.IL2CPP)
                                profile.arm64 = EditorHelper.Toggle(profile.arm64, "ARM64", 120, 230);
                            profile.buildAppBundle = EditorHelper.Toggle(profile.buildAppBundle, "Android App Bundle", 120, 230);
                            if (profile.developmentBuild)
                            {
                                EditorHelper.BoxHorizontal(() =>
                                {
                                    profile.developmentBuild = EditorHelper.Toggle(profile.developmentBuild, "Development Build", 120, 230);
                                    profile.autoConnectProfiler = EditorHelper.Toggle(profile.autoConnectProfiler, "Auto Connect Profiler", 120, 230);
                                    profile.allowDebugging = EditorHelper.Toggle(profile.allowDebugging, "Allow Debugging", 120, 230);
                                });
                            }
                            else
                                profile.developmentBuild = EditorHelper.Toggle(profile.developmentBuild, "Development Build", 120, 230);
                            EditorHelper.Seperator();
                            //--- Draw Scene Section
                            var btnAddNewScene = new EditorButton();
                            btnAddNewScene.color = Color.green;
                            btnAddNewScene.label = "+ New";
                            btnAddNewScene.onPressed = () =>
                            {
                                profile.buildScenes.Add(new SceneAssetReference(null, false));
                            };
                            btnAddNewScene.width = 60;
                            var btnAddCurrentScenes = new EditorButton();
                            btnAddCurrentScenes.color = Color.green;
                            btnAddCurrentScenes.label = "+ Current Build Settings";
                            btnAddCurrentScenes.width = 150;
                            btnAddCurrentScenes.onPressed = () =>
                            {
                                var scenes = EditorBuildSettings.scenes;
                                foreach (var s in scenes)
                                {
                                    profile.RemoveScene(s.path);
                                    profile.AddScene(s.path, s.enabled);
                                }
                            };
                            var btnAddActiveScene = new EditorButton();
                            btnAddActiveScene.label = "+ Opening Scene";
                            btnAddActiveScene.onPressed = () =>
                            {
                                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                                profile.AddScene(scene.path, true);
                            };
                            btnAddActiveScene.color = Color.green;
                            btnAddActiveScene.width = 120;
                            if (EditorHelper.HeaderFoldout("Scenes", "Scenes" + i, false, btnAddNewScene, btnAddCurrentScenes, btnAddActiveScene))
                            {
                                GUILayout.BeginVertical("box");
                                EditorHelper.DragDropBox<SceneAsset>("Drag Drop Scene", (objs) =>
                                {
                                    for (int k = 0; k < objs.Length; k++)
                                    {
                                        var path = AssetDatabase.GetAssetPath(objs[k]);
                                        profile.AddScene(path, true);
                                    }
                                });
                                if (profile.reorderBuildScenes == null)
                                {
                                    profile.reorderBuildScenes = new ReorderableList(profile.buildScenes, typeof(SceneAssetReference), true, true, true, true);
                                    profile.reorderBuildScenes.drawElementCallback += (rect, index, isActive, isFocused) =>
                                    {
                                        var scene = profile.buildScenes[index];
                                        scene.active = EditorGUI.Toggle(new Rect(rect.x, rect.y, 20, 20), scene.active);
                                        scene.asset = (SceneAsset)EditorGUI.ObjectField(new Rect(rect.x + 20, rect.y, rect.width - 20, 20), scene.asset, typeof(SceneAsset), true);
                                    };
                                    profile.reorderBuildScenes.onAddCallback = (list) =>
                                    {
                                        profile.buildScenes.Add(new SceneAssetReference(null, false));
                                    };
                                }
                                profile.reorderBuildScenes.DoLayoutList();
                                GUILayout.EndVertical();
                            }
                            EditorHelper.Seperator();
                            //--- Draw Build Target Section
                            var btnBuilTarget = new EditorButton();
                            btnBuilTarget.color = Color.green;
                            btnBuilTarget.label = "+ New";
                            btnBuilTarget.width = 60;
                            btnBuilTarget.onPressed = () =>
                            {
                                profile.targets.Add(CustomBuldTarget.NoTarget);
                            };
                            if (EditorHelper.HeaderFoldout("Build Targets", "Build Targets" + i, false, btnBuilTarget))
                            {
                                GUILayout.BeginVertical("box");
                                for (int j = 0; j < profile.targets.Count; j++)
                                {
                                    EditorHelper.BoxHorizontal(() =>
                                    {
                                        if (profile.targets[j] != CustomBuldTarget.NoTarget)
                                        {
                                            var icon = BuilderUtil.FindIcon(BuilderUtil.GroupForTarget((BuildTarget)profile.targets[j]));
                                            EditorHelper.DrawTextureIcon(icon, new Vector2(20, 20));
                                        }
                                        if (EditorHelper.ButtonColor("▲", Color.white, 24))
                                        {
                                            if (j > 0)
                                            {
                                                var temp = profile.targets[j];
                                                profile.targets[j] = profile.targets[j - 1];
                                                profile.targets[j - 1] = temp;
                                            }
                                        }
                                        if (EditorHelper.ButtonColor("▼", Color.white, 24))
                                        {
                                            if (j < profile.targets.Count - 1)
                                            {
                                                var temp = profile.targets[j];
                                                profile.targets[j] = profile.targets[j + 1];
                                                profile.targets[j + 1] = temp;
                                            }
                                        }
                                        profile.targets[j] = EditorHelper.DropdownListEnum(profile.targets[j], "");
                                        if (EditorHelper.ButtonColor("X", Color.red, 24))
                                            profile.targets.RemoveAt(j);
                                    });
                                }
                                GUILayout.EndVertical();
                            }
                            EditorHelper.Seperator();
                            //--- Draw Directives Section
                            profile.customDirectives = EditorHelper.Toggle(profile.customDirectives, "Use Custom Directives", 150);
                            if (profile.customDirectives)
                            {
                                var btn1 = new EditorButton()
                                {
                                    label = "+ Current Build Settings",
                                    width = 150,
                                    onPressed = () =>
                                    {
                                        profile.AddDirectives(EditorHelper.GetDirectives());
                                    },
                                    color = Color.green,
                                };
                                var btn2 = new EditorButton()
                                {
                                    label = "Apply",
                                    width = 80,
                                    onPressed = () =>
                                    {
                                        string directivesStr = profile.GetDirectivesString();
                                        var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                                        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, directivesStr);
                                    },
                                    color = Color.yellow
                                };
                                if (EditorHelper.HeaderFoldout("Directives", "Directives" + i, false, btn1, btn2))
                                {
                                    GUILayout.BeginVertical("box");
                                    if (profile.reorderDirectives == null)
                                    {
                                        profile.reorderDirectives = new ReorderableList(profile.directives, typeof(String), true, true, true, true);
                                        profile.reorderDirectives.drawElementCallback += (rect, index, isActive, isFocused) =>
                                        {
                                            var directive = profile.directives[index];
                                            directive.enable = EditorGUI.Toggle(new Rect(rect.x, rect.y, 20, 20), directive.enable);
                                            directive.directive = EditorGUI.TextField(new Rect(rect.x + 20, rect.y, rect.width - 20, 20), directive.directive);
                                        };
                                    }
                                    profile.reorderDirectives.DoLayoutList();
                                    GUILayout.EndVertical();
                                }
                            }
                            EditorHelper.Seperator();
                            EditorHelper.BoxHorizontal(() =>
                            {
                                if (EditorHelper.ButtonColor("Overwrite Unity Build Settings", Color.yellow))
                                    BuilderUtil.OverwritePlayerBuildSettings(profile);
                                if (EditorHelper.ButtonColor("Build Profile", Color.cyan))
                                    EditorApplication.delayCall += () => { Build(profile); };
                                if (EditorHelper.ButtonColor("CLI", Color.cyan, 50))
                                    Debug.Log(mCommandLine.Replace("[index]", i.ToString()).Replace("[path]", profile.outputFolder));
                            });
                        }, Color.gray, true);
                    }
                }
                EditorHelper.BoxHorizontal(() =>
                {
                    if (EditorHelper.ButtonColor("Create New Profile", Color.green, 120))
                    {
                        var s = new BuildProfile();
                        s.Reset();
                        mBuildProfilesCollection.profiles.Add(s);
                    }
                    if (EditorHelper.ButtonColor("Save", Color.green))
                        AssetDatabase.SaveAssets();
                    if (mSelectedCount > 0)
                    {
                        if (EditorHelper.ButtonColor("Build Selected Profiles", Color.cyan))
                            EditorApplication.delayCall += BuildSelectedProfiles;
                    }
                    else if (EditorHelper.ButtonColor("Build Selected Profiles", Color.grey)) { }
                    if (EditorHelper.ButtonColor("Player Settings", Color.white))
                        SettingsService.OpenProjectSettings("Project/Player");
                }, Color.grey, true);
                EditorGUILayout.LabelField("Build By Command Line", EditorStyles.boldLabel);
                EditorGUILayout.TextField(mCommandLine);
            });

            GUILayout.EndScrollView();

            if (GUI.changed)
                EditorUtility.SetDirty(mBuildProfilesCollection);
        }

        private void Init()
        {
            if (mBuildProfilesCollection == null)
                mBuildProfilesCollection = BuildSettingsCollection.LoadOrCreateSettings();

            string projectPath = Application.dataPath.Replace("/Assets", "");
            string[] splits = projectPath.Split('/');
            for (int i = 0; i < splits.Length; i++)
                if (splits[i].Contains(" "))
                    splits[i] = $"\"{splits[i]}\"";
            projectPath = string.Join("\\", splits);

            string unityExePath = EditorApplication.applicationPath;
            splits = unityExePath.Split('/');
            for (int i = 0; i < splits.Length; i++)
                if (splits[i].Contains(" "))
                    splits[i] = $"\"{splits[i]}\"";
            unityExePath = string.Join("\\", splits);

            mCommandLine = $"{unityExePath} -quit -batchmode -projectPath {projectPath} " +
                $"-executeMethod Utilities.Editor.BuilderUtil.BuildByCommandLine -profileIndex [index] -outputFolder [path]";
        }

        private void BuildSelectedProfiles()
        {
            for (int i = 0; i < mBuildProfilesCollection.profiles.Count; i++)
                if (mBuildProfilesCollection.profiles[i].selected)
                    Build(mBuildProfilesCollection.profiles[i]);
        }

        private void Build(BuildProfile pProfile)
        {
            BuilderUtil.OverwritePlayerBuildSettings(pProfile);

            var savedTarget = EditorUserBuildSettings.activeBuildTarget;
            var targetGroup = BuilderUtil.GroupForTarget(savedTarget);
            bool ok = true;
            try
            {
                ok = BuilderUtil.Build(pProfile, (opts, progress, done) =>
                {
                    string message = done ?
                        string.Format("Building {0} Done", opts.target.ToString()) :
                        string.Format("Building {0}...", opts.target.ToString());

                    if (EditorUtility.DisplayCancelableProgressBar("Building project...", message, progress))
                        return false;

                    return true;
                });
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Build error", e.Message, "Close");
                ok = false;
            }

            EditorUtility.ClearProgressBar();
            if (!ok)
                EditorUtility.DisplayDialog("Cancelled", "Build cancelled before finishing.", "Close");
            else
                EditorApplication.delayCall += () =>
                {
                    System.Diagnostics.Process.Start(pProfile.outputFolder);
                };

            // Building can change the active target, can cause warnings or odd behaviour
            // Put it back to how it was
            if (EditorUserBuildSettings.activeBuildTarget != savedTarget)
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(targetGroup, savedTarget);
        }
    }
}