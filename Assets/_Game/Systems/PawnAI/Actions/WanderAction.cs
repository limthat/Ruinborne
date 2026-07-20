using UnityEngine;
using UnityEngine.AI;

namespace Ruinborne.Systems.PawnAI.Actions
{
    public class WanderAction : GoapAction
    {
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float wanderInterval = 5f;

        private float _wanderTimer = 0f;

        protected override void Awake()
        {
            base.Awake();
            actionName = "WanderAction";
            cost = 10f; // 가장 낮은 우선순위
        }

        protected override void SetupConditions()
        {
            // 전제 조건 없음 — 항상 수행 가능
            AddEffect("is_wandering", true);
        }

        public override bool CheckProceduralPrecondition()
        {
            return true; // 항상 가능
        }

        public override bool Perform()
        {
            _wanderTimer -= Time.deltaTime;

            if (_wanderTimer <= 0f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += transform.position;

                UnityEngine.AI.NavMeshHit hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(
                    randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
                {
                    _controller.MoveTo(hit.position);
                    _wanderTimer = wanderInterval;
                }
            }

            // 배회는 영구적으로 실행 (완료 없음)
            // 다른 목표가 생기면 GoapAgent가 재계획
            return false;
        }

        public override void Reset()
        {
            _wanderTimer = 0f;
        }
    }
}
