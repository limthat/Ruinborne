using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "MentalBreakDef_", menuName = "Ruinborne/Definitions/MentalBreakDef")]
    public class MentalBreakDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string breakName;
        public MentalBreakType breakType;
        public MentalBreakSeverity severity;

        [Header("행동 설정")]
        public float durationSeconds = 30f;
        public float wanderRadius = 15f;
        public float moveSpeedFactor = 1f;

        [Header("발동 & 종료 조건")]
        public float triggerWeight = 1f;
        public float moodRecoveryToEnd = 40f;
        public float cooldownAfterBreak = 120f;

        [Header("종족 제한")]
        public RaceDef[] raceBlacklist;
    }
}
