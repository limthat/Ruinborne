using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Systems.Economy;

namespace Ruinborne.Systems.PawnAI.Actions
{
    public class EatAction : GoapAction
    {
        [SerializeField] private float searchRadius = 30f;
        [SerializeField] private float eatDuration = 3f;

        private ResourceObject _targetFood;
        private bool _isEating = false;
        private float _eatTimer = 0f;

        protected override void Awake()
        {
            base.Awake();
            actionName = "EatAction";
        }

        protected override void SetupConditions()
        {
            AddPrecondition("has_food_nearby", true);
            AddEffect("is_hungry", false);
        }

        public override bool CheckProceduralPrecondition()
        {
            _targetFood = FindNearestFood();
            return _targetFood != null;
        }

        public override bool Perform()
        {
            // 타겟 없거나 고갈되면 다시 탐색
            if (_targetFood == null || _targetFood.IsDeplete)
            {
                _targetFood = FindNearestFood();
                if (_targetFood == null) return true;
            }

            // 음식으로 이동
            float dist = Vector3.Distance(transform.position, _targetFood.transform.position);
            if (dist > 1.5f)
            {
                _controller.MoveTo(_targetFood.transform.position);
                return false;
            }

            // 도착 — 식사
            _controller.StopMoving();
            _controller.SetState(PawnController.PawnState.Eating);

            if (!_isEating)
            {
                _isEating = true;
                _eatTimer = eatDuration;
                Debug.Log($"[EatAction] {_controller.PawnName} 식사 시작");
            }

            _eatTimer -= Time.deltaTime;
            if (_eatTimer <= 0f)
            {
                int eaten = _targetFood.Harvest(1);
                _needs?.FulfillNeed(NeedType.Food, eaten * 40f);
                _controller.SetState(PawnController.PawnState.Idle);
                Debug.Log($"[EatAction] {_controller.PawnName} 식사 완료");
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            _isEating = false;
            _eatTimer = 0f;
            _targetFood = null;
        }

        private ResourceObject FindNearestFood()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
            ResourceObject nearest = null;
            float minDist = float.MaxValue;

            foreach (var col in colliders)
            {
                var resource = col.GetComponent<ResourceObject>();
                if (resource == null || resource.IsDeplete) continue;
                if (resource.ResourceDef == null) continue;
                if (resource.ResourceDef.resourceType != ResourceType.RawFood) continue;

                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = resource;
                }
            }
            return nearest;
        }
    }
}
