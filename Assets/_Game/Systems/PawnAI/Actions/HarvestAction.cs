using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Systems.Economy;

namespace Ruinborne.Systems.PawnAI.Actions
{
    public class HarvestAction : GoapAction
    {
        [SerializeField] private float searchRadius = 20f;
        private ResourceObject _targetResource;
        private bool _isHarvesting = false;
        private float _harvestTimer = 0f;

        protected override void SetupConditions()
        {
            AddPrecondition("has_resource_nearby", true);
            AddEffect("is_working", true);
        }

        public override bool CheckProceduralPrecondition()
        {
            _targetResource = FindNearestResource();
            return _targetResource != null;
        }

        public override bool Perform()
        {
            if (_targetResource == null || _targetResource.IsDeplete) return true;

            // 이동
            float dist = Vector3.Distance(transform.position, _targetResource.transform.position);
            if (dist > 1.5f)
            {
                _controller.MoveTo(_targetResource.transform.position);
                return false;
            }

            // 채취
            _controller.StopMoving();
            _controller.SetState(PawnController.PawnState.Working);

            if (!_isHarvesting)
            {
                _isHarvesting = true;
                _harvestTimer = _targetResource.GetHarvestTime();
            }

            _harvestTimer -= Time.deltaTime;
            if (_harvestTimer <= 0f)
            {
                _targetResource.Harvest(1);
                Debug.Log($"[HarvestAction] {_controller.PawnName} 채취 완료");
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            _isHarvesting = false;
            _harvestTimer = 0f;
            _targetResource = null;
        }

        private ResourceObject FindNearestResource()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
            ResourceObject nearest = null;
            float minDist = float.MaxValue;

            foreach (var col in colliders)
            {
                var resource = col.GetComponent<ResourceObject>();
                if (resource == null || resource.IsDeplete) continue;
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
