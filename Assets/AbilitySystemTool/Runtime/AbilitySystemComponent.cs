using UnityEngine;

namespace AbilitySystemTool
{
    [RequireComponent(typeof(AbilityTarget))]
    public sealed class AbilitySystemComponent : MonoBehaviour
    {
        [SerializeField] private int _initialActiveEffectCapacity = 64;
        [SerializeField] private int _initialDistinctEffectCapacity = 16;

        private AbilityTarget _ownerTarget;
        private AbilityCooldownSystem _abilityCooldownSystem;
        private EffectRuntimeSystem _effectSystem;


        private void OnValidate()
        {
            _initialActiveEffectCapacity = Mathf.Max(1, _initialActiveEffectCapacity);
            _initialDistinctEffectCapacity = Mathf.Max(1, _initialDistinctEffectCapacity);
        }

        private void Awake()
        {
            _ownerTarget = GetComponent<AbilityTarget>();
            _effectSystem = new EffectRuntimeSystem(_ownerTarget, _initialActiveEffectCapacity, _initialDistinctEffectCapacity);
            _abilityCooldownSystem = new AbilityCooldownSystem();
        }

        private void Update()
        {
            _effectSystem.Update(Time.deltaTime);
            _abilityCooldownSystem.Update(Time.deltaTime);
        }

        private void ApplyAbility(AbilityTarget abilitySource, AbilitySO abilitySO)
        {
            if (abilitySO == null) return;

            for (int i = 0; i < abilitySO.EffectList.Count; i++)
            {
                EffectSO effectSO = abilitySO.EffectList[i];
                if (effectSO == null) continue;

                _effectSystem.ApplyEffect(abilitySource, abilitySO, effectSO);
            }
        }

        public bool TryCastAbility(AbilitySO ability, AbilityTarget target, out CastFailReason reason)
        {
            reason = CastFailReason.None;

            if (ability == null || target == null)
            {
                reason = CastFailReason.Invalid;
                return false;
            }

            AbilitySystemComponent targetAbilitySystemComponent = target.GetComponent<AbilitySystemComponent>();
            if (targetAbilitySystemComponent == null)
            {
                reason = CastFailReason.NoTargetAbilitySystem;
                return false;
            }

            if (_abilityCooldownSystem.IsOnCooldown(ability))
            {
                reason = CastFailReason.OnCooldown;
                return false;
            }

            // Apply ability effects on target. Source is this owner.
            targetAbilitySystemComponent.ApplyAbility(_ownerTarget, ability);

            // start cooldown after successful application
            _abilityCooldownSystem.TryStartCooldown(ability);

            return true;
        }

        public float GetRemainingCooldown(AbilitySO ability)
        {
            return _abilityCooldownSystem.GetRemainingCooldown(ability);
        }

        public bool HasEffect(EffectSO effect)
        {
            return _effectSystem.HasEffect(effect);
        }

        public int GetStackCount(EffectSO effect)
        {
            return _effectSystem.GetStackCount(effect);
        }

        public bool TryGetEffect(EffectSO effect, out ActiveEffectHandle handle)
        {
            return _effectSystem.TryGetEffect(effect, out handle);
        }

        public int RemoveEffect(EffectSO effect, RemoveReason reason, bool removeAllStacks = true)
        {
            return _effectSystem.RemoveEffect(effect, reason, removeAllStacks);
        }

        public int RemoveEffectsBySource(AbilityTarget source, RemoveReason reason)
        {
            return _effectSystem.RemoveEffectsBySource(source, reason);
        }

#if UNITY_EDITOR
        internal int ActiveEffectCount => _effectSystem.ActiveEffectCount;
        internal int ActiveEffectCapacity => _effectSystem.ActiveEffectCapacity;
        internal int DistinctEffectCount => _effectSystem.DistinctEffectCount;
#endif

    }
}