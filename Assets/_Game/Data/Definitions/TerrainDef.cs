using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "TerrainDef_", menuName = "Ruinborne/Definitions/TerrainDef")]
    public class TerrainDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string terrainName;
        public TileType terrainType;

        [Header("이동 & 건설")]
        public float moveSpeedFactor = 1f;
        public bool canBuildOn = true;

        [Header("농업")]
        [Range(0f, 1.5f)] public float fertility = 1f;

        [Header("환경")]
        [Range(0f, 1f)] public float flammability = 0.3f;
        public float thermalConductivity = 0.1f;
        public bool isWater = false;

        [Header("미관")]
        public float beautyOffset = 0f;
    }
}
