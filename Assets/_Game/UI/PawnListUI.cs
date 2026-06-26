using UnityEngine;
using UnityEngine.UIElements;
using Ruinborne.Core;

namespace Ruinborne.UI
{
    public class PawnListUI : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement _root;
        private VisualElement _pawnListContainer;
        private VisualElement _commandPanel;
        private Label _selectedGroupLabel;

        private int _selectedGroup = -1;

        private void OnEnable()
        {
            if (uiDocument == null) return;
            _root = uiDocument.rootVisualElement;

            _pawnListContainer = _root.Q<VisualElement>("pawn-list");
            _commandPanel = _root.Q<VisualElement>("command-panel");
            _selectedGroupLabel = _root.Q<Label>("selected-group-label");

            BindCommandButtons();
            GameEventBus.Subscribe<PawnSpawnedEvent>(OnPawnSpawned);
            GameEventBus.Subscribe<PawnDiedEvent>(OnPawnDied);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<PawnSpawnedEvent>(OnPawnSpawned);
            GameEventBus.Unsubscribe<PawnDiedEvent>(OnPawnDied);
        }

        private void Update()
        {
            // 그룹 단축키 (1~4번)
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.digit1Key.wasPressedThisFrame) SelectGroup(1);
            if (keyboard.digit2Key.wasPressedThisFrame) SelectGroup(2);
            if (keyboard.digit3Key.wasPressedThisFrame) SelectGroup(3);
            if (keyboard.digit4Key.wasPressedThisFrame) SelectGroup(4);
        }

        private void BindCommandButtons()
        {
            _root.Q<Button>("btn-attack")?.RegisterCallback<ClickEvent>(_ => OnCommandAttack());
            _root.Q<Button>("btn-retreat")?.RegisterCallback<ClickEvent>(_ => OnCommandRetreat());
            _root.Q<Button>("btn-charge")?.RegisterCallback<ClickEvent>(_ => OnCommandCharge());
            _root.Q<Button>("btn-defend")?.RegisterCallback<ClickEvent>(_ => OnCommandDefend());
            _root.Q<Button>("btn-follow")?.RegisterCallback<ClickEvent>(_ => OnCommandFollow());
        }

        private void SelectGroup(int groupIndex)
        {
            _selectedGroup = groupIndex;
            if (_selectedGroupLabel != null)
                _selectedGroupLabel.text = $"그룹 {groupIndex} 선택됨";
            Debug.Log($"[PawnListUI] 그룹 {groupIndex} 선택");
            GameEventBus.Publish(new PawnGroupSelectedEvent { GroupIndex = groupIndex });
        }

        private void OnCommandAttack()
        {
            Debug.Log($"[PawnListUI] 명령: 집중 공격 (그룹 {_selectedGroup})");
            GameEventBus.Publish(new PawnCommandEvent { Command = PawnCommand.Attack, GroupIndex = _selectedGroup });
        }

        private void OnCommandRetreat()
        {
            Debug.Log($"[PawnListUI] 명령: 후퇴 (그룹 {_selectedGroup})");
            GameEventBus.Publish(new PawnCommandEvent { Command = PawnCommand.Retreat, GroupIndex = _selectedGroup });
        }

        private void OnCommandCharge()
        {
            Debug.Log($"[PawnListUI] 명령: 돌격 (그룹 {_selectedGroup})");
            GameEventBus.Publish(new PawnCommandEvent { Command = PawnCommand.Charge, GroupIndex = _selectedGroup });
        }

        private void OnCommandDefend()
        {
            Debug.Log($"[PawnListUI] 명령: 방어 태세 (그룹 {_selectedGroup})");
            GameEventBus.Publish(new PawnCommandEvent { Command = PawnCommand.Defend, GroupIndex = _selectedGroup });
        }

        private void OnCommandFollow()
        {
            Debug.Log($"[PawnListUI] 명령: 따라오기 (그룹 {_selectedGroup})");
            GameEventBus.Publish(new PawnCommandEvent { Command = PawnCommand.Follow, GroupIndex = _selectedGroup });
        }

        private void OnPawnSpawned(PawnSpawnedEvent evt)
        {
            Debug.Log($"[PawnListUI] 폰 추가: {evt.PawnName}");
            // 추후 폰 목록 UI 갱신
        }

        private void OnPawnDied(PawnDiedEvent evt)
        {
            Debug.Log($"[PawnListUI] 폰 제거: {evt.PawnName}");
            // 추후 폰 목록 UI 갱신
        }
    }
}
