using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "ResourceDef_", menuName = "Ruinborne/Definitions/ResourceDef")]
    public class ResourceDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string resourceName;
        public ResourceType resourceType;
        [Range(1, 3)] public int resourceTier = 1;

        [Header("경제")]
        public float marketValue = 3f;
        public int stackLimit = 75;
        public float weight = 0.5f;

        [Header("환경")]
        [Range(0f, 1f)] public float flammability = 0f;
        public float deteriorationRate = 0f;

        [Header("기타")]
        public bool canBeStolen = true;
        public float nutrition = 0f;
    }
}
