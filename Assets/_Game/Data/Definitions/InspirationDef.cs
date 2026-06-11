using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "InspirationDef_", menuName = "Ruinborne/Definitions/InspirationDef")]
    public class InspirationDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string inspirationName;

        [Header("효과")]
        public float workSpeedFactor = 2f;
        public float qualityFactor = 1.5f;
        public float durationSeconds = 180f;

        [Header("발동 조건")]
        public float triggerMoodMin = 50f;
        public float triggerChancePerDay = 0.03f;

        [Header("연관 작업")]
        public WorkType[] relatedWorkTypes;
    }
}
