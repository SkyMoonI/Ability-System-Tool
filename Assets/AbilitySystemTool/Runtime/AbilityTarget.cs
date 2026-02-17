using UnityEngine;

namespace AbilitySystemTool
{
    [RequireComponent(typeof(HealthComponent))]
    public class AbilityTarget : MonoBehaviour
    {
        [SerializeField] private HealthComponent healthComponent;
        public HealthComponent HealthComponent => healthComponent;

        private void Awake()
        {
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
        }

        private void Reset()
        {
            if (healthComponent == null) healthComponent = GetComponent<HealthComponent>();
        }
    }
}