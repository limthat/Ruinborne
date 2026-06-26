using UnityEngine;
using System.Collections.Generic;
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
            // 기본 목표: 작업 수행
            _currentGoal.Clear();
            _currentGoal["is_working"] = true;
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
            }

            bool done = _currentAction.Perform();
            if (done) _currentAction = null;
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
