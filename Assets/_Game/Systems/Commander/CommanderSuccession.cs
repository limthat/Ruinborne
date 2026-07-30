using UnityEngine;
using System.Collections.Generic;
using Ruinborne.Core;
using Ruinborne.Systems.PawnAI;

namespace Ruinborne.Systems.Commander
{
    public class CommanderSuccession : MonoBehaviour
    {
        [SerializeField] private PawnSpawner pawnSpawner;
        [SerializeField] private CommanderPossession commanderPossession;

        private PawnController _designatedSuccessor;

        private void OnEnable()
        {
            GameEventBus.Subscribe<PawnDiedEvent>(OnPawnDied);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<PawnDiedEvent>(OnPawnDied);
        }

        private void OnPawnDied(PawnDiedEvent evt)
        {
            if (pawnSpawner == null) return;
            var commander = pawnSpawner.Commander;
            if (commander == null) return;
            if (commander.PawnName != evt.PawnName) return;

            Debug.Log($"[CommanderSuccession] 지휘관 {evt.PawnName} 사망 — 계승 시작");
            StartSuccession();
        }

        private void StartSuccession()
        {
            // 1. 지정된 계승자 확인
            if (_designatedSuccessor != null && _designatedSuccessor.IsAlive)
            {
                Succeed(_designatedSuccessor);
                return;
            }

            // 2. 자동 계승 — 살아있는 폰 중 첫 번째
            var alivePawns = GetAlivePawns();
            if (alivePawns.Count == 0)
            {
                Debug.Log("[CommanderSuccession] 살아있는 폰 없음 — 게임 오버");
                GameEventBus.Publish(new GameOverEvent { Reason = "모든 폰 사망" });
                return;
            }

            Succeed(alivePawns[0]);
        }

        private void Succeed(PawnController newCommander)
        {
            pawnSpawner.SetCommanderByController(newCommander);
            commanderPossession?.UnpossessPawn();

            GameEventBus.Publish(new CommanderPossessedEvent
            {
                PawnName = newCommander.PawnName,
                Position = newCommander.transform.position
            });

            Debug.Log($"[CommanderSuccession] 계승 완료: {newCommander.PawnName}");
        }

        private List<PawnController> GetAlivePawns()
        {
            var result = new List<PawnController>();
            if (pawnSpawner == null) return result;

            foreach (var pawn in pawnSpawner.AllPawns)
                if (pawn != null && pawn.IsAlive && !pawn.IsCommander)
                    result.Add(pawn);

            return result;
        }

        public void SetDesignatedSuccessor(PawnController pawn)
        {
            _designatedSuccessor = pawn;
            Debug.Log($"[CommanderSuccession] 계승자 지정: {pawn?.PawnName ?? "없음"}");
        }

        public PawnController DesignatedSuccessor => _designatedSuccessor;
    }
}
