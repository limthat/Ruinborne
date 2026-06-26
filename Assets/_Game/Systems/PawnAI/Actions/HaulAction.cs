using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;

namespace Ruinborne.Systems.PawnAI.Actions
{
    public class HaulAction : GoapAction
    {
        [SerializeField] private float searchRadius = 30f;

        private Economy.ResourceObject _targetResource;
        private Economy.Stockpile _targetStockpile;
        private bool _isHauling = false;
        private float _haulTimer = 0f;
        private bool _pickedUp = false;

        protected override void SetupConditions()
        {
            AddPrecondition("has_resource_to_haul", true);
            AddEffect("is_working", true);
        }

        public override bool CheckProceduralPrecondition()
        {
            _targetResource = FindNearestResource();
            _targetStockpile = FindNearestStockpile();
            return _targetResource != null && _targetStockpile != null;
        }

        public override bool Perform()
        {
            if (_targetResource == null || _targetStockpile == null) return true;

            // 1단계: 자원으로 이동
            if (!_pickedUp)
            {
                float distToResource = Vector3.Distance(
                    transform.position, _targetResource.transform.position);

                if (distToResource > 1.5f)
                {
                    _controller.MoveTo(_targetResource.transform.position);
                    return false;
                }

                // 자원 집기
                _controller.StopMoving();
                _controller.SetState(PawnController.PawnState.Working);

                if (!_isHauling)
                {
                    _isHauling = true;
                    _haulTimer = _targetResource.GetHarvestTime();
                }

                _haulTimer -= Time.deltaTime;
                if (_haulTimer <= 0f)
                {
                    _targetResource.Harvest(1);
                    _pickedUp = true;
                    _isHauling = false;
                    Debug.Log($"[HaulAction] {_controller.PawnName} 자원 픽업 완료");
                }

                return false;
            }

            // 2단계: 창고로 이동
            float distToStockpile = Vector3.Distance(
                transform.position, _targetStockpile.transform.position);

            if (distToStockpile > _targetStockpile.Radius)
            {
                _controller.MoveTo(_targetStockpile.transform.position);
                return false;
            }

            // 창고에 도착 — 이미 ResourceManager에 추가됨 (Harvest에서 처리)
            _controller.StopMoving();
            _controller.SetState(PawnController.PawnState.Idle);
            Debug.Log($"[HaulAction] {_controller.PawnName} 운반 완료");
            return true;
        }

        public override void Reset()
        {
            _isHauling = false;
            _haulTimer = 0f;
            _pickedUp = false;
            _targetResource = null;
            _targetStockpile = null;
        }

        private Economy.ResourceObject FindNearestResource()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
            Economy.ResourceObject nearest = null;
            float minDist = float.MaxValue;

            foreach (var col in colliders)
            {
                var resource = col.GetComponent<Economy.ResourceObject>();
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

        private Economy.Stockpile FindNearestStockpile()
        {
            Economy.Stockpile[] stockpiles = FindObjectsByType<Economy.Stockpile>(
                FindObjectsSortMode.None);
            Economy.Stockpile nearest = null;
            float minDist = float.MaxValue;

            foreach (var stockpile in stockpiles)
            {
                float dist = Vector3.Distance(transform.position, stockpile.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = stockpile;
                }
            }
            return nearest;
        }
    }
}
