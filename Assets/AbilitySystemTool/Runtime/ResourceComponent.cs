using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    public class ResourceComponent : MonoBehaviour
    {
        [SerializeField] private List<ResourceEntry> _resourceList = new List<ResourceEntry>();

        public bool HasAndEnoughResource(ResourceIdSO resourceId, float costAmount)
        {
            if (resourceId == null) return false;
            if (costAmount < 0f) return false;
            if (costAmount == 0f) return true;

            for (int i = 0; i < _resourceList.Count; i++)
            {
                if (_resourceList[i].ResourceId == resourceId)
                {
                    return _resourceList[i].CurrentResourceAmount >= costAmount;
                }
            }

            return false; // resource not found
        }

        public bool TryConsume(ResourceIdSO resourceId, float costAmount, out float currentResourceAmount, out float maxResourceAmount)
        {
            currentResourceAmount = 0f;
            maxResourceAmount = 0f;

            if (resourceId == null) return false;
            if (costAmount < 0f) return false;
            if (costAmount == 0f) return true;

            for (int i = 0; i < _resourceList.Count; i++)
            {
                if (_resourceList[i].ResourceId != resourceId) continue;

                ResourceEntry tempResourceEntry = _resourceList[i];

                if (tempResourceEntry.CurrentResourceAmount < costAmount)
                {
                    currentResourceAmount = tempResourceEntry.CurrentResourceAmount;
                    maxResourceAmount = tempResourceEntry.MaxResourceCapacity;
                    return false;
                }

                tempResourceEntry.CurrentResourceAmount -= costAmount;
                _resourceList[i] = tempResourceEntry;

                currentResourceAmount = _resourceList[i].CurrentResourceAmount;
                maxResourceAmount = _resourceList[i].MaxResourceCapacity;
                return true;
            }

            return false; // resource not found
        }

        public bool TryGet(ResourceIdSO resourceId, out float currentResourceAmount, out float maxResourceAmount)
        {
            currentResourceAmount = 0f;
            maxResourceAmount = 0f;

            if (resourceId == null) return false;

            for (int i = 0; i < _resourceList.Count; i++)
            {
                if (_resourceList[i].ResourceId == resourceId)
                {
                    currentResourceAmount = _resourceList[i].CurrentResourceAmount;
                    maxResourceAmount = _resourceList[i].MaxResourceCapacity;
                    return true;
                }
            }

            return false;
        }

        [System.Serializable]
        private struct ResourceEntry
        {
            public ResourceIdSO ResourceId;
            public float CurrentResourceAmount;
            public float MaxResourceCapacity;
        }
    }
}