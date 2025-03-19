using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Common;
using Debug = UnityEngine.Debug;

namespace FoodZombie
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : Editor
    {
        private SoundManager mScript;

        private void OnEnable()
        {
            mScript = (SoundManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorHelper.Button("BUILD", () =>
            {
                string musicPath = @"Assets\Core\Sounds\Musics";
                string sfxPath = @"Assets\Core\Sounds\Sfxs";
                var musicFiles = EditorHelper.GetObjects<AudioClip>(musicPath, "t:AudioClip");
                var sfxFiles = EditorHelper.GetObjects<AudioClip>(sfxPath, "t:AudioClip");
                //We dont encrypt music list because it not export with other excels data, we export it separately for only purpose importing to editor
                //string musicsData = GameData.GetTextContent("Data/MusicList", false);
                //string sfxsData = GameData.GetTextContent("Data/SFXList", false);
                string musicsData = GameData.GetTextContent("Data/MusicList");
                string sfxsData = GameData.GetTextContent("Data/SFXList");
                //Build Music List following Id
                var musics = JsonHelper.GetJsonList<SoundManager.Sound>(musicsData);
                musics.Sort();
                mScript.musicClips = new List<SoundManager.Sound>();
                for (int i = 0; i < musics.Count; i++)
                {
                    AudioClip file = null;
                    foreach (AudioClip f in musicFiles)
                    {
                        if (f.name.ToLower() == musics[i].fileName.ToLower())
                        {
                            file = f;
                            break;
                        }
                    }
                    mScript.musicClips.Add(new SoundManager.Sound()
                    {
                        id = musics[i].id,
                        clip = file,
                        fileName = musics[i].fileName,
                        limitNumber = musics[i].limitNumber
                    });
                }
                //Build Sfx list following id
                var sfxs = JsonHelper.GetJsonList<SoundManager.Sound>(sfxsData);
                sfxs.Sort();
                mScript.SFXClips = new List<SoundManager.Sound>();
                for (int i = 0; i < sfxs.Count; i++)
                {
                    AudioClip file = null;
                    foreach (AudioClip f in sfxFiles)
                    {
                        if (f.name.ToLower() == sfxs[i].fileName.ToLower())
                        {
                            file = f;
                            break;
                        }
                    }
                    if (file == null)
                        Debug.LogWarning("Not found " + sfxs[i].fileName);

                    mScript.SFXClips.Add(new SoundManager.Sound()
                    {
                        id = sfxs[i].id,
                        clip = file,
                        fileName = sfxs[i].fileName,
                        limitNumber = sfxs[i].limitNumber
                    });
                }
            });

            //EditorHelper.DrawSeparator();
            //EditorHelper.ShowList(ref mScript.musicClips, "Musics", false);
            //EditorHelper.ShowList(ref mScript.SFXClips, "SFXs", false);

            //EditorHelper.Button("Add Musics To List", () =>
            //{
            //    string musicPath = @"Assets\Game\Game Resources\Sounds\Musics";
            //    var clips = EditorHelper.GetObjects<AudioClip>(musicPath, "t:AudioClip");
            //    if (clips != null)
            //        mScript.musicClips = clips;
            //});

            //EditorHelper.Button("Add SFX To List", () =>
            //{
            //    string sfxPath = @"Assets\Game\Game Resources\Sounds\Sfxs";
            //    var clips = EditorHelper.GetObjects<AudioClip>(sfxPath, "t:AudioClip");
            //    if (clips != null)
            //        mScript.SFXClips = clips;
            //});

            //EditorHelper.Button("Build Keys", () =>
            //{
            //    var template = AssetDatabase.LoadAssetAtPath<TextAsset>(@"Assets\Game\Scripts\MyKingdom\SoundIdsTemplate.txt");
            //    var text = template.text;

            //    //==================

            //    StringBuilder sbEnum = new StringBuilder();
            //    StringBuilder sbNumber = new StringBuilder();

            //    sbEnum.Append("\tpublic enum SFXKey { ");
            //    for (int i = 0; i < mScript.SFXClips.Count; i++)
            //    {
            //        var clip = mScript.SFXClips[i];
            //        sbEnum.Append(clip.name).Append(" = ").Append(i);
            //        sbNumber.Append("\t\tpublic const int Sfx_").Append(clip.name.Replace(" ", "_")).Append(" = ").Append(i).Append(";");

            //        if (i < mScript.SFXClips.Count - 1)
            //        {
            //            sbEnum.Append(", ");
            //            sbNumber.AppendLine();
            //        }
            //    }
            //    sbEnum.Append(" }");

            //    text = text.Replace("//SFX_ENUM_KEY", sbEnum.ToString());
            //    text = text.Replace("//SFX_INSTANCE_ID", sbNumber.ToString());

            //    //====================

            //    sbEnum = new StringBuilder();
            //    sbNumber = new StringBuilder();
            //    sbEnum.Append("\tpublic enum MusicKey { ");
            //    for (int i = 0; i < mScript.musicClips.Count; i++)
            //    {
            //        var clip = mScript.musicClips[i];
            //        sbEnum.Append(clip.name).Append(" = ").Append(i);
            //        sbNumber.Append("\t\tpublic const int Music_").Append(clip.name.Replace(" ", "_")).Append(" = ").Append(i).Append(";");

            //        if (i < mScript.musicClips.Count - 1)
            //        {
            //            sbEnum.Append(", ");
            //            sbNumber.AppendLine();
            //        }
            //    }
            //    sbEnum.Append(" }");

            //    text = text.Replace("//MUSIC_ENUM_KEY", sbEnum.ToString());
            //    text = text.Replace("//MUSIC_INSTANCE_ID", sbNumber.ToString());

            //    Debug.Log(text);

            //    //====================

            //    string outputPath = @"Assets\Game\Scripts\MyKingdom\SoundManager.cs";
            //    if (File.Exists(outputPath))
            //    {
            //        using (StreamReader sr = new StreamReader(outputPath))
            //        {
            //            string content = sr.ReadToEnd();
            //            var startMark = "#region Sounds Key";
            //            var startMarkLen = startMark.Length;
            //            var startIndex = content.IndexOf(startMark);
            //            var endIndex = content.IndexOf("#endregion Sounds Key");
            //            content = content.Remove(startIndex + startMarkLen, endIndex - startIndex - startMarkLen);
            //            content = content.Insert(startIndex + startMarkLen, "\n\n" + text.ToString() + "\n\n\t");
            //            sr.Close();

            //            using (StreamWriter sw = new StreamWriter(outputPath))
            //            {
            //                sw.WriteLine(content);
            //                sw.Close();
            //            }
            //        }
            //    }

            //    AssetDatabase.Refresh();
            //});
        }
    }
}