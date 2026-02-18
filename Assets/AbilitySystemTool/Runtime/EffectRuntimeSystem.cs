using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    public sealed class EffectRuntimeSystem
    {
        private AbilityTarget _ownerTarget;
        private List<ActiveEffect> _activeEffectList;
        private Dictionary<EffectSO, int> _activeEffectCountDictionary;
        private int _nextInstanceId = 1;

        public EffectRuntimeSystem(AbilityTarget ownerTarget, int initialCapacity = 32)
        {
            _ownerTarget = ownerTarget;
            _activeEffectList = new List<ActiveEffect>(initialCapacity);
            _activeEffectCountDictionary = new Dictionary<EffectSO, int>(Mathf.Max(8, initialCapacity / 2));
        }

        public void Update(float deltaTime)
        {
            if (deltaTime <= 0f) return;
            if (_activeEffectList.Count == 0) return;
            UpdateActiveEffects(deltaTime);
        }

        public void ApplyEffect(EffectSO effectSO)
        {
            if (effectSO == null) return;
            if (_ownerTarget == null) return;

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
                            activeEffect.effectSO.EffectActionSO?.OnExpire(_ownerTarget, activeEffect.instanceId, activeEffect.effectSO);

                            ActiveEffect replacedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration);
                            _activeEffectList[i] = replacedActiveEffect;

                            effectSO.EffectActionSO?.OnApply(_ownerTarget, replacedActiveEffect.instanceId, effectSO);

                            Debug.Log($"[REPLACE] {effectSO.name} (EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={replacedActiveEffect.instanceId})");
                            break;
                        case StackingPolicy.Stack:
                            ActiveEffect stackedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration);
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

                            effectSO.EffectActionSO?.OnApply(_ownerTarget, stackedActiveEffect.instanceId, effectSO);

                            Debug.Log($"[STACK] {effectSO.name} (count={stackCount}, EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={stackedActiveEffect.instanceId})");
                            break;
                    }
                    return;
                }
            }

            // Runtime instance
            ActiveEffect newActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration);

            _activeEffectList.Add(newActiveEffect);
            if (_activeEffectCountDictionary.TryGetValue(effectSO, out int newStackCount))
            {
                newStackCount++;
                _activeEffectCountDictionary[effectSO] = newStackCount;
            }
            else
            {
                newStackCount = 1;
                _activeEffectCountDictionary.Add(effectSO, newStackCount);
            }

            effectSO.EffectActionSO?.OnApply(_ownerTarget, newActiveEffect.instanceId, newActiveEffect.effectSO);

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
                        activeEffect.effectSO.EffectActionSO?.OnTick(_ownerTarget, activeEffect.instanceId, activeEffect.effectSO);
                    }
                }

                // If the effect has expired, remove it
                if (activeEffect.remainingDuration <= 0f)
                {
                    Debug.Log($"[EXPIRE] {activeEffect.effectSO.name} (id={activeEffect.instanceId})");

                    activeEffect.effectSO.EffectActionSO?.OnExpire(_ownerTarget, activeEffect.instanceId, activeEffect.effectSO);

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

                if (effectSO.HasTick)
                    timeUntilNextTick = Mathf.Max(0.01f, effectSO.TickInterval);
                else
                    timeUntilNextTick = 0f;
            }
        }
    }
}