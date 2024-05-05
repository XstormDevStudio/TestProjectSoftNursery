using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Exoa.Designer.Utils
{
    public class ThumbnailGeneratorUtils
    {
        public static void Exists(string fileName, Action<bool> callback)
        {
            HDLogger.Log("Thumb Exists:" + fileName, HDLogger.LogCategory.Screenshot);
            SaveSystem.Create(SaveSystem.Mode.FILE_SYSTEM)
                .Exists(fileName, HDSettings.EXT_THUMBNAIL_FOLDER, ".png", (bool exists) =>
            {
                if (!exists)
                {
                    SaveSystem.Create(SaveSystem.Mode.RESOURCES).Exists(fileName, HDSettings.EMBEDDED_THUMBNAIL_FOLDER, ".png", (bool exists2) =>
                    {
                        callback?.Invoke(exists2);
                    });
                }
                else
                {
                    callback?.Invoke(exists);
                }
            });
        }

        public static void Load(string fileName, Action<Texture2D> callback, Action<UnityWebRequest, string> errorCallback)
        {
            HDLogger.Log("Thumb Load:" + fileName, HDLogger.LogCategory.Screenshot);
            SaveSystem.Create(SaveSystem.Mode.FILE_SYSTEM)
                .LoadTextureItem(fileName, HDSettings.EXT_THUMBNAIL_FOLDER, (Texture2D t) =>
                {
                    if (t == null)
                    {
                        LoadBackupTexture(fileName, callback, errorCallback);
                    }
                    else
                    {
                        callback?.Invoke(t);
                    }
                }, (UnityWebRequest req, string res) =>
                {
                    LoadBackupTexture(fileName, callback, errorCallback);
                }, ".png");

        }

        private static void LoadBackupTexture(string fileName, Action<Texture2D> callback, Action<UnityWebRequest, string> errorCallback)
        {
            HDLogger.Log("LoadBackupTexture in Resources:" + fileName, HDLogger.LogCategory.FileSystem);

            SaveSystem.Create(SaveSystem.Mode.RESOURCES)
                .LoadTextureItem(fileName, HDSettings.EMBEDDED_THUMBNAIL_FOLDER, (Texture2D t2) =>
                {
                    callback?.Invoke(t2);
                }, (UnityWebRequest req, string res) =>
                {
                    if (fileName != "EmptyThumb")
                        LoadBackupTexture("EmptyThumb", callback, errorCallback);
                },
                ".png");
        }



        public static void TakeAndSaveScreenshot(Transform target, string fileName, bool orthographic, Vector3 direction)
        {
            RuntimePreviewGenerator.BackgroundColor = HDSettings.THUMBNAIL_BACKGROUND;
            RuntimePreviewGenerator.MarkTextureNonReadable = false;
            RuntimePreviewGenerator.OrthographicMode = orthographic;
            RuntimePreviewGenerator.PreviewDirection = direction;

            Texture2D tex = RuntimePreviewGenerator.GenerateModelPreview(target, 256, 256);

            try
            {
                byte[] _bytes = tex.EncodeToPNG();

                //Debug.Log("Saving Thumbnail path:" + filaName);

                SaveSystem.Create(SaveSystem.Mode.FILE_SYSTEM)
                    .SaveFileItem(fileName.Replace(".png", ""),
                    HDSettings.EXT_THUMBNAIL_FOLDER, ".png", _bytes);
            }
            catch (Exception e) { Debug.LogError(e.Message); }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

        }

        public static void Duplicate(string v1, string v2)
        {
            try
            {
                SaveSystem.Create(SaveSystem.Mode.FILE_SYSTEM).CopyFileItem(v1, v2, HDSettings.EXT_THUMBNAIL_FOLDER, null, ".png");
            }
            catch (Exception e) { Debug.LogError(e.Message); }

        }
    }
}
