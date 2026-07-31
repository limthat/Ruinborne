using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Ruinborne.Systems.PawnAI;
using Ruinborne.Systems.Commander;

namespace Ruinborne.UI
{
    public class CommanderSelectUI : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private PawnSpawner pawnSpawner;
        [SerializeField] private CommanderPossession commanderPossession;

        private VisualElement _root;
        private List<Button> _pawnButtons = new List<Button>();

        private void Start()
        {
            // NavMeshBake + PawnSpawn 완료 후 실행 (2초 딜레이)
            Invoke(nameof(ShowUI), 2f);
        }

        private void ShowUI()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            if (uiDocument == null) return;
            _root = uiDocument.rootVisualElement;
            _root.style.display = DisplayStyle.Flex;

            BuildPawnList();
        }

        private void BuildPawnList()
        {
            if (pawnSpawner == null)
            {
                Debug.LogError("[CommanderSelectUI] PawnSpawner 없음");
                return;
            }

            var container = _root.Q<VisualElement>("pawn-select-container");
            if (container == null)
            {
                Debug.LogError("[CommanderSelectUI] pawn-select-container 없음");
                return;
            }

            var pawns = pawnSpawner.AllPawns;
            Debug.Log($"[CommanderSelectUI] 폰 수: {pawns.Count}");

            container.Clear();
            _pawnButtons.Clear();

            for (int i = 0; i < pawns.Count; i++)
            {
                int index = i;
                var pawn = pawns[i];
                var btn = new Button(() => OnPawnSelected(index));
                btn.text = $"{pawn.PawnName} ({pawn.data.raceType})";
                container.Add(btn);
                _pawnButtons.Add(btn);
                Debug.Log($"[CommanderSelectUI] 버튼 추가: {pawn.PawnName}");
            }
        }

        private void OnPawnSelected(int index)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;

            if (pawnSpawner == null) return;
            pawnSpawner.SetCommander(index);

            Debug.Log($"[CommanderSelectUI] 지휘관 선택: {index}번 폰");

            // UI 숨기기
            _root.style.display = DisplayStyle.None;

            // 지휘관 빙의 직접 호출
            var commander = pawnSpawner.Commander;
            if (commander != null && commanderPossession != null)
                commanderPossession.PossessByName(commander.PawnName);
        }
    }
}
