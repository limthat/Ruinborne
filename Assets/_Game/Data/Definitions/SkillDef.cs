using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "SkillDef_", menuName = "Ruinborne/Definitions/SkillDef")]
    public class SkillDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string skillName;
        public SkillType skillType;

        [Header("연관 작업")]
        public WorkType[] relatedWorkTypes;

        [Header("레벨당 보정")]
        public float speedBonusPerLevel = 0.05f;
        public float qualityBonusPerLevel = 0.04f;

        [Header("경험치")]
        public float xpPerAction = 50f;
        public float[] xpPerLevelUp;

        [Header("열정")]
        public bool canBePassion = true;

        [Header("종족 제한")]
        public RaceDef[] raceBlacklist;
    }
}
