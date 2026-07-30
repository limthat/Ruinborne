using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Systems.PawnAI;

namespace Ruinborne.Systems.Commander
{
    public class CommanderPossession : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private FirstPersonController firstPersonController;
        [SerializeField] private Camera playerCamera;

        private PawnController _possessedPawn;

        private void OnEnable()
        {
            GameEventBus.Subscribe<CommanderPossessedEvent>(OnCommanderPossessed);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<CommanderPossessedEvent>(OnCommanderPossessed);
        }

        private void OnCommanderPossessed(CommanderPossessedEvent evt)
        {
            PawnController[] allPawns = FindObjectsByType<PawnController>(FindObjectsSortMode.None);
            foreach (var pawn in allPawns)
            {
                if (pawn.PawnName == evt.PawnName)
                {
                    PossessPawn(pawn);
                    break;
                }
            }
        }

        private void PossessPawn(PawnController pawn)
        {
            _possessedPawn = pawn;

            if (firstPersonController != null)
            {
                firstPersonController.transform.position = pawn.transform.position;
                firstPersonController.enabled = true;
            }

            if (playerCamera != null)
            {
                playerCamera.transform.SetParent(firstPersonController.transform);
                playerCamera.transform.localPosition = new Vector3(0f, 1.7f, 0f);
                playerCamera.transform.localRotation = Quaternion.identity;
            }

            var goapAgent = pawn.GetComponent<GoapAgent>();
            if (goapAgent != null) goapAgent.enabled = false;

            var scheduler = pawn.GetComponent<PawnScheduler>();
            if (scheduler != null) scheduler.enabled = false;

            Debug.Log($"[CommanderPossession] {pawn.PawnName} 빙의 완료");
        }

        public void UnpossessPawn()
        {
            if (_possessedPawn == null) return;

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
