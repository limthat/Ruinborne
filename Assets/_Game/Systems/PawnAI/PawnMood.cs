using UnityEngine;
using System.Collections.Generic;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.PawnAI
{
    public class PawnMood : MonoBehaviour
    {
        [Header("기분 설정")]
        [SerializeField] private float baseMood = 50f;
        [SerializeField] private float moodMin = 0f;
        [SerializeField] private float moodMax = 100f;

        [Header("정신 붕괴 설정 (MentalBreakDef ScriptableObject)")]
        [SerializeField] private MentalBreakDef[] mentalBreakDefs;
        [SerializeField] private float meltdownThreshold = 20f;

        [Header("영감 설정 (InspirationDef ScriptableObject)")]
        [SerializeField] private InspirationDef[] inspirationDefs;
        [SerializeField] private float inspirationThreshold = 50f;

        private float _currentMood;
        private List<ActiveThought> _activeThoughts = new List<ActiveThought>();
        private PawnController _controller;
        private bool _isMeltingDown = false;
        private float _inspirationCheckTimer = 0f;
        private const float InspCheckInterval = 60f;

        private struct ActiveThought
        {
            public MoodThoughtDef Def;
            public float RemainingDays;
        }

        private void Awake()
        {
            _controller = GetComponent<PawnController>();
            _currentMood = baseMood;
        }

        private void Update()
        {
            if (_controller == null || !_controller.IsAlive) return;

            UpdateThoughts();
            CheckMeltdown();
            CheckInspiration();
        }

        private void UpdateThoughts()
        {
            float moodOffset = 0f;
            for (int i = _activeThoughts.Count - 1; i >= 0; i--)
            {
                var thought = _activeThoughts[i];
                if (thought.Def == null) continue;

                // 지속 시간 감소 (0이면 영구)
                if (thought.Def.durationDays > 0f)
                {
                    var updated = thought;
                    updated.RemainingDays -= Time.deltaTime / 60f; // 1분 = 1인게임 일
                    if (updated.RemainingDays <= 0f)
                    {
                        _activeThoughts.RemoveAt(i);
                        continue;
                    }
                    _activeThoughts[i] = updated;
                }

                moodOffset += thought.Def.moodEffect;
            }

            _currentMood = Mathf.Clamp(baseMood + moodOffset, moodMin, moodMax);
        }

        private void CheckMeltdown()
        {
            if (_isMeltingDown) return;
            if (_currentMood > meltdownThreshold) return;
            if (_controller.State == PawnController.PawnState.MeltingDown) return;

            MentalBreakDef breakDef = PickMentalBreak();
            if (breakDef == null) return;

            _isMeltingDown = true;
            _controller.SetState(PawnController.PawnState.MeltingDown);
            GameEventBus.Publish(new PawnMeltdownEvent
            {
                PawnName = _controller.PawnName,
                Severity = breakDef.severity
            });
            Debug.Log($"[PawnMood] {_controller.PawnName} 정신 붕괴: {breakDef.breakName} ({breakDef.severity})");

            StartCoroutine(MeltdownRoutine(breakDef));
        }

        private System.Collections.IEnumerator MeltdownRoutine(MentalBreakDef def)
        {
            yield return new WaitForSeconds(def.durationSeconds);
            _isMeltingDown = false;
            _controller.SetState(PawnController.PawnState.Idle);
            Debug.Log($"[PawnMood] {_controller.PawnName} 정신 붕괴 종료");
        }

        private void CheckInspiration()
        {
            _inspirationCheckTimer -= Time.deltaTime;
            if (_inspirationCheckTimer > 0f) return;
            _inspirationCheckTimer = InspCheckInterval;

            if (_currentMood < inspirationThreshold) return;
            if (inspirationDefs == null || inspirationDefs.Length == 0) return;

            foreach (var def in inspirationDefs)
            {
                if (def == null) continue;
                if (Random.value < def.triggerChancePerDay)
                {
                    Debug.Log($"[PawnMood] {_controller.PawnName} 영감 발동: {def.inspirationName}");
                    StartCoroutine(InspirationRoutine(def));
                    break;
                }
            }
        }

        private System.Collections.IEnumerator InspirationRoutine(InspirationDef def)
        {
            Debug.Log($"[PawnMood] {_controller.PawnName} 영감 시작: {def.inspirationName} ({def.durationSeconds}초)");
            yield return new WaitForSeconds(def.durationSeconds);
            Debug.Log($"[PawnMood] {_controller.PawnName} 영감 종료: {def.inspirationName}");
        }

        public void AddThought(MoodThoughtDef thoughtDef)
        {
            if (thoughtDef == null) return;

            // 스택 제한 확인
            int count = 0;
            foreach (var t in _activeThoughts)
                if (t.Def == thoughtDef) count++;
            if (count >= thoughtDef.stackLimit) return;

            _activeThoughts.Add(new ActiveThought
            {
                Def = thoughtDef,
                RemainingDays = thoughtDef.durationDays
            });
            Debug.Log($"[PawnMood] {_controller?.PawnName} Thought 추가: {thoughtDef.thoughtName} ({thoughtDef.moodEffect:+0;-0})");
        }

        public void RemoveThought(MoodThoughtDef thoughtDef)
        {
            _activeThoughts.RemoveAll(t => t.Def == thoughtDef);
        }

        private MentalBreakDef PickMentalBreak()
        {
            if (mentalBreakDefs == null || mentalBreakDefs.Length == 0) return null;
            float totalWeight = 0f;
            foreach (var def in mentalBreakDefs)
                if (def != null) totalWeight += def.triggerWeight;

            float rand = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            foreach (var def in mentalBreakDefs)
            {
                if (def == null) continue;
                cumulative += def.triggerWeight;
                if (rand <= cumulative) return def;
            }
            return mentalBreakDefs[0];
        }

        public float CurrentMood => _currentMood;
        public bool IsMeltingDown => _isMeltingDown;
    }
}
