using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "ThoughtDef_", menuName = "Ruinborne/Definitions/MoodThoughtDef")]
    public class MoodThoughtDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string thoughtName;
        public ThoughtCategory thoughtCategory;

        [Header("기분 효과")]
        public float moodEffect;
        [Tooltip("인게임 일 기준. 0이면 영구")] public float durationDays;
        public int stackLimit = 1;

        [Header("적용 조건")]
        public bool requiresRelationship = false;
        public float minRelationship = 0f;

        [Header("종족 제한 (비어있으면 전체 적용)")]
        public RaceDef[] raceRestriction;
    }
}
