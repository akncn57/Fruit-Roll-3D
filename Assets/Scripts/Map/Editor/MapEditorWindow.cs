using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Items;

namespace Map.Editor
{
    public class MapEditorWindow : EditorWindow
    {
        private MapData _currentMapData;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Map Editor")]
        public static void ShowWindow()
        {
            GetWindow<MapEditorWindow>("Map Editor");
        }

        private void OnEnable()
        {
            if (_currentMapData == null)
            {
                _currentMapData = new MapData();
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Map Settings", EditorStyles.boldLabel);

            _currentMapData.MapName = EditorGUILayout.TextField("Map Name", _currentMapData.MapName);
            
            EditorGUI.BeginChangeCheck();
            int newTotalSteps = EditorGUILayout.IntField("Total Steps", _currentMapData.TotalSteps);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateStepsList(newTotalSteps);
            }

            GUILayout.Space(10);
            GUILayout.Label("Steps Configuration", EditorStyles.boldLabel);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            if (_currentMapData.Steps != null)
            {
                for (int i = 0; i < _currentMapData.Steps.Count; i++)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label($"Step {i + 1}", EditorStyles.boldLabel);
                    
                    _currentMapData.Steps[i].StepIndex = i;

                    if (_currentMapData.Steps[i].Reward == null)
                        _currentMapData.Steps[i].Reward = new ItemData();

                    _currentMapData.Steps[i].Reward.Type = (ItemType)EditorGUILayout.EnumPopup("Reward Type", _currentMapData.Steps[i].Reward.Type);
                    _currentMapData.Steps[i].Reward.Amount = EditorGUILayout.IntField("Reward Amount", _currentMapData.Steps[i].Reward.Amount);
                    
                    GUILayout.EndVertical();
                    GUILayout.Space(5);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);
            
            if (GUILayout.Button("Save Map as JSON", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(_currentMapData.MapName))
                {
                    EditorUtility.DisplayDialog("Error", "Please enter a valid Map Name before saving.", "OK");
                    return;
                }
                
                MapService.SaveMap(_currentMapData);
                EditorUtility.DisplayDialog("Success", $"Map '{_currentMapData.MapName}' saved successfully to Resources/Maps!", "OK");
            }
            
            GUILayout.Space(5);

            if (GUILayout.Button("Load Map from JSON (by Name)", GUILayout.Height(30)))
            {
                if (string.IsNullOrEmpty(_currentMapData.MapName))
                {
                    EditorUtility.DisplayDialog("Error", "Please enter a Map Name to load.", "OK");
                    return;
                }
                
                MapData loadedMap = MapService.LoadMap(_currentMapData.MapName);
                if (loadedMap != null)
                {
                    _currentMapData = loadedMap;
                    EditorUtility.DisplayDialog("Success", $"Map '{_currentMapData.MapName}' loaded successfully!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", $"Could not find map named: '{_currentMapData.MapName}'", "OK");
                }
            }
        }

        private void UpdateStepsList(int newCount)
        {
            if (newCount < 0) newCount = 0;
            
            if (_currentMapData.Steps == null)
            {
                _currentMapData.Steps = new List<MapStepData>();
            }

            _currentMapData.TotalSteps = newCount;

            if (newCount > _currentMapData.Steps.Count)
            {
                int itemsToAdd = newCount - _currentMapData.Steps.Count;
                for (int i = 0; i < itemsToAdd; i++)
                {
                    _currentMapData.Steps.Add(new MapStepData() { Reward = new ItemData { Type = ItemType.None, Amount = 1 } });
                }
            }
            else if (newCount < _currentMapData.Steps.Count)
            {
                int itemsToRemove = _currentMapData.Steps.Count - newCount;
                for (int i = 0; i < itemsToRemove; i++)
                {
                    _currentMapData.Steps.RemoveAt(_currentMapData.Steps.Count - 1);
                }
            }
        }
    }
}
