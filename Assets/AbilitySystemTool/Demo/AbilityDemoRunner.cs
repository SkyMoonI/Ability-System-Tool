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
            if (abilitySO != null && Input.GetKeyDown(KeyCode.Space))
                CastAbility(abilitySO);

            UpdateActiveEffects(Time.deltaTime);
        }

        private void CastAbility(AbilitySO abilitySO)
        {
            Debug.Log($"[CAST] {abilitySO.name}");

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
                if (activeEffect.effectSO != effectSO) continue;

                switch (effectSO.StackingPolicy)
                {
                    case StackingPolicy.Refresh:
                        activeEffect.remainingDuration = effectSO.EffectDuration;
                        _activeEffectList[i] = activeEffect;
                        Debug.Log($"[REFRESH] {effectSO.name} (duration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={activeEffect.instanceId})");
                        break;

                    case StackingPolicy.Replace:
                        // Replace semantics: expire old instance, create a new instance, apply again.
                        activeEffect.effectSO.EffectActionSO?.OnExpire(activeEffect.abilityTarget, activeEffect.instanceId, activeEffect.effectSO);

                        ActiveEffect replaced = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration, activeEffect.abilityTarget);
                        _activeEffectList[i] = replaced;

                        effectSO.EffectActionSO?.OnApply(replaced.abilityTarget, replaced.instanceId, effectSO);

                        Debug.Log($"[REPLACE] {effectSO.name} (duration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={replaced.instanceId})");
                        break;

                    case StackingPolicy.Stack:
                        ActiveEffect stacked = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration, activeEffect.abilityTarget);
                        _activeEffectList.Add(stacked);

                        effectSO.EffectActionSO?.OnApply(stacked.abilityTarget, stacked.instanceId, effectSO);

                        if (_activeEffectCountDictionary.TryGetValue(effectSO, out int stackCount))
                            _activeEffectCountDictionary[effectSO] = ++stackCount;
                        else
                            _activeEffectCountDictionary.Add(effectSO, stackCount = 1);

                        Debug.Log($"[STACK] {effectSO.name} (count={stackCount}, duration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={stacked.instanceId})");
                        break;
                }

                return;
            }

            // No existing instance -> create a new one.
            ActiveEffect created = new ActiveEffect(_nextInstanceId++, effectSO, effectSO.EffectDuration, currentAbilityTarget);
            _activeEffectList.Add(created);

            if (_activeEffectCountDictionary.TryGetValue(effectSO, out int count))
                _activeEffectCountDictionary[effectSO] = count + 1;
            else
                _activeEffectCountDictionary.Add(effectSO, 1);

            effectSO.EffectActionSO?.OnApply(created.abilityTarget, created.instanceId, effectSO);

            Debug.Log($"[APPLY] {effectSO.name} (duration={effectSO.EffectDuration}, tick={effectSO.HasTick}, id={created.instanceId})");
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

                if (activeEffect.remainingDuration <= 0f)
                {
                    Debug.Log($"[EXPIRE] {activeEffect.effectSO.name} (id={activeEffect.instanceId})");
                    activeEffect.effectSO.EffectActionSO?.OnExpire(activeEffect.abilityTarget, activeEffect.instanceId, activeEffect.effectSO);

                    _activeEffectList.RemoveAt(i);

                    if (_activeEffectCountDictionary.TryGetValue(activeEffect.effectSO, out int c))
                    {
                        c--;
                        if (c <= 0) _activeEffectCountDictionary.Remove(activeEffect.effectSO);
                        else _activeEffectCountDictionary[activeEffect.effectSO] = c;
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

                timeUntilNextTick = effectSO.HasTick ? Mathf.Max(0.01f, effectSO.TickInterval) : 0f;
            }
        }
    }
}