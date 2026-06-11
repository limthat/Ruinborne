using UnityEngine;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "SchedulerConfig_", menuName = "Ruinborne/Definitions/SchedulerConfigDef")]
    public class SchedulerConfigDef : ScriptableObject
    {
        [Header("AI 주기")]
        public float scheduleIntervalSec = 2f;

        [Header("탐색 반경")]
        public float resourceSearchRadius = 20f;
        public float socialSearchRadius = 8f;
        public float eatSourceSearchRadius = 30f;
        public float sleepSpotSearchRadius = 40f;

        [Header("접근 거리")]
        public float socialInteractionDistance = 3f;

        [Header("타임아웃")]
        public float moveToTargetTimeout = 15f;
        public float haulToStockpileTimeout = 20f;

        [Header("행동 지속 시간")]
        public float socialInteractionDuration = 2f;
        public float feedingDuration = 3f;
    }
}
