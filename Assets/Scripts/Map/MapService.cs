using UnityEngine;
using System.IO;
using Utils;

namespace Map
{
    public static class MapService
    {
        private static string GetDirectoryPath()
        {
            return Path.Combine(Application.dataPath, "Resources", "Maps");
        }

        public static void SaveMap(MapData mapData)
        {
            var dir = GetDirectoryPath();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var filePath = Path.Combine(dir, $"{mapData.MapName}.json");
            var json = JsonUtility.ToJson(mapData, true);
            File.WriteAllText(filePath, json);
            EditorLogger.Log(nameof(MapService), $"Map '{mapData.MapName}' saved to: {filePath}");
            
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public static MapData LoadMap(string mapName)
        {
            // At runtime, we load from Resources folder. Unity expects paths relative to Resources and without extensions.
            var jsonAsset = Resources.Load<TextAsset>($"Maps/{mapName}");
            if (jsonAsset != null)
            {
                return JsonUtility.FromJson<MapData>(jsonAsset.text);
            }
            else
            {
                EditorLogger.Error(nameof(MapService), $"Map '{mapName}' not found in Resources/Maps.");
                return null;
            }
        }
    }
}
