using UnityEngine;
using System.Collections.Generic;
using Ruinborne.Core;
using Ruinborne.Data;

namespace Ruinborne.Systems.PawnAI
{
    public class PawnSpawner : ManagerBase<PawnSpawner>
    {
        [Header("폰 프리팹")]
        [SerializeField] private GameObject pawnPrefab;

        [Header("시작 설정")]
        [SerializeField] private int startingPawnCount = 3;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private RaceType startingRace = RaceType.Human;

        [Header("지휘관 설정")]
        [SerializeField] private int commanderIndex = 0; // 0번 폰이 기본 지휘관

        private List<PawnController> _allPawns = new List<PawnController>();
        private PawnController _commander;

        private void Start()
        {
            GameEventBus.Subscribe<NavMeshBakedEvent>(OnNavMeshBaked);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEventBus.Unsubscribe<NavMeshBakedEvent>(OnNavMeshBaked);
        }

        private void OnNavMeshBaked(NavMeshBakedEvent evt)
        {
            SpawnStartingPawns();
        }

        private void SpawnStartingPawns()
        {
            if (pawnPrefab == null)
            {
                Debug.LogError("[PawnSpawner] 폰 프리팹이 없음");
                return;
            }

            for (int i = 0; i < startingPawnCount; i++)
            {
                Vector3 spawnPos = GetSpawnPosition(i);
                GameObject pawnObj = Instantiate(pawnPrefab, spawnPos, Quaternion.identity);
                pawnObj.name = $"Pawn_{i}";

                PawnController controller = pawnObj.GetComponent<PawnController>();
                if (controller == null) continue;

                // 폰 데이터 초기화
                controller.data.pawnName = GeneratePawnName(i);
                controller.data.raceType = startingRace;

                _allPawns.Add(controller);
                Debug.Log($"[PawnSpawner] 폰 생성: {controller.data.pawnName} at {spawnPos}");
            }

            // 지휘관 지정
            if (_allPawns.Count > commanderIndex)
                SetCommander(commanderIndex);
        }

        private Vector3 GetSpawnPosition(int index)
        {
            if (spawnPoints != null && index < spawnPoints.Length && spawnPoints[index] != null)
                return spawnPoints[index].position;

            // 스폰 포인트 없으면 원형 배치 — Y를 1.5f로 올려서 타일 위에 스폰
            float angle = index * (360f / startingPawnCount) * Mathf.Deg2Rad;
            return transform.position + new Vector3(
                Mathf.Cos(angle) * 1.5f, 1.5f, Mathf.Sin(angle) * 1.5f);
        }

        public void SetCommander(int pawnIndex)
        {
            if (pawnIndex < 0 || pawnIndex >= _allPawns.Count) return;

            // 기존 지휘관 해제
            if (_commander != null)
                _commander.SetAsCommander(false);

            _commander = _allPawns[pawnIndex];
            _commander.SetAsCommander(true);

            GameEventBus.Publish(new CommanderAssignedEvent
            {
                PawnName = _commander.PawnName,
                PawnIndex = pawnIndex
            });

            Debug.Log($"[PawnSpawner] 지휘관 지정: {_commander.PawnName} (index {pawnIndex})");
        }

        public void SetCommanderByController(PawnController pawn)
        {
            int index = _allPawns.IndexOf(pawn);
            if (index >= 0) SetCommander(index);
        }

        private string GeneratePawnName(int index)
        {
            string[] names = {
                "아론", "베른", "카일", "도린", "에린",
                "파론", "가렛", "헤린", "이든", "자린"
            };
            return index < names.Length ? names[index] : $"폰_{index}";
        }

        public PawnController Commander => _commander;
        public List<PawnController> AllPawns => _allPawns;
        public int PawnCount => _allPawns.Count;
    }
}
