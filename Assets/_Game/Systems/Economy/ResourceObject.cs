using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.Economy
{
    public class ResourceObject : MonoBehaviour
    {
        [Header("자원 설정 (ScriptableObject)")]
        [SerializeField] private ResourceDef resourceDef;

        [Header("수량")]
        [SerializeField] private int amount = 10;
        [SerializeField] private int maxAmount = 10;

        [Header("채취 설정")]
        [SerializeField] private float harvestTime = 2f;

        public ResourceDef ResourceDef => resourceDef;
        public int Amount => amount;
        public bool IsDeplete => amount <= 0;

        private void Awake()
        {
            amount = maxAmount;
        }

        public int Harvest(int requestAmount)
        {
            int harvested = Mathf.Min(requestAmount, amount);
            amount -= harvested;

            GameEventBus.Publish(new ResourceHarvestedEvent
            {
                Type = resourceDef != null ? resourceDef.resourceType : ResourceType.RawWood,
                Amount = harvested,
                Position = transform.position
            });

            Debug.Log($"[ResourceObject] {resourceDef?.resourceName ?? "Unknown"} 채취: {harvested}개 (남은: {amount})");

            if (IsDeplete)
            {
                Debug.Log($"[ResourceObject] {resourceDef?.resourceName ?? "Unknown"} 고갈");
                Destroy(gameObject);
            }

            return harvested;
        }

        public float GetHarvestTime() => harvestTime;
    }
}
