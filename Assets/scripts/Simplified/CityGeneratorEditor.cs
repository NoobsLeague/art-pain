#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Demo {
    [CustomEditor(typeof(CityGenerator))]
    public class CityGeneratorEditor : Editor {
        private CityGenerator generator;
        
        void OnEnable() {
            generator = (CityGenerator)target;
        }
        
        public override void OnInspectorGUI() {
            // Header
            GUILayout.Space(10);
            EditorGUILayout.LabelField("City Generator", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Generate multiple buildings in a grid layout", EditorStyles.miniLabel);
            
            GUILayout.Space(10);
            
            // Main action buttons
            EditorGUILayout.BeginHorizontal();
            
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("Generate City", GUILayout.Height(40))) {
                generator.GenerateCity();
            }
            
            GUI.backgroundColor = new Color(0.8f, 0.4f, 0.4f);
            if (GUILayout.Button("Clear City", GUILayout.Height(40))) {
                generator.ClearCity();
            }
            
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Quick presets
            EditorGUILayout.LabelField("Quick Presets:", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Small Town\n(3x3)", GUILayout.Height(35))) {
                generator.cityRows = 3;
                generator.cityColumns = 3;
                generator.heightRange = new Vector2Int(2, 4);
                generator.expansionProbability = 0.1f;
                generator.signProbability = 0.3f;
                generator.GenerateCity();
            }
            
            if (GUILayout.Button("Medium City\n(5x5)", GUILayout.Height(35))) {
                generator.cityRows = 5;
                generator.cityColumns = 5;
                generator.heightRange = new Vector2Int(3, 8);
                generator.expansionProbability = 0.2f;
                generator.signProbability = 0.5f;
                generator.GenerateCity();
            }
            
            if (GUILayout.Button("Large Metropolis\n(8x8)", GUILayout.Height(35))) {
                generator.cityRows = 8;
                generator.cityColumns = 8;
                generator.heightRange = new Vector2Int(5, 12);
                generator.expansionProbability = 0.3f;
                generator.signProbability = 0.7f;
                generator.GenerateCity();
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            // Show city stats if buildings exist
            if (Application.isPlaying && generator.transform.childCount > 0) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(generator.GetCityStats(), EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
            
            // Preview information
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("City Preview:", EditorStyles.boldLabel);
            
            int totalBuildings = generator.cityRows * generator.cityColumns;
            float cityWidth = (generator.cityColumns - 1) * (generator.buildingSpacing + generator.streetWidth);
            float cityDepth = (generator.cityRows - 1) * (generator.buildingSpacing + generator.streetWidth);
            
            EditorGUILayout.LabelField($"Total Buildings: {totalBuildings}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"City Dimensions: {cityWidth:F1} x {cityDepth:F1} units", EditorStyles.miniLabel);
            
            if (generator.randomizeBuildingTypes) {
                EditorGUILayout.LabelField("Building Types: Random mix of all types", EditorStyles.miniLabel);
            } else {
                EditorGUILayout.LabelField($"Building Types: All {generator.defaultBuildingType}", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(10);
            
            // Controls hint
            if (Application.isPlaying) {
                EditorGUILayout.HelpBox("Press 'G' in play mode to regenerate city\nPress 'C' to clear city", MessageType.Info);
            }
            
            GUILayout.Space(10);
            
            // Default inspector
            DrawDefaultInspector();
            
            // Validation
            GUILayout.Space(10);
            ValidateSettings();
        }
        
        void ValidateSettings() {
            bool hasWarnings = false;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Validation:", EditorStyles.boldLabel);
            
            if (generator.wallPrefabs == null || generator.wallPrefabs.Length == 0) {
                EditorGUILayout.HelpBox("Wall Prefabs are required!", MessageType.Error);
                hasWarnings = true;
            }
            
            if (generator.windowPrefabs == null || generator.windowPrefabs.Length == 0) {
                EditorGUILayout.HelpBox("Window Prefabs recommended for variety", MessageType.Warning);
                hasWarnings = true;
            }
            
            if (generator.roofPrefabs == null || generator.roofPrefabs.Length == 0) {
                EditorGUILayout.HelpBox("Roof Prefabs recommended", MessageType.Warning);
                hasWarnings = true;
            }
            
            if (generator.enableRandomSigns && (generator.neonSignPrefabs == null || generator.neonSignPrefabs.Length == 0)) {
                EditorGUILayout.HelpBox("Neon Sign Prefabs needed for sign generation", MessageType.Warning);
                hasWarnings = true;
            }
            
            if (generator.cityRows * generator.cityColumns > 100) {
                EditorGUILayout.HelpBox("Large city! Generation may take time.", MessageType.Info);
                hasWarnings = true;
            }
            
            if (!hasWarnings) {
                EditorGUILayout.LabelField("✓ All settings look good!", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}
#endif