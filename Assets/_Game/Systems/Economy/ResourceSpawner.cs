using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;
using Ruinborne.Systems.Grid;

namespace Ruinborne.Systems.Economy
{
    public class ResourceSpawner : MonoBehaviour
    {
        [Header("자원 설정")]
        [SerializeField] private ResourceSpawnEntry[] spawnEntries;
        [SerializeField] private GridManager gridManager;

        [Header("스폰 설정")]
        [SerializeField] private int spawnAttempts = 200;
        [SerializeField] private float minDistanceBetweenResources = 3f;

        private void Start()
        {
            GameEventBus.Subscribe<NavMeshBakedEvent>(OnNavMeshBaked);
        }

        private void OnDestroy()
        {
            GameEventBus.Unsubscribe<NavMeshBakedEvent>(OnNavMeshBaked);
        }

        private void OnNavMeshBaked(NavMeshBakedEvent evt)
        {
            SpawnResources();
        }

        private void SpawnResources()
        {
            if (gridManager == null)
                gridManager = ServiceLocator.Get<Ruinborne.Systems.Grid.GridManager>();

            if (spawnEntries == null) return;

            foreach (var entry in spawnEntries)
            {
                int spawned = 0;
                int attempts = 0;

                while (spawned < entry.count && attempts < spawnAttempts)
                {
                    attempts++;

                    int x = Random.Range(5, gridManager.Width - 5);
                    int z = Random.Range(5, gridManager.Height - 5);

                    var cell = gridManager.GetCell(x, z);
                    if (cell == null) continue;
                    if (!IsTileTypeAllowed(cell.TileType, entry.allowedTileTypes)) continue;
                    if (cell.IsOccupied) continue;

                    Vector3 pos = gridManager.GetWorldPosition(x, z);
                    pos.y = 0.5f;

                    // 너무 가까운 자원 있으면 스킵
                    if (IsTooClose(pos)) continue;

                    GameObject obj = Instantiate(entry.prefab, pos, Quaternion.identity, transform);
                    var resourceObj = obj.GetComponent<ResourceObject>();
                    if (resourceObj == null) continue;

                    cell.IsOccupied = true;
                    spawned++;
                }

                Debug.Log($"[ResourceSpawner] {entry.resourceName} 스폰: {spawned}개");
            }
        }

        private bool IsTileTypeAllowed(TileType tileType, TileType[] allowed)
        {
            if (allowed == null || allowed.Length == 0) return true;
            foreach (var t in allowed)
                if (t == tileType) return true;
            return false;
        }

        private bool IsTooClose(Vector3 pos)
        {
            Collider[] cols = Physics.OverlapSphere(pos, minDistanceBetweenResources);
            foreach (var col in cols)
                if (col.GetComponent<ResourceObject>() != null) return true;
            return false;
        }
    }

    [System.Serializable]
    public class ResourceSpawnEntry
    {
        public string resourceName;
        public GameObject prefab;
        public int count = 20;
        public TileType[] allowedTileTypes;
    }
}
