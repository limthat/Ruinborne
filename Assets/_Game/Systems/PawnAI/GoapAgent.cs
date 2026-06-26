using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Ruinborne.Core;

namespace Ruinborne.Systems.PawnAI
{
    public class GoapAgent : MonoBehaviour
    {
        private PawnController _controller;
        private PawnNeeds _needs;
        private List<GoapAction> _availableActions = new List<GoapAction>();
        private Queue<GoapAction> _currentPlan;
        private GoapAction _currentAction;

        // 현재 월드 상태
        private Dictionary<string, bool> _worldState = new Dictionary<string, bool>();
        // 현재 목표
        private Dictionary<string, bool> _currentGoal = new Dictionary<string, bool>();

        private float _planInterval = 2f;
        private float _planTimer = 0f;

        private void Awake()
        {
            _controller = GetComponent<PawnController>();
            _needs = GetComponent<PawnNeeds>();
            _availableActions.AddRange(GetComponents<GoapAction>());
        }

        private void Update()
        {
            if (_controller == null || !_controller.IsAlive) return;

            _planTimer -= Time.deltaTime;
            if (_planTimer <= 0f)
            {
                _planTimer = _planInterval;
                UpdateWorldState();
                DetermineGoal();
                Plan();
            }

            ExecuteCurrentAction();
        }

        private void UpdateWorldState()
        {
            if (_needs == null) return;

            _worldState["is_hungry"] = _needs.IsCritical(Data.NeedType.Food);
            _worldState["is_tired"] = _needs.IsCritical(Data.NeedType.Sleep);
            _worldState["is_lonely"] = _needs.IsCritical(Data.NeedType.Social);
            _worldState["has_food"] = ServiceLocator.Get<Economy.ResourceManager>()
                ?.GetAmount(Data.ResourceType.RawFood) > 0;

            // 근처 자원 확인
            Collider[] cols = Physics.OverlapSphere(transform.position, 30f);
            bool hasResourceNearby = false;
            foreach (var col in cols)
            {
                if (col.GetComponent<Economy.ResourceObject>() != null)
                {
                    hasResourceNearby = true;
                    break;
                }
            }
            _worldState["has_resource_nearby"] = hasResourceNearby;
            _worldState["has_resource_to_haul"] = hasResourceNearby;
            _worldState["has_sleep_spot"] = true; // 임시: 항상 수면 가능
            _worldState["has_pawn_nearby"] = FindObjectsByType<PawnController>(
                FindObjectsSortMode.None).Length > 1;

            Debug.Log($"[GoapAgent] WorldState — hungry:{_worldState.GetValueOrDefault("is_hungry")} tired:{_worldState.GetValueOrDefault("is_tired")} resource:{_worldState.GetValueOrDefault("has_resource_nearby")} goal:{string.Join(",", _currentGoal.Select(kv => kv.Key + "=" + kv.Value))}");
        }

        private void DetermineGoal()
        {
            // 가장 긴급한 욕구를 목표로 설정
            if (_needs != null && _needs.IsCritical(Data.NeedType.Food))
            {
                _currentGoal.Clear();
                _currentGoal["is_hungry"] = false;
                return;
            }
            if (_needs != null && _needs.IsCritical(Data.NeedType.Sleep))
            {
                _currentGoal.Clear();
                _currentGoal["is_tired"] = false;
                return;
            }
            // 기본 목표: 배회 (할 일 없을 때)
            _currentGoal.Clear();
            _currentGoal["is_wandering"] = true;
        }

        private void Plan()
        {
            _currentPlan = GoapPlanner.Plan(_availableActions, _worldState, _currentGoal);
            _currentAction = null;
        }

        private void ExecuteCurrentAction()
        {
            if (_currentAction == null)
            {
                if (_currentPlan == null || _currentPlan.Count == 0) return;
                _currentAction = _currentPlan.Dequeue();
                _currentAction.Reset();
                Debug.Log($"[GoapAgent] 액션 시작: {_currentAction.actionName}");
            }

            bool done = _currentAction.Perform();
            if (done)
            {
                Debug.Log($"[GoapAgent] 액션 완료: {_currentAction.actionName}");
                _currentAction = null;
            }
        }

        public void AddAction(GoapAction action)
        {
            if (!_availableActions.Contains(action))
                _availableActions.Add(action);
        }

        public Dictionary<string, bool> GetWorldState() => _worldState;
        public Dictionary<string, bool> GetCurrentGoal() => _currentGoal;
    }
}
