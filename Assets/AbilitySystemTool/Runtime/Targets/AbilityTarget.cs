using UnityEngine;

namespace AbilitySystemTool
{
    [RequireComponent(typeof(HealthComponent))]
    public class AbilityTarget : MonoBehaviour
    {
        [SerializeField] private HealthComponent healthComponent;
        public HealthComponent HealthComponent => healthComponent;

        [SerializeField] private StatsComponent statsComponent;
        public StatsComponent StatsComponent => statsComponent;

        [SerializeField] private ResourceComponent resourceComponent;
        public ResourceComponent ResourceComponent => resourceComponent;

        private void Awake()
        {
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
            if (statsComponent == null) statsComponent = GetComponent<StatsComponent>();
            if (resourceComponent == null) resourceComponent = GetComponent<ResourceComponent>();
        }

        private void Reset()
        {
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
            if (statsComponent == null) statsComponent = GetComponent<StatsComponent>();
            if (resourceComponent == null) resourceComponent = GetComponent<ResourceComponent>();
        }
    }
}