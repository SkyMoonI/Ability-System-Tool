using UnityEngine;

namespace AbilitySystemTool
{
    public abstract class EffectActionSO : ScriptableObject
    {
        // Called once when the effect instance is applied.
        public virtual void OnApply(AbilityTarget target, int instanceId, EffectSO effectSO) { }

        // Called on each tick (only if the effect has tick enabled).
        public virtual void OnTick(AbilityTarget target, int instanceId, EffectSO effectSO) { }

        // Called once when the effect instance expires (or is removed).
        public virtual void OnExpire(AbilityTarget target, int instanceId, EffectSO effectSO) { }
    }
}