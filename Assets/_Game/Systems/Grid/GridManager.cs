using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.Grid
{
    public class GridManager : ManagerBase<GridManager>
    {
        [Header("그리드 설정")]
        [SerializeField] private int width = 100;
        [SerializeField] private int height = 100;
        [SerializeField] private float cellSize = 1f;

        [Header("TerrainDef 참조 (ScriptableObject)")]
        [SerializeField] private TerrainDef terrainGrass;
        [SerializeField] private TerrainDef terrainForest;
        [SerializeField] private TerrainDef terrainMountain;
        [SerializeField] private TerrainDef terrainWater;
        [SerializeField] private TerrainDef terrainDesert;

        private GridCell[,] _grid;

        protected override void Awake()
        {
            base.Awake();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            _grid = new GridCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    _grid[x, z] = new GridCell(x, z);
                }
            }
            Debug.Log($"[GridManager] 그리드 초기화 완료: {width}x{height}");
        }

        public GridCell GetCell(int x, int z)
        {
            if (x < 0 || x >= width || z < 0 || z >= height) return null;
            return _grid[x, z];
        }

        public GridCell GetCellFromWorldPos(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / cellSize);
            int z = Mathf.FloorToInt(worldPos.z / cellSize);
            return GetCell(x, z);
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x * cellSize, 0f, z * cellSize);
        }

        public void SetTerrainDef(int x, int z, TerrainDef terrainDef)
        {
            var cell = GetCell(x, z);
            if (cell == null) return;
            cell.TerrainDef = terrainDef;
            cell.TileType = terrainDef.terrainType;
        }

        public TerrainDef GetTerrainDefForTileType(TileType tileType)
        {
            return tileType switch
            {
                TileType.Grass => terrainGrass,
                TileType.Forest => terrainForest,
                TileType.Mountain => terrainMountain,
                TileType.Water => terrainWater,
                TileType.Desert => terrainDesert,
                _ => terrainGrass
            };
        }

        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;
    }
}
