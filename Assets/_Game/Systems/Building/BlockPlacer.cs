using UnityEngine;
using UnityEngine.InputSystem;
using Ruinborne.Core;
using Ruinborne.Systems.Grid;

namespace Ruinborne.Systems.Building
{
    public class BlockPlacer : MonoBehaviour
    {
        [Header("배치 설정")]
        [SerializeField] private float placeDistance = 5f;
        [SerializeField] private LayerMask placementLayer;
        [SerializeField] private LayerMask blockLayer;

        [Header("블록 프리팹")]
        [SerializeField] private GameObject woodBlockPrefab;
        [SerializeField] private GameObject stoneBlockPrefab;
        [SerializeField] private GameObject metalBlockPrefab;
        [SerializeField] private GameObject arcaneBlockPrefab;
        [SerializeField] private GameObject glassBlockPrefab;
        [SerializeField] private GameObject doorBlockPrefab;

        [Header("프리뷰")]
        [SerializeField] private GameObject previewObject;
        [SerializeField] private Material previewMaterialValid;
        [SerializeField] private Material previewMaterialInvalid;

        [Header("참조")]
        [SerializeField] private Camera playerCamera;

        private BlockType _selectedBlockType = BlockType.Wood;
        private bool _isBuildMode = false;
        private bool _isDragMode = false;
        private Vector3 _dragStartPos;
        private GridManager _gridManager;

        private void Start()
        {
            _gridManager = ServiceLocator.Get<GridManager>();
        }

        private void Update()
        {
            if (!_isBuildMode) return;

            UpdatePreview();
            HandleInput();
        }

        private void UpdatePreview()
        {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, placeDistance, placementLayer))
            {
                Vector3 snappedPos = SnapToGrid(hit.point, hit.normal);
                if (previewObject != null)
                {
                    previewObject.SetActive(true);
                    previewObject.transform.position = snappedPos;
                }
            }
            else
            {
                if (previewObject != null)
                    previewObject.SetActive(false);
            }
        }

        private void HandleInput()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            // 블록 배치 (좌클릭)
            if (mouse.leftButton.wasPressedThisFrame)
            {
                if (keyboard.leftShiftKey.isPressed)
                    StartDrag();
                else
                    PlaceSingleBlock();
            }

            // 드래그 종료 (좌클릭 해제)
            if (mouse.leftButton.wasReleasedThisFrame && _isDragMode)
                EndDrag();

            // 블록 철거 (우클릭)
            if (mouse.rightButton.wasPressedThisFrame)
                RemoveBlock();

            // 건축 모드 토글 (B키)
            if (keyboard.bKey.wasPressedThisFrame)
                ToggleBuildMode();

            // 블록 타입 선택 (숫자키)
            if (keyboard.digit1Key.wasPressedThisFrame) SelectBlock(BlockType.Wood);
            if (keyboard.digit2Key.wasPressedThisFrame) SelectBlock(BlockType.Stone);
            if (keyboard.digit3Key.wasPressedThisFrame) SelectBlock(BlockType.Metal);
            if (keyboard.digit4Key.wasPressedThisFrame) SelectBlock(BlockType.Arcane);
            if (keyboard.digit5Key.wasPressedThisFrame) SelectBlock(BlockType.Glass);
            if (keyboard.digit6Key.wasPressedThisFrame) SelectBlock(BlockType.Door);
        }

        private void PlaceSingleBlock()
        {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, placeDistance, placementLayer)) return;

            Vector3 snappedPos = SnapToGrid(hit.point, hit.normal);
            GameObject prefab = GetPrefabForBlockType(_selectedBlockType);
            if (prefab == null) return;

            Instantiate(prefab, snappedPos, Quaternion.identity);
            GameEventBus.Publish(new BlockPlacedEvent
            {
                Position = snappedPos,
                BlockType = _selectedBlockType
            });
            Debug.Log($"[BlockPlacer] 블록 배치: {_selectedBlockType} at {snappedPos}");
        }

        private void StartDrag()
        {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, placeDistance, placementLayer)) return;
            _dragStartPos = SnapToGrid(hit.point, hit.normal);
            _isDragMode = true;
            Debug.Log($"[BlockPlacer] 드래그 시작: {_dragStartPos}");
        }

        private void EndDrag()
        {
            _isDragMode = false;
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, placeDistance, placementLayer)) return;

            Vector3 dragEndPos = SnapToGrid(hit.point, hit.normal);
            PlaceBlocksInRange(_dragStartPos, dragEndPos);
        }

        private void PlaceBlocksInRange(Vector3 start, Vector3 end)
        {
            GameObject prefab = GetPrefabForBlockType(_selectedBlockType);
            if (prefab == null) return;

            int minX = Mathf.Min(Mathf.RoundToInt(start.x), Mathf.RoundToInt(end.x));
            int maxX = Mathf.Max(Mathf.RoundToInt(start.x), Mathf.RoundToInt(end.x));
            int minY = Mathf.Min(Mathf.RoundToInt(start.y), Mathf.RoundToInt(end.y));
            int maxY = Mathf.Max(Mathf.RoundToInt(start.y), Mathf.RoundToInt(end.y));
            int minZ = Mathf.Min(Mathf.RoundToInt(start.z), Mathf.RoundToInt(end.z));
            int maxZ = Mathf.Max(Mathf.RoundToInt(start.z), Mathf.RoundToInt(end.z));

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        Instantiate(prefab, pos, Quaternion.identity);
                    }

            Debug.Log($"[BlockPlacer] 드래그 배치 완료: {start} ~ {end}");
        }

        private void RemoveBlock()
        {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, placeDistance, blockLayer)) return;

            GameEventBus.Publish(new BlockRemovedEvent { Position = hit.transform.position });
            Destroy(hit.transform.gameObject);
            Debug.Log($"[BlockPlacer] 블록 철거: {hit.transform.position}");
        }

        private Vector3 SnapToGrid(Vector3 hitPoint, Vector3 hitNormal)
        {
            Vector3 pos = hitPoint + hitNormal * 0.5f;
            return new Vector3(
                Mathf.RoundToInt(pos.x),
                Mathf.RoundToInt(pos.y),
                Mathf.RoundToInt(pos.z)
            );
        }

        private void ToggleBuildMode()
        {
            _isBuildMode = !_isBuildMode;
            if (previewObject != null)
                previewObject.SetActive(_isBuildMode);
            Debug.Log($"[BlockPlacer] 건축 모드: {_isBuildMode}");
        }

        private void SelectBlock(BlockType blockType)
        {
            _selectedBlockType = blockType;
            Debug.Log($"[BlockPlacer] 선택된 블록: {blockType}");
        }

        private GameObject GetPrefabForBlockType(BlockType blockType)
        {
            return blockType switch
            {
                BlockType.Wood   => woodBlockPrefab,
                BlockType.Stone  => stoneBlockPrefab,
                BlockType.Metal  => metalBlockPrefab,
                BlockType.Arcane => arcaneBlockPrefab,
                BlockType.Glass  => glassBlockPrefab,
                BlockType.Door   => doorBlockPrefab,
                _                => woodBlockPrefab
            };
        }

        public BlockType SelectedBlockType => _selectedBlockType;
        public bool IsBuildMode => _isBuildMode;
    }
}
