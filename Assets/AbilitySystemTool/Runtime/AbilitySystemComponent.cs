using UnityEngine;

namespace AbilitySystemTool
{
    [RequireComponent(typeof(AbilityTarget))]
    public sealed class AbilitySystemComponent : MonoBehaviour
    {
        private AbilityTarget _ownerTarget;
        private EffectRuntimeSystem _effectSystem;

        private void Awake()
        {
            _ownerTarget = GetComponent<AbilityTarget>();
            _effectSystem = new EffectRuntimeSystem(_ownerTarget);
        }

        private void Update()
        {
            _effectSystem.Update(Time.deltaTime);

        }

        public void ApplyAbility(AbilityTarget abilitySource, AbilitySO abilitySO)
        {
            if (abilitySO == null) return;

            for (int i = 0; i < abilitySO.EffectList.Count; i++)
            {
                EffectSO effectSO = abilitySO.EffectList[i];
                if (effectSO == null) continue;

                _effectSystem.ApplyEffect(abilitySource, abilitySO, effectSO);
            }
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
    }
}