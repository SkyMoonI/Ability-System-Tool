using UnityEngine;

namespace AbilitySystemTool
{
    public sealed class AbilityRunner : MonoBehaviour
    {
        [SerializeField] private AbilitySO _abilitySO;
        [SerializeField] private AbilityTarget _currentAbilityTarget;

        private void Update()
        {
            if (_abilitySO != null && _currentAbilityTarget != null && Input.GetKeyDown(KeyCode.Space))
            {
                CastAbility(_abilitySO, _currentAbilityTarget);
            }
        }

        public void CastAbility(AbilitySO abilitySO, AbilityTarget abilityTarget)
        {
            if (abilitySO == null || abilityTarget == null) return;

            AbilitySystemComponent abilitySystemComponent = abilityTarget.GetComponent<AbilitySystemComponent>();
            if (abilitySystemComponent == null)
            {
                Debug.LogWarning($"[CAST FAIL] Target has no AbilitySystemComponent: {abilityTarget.name}");
                return;
            }

            Debug.Log($"[CAST] {abilitySO.name} (target={abilityTarget.name})");
            abilitySystemComponent.ApplyAbility(abilitySO);
        }
    }
}