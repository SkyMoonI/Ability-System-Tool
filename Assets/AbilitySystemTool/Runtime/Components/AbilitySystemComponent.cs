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

        public bool CanCast(AbilitySO ability, AbilityTarget target, out CastFailReason reason)
        {
            reason = CastFailReason.None;

            if (ability == null || target == null)
            {
                reason = CastFailReason.Invalid;
                return false;
            }

            if (!target.TryGetComponent(out AbilitySystemComponent _))
            {
                reason = CastFailReason.NoTargetAbilitySystem;
                return false;
            }

            // Ability has no effects => misconfigured
            if (ability.EffectList == null || ability.EffectList.Count == 0)
            {
                reason = CastFailReason.MisconfiguredAbility;
                return false;
            }

            bool hasAtLeastOneRunnableEffect = false;
            for (int i = 0; i < ability.EffectList.Count; i++)
            {
                EffectSO effect = ability.EffectList[i];
                if (effect == null) continue;
                if (effect.EffectActionSO == null) continue; // runnable değil
                hasAtLeastOneRunnableEffect = true;
                break;
            }
            if (!hasAtLeastOneRunnableEffect)
            {
                reason = CastFailReason.MisconfiguredAbility;
                return false;
            }

            if (_abilityCooldownSystem.IsOnCooldown(ability))
            {
                reason = CastFailReason.OnCooldown;
                return false;
            }

            AbilityCastContext castContext = new AbilityCastContext(
                this,
                _ownerTarget,
                target,
                ability,
                _ownerTarget.ResourceComponent
            );

            // Requirements
            for (int i = 0; i < ability.RequirementList.Count; i++)
            {
                AbilityRequirementSO requirement = ability.RequirementList[i];
                if (requirement == null)
                {
                    reason = CastFailReason.MisconfiguredAbility;
                    return false;
                }

                if (!requirement.IsMet(in castContext, out reason))
                {
                    if (reason == CastFailReason.None)
                    {
                        reason = CastFailReason.RequirementFailed;
                    }

                    return false;
                }
            }

            // Costs
            for (int i = 0; i < ability.CostList.Count; i++)
            {
                AbilityCostSO cost = ability.CostList[i];
                if (cost == null)
                {
                    reason = CastFailReason.MisconfiguredAbility;
                    return false;
                }

                if (!cost.CanPay(in castContext, out reason))
                {
                    if (reason == CastFailReason.None)
                    {
                        reason = CastFailReason.CostFailed;
                    }

                    return false;
                }
            }

            return true;
        }

        public bool TryCastAbility(AbilitySO ability, AbilityTarget target, out CastFailReason reason)
        {
            if (!CanCast(ability, target, out reason)) return false;

            if (!target.TryGetComponent(out AbilitySystemComponent targetAbilitySystemComponent))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                RuntimeLogger.DevAssert(false, "[AbilitySystemComponent] CanCast passed but target ASC is missing.", this);
#endif
                reason = CastFailReason.NoTargetAbilitySystem;
                return false;
            }

            // Apply ability effects on target. Source is this owner.
            targetAbilitySystemComponent.ApplyAbility(_ownerTarget, ability);

            // Pay costs
            AbilityCastContext castContext = new AbilityCastContext(this, _ownerTarget, target, ability, _ownerTarget.ResourceComponent);
            for (int i = 0; i < ability.CostList.Count; i++)
            {
                AbilityCostSO cost = ability.CostList[i];
                if (cost == null) continue;

                cost.Pay(in castContext);
            }

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