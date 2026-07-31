using UnityEngine;
using Ruinborne.Systems.PawnAI;

namespace Ruinborne.Systems.Commander
{
    public class CommanderPossession : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.7f, 0f);

        private PawnController _possessedPawn;
        private FirstPersonController _activeFPC;

        public void PossessByName(string pawnName)
        {
            PawnController[] allPawns = FindObjectsByType<PawnController>(
                FindObjectsSortMode.None);
            foreach (var pawn in allPawns)
            {
                if (pawn.PawnName == pawnName)
                {
                    PossessPawn(pawn);
                    break;
                }
            }
        }

        private void PossessPawn(PawnController pawn)
        {
            // 기존 빙의 해제
            if (_activeFPC != null)
                _activeFPC.enabled = false;

            _possessedPawn = pawn;

            // 폰의 FirstPersonController 활성화
            var fpc = pawn.GetComponent<FirstPersonController>();
            if (fpc != null)
            {
                fpc.enabled = true;
                _activeFPC = fpc;
            }

            // 카메라를 폰 자식으로 이동
            if (mainCamera != null)
            {
                mainCamera.transform.SetParent(pawn.transform);
                mainCamera.transform.localPosition = cameraOffset;
                mainCamera.transform.localRotation = Quaternion.identity;
                // 카메라 월드 회전 초기화
                mainCamera.transform.forward = pawn.transform.forward;
            }

            // 폰 AI 비활성화
            var goapAgent = pawn.GetComponent<GoapAgent>();
            if (goapAgent != null) goapAgent.enabled = false;

            var scheduler = pawn.GetComponent<PawnScheduler>();
            if (scheduler != null) scheduler.enabled = false;

            var navAgent = pawn.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null) navAgent.enabled = false;

            // CharacterController 위치를 NavMesh 위로 보정
            var cc = pawn.GetComponent<CharacterController>();
            if (cc != null)
            {
                // 현재 위치에서 아래로 레이캐스트해서 NavMesh 위에 올려놓기
                if (UnityEngine.AI.NavMesh.SamplePosition(
                    pawn.transform.position, out UnityEngine.AI.NavMeshHit hit, 2f,
                    UnityEngine.AI.NavMesh.AllAreas))
                {
                    cc.enabled = false;
                    pawn.transform.position = hit.position + Vector3.up * 0.1f;
                    cc.enabled = true;
                }
            }

            Debug.Log($"[CommanderPossession] {pawn.PawnName} 빙의 완료");
        }

        public void UnpossessPawn()
        {
            if (_possessedPawn == null) return;

            if (_activeFPC != null)
            {
                _activeFPC.enabled = false;
                _activeFPC = null;
            }

            if (mainCamera != null)
                mainCamera.transform.SetParent(null);

            var goapAgent = _possessedPawn.GetComponent<GoapAgent>();
            if (goapAgent != null) goapAgent.enabled = true;

            var scheduler = _possessedPawn.GetComponent<PawnScheduler>();
            if (scheduler != null) scheduler.enabled = true;

            _possessedPawn = null;
            Debug.Log("[CommanderPossession] 빙의 해제");
        }

        public PawnController PossessedPawn => _possessedPawn;
    }
}
