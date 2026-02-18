using UnityEngine;

namespace AbilitySystemTool
{
    [RequireComponent(typeof(AbilityTarget))]
    public sealed class AbilitySystemComponent : MonoBehaviour
    {
        private AbilityTarget _ownerTarget;
        private EffectRuntimeSystem _effects;

        private void Awake()
        {
            _ownerTarget = GetComponent<AbilityTarget>();
            _effects = new EffectRuntimeSystem(_ownerTarget);
        }

        private void Update()
        {
            _effects.Update(Time.deltaTime);
        }

        public void ApplyAbility(AbilitySO abilitySO)
        {
            if (abilitySO == null) return;

            for (int i = 0; i < abilitySO.EffectList.Count; i++)
            {
                EffectSO effectSO = abilitySO.EffectList[i];
                if (effectSO == null) continue;

                _effects.ApplyEffect(effectSO);
            }
        }
    }
}