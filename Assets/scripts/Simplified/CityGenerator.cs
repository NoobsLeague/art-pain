using UnityEngine;
using System.Collections.Generic;

namespace Demo {
    public class CityGenerator : MonoBehaviour {
        [Header("City Layout")]
        [SerializeField] public int cityRows = 5;
        [SerializeField] public int cityColumns = 5;
        [SerializeField] public float buildingSpacing = 15f;
        [SerializeField] public float streetWidth = 5f;
        
        [Header("Building Variety")]
        [SerializeField] public bool randomizeBuildingTypes = true;
        [SerializeField] public bool randomizeBuildingDimensions = true;
        [SerializeField] public bool randomizeBuildingHeights = true;
        
        [Header("Building Dimensions (if not randomized)")]
        [SerializeField] public BuildingGenerationType defaultBuildingType = BuildingGenerationType.Type1_CornerWallsOnly;
        [SerializeField] public int defaultWidth = 10;
        [SerializeField] public int defaultDepth = 10;
        [SerializeField] public int defaultHeight = 5;
        
        [Header("Random Ranges (if randomized)")]
        [SerializeField] public Vector2Int widthRange = new Vector2Int(8, 12);
        [SerializeField] public Vector2Int depthRange = new Vector2Int(8, 12);
        [SerializeField] public Vector2Int heightRange = new Vector2Int(3, 8);
        
        [Header("Building Features")]
        [SerializeField] public bool enableRandomRoofTypes = true;
        [SerializeField] [Range(0f, 1f)] public float flatRoofProbability = 0.3f;
        [SerializeField] public bool enableRandomExpansions = true;
        [SerializeField] [Range(0f, 1f)] public float expansionProbability = 0.2f;
        [SerializeField] public Vector2Int expansionHeightRange = new Vector2Int(2, 4);
        [SerializeField] public Vector2Int expansionLayersRange = new Vector2Int(1, 2);
        
        [Header("Signs")]
        [SerializeField] public bool enableRandomSigns = true;
        [SerializeField] [Range(0f, 1f)] public float signProbability = 0.5f;
        
        [Header("Prefab Collections (Shared by all buildings)")]
        [SerializeField] public GameObject[] wallPrefabs;
        [SerializeField] public GameObject[] windowPrefabs;
        [SerializeField] public GameObject[] roofPrefabs;
        [SerializeField] public GameObject cornerPrefab;
        [SerializeField] public GameObject[] neonSignPrefabs;
        
        [Header("Type-Specific Settings")]
        [SerializeField] public int type3WindowInterval = 5;
        
        [Header("Generation Settings")]
        [SerializeField] public float buildDelayPerBuilding = 0.1f;
        [SerializeField] public bool generateOnStart = false;
        
        private List<CentralBuildingGenerator> generatedBuildings = new List<CentralBuildingGenerator>();
        
        void Start() {
            if (generateOnStart) {
                GenerateCity();
            }
        }
        
        void Update() {
            if (Input.GetKeyDown(KeyCode.G)) {
                ClearCity();
                GenerateCity();
            }
            
            if (Input.GetKeyDown(KeyCode.C)) {
                ClearCity();
            }
        }
        
        public void GenerateCity() {
            ClearCity();
            
            float totalSpacing = buildingSpacing + streetWidth;
            
            // Calculate city center offset
            float cityWidth = (cityColumns - 1) * totalSpacing;
            float cityDepth = (cityRows - 1) * totalSpacing;
            Vector3 cityOffset = new Vector3(-cityWidth / 2f, 0, -cityDepth / 2f);
            
            for (int row = 0; row < cityRows; row++) {
                for (int col = 0; col < cityColumns; col++) {
                    // Calculate position
                    Vector3 position = new Vector3(col * totalSpacing, 0, row * totalSpacing) + cityOffset;
                    
                    // Create building
                    CreateBuilding(position, row, col);
                }
            }
        }
        
        void CreateBuilding(Vector3 position, int row, int col) {
            // Create a new GameObject for the building
            GameObject buildingObj = new GameObject($"Building_{row}_{col}");
            buildingObj.transform.parent = transform;
            buildingObj.transform.position = position;
            
            // Add the CentralBuildingGenerator component
            CentralBuildingGenerator building = buildingObj.AddComponent<CentralBuildingGenerator>();
            
            // Configure building type
            if (randomizeBuildingTypes) {
                building.buildingType = (BuildingGenerationType)Random.Range(1, 4); // 1, 2, or 3
            } else {
                building.buildingType = defaultBuildingType;
            }
            
            // Configure dimensions
            if (randomizeBuildingDimensions) {
                building.buildingWidth = Random.Range(widthRange.x, widthRange.y + 1);
                building.buildingDepth = Random.Range(depthRange.x, depthRange.y + 1);
            } else {
                building.buildingWidth = defaultWidth;
                building.buildingDepth = defaultDepth;
            }
            
            // Configure height
            if (randomizeBuildingHeights) {
                building.buildingHeight = Random.Range(heightRange.x, heightRange.y + 1);
            } else {
                building.buildingHeight = defaultHeight;
            }
            
            // Configure roof
            if (enableRandomRoofTypes) {
                building.forceFlatRoof = Random.value < flatRoofProbability;
            }
            
            // Configure expansion
            if (enableRandomExpansions && Random.value < expansionProbability) {
                building.enableExpansion = true;
                building.expansionHeight = Random.Range(expansionHeightRange.x, expansionHeightRange.y + 1);
                building.expansionLayers = Random.Range(expansionLayersRange.x, expansionLayersRange.y + 1);
            } else {
                building.enableExpansion = false;
            }
            
            // Configure signs
            if (enableRandomSigns && Random.value < signProbability) {
                building.enableSignGeneration = true;
                building.signWallOffset = Random.Range(0.1f, 0.3f); // Always outside
                building.signHeightThreshold = Random.Range(0.3f, 0.6f);
                building.signSpawnChance = Random.Range(0.5f, 0.9f);
            } else {
                building.enableSignGeneration = false;
            }
            
            // Assign prefabs
            building.wallPrefabs = wallPrefabs;
            building.windowPrefabs = windowPrefabs;
            building.roofPrefabs = roofPrefabs;
            building.cornerPrefab = cornerPrefab;
            building.neonSignPrefabs = neonSignPrefabs;
            
            // Type-specific settings
            building.windowInterval = type3WindowInterval;
            
            // Set build delay
            building.buildDelay = buildDelayPerBuilding;
            
            // Generate the building
            building.Generate();
            
            // Add to our list
            generatedBuildings.Add(building);
        }
        
        public void ClearCity() {
            // Clear all generated buildings
            foreach (var building in generatedBuildings) {
                if (building != null) {
                    building.DeleteGenerated();
                    DestroyImmediate(building.gameObject);
                }
            }
            
            generatedBuildings.Clear();
            
            // Also clear any orphaned buildings
            for (int i = transform.childCount - 1; i >= 0; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        
        public void RegenerateBuilding(int index) {
            if (index >= 0 && index < generatedBuildings.Count) {
                var building = generatedBuildings[index];
                if (building != null) {
                    building.DeleteGenerated();
                    building.Generate();
                }
            }
        }
        
        // Utility method to get stats about the generated city
        public string GetCityStats() {
            int type1Count = 0, type2Count = 0, type3Count = 0;
            int expansionCount = 0, signCount = 0;
            
            foreach (var building in generatedBuildings) {
                if (building == null) continue;
                
                switch (building.buildingType) {
                    case BuildingGenerationType.Type1_CornerWallsOnly:
                        type1Count++;
                        break;
                    case BuildingGenerationType.Type2_HeightBasedPrefabs:
                        type2Count++;
                        break;
                    case BuildingGenerationType.Type3_CornerWallsWithWindows:
                        type3Count++;
                        break;
                }
                
                if (building.enableExpansion) expansionCount++;
                if (building.enableSignGeneration) signCount++;
            }
            
            return $"City Stats:\n" +
                   $"Total Buildings: {generatedBuildings.Count}\n" +
                   $"Type 1: {type1Count}, Type 2: {type2Count}, Type 3: {type3Count}\n" +
                   $"With Expansion: {expansionCount}\n" +
                   $"With Signs: {signCount}";
        }
    }
}