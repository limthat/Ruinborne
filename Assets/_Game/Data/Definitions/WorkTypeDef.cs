using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "WorkTypeDef_", menuName = "Ruinborne/Definitions/WorkTypeDef")]
    public class WorkTypeDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string workTypeName;
        public WorkType workType;

        [Header("연관 스킬")]
        public SkillDef relatedSkill;

        [Header("우선순위")]
        [Range(0, 4)] public int defaultPriority = 3;

        [Header("제한")]
        public RaceDef[] raceBlacklist;
        public TraitDef[] traitBlacklist;

        [Header("자동 수행 여부")]
        [Tooltip("false이면 플레이어가 명시적으로 지정해야 수행")]
        public bool autoPerform = true;

        [Header("UI")]
        public int uiOrder;
    }
}
