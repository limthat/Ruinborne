using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Ruinborne.Definitions;
using Ruinborne.Data;

namespace Ruinborne.Editor
{
    public class DataTableImporter : EditorWindow
    {
        [MenuItem("Ruinborne/Data Table Importer")]
        public static void ShowWindow()
        {
            GetWindow<DataTableImporter>("Data Table Importer");
        }

        private string _jsonFolderPath = "Assets/_Game/Data/JSON";
        private Vector2 _scrollPos;

        private void OnGUI()
        {
            GUILayout.Label("Ruinborne Data Table Importer", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _jsonFolderPath = EditorGUILayout.TextField("JSON 폴더 경로", _jsonFolderPath);
            EditorGUILayout.Space();

            if (GUILayout.Button("NeedsDef JSON → ScriptableObject 생성"))
                ImportNeedsDefs();

            if (GUILayout.Button("TerrainDef JSON → ScriptableObject 생성"))
                ImportTerrainDefs();

            if (GUILayout.Button("ResourceDef JSON → ScriptableObject 생성"))
                ImportResourceDefs();

            EditorGUILayout.Space();
            if (GUILayout.Button("전체 임포트"))
            {
                ImportNeedsDefs();
                ImportTerrainDefs();
                ImportResourceDefs();
            }
        }

        private void ImportNeedsDefs()
        {
            string path = Path.Combine(_jsonFolderPath, "NeedsDefs.json");
            if (!File.Exists(path))
            {
                Debug.LogError($"[DataTableImporter] 파일 없음: {path}");
                return;
            }

            string json = File.ReadAllText(path);
            NeedsDefData[] dataArray = JsonHelper.FromJson<NeedsDefData>(json);
            if (dataArray == null) return;

            string outputPath = "Assets/_Game/Data/Instances/Needs";
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

            foreach (var data in dataArray)
            {
                NeedsDef asset = ScriptableObject.CreateInstance<NeedsDef>();
                asset.needType = data.needType;
                asset.needName = data.needName;
                asset.decayRatePerSec = data.decayRatePerSec;
                asset.criticalThreshold = data.criticalThreshold;
                asset.warningThreshold = data.warningThreshold;
                asset.fulfillAmountPerAction = data.fulfillAmountPerAction;
                asset.starvationDamagePerSec = data.starvationDamagePerSec;

                string assetPath = $"{outputPath}/NeedsDef_{data.needType}.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                Debug.Log($"[DataTableImporter] NeedsDef 생성: {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[DataTableImporter] NeedsDefs 임포트 완료");
        }

        private void ImportTerrainDefs()
        {
            string path = Path.Combine(_jsonFolderPath, "TerrainDefs.json");
            if (!File.Exists(path))
            {
                Debug.LogError($"[DataTableImporter] 파일 없음: {path}");
                return;
            }

            string json = File.ReadAllText(path);
            TerrainDefData[] dataArray = JsonHelper.FromJson<TerrainDefData>(json);
            if (dataArray == null) return;

            string outputPath = "Assets/_Game/Data/Instances/Terrain";
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

            foreach (var data in dataArray)
            {
                TerrainDef asset = ScriptableObject.CreateInstance<TerrainDef>();
                asset.terrainName = data.terrainName;
                asset.terrainType = data.terrainType;
                asset.moveSpeedFactor = data.moveSpeedFactor;
                asset.fertility = data.fertility;
                asset.flammability = data.flammability;
                asset.thermalConductivity = data.thermalConductivity;
                asset.isWater = data.isWater;
                asset.canBuildOn = data.canBuildOn;
                asset.beautyOffset = data.beautyOffset;

                string assetPath = $"{outputPath}/TerrainDef_{data.terrainType}.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                Debug.Log($"[DataTableImporter] TerrainDef 생성: {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[DataTableImporter] TerrainDefs 임포트 완료");
        }

        private void ImportResourceDefs()
        {
            string path = Path.Combine(_jsonFolderPath, "ResourceDefs.json");
            if (!File.Exists(path))
            {
                Debug.LogError($"[DataTableImporter] 파일 없음: {path}");
                return;
            }

            string json = File.ReadAllText(path);
            ResourceDefData[] dataArray = JsonHelper.FromJson<ResourceDefData>(json);
            if (dataArray == null) return;

            string outputPath = "Assets/_Game/Data/Instances/Resources";
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

            foreach (var data in dataArray)
            {
                ResourceDef asset = ScriptableObject.CreateInstance<ResourceDef>();
                asset.resourceName = data.resourceName;
                asset.resourceType = data.resourceType;
                asset.resourceTier = data.resourceTier;
                asset.marketValue = data.marketValue;
                asset.stackLimit = data.stackLimit;
                asset.weight = data.weight;
                asset.flammability = data.flammability;
                asset.deteriorationRate = data.deteriorationRate;
                asset.canBeStolen = data.canBeStolen;
                asset.nutrition = data.nutrition;

                string assetPath = $"{outputPath}/ResourceDef_{data.resourceType}.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                Debug.Log($"[DataTableImporter] ResourceDef 생성: {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[DataTableImporter] ResourceDefs 임포트 완료");
        }
    }

    // JSON 배열 파싱 헬퍼
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string wrapped = "{\"items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
            return wrapper?.items;
        }

        [System.Serializable]
        private class Wrapper<T> { public T[] items; }
    }

    // JSON 데이터 구조체
    [System.Serializable]
    public class NeedsDefData
    {
        public NeedType needType;
        public string needName;
        public float decayRatePerSec;
        public float criticalThreshold;
        public float warningThreshold;
        public float fulfillAmountPerAction;
        public float starvationDamagePerSec;
    }

    [System.Serializable]
    public class TerrainDefData
    {
        public string terrainName;
        public TileType terrainType;
        public float moveSpeedFactor;
        public float fertility;
        public float flammability;
        public float thermalConductivity;
        public bool isWater;
        public bool canBuildOn;
        public float beautyOffset;
    }

    [System.Serializable]
    public class ResourceDefData
    {
        public string resourceName;
        public ResourceType resourceType;
        public int resourceTier;
        public float marketValue;
        public int stackLimit;
        public float weight;
        public float flammability;
        public float deteriorationRate;
        public bool canBeStolen;
        public float nutrition;
    }
}
