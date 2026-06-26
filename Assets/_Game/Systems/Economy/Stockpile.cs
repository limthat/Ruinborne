using UnityEngine;
using System.Collections.Generic;
using Ruinborne.Core;
using Ruinborne.Data;

namespace Ruinborne.Systems.Economy
{
    public class Stockpile : MonoBehaviour
    {
        [Header("창고 설정")]
        [SerializeField] private float radius = 3f;
        [SerializeField] private List<ResourceType> allowedTypes = new List<ResourceType>();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public bool AcceptsType(ResourceType type)
        {
            if (allowedTypes == null || allowedTypes.Count == 0) return true;
            return allowedTypes.Contains(type);
        }

        public float Radius => radius;
    }
}
