using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AbilitySystemTool
{
    public class AbilityDemoRunner : MonoBehaviour
    {
        [SerializeField] private AbilitySO abilitySO;

        private readonly List<ActiveEffect> _activeEffectList = new List<ActiveEffect>();

        private void Update()
        {
            // 1) Input: ability cast
            if (abilitySO != null && Input.GetKeyDown(KeyCode.Space))
            {
                CastAbility(abilitySO);
            }

            UpdateActiveEffects(Time.deltaTime);
        }

        private void CastAbility(AbilitySO abilitySO)
        {
            Debug.Log($"[CAST] {abilitySO.name}");

            // Start every effect in the ability
            for (int i = 0; i < abilitySO.effectList.Count; i++)
            {
                EffectSO effectSO = abilitySO.effectList[i];

                if (effectSO == null) continue;

                // Runtime instance
                ActiveEffect activeEffect = new ActiveEffect(effectSO, effectSO.effectDuration);

                _activeEffectList.Add(activeEffect);

                Debug.Log($"[APPLY] {effectSO.name} (effectDuration={effectSO.effectDuration}, tick={effectSO.hasTick}, key={effectSO.StackKey})");
            }
        }

        private void UpdateActiveEffects(float deltaTime)
        {
            for (int i = _activeEffectList.Count - 1; i >= 0; i--)
            {
                ActiveEffect activeEffect = _activeEffectList[i];

                activeEffect.remainingDuration -= deltaTime;

                if (activeEffect.effectSO.hasTick)
                {
                    activeEffect.timeUntilNextTick -= deltaTime;

                    if (activeEffect.timeUntilNextTick <= 0f)
                    {
                        activeEffect.timeUntilNextTick += activeEffect.effectSO.tickInterval;

                        Debug.Log($"[TICK] {activeEffect.effectSO.name} (key={activeEffect.effectSO.StackKey})");
                    }
                }

                // If the effect has expired, remove it
                if (activeEffect.remainingDuration <= 0f)
                {
                    Debug.Log($"[EXPIRE] {activeEffect.effectSO.name} (key={activeEffect.effectSO.StackKey})");
                    _activeEffectList.RemoveAt(i);
                    continue;
                }

                _activeEffectList[i] = activeEffect;
            }
        }

        private struct ActiveEffect
        {
            public EffectSO effectSO;
            public float remainingDuration;
            public float timeUntilNextTick;

            public ActiveEffect(EffectSO effectSO, float remainingDuration)
            {
                this.effectSO = effectSO;
                this.remainingDuration = remainingDuration;

                if (effectSO.hasTick)
                    timeUntilNextTick = Mathf.Max(0.01f, effectSO.tickInterval);
                else
                    timeUntilNextTick = 0f;
            }
        }
    }
}