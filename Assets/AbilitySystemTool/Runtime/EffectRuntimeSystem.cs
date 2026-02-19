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

        public void ApplyEffect(AbilityTarget abilitySource, AbilitySO abilitySO, EffectSO effectSO)
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

                            // keep latest applier
                            activeEffect.abilitySource = abilitySource;
                            activeEffect.abilitySO = abilitySO;

                            _activeEffectList[i] = activeEffect;
                            Debug.Log($"[REFRESH] {effectSO.name} (EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={activeEffect.instanceId})");
                            break;
                        case StackingPolicy.Replace:
                            Debug.Log($"[REPLACE] {effectSO.name} (EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={activeEffect.instanceId})");

                            int count = _activeEffectCountDictionary.TryGetValue(effectSO, out int c) ? c : 1;

                            EffectContext expiredEffectContext = BuildContext(in activeEffect, count);
                            activeEffect.effectSO.EffectActionSO?.OnExpire(in expiredEffectContext);

                            ActiveEffect replacedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, abilitySource, abilitySO, effectSO.EffectDuration);
                            _activeEffectList[i] = replacedActiveEffect;

                            EffectContext replacedEffectContext = BuildContext(in replacedActiveEffect, count);
                            effectSO.EffectActionSO?.OnApply(in replacedEffectContext);
                            break;
                        case StackingPolicy.Stack:
                            ActiveEffect stackedActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, abilitySource, abilitySO, effectSO.EffectDuration);
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

                            EffectContext newEffectContext = BuildContext(in stackedActiveEffect, stackCount);

                            effectSO.EffectActionSO?.OnApply(in newEffectContext);

                            Debug.Log($"[STACK] {effectSO.name} (count={stackCount}, EffectDuration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={stackedActiveEffect.instanceId})");
                            break;
                    }
                    return;
                }
            }

            // Runtime instance
            ActiveEffect newActiveEffect = new ActiveEffect(_nextInstanceId++, effectSO, abilitySource, abilitySO, effectSO.EffectDuration);

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

            EffectContext effectContext = BuildContext(in newActiveEffect, newStackCount);

            effectSO.EffectActionSO?.OnApply(in effectContext);

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

                        int stackCount = _activeEffectCountDictionary.TryGetValue(activeEffect.effectSO, out int count) ? count : 1;
                        EffectContext effectContext = BuildContext(in activeEffect, stackCount);


                        Debug.Log($"[TICK] {activeEffect.effectSO.name} (id={activeEffect.instanceId})");
                        activeEffect.effectSO.EffectActionSO?.OnTick(in effectContext);
                    }
                }

                // If the effect has expired, remove it
                if (activeEffect.remainingDuration <= 0f)
                {
                    int stackCountBefore = _activeEffectCountDictionary.TryGetValue(activeEffect.effectSO, out int count) ? count : 1;

                    EffectContext effectContext = BuildContext(in activeEffect, stackCountBefore);

                    activeEffect.effectSO.EffectActionSO?.OnExpire(in effectContext);

                    Debug.Log($"[EXPIRE] {activeEffect.effectSO.name} (id={activeEffect.instanceId})");

                    _activeEffectList.RemoveAt(i);

                    int stackCountAfter = stackCountBefore - 1;
                    if (stackCountAfter <= 0) _activeEffectCountDictionary.Remove(activeEffect.effectSO);
                    else _activeEffectCountDictionary[activeEffect.effectSO] = stackCountAfter;

                    continue;
                }

                _activeEffectList[i] = activeEffect;
            }
        }

        private EffectContext BuildContext(in ActiveEffect activeEffect, int stackCount)
        {
            return new EffectContext(
                activeEffect.abilitySource,
                _ownerTarget,
                activeEffect.abilitySO,
                activeEffect.effectSO,
                activeEffect.instanceId,
                stackCount);
        }

        private struct ActiveEffect
        {
            public int instanceId;
            public EffectSO effectSO;

            public AbilityTarget abilitySource;
            public AbilitySO abilitySO;

            public float remainingDuration;
            public float timeUntilNextTick;

            public ActiveEffect(int instanceId, EffectSO effectSO, AbilityTarget abilitySource, AbilitySO abilitySO, float remainingDuration)
            {
                this.instanceId = instanceId;
                this.effectSO = effectSO;
                this.abilitySource = abilitySource;
                this.abilitySO = abilitySO;
                this.remainingDuration = remainingDuration;

                if (effectSO.HasTick)
                    timeUntilNextTick = Mathf.Max(0.01f, effectSO.TickInterval);
                else
                    timeUntilNextTick = 0f;
            }
        }
    }
}