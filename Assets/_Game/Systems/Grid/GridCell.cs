using UnityEngine;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.Grid
{
    public class GridCell
    {
        public int X { get; private set; }
        public int Z { get; private set; }
        public TileType TileType { get; set; }
        public TerrainDef TerrainDef { get; set; }
        public bool IsOccupied { get; set; }
        public GameObject OccupyingObject { get; set; }

        public Vector3 WorldPosition => new Vector3(X, 0f, Z);

        public GridCell(int x, int z, TileType tileType = TileType.Grass)
        {
            X = x;
            Z = z;
            TileType = tileType;
        }

        public bool IsWalkable()
        {
            if (TerrainDef != null) return TerrainDef.canBuildOn && !IsOccupied;
            return TileType != TileType.Water && TileType != TileType.Lava && !IsOccupied;
        }

        public float GetMoveSpeedFactor()
        {
            if (TerrainDef != null) return TerrainDef.moveSpeedFactor;
            return 1f;
        }
    }
}
