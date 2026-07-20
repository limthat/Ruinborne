using UnityEngine;
using System.Collections;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.PawnAI
{
    public class PawnScheduler : MonoBehaviour
    {
        [Header("스케줄러 설정 (ScriptableObject)")]
        [SerializeField] private SchedulerConfigDef config;

        private PawnController _controller;
        private PawnNeeds _needs;
        private PawnMood _mood;
        private GoapAgent _goapAgent;

        private float _scheduleTimer = 0f;

        private void Awake()
        {
            _controller = GetComponent<PawnController>();
            _needs = GetComponent<PawnNeeds>();
            _mood = GetComponent<PawnMood>();
            _goapAgent = GetComponent<GoapAgent>();
        }

        private void Update()
        {
            if (_controller == null || !_controller.IsAlive) return;
            if (_controller.State == PawnController.PawnState.MeltingDown) return;
            if (_controller.State == PawnController.PawnState.Dead) return;

            _scheduleTimer -= Time.deltaTime;
            if (_scheduleTimer <= 0f)
            {
                _scheduleTimer = config != null ? config.scheduleIntervalSec : 2f;
                EvaluateSchedule();
            }
        }

        private void EvaluateSchedule()
        {
            // 1순위: 음식 위기
            if (_needs != null && _needs.IsCritical(NeedType.Food))
            {
                return; // GoapAgent가 자동 처리
            }

            // 2순위: 수면 위기
            if (_needs != null && _needs.IsCritical(NeedType.Sleep))
            {
                return;
            }

            // 3순위: 사교 위기
            if (_needs != null && _needs.IsCritical(NeedType.Social))
            {
                return;
            }

            // 4순위: 기분 경고
            if (_mood != null && _mood.CurrentMood < 30f)
            {
                Debug.Log($"[PawnScheduler] {_controller.PawnName} — 기분 저하 ({_mood.CurrentMood:F1})");
                return;
            }

            // 5순위: 작업 우선순위 기반 작업 수행
            WorkType highestWork = GetHighestPriorityWork();
            if (highestWork != WorkType.ChopWood) // 기본값이 아닌 경우
            {
                Debug.Log($"[PawnScheduler] {_controller.PawnName} — 작업 수행: {highestWork}");
            }
        }

        private WorkType GetHighestPriorityWork()
        {
            if (_controller.data == null) return WorkType.ChopWood;

            WorkType best = WorkType.ChopWood;
            int bestPriority = 0;

            foreach (WorkType workType in System.Enum.GetValues(typeof(WorkType)))
            {
                int priority = _controller.data.GetWorkPriority(workType);
                if (priority > bestPriority)
                {
                    bestPriority = priority;
                    best = workType;
                }
            }

            return best;
        }

        public void SetConfig(SchedulerConfigDef newConfig)
        {
            config = newConfig;
        }
    }
}
