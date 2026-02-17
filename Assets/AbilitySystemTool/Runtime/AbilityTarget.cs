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

        private void Awake()
        {
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
            if (statsComponent == null) statsComponent = GetComponent<StatsComponent>();
        }

        private void Reset()
        {
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
            if (statsComponent == null) statsComponent = GetComponent<StatsComponent>();
        }
    }
}