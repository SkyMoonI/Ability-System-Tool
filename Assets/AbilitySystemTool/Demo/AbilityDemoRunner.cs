using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    public class AbilityDemoRunner : MonoBehaviour
    {
        [SerializeField] private AbilitySO abilitySO;

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
            for (int i = 0; i < abilitySO.effectList.Count; i++)
            {
                EffectSO effectSO = abilitySO.effectList[i];

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
                    switch (effectSO.stackingPolicy)
                    {
                        case StackingPolicy.Refresh:
                            activeEffect.remainingDuration = effectSO.effectDuration;
                            _activeEffectList[i] = activeEffect;
                            Debug.Log($"[REFRESH] {effectSO.name} (effectDuration={effectSO.effectDuration}, tick={effectSO.hasTick}, id={activeEffect.instanceId})");
                            break;
                        case StackingPolicy.Replace:
                            ActiveEffect replacedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.effectDuration);
                            _activeEffectList[i] = replacedActiveEffect;
                            Debug.Log($"[REPLACE] {effectSO.name} (effectDuration={effectSO.effectDuration}, tick={effectSO.hasTick}, id={replacedActiveEffect.instanceId})");
                            break;
                        case StackingPolicy.Stack:
                            ActiveEffect stackedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.effectDuration);
                            _activeEffectList.Add(stackedActiveEffect);


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

                            Debug.Log($"[STACK] {effectSO.name} (count={stackCount}, effectDuration={effectSO.effectDuration}, tick={effectSO.hasTick}, id={stackedActiveEffect.instanceId})");
                            break;
                    }
                    return;
                }
            }

            // Runtime instance
            ActiveEffect newActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.effectDuration);

            _activeEffectList.Add(newActiveEffect);
            _activeEffectCountDictionary[effectSO] = 1;

            Debug.Log($"[APPLY] {effectSO.name} (effectDuration={effectSO.effectDuration}, tick={effectSO.hasTick}, id={newActiveEffect.instanceId})");
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

                        Debug.Log($"[TICK] {activeEffect.effectSO.name}, id={activeEffect.instanceId}");
                    }
                }

                // If the effect has expired, remove it
                if (activeEffect.remainingDuration <= 0f)
                {
                    Debug.Log($"[EXPIRE] {activeEffect.effectSO.name}, id={activeEffect.instanceId}");
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

            public ActiveEffect(int instanceId, EffectSO effectSO, float remainingDuration)
            {
                this.instanceId = instanceId;
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