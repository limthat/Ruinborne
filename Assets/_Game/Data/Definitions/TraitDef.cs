using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "TraitDef_", menuName = "Ruinborne/Definitions/TraitDef")]
    public class TraitDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string traitName;
        public TraitFlag specialFlag = TraitFlag.None;

        [Header("기분 & 스탯 보정")]
        public float moodOffset = 0f;
        public float workSpeedFactor = 1f;
        public float meltdownThresholdOffset = 0f;
        public float socialImpactFactor = 1f;
        public float combatStatFactor = 1f;

        [Header("제한")]
        public WorkType[] disabledWorkTypes;
        public TraitDef[] conflictTraits;

        [Header("종족 제한 (비어있으면 전체)")]
        public RaceDef[] raceRestriction;

        [Header("생성 가중치")]
        public float spawnWeight = 10f;
    }
}
