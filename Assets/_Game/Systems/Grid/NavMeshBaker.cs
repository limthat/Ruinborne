using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Ruinborne.Core;

namespace Ruinborne.Systems.Grid
{
    public class NavMeshBaker : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navMeshSurface;

        private void Start()
        {
            GameEventBus.Subscribe<MapGeneratedEvent>(OnMapGenerated);
        }

        private void OnDestroy()
        {
            GameEventBus.Unsubscribe<MapGeneratedEvent>(OnMapGenerated);
        }

        private void OnMapGenerated(MapGeneratedEvent evt)
        {
            Invoke(nameof(BakeNavMesh), 0.5f);
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
