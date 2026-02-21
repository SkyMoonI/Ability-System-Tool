using UnityEngine;

namespace AbilitySystemTool
{
    public abstract class EffectActionSO : ScriptableObject
    {
        // Called once when the effect instance is applied.
        public virtual void OnApply(in EffectContext context) { }

        // Called on each tick (only if the effect has tick enabled).
        public virtual void OnTick(in EffectContext context) { }

        // Called once when the effect instance expires (or is removed).
        public virtual void OnExpire(in EffectContext context) { }
    }
}