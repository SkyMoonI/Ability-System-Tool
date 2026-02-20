using UnityEngine;

namespace AbilitySystemTool
{
    [RequireComponent(typeof(AbilityTarget), typeof(AbilitySystemComponent))]
    public sealed class AbilityRunner : MonoBehaviour
    {
        [SerializeField] private AbilitySO _abilitySO;
        [SerializeField] private AbilityTarget _currentAbilityTarget;

        private AbilityTarget _casterAbilityTarget;
        private AbilitySystemComponent _casterAbilitySystemComponent;

        [Header("Debug")]
        [SerializeField] private EffectSO _debugRemoveEffect;
        [SerializeField] private bool _debugRemoveAllStacks = true;


        private void Awake()
        {
            _casterAbilityTarget = GetComponent<AbilityTarget>();
            _casterAbilitySystemComponent = GetComponent<AbilitySystemComponent>();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Assert(_casterAbilitySystemComponent != null,
                "[AbilityRunner] Missing AbilitySystemComponent (RequireComponent should prevent this).",
                this);
#endif
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

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (_currentAbilityTarget != null &&
                        _currentAbilityTarget.TryGetComponent(out AbilitySystemComponent targetAsc))
                {
                    DebugCapacity(targetAsc);
                }
            }
#endif
        }

        public void CastAbility(AbilitySO abilitySO, AbilityTarget abilityTarget)
        {
            if (abilitySO == null || abilityTarget == null) return;

            bool castResult = _casterAbilitySystemComponent.TryCastAbility(abilitySO, abilityTarget, out CastFailReason reason);

            if (!castResult && reason == CastFailReason.OnCooldown)
            {
                float remaining = _casterAbilitySystemComponent.GetRemainingCooldown(abilitySO);
                DemoLogger.Warn($"[CAST FAIL] {abilitySO.name} (target={abilityTarget.name}, reason={reason}, remaining={remaining:0.00}s)");
                return;
            }

            DemoLogger.Log($"[CAST] {abilitySO.name} (target={abilityTarget.name}, reason={reason})");
        }

        private void DebugRemoveEffect()
        {
            if (_debugRemoveEffect == null || _currentAbilityTarget == null) return;

            AbilitySystemComponent asc = _currentAbilityTarget.GetComponent<AbilitySystemComponent>();
            if (asc == null) return;

            int removed = asc.RemoveEffect(_debugRemoveEffect, RemoveReason.Debug, _debugRemoveAllStacks);
            DemoLogger.Log($"[DEBUG] RemoveEffect {_debugRemoveEffect.name} removed={removed} allStacks={_debugRemoveAllStacks}");
        }

        private void DebugRemoveBySource()
        {
            if (_currentAbilityTarget == null || _casterAbilityTarget == null) return;

            AbilitySystemComponent asc = _currentAbilityTarget.GetComponent<AbilitySystemComponent>();
            if (asc == null) return;

            int removed = asc.RemoveEffectsBySource(_casterAbilityTarget, RemoveReason.SourceRemoved);
            DemoLogger.Log($"[DEBUG] RemoveEffectsBySource source={_casterAbilityTarget.name} removed={removed}");
        }

#if UNITY_EDITOR
        private void DebugCapacity(AbilitySystemComponent abilitySystemComponent)
        {
            Debug.Log($"ActiveEffectCount: {abilitySystemComponent.ActiveEffectCount} ActiveEffectCapacity: {abilitySystemComponent.ActiveEffectCapacity}, DistinctEffectCount: {abilitySystemComponent.DistinctEffectCount} ");
        }
#endif
    }
}