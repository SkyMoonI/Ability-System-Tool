using UnityEngine;

namespace AbilitySystemTool
{
    public enum StackingPolicy
    {
        Refresh,
        Replace,
        Stack
    }

    [CreateAssetMenu(menuName = "Ability System Tool/Effect", fileName = "newEffect")]
    public class EffectSO : ScriptableObject
    {
        [Min(0f)] public float effectDuration = 1f;

        [Header("Tick (optional)")]
        public bool hasTick = false;
        [Min(0.01f)] public float tickInterval = 1f; // apply the effect every x seconds

        [Header("Stacking")]
        public StackingPolicy stackingPolicy = StackingPolicy.Refresh; // to determine how to stack the effect
    }
}