using UnityEngine;
using System.Collections.Generic;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.Economy
{
    public class ResourceManager : ManagerBase<ResourceManager>
    {
        private Dictionary<ResourceType, int> _stockpile = new Dictionary<ResourceType, int>();

        protected override void Awake()
        {
            base.Awake();
            InitializeStockpile();
            GameEventBus.Subscribe<ResourceHarvestedEvent>(OnResourceHarvested);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEventBus.Unsubscribe<ResourceHarvestedEvent>(OnResourceHarvested);
        }

        private void InitializeStockpile()
        {
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
                _stockpile[type] = 0;
        }

        private void OnResourceHarvested(ResourceHarvestedEvent evt)
        {
            AddResource(evt.Type, evt.Amount);
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (!_stockpile.ContainsKey(type))
                _stockpile[type] = 0;
            _stockpile[type] += amount;

            GameEventBus.Publish(new ResourceChangedEvent
            {
                Type = type,
                Amount = _stockpile[type],
                Delta = amount
            });

            Debug.Log($"[ResourceManager] {type} +{amount} → 총 {_stockpile[type]}");
        }

        public bool ConsumeResource(ResourceType type, int amount)
        {
            if (!HasResource(type, amount)) return false;
            _stockpile[type] -= amount;

            GameEventBus.Publish(new ResourceChangedEvent
            {
                Type = type,
                Amount = _stockpile[type],
                Delta = -amount
            });

            Debug.Log($"[ResourceManager] {type} -{amount} → 총 {_stockpile[type]}");
            return true;
        }

        public bool HasResource(ResourceType type, int amount)
        {
            return _stockpile.TryGetValue(type, out int current) && current >= amount;
        }

        public int GetAmount(ResourceType type)
        {
            return _stockpile.TryGetValue(type, out int amount) ? amount : 0;
        }

        public IReadOnlyDictionary<ResourceType, int> GetStockpile() => _stockpile;
    }
}
