using UnityEngine;

namespace AbilitySystemTool
{
    [RequireComponent(typeof(AbilityTarget))]
    public sealed class AbilityRunner : MonoBehaviour
    {
        [SerializeField] private AbilitySO _abilitySO;
        [SerializeField] private AbilityTarget _currentAbilityTarget;
        private AbilityTarget _abilityTarget;

        [SerializeField] private EffectSO _debugRemoveEffect;
        [SerializeField] private bool _debugRemoveAllStacks = true;


        private void Awake()
        {
            _abilityTarget = GetComponent<AbilityTarget>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_abilitySO != null && _currentAbilityTarget != null)
                {
                    CastAbility(_abilitySO, _currentAbilityTarget);
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                DebugRemoveEffect();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                DebugRemoveBySource();
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
            abilitySystemComponent.ApplyAbility(_abilityTarget, abilitySO);
        }

        private void DebugRemoveEffect()
        {
            if (_debugRemoveEffect == null || _currentAbilityTarget == null) return;

            AbilitySystemComponent asc = _currentAbilityTarget.GetComponent<AbilitySystemComponent>();
            if (asc == null) return;

            int removed = asc.RemoveEffect(_debugRemoveEffect, RemoveReason.Debug, _debugRemoveAllStacks);
            Debug.Log($"[DEBUG] RemoveEffect {_debugRemoveEffect.name} removed={removed} allStacks={_debugRemoveAllStacks}");
        }

        private void DebugRemoveBySource()
        {
            if (_currentAbilityTarget == null || _abilityTarget == null) return;

            AbilitySystemComponent asc = _currentAbilityTarget.GetComponent<AbilitySystemComponent>();
            if (asc == null) return;

            int removed = asc.RemoveEffectsBySource(_abilityTarget, RemoveReason.SourceRemoved);
            Debug.Log($"[DEBUG] RemoveEffectsBySource source={_abilityTarget.name} removed={removed}");
        }

    }
}