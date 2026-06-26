using UnityEngine;
using UnityEngine.AI;
using Ruinborne.Core;
using Ruinborne.Data;

namespace Ruinborne.Systems.PawnAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PawnController : MonoBehaviour
    {
        [Header("폰 데이터")]
        [SerializeField] public PawnData data = new PawnData();

        public enum PawnState { Idle, Moving, Working, Eating, Sleeping, MeltingDown, Dead }
        public PawnState State { get; private set; } = PawnState.Idle;

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent != null)
            {
                _agent.speed = data.moveSpeed;
                _agent.stoppingDistance = 0.5f;
            }
        }

        private void Start()
        {
            GameEventBus.Publish(new PawnSpawnedEvent
            {
                PawnName = data.pawnName,
                Position = transform.position
            });
        }

        private void OnDestroy()
        {
            GameEventBus.Publish(new PawnDiedEvent
            {
                PawnName = data.pawnName,
                Position = transform.position
            });
        }

        public void MoveTo(Vector3 destination)
        {
            if (_agent == null || !_agent.isOnNavMesh) return;
            _agent.SetDestination(destination);
            SetState(PawnState.Moving);
        }

        public void StopMoving()
        {
            if (_agent != null && _agent.isOnNavMesh)
                _agent.ResetPath();
            SetState(PawnState.Idle);
        }

        public void TakeDamage(float amount)
        {
            if (State == PawnState.Dead) return;
            data.TakeDamage(amount);
            if (!data.IsAlive) Die();
        }

        private void Die()
        {
            SetState(PawnState.Dead);
            if (_agent != null) _agent.enabled = false;
            Debug.Log($"[PawnController] {data.pawnName} 사망");
        }

        public void SetState(PawnState newState)
        {
            if (State == PawnState.Dead) return;
            State = newState;
        }

        public void SetAsCommander(bool isCommander)
        {
            data.isCommander = isCommander;
            Debug.Log($"[PawnController] {data.pawnName} 지휘관 지정: {isCommander}");
        }

        public bool IsCommander => data.isCommander;
        public bool IsAlive => data.IsAlive;
        public string PawnName => data.pawnName;
    }
}
