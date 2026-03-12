using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] private string mapNameToLoad = "Level1"; 
        [SerializeField] private float stepDistanceZ = -4.986f;

        [Header("Prefabs")]
        [SerializeField] private GameObject normalTilePrefab;
        [SerializeField] private GameObject emptyTilePrefab;
        
        [Header("Container")]
        [SerializeField] private Transform mapContainer;

        private void Start()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            var mapData = MapService.LoadMap(mapNameToLoad);

            if (mapData == null)
            {
                Debug.LogError($"Failed to generate map. Map data for '{mapNameToLoad}' could not be loaded.");
                return;
            }

            if (mapContainer == null)
            {
                mapContainer = new GameObject("Map Container").transform;
            }

            var tileCount = 0;
            
            foreach (var stepData in mapData.Steps)
            {
                var tile1Position = new Vector3(0, 0, tileCount * stepDistanceZ);
                var tile1Instance = Instantiate(normalTilePrefab, tile1Position, Quaternion.identity, mapContainer);
                tile1Instance.name = $"Tile1_Step_{stepData.StepIndex}";
                
                tileCount++;

                var tile2Position = new Vector3(0, 0, tileCount * stepDistanceZ);
                var tile2Instance = Instantiate(emptyTilePrefab, tile2Position, Quaternion.identity, mapContainer);
                tile2Instance.name = $"Tile2_Empty_{stepData.StepIndex}";
                tileCount++;
            }
            
            Debug.Log($"Map '{mapData.MapName}' generated successfully with {mapData.TotalSteps} reward steps ({tileCount} total tiles).");
        }
    }
}
