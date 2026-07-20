using Unity.AI.Navigation;
using UnityEngine;
using Ruinborne.Core;

namespace Ruinborne.Systems.Grid
{
    public class NavMeshBaker : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private float bakeDelay = 1f;

        private void Start()
        {
            // 이벤트 방식 대신 일정 시간 후 무조건 베이크
            Invoke(nameof(BakeNavMesh), bakeDelay);
        }

        private void BakeNavMesh()
        {
            if (navMeshSurface == null)
            {
                Debug.LogError("[NavMeshBaker] NavMeshSurface가 없음");
                return;
            }
            navMeshSurface.BuildNavMesh();
            Debug.Log("[NavMeshBaker] NavMesh 베이크 완료");
            GameEventBus.Publish(new NavMeshBakedEvent());
        }
    }
}
