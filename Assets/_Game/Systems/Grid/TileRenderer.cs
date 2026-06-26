using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;

namespace Ruinborne.Systems.Grid
{
    public class TileRenderer : ManagerBase<TileRenderer>
    {
        [Header("타일 프리팹")]
        [SerializeField] private GameObject grassPrefab;
        [SerializeField] private GameObject forestPrefab;
        [SerializeField] private GameObject mountainPrefab;
        [SerializeField] private GameObject waterPrefab;
        [SerializeField] private GameObject desertPrefab;
        [SerializeField] private GameObject snowPrefab;

        [Header("렌더링 설정")]
        [SerializeField] private Transform tileParent;
        private GridManager _gridManager;
        private GameObject[,] _tileObjects;

        private void Start()
        {
            _gridManager = ServiceLocator.Get<GridManager>();
            GameEventBus.Subscribe<MapGeneratedEvent>(OnMapGenerated);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEventBus.Unsubscribe<MapGeneratedEvent>(OnMapGenerated);
        }

        private void OnMapGenerated(MapGeneratedEvent evt)
        {
            RenderMap();
        }

        public void RenderMap()
        {
            if (_gridManager == null) return;

            // 기존 타일 제거
            if (_tileObjects != null)
            {
                for (int x = 0; x < _gridManager.Width; x++)
                    for (int z = 0; z < _gridManager.Height; z++)
                        if (_tileObjects[x, z] != null)
                            Destroy(_tileObjects[x, z]);
            }

            _tileObjects = new GameObject[_gridManager.Width, _gridManager.Height];

            for (int x = 0; x < _gridManager.Width; x++)
            {
                for (int z = 0; z < _gridManager.Height; z++)
                {
                    var cell = _gridManager.GetCell(x, z);
                    if (cell == null) continue;

                    GameObject prefab = GetPrefabForTileType(cell.TileType);
                    if (prefab == null) continue;

                    Vector3 pos = _gridManager.GetWorldPosition(x, z);
                    GameObject tile = Instantiate(prefab, pos, Quaternion.identity, tileParent);
                    tile.name = $"Tile_{x}_{z}";
                    _tileObjects[x, z] = tile;
                }
            }

            Debug.Log($"[TileRenderer] 타일 렌더링 완료: {_gridManager.Width}x{_gridManager.Height}");
        }

        private GameObject GetPrefabForTileType(TileType tileType)
        {
            return tileType switch
            {
                TileType.Grass    => grassPrefab,
                TileType.Forest   => forestPrefab,
                TileType.Mountain => mountainPrefab,
                TileType.Water    => waterPrefab,
                TileType.Desert   => desertPrefab,
                TileType.Snow     => snowPrefab,
                _                 => grassPrefab
            };
        }
    }
}
