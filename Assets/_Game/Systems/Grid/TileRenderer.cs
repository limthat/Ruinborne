using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;

namespace Ruinborne.Systems.Grid
{
    public class TileRenderer : ManagerBase<TileRenderer>
    {
        [Header("타일 프리팹 (없으면 기본 프리미티브 사용)")]
        [SerializeField] private GameObject grassPrefab;
        [SerializeField] private GameObject forestPrefab;
        [SerializeField] private GameObject mountainPrefab;
        [SerializeField] private GameObject waterPrefab;
        [SerializeField] private GameObject desertPrefab;
        [SerializeField] private GameObject snowPrefab;

        [Header("기본 타일 색상 (프리팹 없을 시 사용)")]
        [SerializeField] private Color grassColor    = new Color(0.4f, 0.7f, 0.3f);
        [SerializeField] private Color forestColor   = new Color(0.1f, 0.4f, 0.1f);
        [SerializeField] private Color mountainColor = new Color(0.5f, 0.5f, 0.5f);
        [SerializeField] private Color waterColor    = new Color(0.2f, 0.4f, 0.8f);
        [SerializeField] private Color desertColor   = new Color(0.9f, 0.8f, 0.4f);
        [SerializeField] private Color snowColor     = new Color(0.9f, 0.9f, 0.95f);
        [SerializeField] private Color sandColor     = new Color(0.85f, 0.75f, 0.5f);
        [SerializeField] private Color lavaColor     = new Color(0.9f, 0.2f, 0.0f);
        [SerializeField] private Color soilColor     = new Color(0.6f, 0.4f, 0.2f);

        [Header("렌더링 설정")]
        [SerializeField] private Transform tileParent;
        [SerializeField] private float tileHeight = 0.1f;

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

            ClearTiles();

            _tileObjects = new GameObject[_gridManager.Width, _gridManager.Height];
            Transform parent = tileParent != null ? tileParent : transform;

            for (int x = 0; x < _gridManager.Width; x++)
            {
                for (int z = 0; z < _gridManager.Height; z++)
                {
                    var cell = _gridManager.GetCell(x, z);
                    if (cell == null) continue;

                    GameObject tile = CreateTile(cell.TileType, x, z, parent);
                    _tileObjects[x, z] = tile;
                }
            }

            Debug.Log($"[TileRenderer] 타일 렌더링 완료: {_gridManager.Width}x{_gridManager.Height}");
        }

        private GameObject CreateTile(TileType tileType, int x, int z, Transform parent)
        {
            GameObject prefab = GetPrefabForTileType(tileType);
            Vector3 pos = _gridManager.GetWorldPosition(x, z);
            GameObject tile;

            if (prefab != null)
            {
                tile = Instantiate(prefab, pos, Quaternion.identity, parent);
            }
            else
            {
                // 프리팹 없으면 기본 Cube로 생성
                tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.transform.SetParent(parent);
                tile.transform.position = pos;
                tile.transform.localScale = new Vector3(
                    _gridManager.CellSize, tileHeight, _gridManager.CellSize);

                // 색상 적용
                var renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    mat.color = GetColorForTileType(tileType);
                    renderer.material = mat;
                }

                // 콜라이더를 타일 크기에 맞게 조정
                var collider = tile.GetComponent<BoxCollider>();
                if (collider != null) collider.enabled = true;
            }

            tile.name = $"Tile_{x}_{z}_{tileType}";
            tile.layer = LayerMask.NameToLayer("Default");
            return tile;
        }

        private void ClearTiles()
        {
            if (_tileObjects == null) return;
            for (int x = 0; x < _gridManager.Width; x++)
                for (int z = 0; z < _gridManager.Height; z++)
                    if (_tileObjects[x, z] != null)
                        Destroy(_tileObjects[x, z]);
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
                _                 => null
            };
        }

        private Color GetColorForTileType(TileType tileType)
        {
            return tileType switch
            {
                TileType.Grass    => grassColor,
                TileType.Forest   => forestColor,
                TileType.Mountain => mountainColor,
                TileType.Water    => waterColor,
                TileType.Desert   => desertColor,
                TileType.Snow     => snowColor,
                TileType.Sand     => sandColor,
                TileType.Lava     => lavaColor,
                TileType.Soil     => soilColor,
                _                 => grassColor
            };
        }
    }
}
