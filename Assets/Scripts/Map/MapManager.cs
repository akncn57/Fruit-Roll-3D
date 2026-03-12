using TMPro;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] private string _mapNameToLoad = "Level1"; 
        [SerializeField] private float _stepDistanceZ = -4.986f; // The Z distance between each tile

        [Header("Prefabs")]
        [SerializeField] private GameObject _tile1Prefab; // Reward Tile
        [SerializeField] private GameObject _tile2Prefab; // Empty Tree Tile
        
        [Header("Container")]
        [SerializeField] private Transform _mapContainer; // Parent transform to keep hierarchy clean

        private void Start()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            MapData mapData = MapService.LoadMap(_mapNameToLoad);

            if (mapData == null)
            {
                Debug.LogError($"Failed to generate map. Map data for '{_mapNameToLoad}' could not be loaded.");
                return;
            }

            if (_mapContainer == null)
            {
                _mapContainer = new GameObject("Map Container").transform;
            }

            int tileCount = 0;
            
            // Loop through all steps in our data
            foreach (var stepData in mapData.Steps)
            {
                // 1. Instantiate Tile 1 (The Reward Tile)
                Vector3 tile1Position = new Vector3(0, 0, tileCount * _stepDistanceZ);
                GameObject tile1Instance = Instantiate(_tile1Prefab, tile1Position, Quaternion.identity, _mapContainer);
                tile1Instance.name = $"Tile1_Step_{stepData.StepIndex}";
                
                tileCount++;

                // 2. Instantiate Tile 2 (The Empty Tree Tile) immediately after
                Vector3 tile2Position = new Vector3(0, 0, tileCount * _stepDistanceZ);
                GameObject tile2Instance = Instantiate(_tile2Prefab, tile2Position, Quaternion.identity, _mapContainer);
                tile2Instance.name = $"Tile2_Empty_{stepData.StepIndex}";
                tileCount++;
            }
            
            Debug.Log($"Map '{mapData.MapName}' generated successfully with {mapData.TotalSteps} reward steps ({tileCount} total tiles).");
        }
    }
}
