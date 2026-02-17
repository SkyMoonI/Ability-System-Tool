using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    public class AbilityDemoRunner : MonoBehaviour
    {
        [SerializeField] private AbilitySO abilitySO;
        [SerializeField] private AbilityTarget currentAbilityTarget;

        private List<ActiveEffect> _activeEffectList;
        private Dictionary<EffectSO, int> _activeEffectCountDictionary;
        private int _nextInstanceId = 1;


        private void Awake()
        {
            _activeEffectList = new List<ActiveEffect>(32);
            _activeEffectCountDictionary = new Dictionary<EffectSO, int>(16);
        }

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
            for (int i = 0; i < abilitySO.EffectList.Count; i++)
            {
                EffectSO effectSO = abilitySO.EffectList[i];

                if (effectSO == null) continue;

                ApplyEffect(effectSO);
            }
        }

        private void ApplyEffect(EffectSO effectSO)
        {
            for (int i = 0; i < _activeEffectList.Count; i++)
            {
                ActiveEffect activeEffect = _activeEffectList[i];
                if (activeEffect.effectSO == effectSO)
                {
                    switch (effectSO.StackingPolicy)
                    {
                        case StackingPolicy.Refresh:
                            activeEffect.remainingDuration = effectSO.EffectDuration;
                            _activeEffectList[i] = activeEffect;
                            Debug.Log($"[REFRESH] {effectSO.name} (EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={activeEffect.instanceId})");
                            break;
                        case StackingPolicy.Replace:
                            activeEffect.effectSO.EffectActionSO?.OnExpire(activeEffect.abilityTarget, activeEffect.instanceId, activeEffect.effectSO);

                            ActiveEffect replacedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration, activeEffect.abilityTarget);
                            _activeEffectList[i] = replacedActiveEffect;

                            effectSO.EffectActionSO?.OnApply(activeEffect.abilityTarget, replacedActiveEffect.instanceId, effectSO);

                            Debug.Log($"[REPLACE] {effectSO.name} (EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={replacedActiveEffect.instanceId})");
                            break;
                        case StackingPolicy.Stack:
                            ActiveEffect stackedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration, activeEffect.abilityTarget);
                            _activeEffectList.Add(stackedActiveEffect);

                            effectSO.EffectActionSO?.OnApply(activeEffect.abilityTarget, stackedActiveEffect.instanceId, effectSO);

                            if (_activeEffectCountDictionary.TryGetValue(effectSO, out int stackCount))
                            {
                                stackCount++;
                                _activeEffectCountDictionary[effectSO] = stackCount;
                            }
                            else
                            {
                                stackCount = 1;
                                _activeEffectCountDictionary.Add(effectSO, 1);
                            }

                            Debug.Log($"[STACK] {effectSO.name} (count={stackCount}, EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={stackedActiveEffect.instanceId})");
                            break;
                    }
                    return;
                }
            }

            // Runtime instance
            ActiveEffect newActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration, currentAbilityTarget);

            _activeEffectList.Add(newActiveEffect);
            _activeEffectCountDictionary[effectSO] = 1;

            effectSO.EffectActionSO?.OnApply(newActiveEffect.abilityTarget, newActiveEffect.instanceId, newActiveEffect.effectSO);

            Debug.Log($"[APPLY] {effectSO.name} (EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={newActiveEffect.instanceId})");
        }

        private void UpdateActiveEffects(float deltaTime)
        {
            for (int i = _activeEffectList.Count - 1; i >= 0; i--)
            {
                ActiveEffect activeEffect = _activeEffectList[i];

                activeEffect.remainingDuration -= deltaTime;

                if (activeEffect.effectSO.HasTick)
                {
                    activeEffect.timeUntilNextTick -= deltaTime;

                    if (activeEffect.timeUntilNextTick <= 0f)
                    {
                        activeEffect.timeUntilNextTick += activeEffect.effectSO.TickInterval;

                        Debug.Log($"[TICK] {activeEffect.effectSO.name} (id={activeEffect.instanceId})");
                        activeEffect.effectSO.EffectActionSO?.OnTick(activeEffect.abilityTarget, activeEffect.instanceId, activeEffect.effectSO);
                    }
                }

                // If the effect has expired, remove it
                if (activeEffect.remainingDuration <= 0f)
                {
                    Debug.Log($"[EXPIRE] {activeEffect.effectSO.name}, id={activeEffect.instanceId}");

                    activeEffect.effectSO.EffectActionSO?.OnExpire(activeEffect.abilityTarget, activeEffect.instanceId, activeEffect.effectSO);

                    _activeEffectList.RemoveAt(i);

                    if (_activeEffectCountDictionary.TryGetValue(activeEffect.effectSO, out int count))
                    {
                        count--;
                        if (count <= 0) _activeEffectCountDictionary.Remove(activeEffect.effectSO);
                        else _activeEffectCountDictionary[activeEffect.effectSO] = count;
                    }
                    continue;
                }

                _activeEffectList[i] = activeEffect;
            }
        }

        private struct ActiveEffect
        {
            public int instanceId;
            public EffectSO effectSO;
            public float remainingDuration;
            public float timeUntilNextTick;
            public AbilityTarget abilityTarget;

            public ActiveEffect(int instanceId, EffectSO effectSO, float remainingDuration, AbilityTarget abilityTarget)
            {
                this.instanceId = instanceId;
                this.effectSO = effectSO;
                this.remainingDuration = remainingDuration;
                this.abilityTarget = abilityTarget;

                if (effectSO.HasTick)
                    timeUntilNextTick = Mathf.Max(0.01f, effectSO.TickInterval);
                else
                    timeUntilNextTick = 0f;
            }
        }
    }
}