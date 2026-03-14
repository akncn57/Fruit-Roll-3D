using UnityEngine;
using System;
using System.Collections;
using Utils;

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
        [SerializeField] private GameObject resetDataTilePrefab;
        [SerializeField] private GameObject returnToStartTilePrefab;
        [SerializeField] private GameObject emptyTilePrefab;
        
        [Header("Container")]
        [SerializeField] private Transform mapContainer;

        private MapData _currentMapData;

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
            _currentMapData = MapService.LoadMap(mapNameToLoad);

            if (_currentMapData == null)
            {
                EditorLogger.Error(nameof(MapManager), $"Failed to generate map. Map data for '{mapNameToLoad}' could not be loaded.");
                return;
            }

            if (mapContainer == null)
            {
                mapContainer = new GameObject("Map Container").transform;
            }

            var tileCount = 0;
            
            foreach (var stepData in _currentMapData.Steps)
            {
                GameObject prefabToUse = normalTilePrefab;

                if (stepData.Type == StepType.ResetData)
                {
                    prefabToUse = resetDataTilePrefab != null ? resetDataTilePrefab : normalTilePrefab;
                }
                else if (stepData.Type == StepType.ReturnToStart)
                {
                    prefabToUse = returnToStartTilePrefab != null ? returnToStartTilePrefab : normalTilePrefab;
                }

                var tile1Position = new Vector3(0, 0, tileCount * stepDistanceZ);
                var tile1Instance = Instantiate(prefabToUse, tile1Position, Quaternion.identity, mapContainer);
                tile1Instance.name = $"Tile1_{stepData.Type}_Step_{stepData.StepIndex}";
                
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
            EditorLogger.Log(nameof(MapManager), $"Map '{_currentMapData.MapName}' generated successfully with {_currentMapData.TotalSteps} reward steps ({tileCount} total tiles).");
        }
        
        // Event triggered when the map finishes moving to the target step
        public event Action OnMapMovementStarted;
        public event Action OnMapMovementCompleted;
        
        public void MoveMapToStep(int targetStepIndex, float unitsPerSecond = 20f)
        {
            var targetZPosition = -(targetStepIndex * 2) * stepDistanceZ;
            var startPosition = mapContainer.position;
            var targetPosition = new Vector3(startPosition.x, startPosition.y, targetZPosition);
            var distance = Vector3.Distance(startPosition, targetPosition);
            var moveDuration = distance / unitsPerSecond;
            OnMapMovementStarted?.Invoke();
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

        public MapStepData GetStepData(int stepIndex)
        {
            if (_currentMapData != null && stepIndex < _currentMapData.Steps.Count)
            {
                return _currentMapData.Steps[stepIndex];
            }
            return null;
        }
    }
}
