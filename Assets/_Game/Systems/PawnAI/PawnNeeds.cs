using UnityEngine;
using System.Collections.Generic;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.PawnAI
{
    public class PawnNeeds : MonoBehaviour
    {
        [Header("욕구 정의 (NeedsDef ScriptableObject)")]
        [SerializeField] private NeedsDef[] needsDefs;

        private Dictionary<NeedType, float> _needs = new Dictionary<NeedType, float>();
        private PawnController _controller;

        private void Awake()
        {
            _controller = GetComponent<PawnController>();
            InitializeNeeds();
        }

        private void InitializeNeeds()
        {
            _needs.Clear();
            if (needsDefs == null) return;

            foreach (var def in needsDefs)
            {
                if (def == null) continue;
                _needs[def.needType] = 100f;
            }
        }

        private void Update()
        {
            if (_controller == null || !_controller.IsAlive) return;
            DecayNeeds();
            CheckCriticalNeeds();
        }

        private void DecayNeeds()
        {
            if (needsDefs == null) return;
            foreach (var def in needsDefs)
            {
                if (def == null) continue;
                if (!_needs.ContainsKey(def.needType)) continue;
                _needs[def.needType] = Mathf.Max(0f,
                    _needs[def.needType] - def.decayRatePerSec * Time.deltaTime);
            }
        }

        private void CheckCriticalNeeds()
        {
            if (needsDefs == null) return;
            foreach (var def in needsDefs)
            {
                if (def == null) continue;
                if (!_needs.ContainsKey(def.needType)) continue;

                float current = _needs[def.needType];

                // 0% 도달 시 체력 피해
                if (current <= 0f && def.starvationDamagePerSec > 0f)
                    _controller.TakeDamage(def.starvationDamagePerSec * Time.deltaTime);

                // 위기 임계값 이하 시 이벤트 발행
                if (current <= def.criticalThreshold)
                    GameEventBus.Publish(new PawnNeedCriticalEvent
                    {
                        PawnName = _controller.PawnName,
                        NeedType = def.needType,
                        CurrentValue = current
                    });
            }
        }

        public void FulfillNeed(NeedType needType, float amount)
        {
            if (!_needs.ContainsKey(needType)) return;
            _needs[needType] = Mathf.Min(100f, _needs[needType] + amount);
            Debug.Log($"[PawnNeeds] {_controller?.PawnName} {needType} +{amount} → {_needs[needType]:F1}");
        }

        public float GetNeedValue(NeedType needType)
        {
            return _needs.TryGetValue(needType, out float val) ? val : 100f;
        }

        public bool IsCritical(NeedType needType, float threshold = -1f)
        {
            NeedsDef def = GetNeedsDef(needType);
            float t = threshold >= 0f ? threshold : (def?.criticalThreshold ?? 25f);
            return GetNeedValue(needType) <= t;
        }

        public bool IsWarning(NeedType needType)
        {
            NeedsDef def = GetNeedsDef(needType);
            if (def == null) return false;
            return GetNeedValue(needType) <= def.warningThreshold;
        }

        private NeedsDef GetNeedsDef(NeedType needType)
        {
            if (needsDefs == null) return null;
            foreach (var def in needsDefs)
                if (def != null && def.needType == needType) return def;
            return null;
        }

        public IReadOnlyDictionary<NeedType, float> GetAllNeeds() => _needs;
    }
}
