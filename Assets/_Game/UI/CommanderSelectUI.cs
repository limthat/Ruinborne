using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Ruinborne.Core;
using Ruinborne.Systems.PawnAI;

namespace Ruinborne.UI
{
    public class CommanderSelectUI : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private PawnSpawner pawnSpawner;

        private VisualElement _root;
        private List<Button> _pawnButtons = new List<Button>();

        private void OnEnable()
        {
            GameEventBus.Subscribe<NavMeshBakedEvent>(OnNavMeshBaked);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<NavMeshBakedEvent>(OnNavMeshBaked);
        }

        private void OnNavMeshBaked(NavMeshBakedEvent evt)
        {
            // 폰 스폰 후 UI 표시
            Invoke(nameof(ShowUI), 0.5f);
        }

        private void ShowUI()
        {
            if (uiDocument == null) return;
            _root = uiDocument.rootVisualElement;
            _root.style.display = DisplayStyle.Flex;

            BuildPawnList();
        }

        private void BuildPawnList()
        {
            if (pawnSpawner == null) return;

            var container = _root.Q<VisualElement>("pawn-select-container");
            if (container == null) return;

            container.Clear();
            _pawnButtons.Clear();

            var pawns = pawnSpawner.AllPawns;
            for (int i = 0; i < pawns.Count; i++)
            {
                int index = i;
                var pawn = pawns[i];

                var btn = new Button(() => OnPawnSelected(index));
                btn.text = $"{pawn.PawnName} ({pawn.data.raceType})";
                btn.AddToClassList("pawn-select-btn");
                container.Add(btn);
                _pawnButtons.Add(btn);
            }
        }

        private void OnPawnSelected(int index)
        {
            if (pawnSpawner == null) return;
            pawnSpawner.SetCommander(index);

            Debug.Log($"[CommanderSelectUI] 지휘관 선택: {index}번 폰");

            // UI 숨기기
            _root.style.display = DisplayStyle.None;

            // 지휘관 빙의 이벤트 발행
            var commander = pawnSpawner.Commander;
            if (commander != null)
                GameEventBus.Publish(new CommanderPossessedEvent
                {
                    PawnName = commander.PawnName,
                    Position = commander.transform.position
                });
        }
    }
}
