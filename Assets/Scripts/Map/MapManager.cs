using UnityEngine;
using System;
using System.Collections;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }
        
        [Header("Map Settings")]
        [SerializeField] private string mapNameToLoad = "Level1"; 
        [SerializeField] private float stepDistanceZ = -4.986f;

        [Header("Prefabs")]
        [SerializeField] private GameObject normalTilePrefab;
        [SerializeField] private GameObject emptyTilePrefab;
        
        [Header("Container")]
        [SerializeField] private Transform mapContainer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
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
                
                // Fetch the MapTile controller and initialize the Visuals (Text, Objects inside box)
                var mapTileScript = tile1Instance.GetComponent<MapTile>();
                if (mapTileScript != null)
                {
                    mapTileScript.Initialize(stepData);
                }
                
                tileCount++;

                var tile2Position = new Vector3(0, 0, tileCount * stepDistanceZ);
                var tile2Instance = Instantiate(emptyTilePrefab, tile2Position, Quaternion.identity, mapContainer);
                tile2Instance.name = $"Tile2_Empty_{stepData.StepIndex}";
                tileCount++;
            }
            
            Debug.Log($"Map '{mapData.MapName}' generated successfully with {mapData.TotalSteps} reward steps ({tileCount} total tiles).");
        }
        // Event triggered when the map finishes moving to the target step
        public event Action OnMapMovementCompleted;
        
        public void MoveMapToStep(int targetStepIndex, float unitsPerSecond = 20f)
        {
            var targetZPosition = -(targetStepIndex * 2) * stepDistanceZ;
            var startPosition = mapContainer.position;
            var targetPosition = new Vector3(startPosition.x, startPosition.y, targetZPosition);
            var distance = Vector3.Distance(startPosition, targetPosition);
            var moveDuration = distance / unitsPerSecond;
            
            StartCoroutine(MoveContainerCoroutine(startPosition, targetPosition, moveDuration));
        }

        private IEnumerator MoveContainerCoroutine(Vector3 start, Vector3 target, float duration)
        {
            var elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                
                var t = Mathf.Clamp01(elapsed / duration);
                
                t = Mathf.SmoothStep(0f, 1f, t);
                mapContainer.position = Vector3.Lerp(start, target, t);
                yield return null;
            }
            
            mapContainer.position = target;
            
            OnMapMovementCompleted?.Invoke();
        }
    }
}
