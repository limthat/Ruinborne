using UnityEngine;
using UnityEngine.InputSystem;
using Ruinborne.Core;
using Ruinborne.Systems.PawnAI;
using Ruinborne.Systems.Economy;

namespace Ruinborne.Systems.Commander
{
    public class InteractionController : MonoBehaviour
    {
        [Header("설정")]
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private LayerMask pawnLayer;
        [SerializeField] private LayerMask resourceLayer;
        [SerializeField] private Camera playerCamera;

        private PawnController _focusedPawn;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // E키 — 폰 상호작용
            if (keyboard.eKey.wasPressedThisFrame)
                TryInteractWithPawn();

            // F키 — 자원 채취 지시
            if (keyboard.fKey.wasPressedThisFrame)
                TryAssignHarvest();

            // G키 — 집중 공격 지시
            if (keyboard.gKey.wasPressedThisFrame)
                TryAssignAttack();
        }

        private void TryInteractWithPawn()
        {
            Ray ray = playerCamera.ScreenPointToRay(
                new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, pawnLayer)) return;

            var pawn = hit.collider.GetComponent<PawnController>();
            if (pawn == null || !pawn.IsAlive) return;

            _focusedPawn = pawn;
            Debug.Log($"[InteractionController] 폰 선택: {pawn.PawnName}");

            // 컨텍스트 메뉴 이벤트 발행
            GameEventBus.Publish(new PawnInteractedEvent
            {
                PawnName = pawn.PawnName,
                Position = pawn.transform.position
            });
        }

        private void TryAssignHarvest()
        {
            Ray ray = playerCamera.ScreenPointToRay(
                new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, resourceLayer)) return;

            var resource = hit.collider.GetComponent<ResourceObject>();
            if (resource == null || resource.IsDeplete) return;

            // 가장 가까운 폰에게 채취 지시
            PawnController nearest = FindNearestAvailablePawn(hit.point);
            if (nearest == null)
            {
                Debug.Log("[InteractionController] 가용 폰 없음");
                return;
            }

            nearest.MoveTo(resource.transform.position);
            Debug.Log($"[InteractionController] {nearest.PawnName} → {resource.ResourceDef?.resourceName ?? "자원"} 채취 지시");

            GameEventBus.Publish(new PawnCommandIssuedEvent
            {
                PawnName = nearest.PawnName,
                CommandType = "Harvest",
                TargetPosition = resource.transform.position
            });
        }

        private void TryAssignAttack()
        {
            Ray ray = playerCamera.ScreenPointToRay(
                new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance)) return;

            Debug.Log($"[InteractionController] 집중 공격 지시: {hit.point}");

            GameEventBus.Publish(new PawnCommandIssuedEvent
            {
                PawnName = "Group",
                CommandType = "Attack",
                TargetPosition = hit.point
            });
        }

        private PawnController FindNearestAvailablePawn(Vector3 position)
        {
            PawnController[] allPawns = FindObjectsByType<PawnController>(FindObjectsSortMode.None);
            PawnController nearest = null;
            float minDist = float.MaxValue;

            foreach (var pawn in allPawns)
            {
                if (!pawn.IsAlive) continue;
                if (pawn.IsCommander) continue;
                if (pawn.State == PawnController.PawnState.MeltingDown) continue;

                float dist = Vector3.Distance(position, pawn.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = pawn;
                }
            }

            return nearest;
        }

        public PawnController FocusedPawn => _focusedPawn;
    }
}
