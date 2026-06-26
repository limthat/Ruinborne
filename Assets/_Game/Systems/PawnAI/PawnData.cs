using UnityEngine;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.PawnAI
{
    [System.Serializable]
    public class PawnData
    {
        [Header("기본 정보")]
        public string pawnName;
        public RaceType raceType;
        public RaceDef raceDef;

        [Header("스탯")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float moveSpeed = 4.6f;
        public float workSpeed = 1f;

        [Header("스킬 레벨 (0~20)")]
        public int[] skillLevels = new int[19];

        [Header("작업 우선순위 (1~4, 0=비활성)")]
        public int[] workPriorities = new int[19];

        [Header("그룹")]
        public int groupIndex = 0;

        [Header("지휘관 여부")]
        public bool isCommander = false;

        public bool IsAlive => currentHealth > 0f;

        public void TakeDamage(float amount)
        {
            currentHealth = Mathf.Max(0f, currentHealth - amount);
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        public int GetSkillLevel(SkillType skillType)
        {
            int index = (int)skillType;
            if (index < 0 || index >= skillLevels.Length) return 0;
            return skillLevels[index];
        }

        public int GetWorkPriority(WorkType workType)
        {
            int index = (int)workType;
            if (index < 0 || index >= workPriorities.Length) return 0;
            return workPriorities[index];
        }

        public void SetWorkPriority(WorkType workType, int priority)
        {
            int index = (int)workType;
            if (index < 0 || index >= workPriorities.Length) return;
            workPriorities[index] = Mathf.Clamp(priority, 0, 4);
        }
    }
}
