using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Systems.PawnAI.Actions
{
    public class SocialAction : GoapAction
    {
        [SerializeField] private float searchRadius = 8f;
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private float interactionDuration = 2f;

        private PawnController _targetPawn;
        private bool _isInteracting = false;
        private float _interactionTimer = 0f;

        protected override void SetupConditions()
        {
            AddPrecondition("has_pawn_nearby", true);
            AddEffect("is_lonely", false);
        }

        public override bool CheckProceduralPrecondition()
        {
            _targetPawn = FindNearbyPawn();
            return _targetPawn != null;
        }

        public override bool Perform()
        {
            if (_targetPawn == null || !_targetPawn.IsAlive) return true;

            // 이동
            float dist = Vector3.Distance(transform.position, _targetPawn.transform.position);
            if (dist > interactionDistance)
            {
                _controller.MoveTo(_targetPawn.transform.position);
                return false;
            }

            // 대화
            _controller.StopMoving();
            _controller.SetState(PawnController.PawnState.Idle);

            if (!_isInteracting)
            {
                _isInteracting = true;
                _interactionTimer = interactionDuration;
                Debug.Log($"[SocialAction] {_controller.PawnName} → {_targetPawn.PawnName} 대화 시작");
            }

            _interactionTimer -= Time.deltaTime;
            if (_interactionTimer <= 0f)
            {
                // 사교 욕구 충족
                _needs?.FulfillNeed(NeedType.Social, 30f);

                // 상대방 사교 욕구도 소폭 충족
                var targetNeeds = _targetPawn.GetComponent<PawnNeeds>();
                targetNeeds?.FulfillNeed(NeedType.Social, 15f);

                Debug.Log($"[SocialAction] {_controller.PawnName} → {_targetPawn.PawnName} 대화 완료");
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            _isInteracting = false;
            _interactionTimer = 0f;
            _targetPawn = null;
        }

        private PawnController FindNearbyPawn()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
            foreach (var col in colliders)
            {
                var pawn = col.GetComponent<PawnController>();
                if (pawn == null || pawn == _controller) continue;
                if (!pawn.IsAlive) continue;
                if (pawn.State == PawnController.PawnState.MeltingDown) continue;
                return pawn;
            }
            return null;
        }
    }
}
