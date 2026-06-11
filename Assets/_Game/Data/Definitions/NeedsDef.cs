using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "NeedsDef_", menuName = "Ruinborne/Definitions/NeedsDef")]
    public class NeedsDef : ScriptableObject
    {
        [Header("기본 정보")]
        public NeedType needType;
        public string needName;

        [Header("감소 & 임계값")]
        public float decayRatePerSec = 1.5f;
        public float criticalThreshold = 25f;
        public float warningThreshold = 35f;

        [Header("충족")]
        public float fulfillAmountPerAction = 40f;

        [Header("0% 도달 시 피해")]
        public float starvationDamagePerSec = 0f;
    }
}
