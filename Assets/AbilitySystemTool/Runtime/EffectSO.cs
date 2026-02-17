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
        [SerializeField, Min(0f)] private float _effectDuration = 1f;
        public float EffectDuration => _effectDuration;

        [Header("Tick")]
        [SerializeField] private bool _hasTick = false;
        public bool HasTick => _hasTick;
        [SerializeField, Min(0.01f)] private float _tickInterval = 1f; // apply the effect every x seconds
        public float TickInterval => _tickInterval;

        [Header("Action")]
        [SerializeField] private EffectActionSO _effectActionSO;
        public EffectActionSO EffectActionSO => _effectActionSO;

        [Header("Stacking")]
        [SerializeField] private StackingPolicy _stackingPolicy = StackingPolicy.Refresh; // to determine how to stack the effect
        public StackingPolicy StackingPolicy => _stackingPolicy;
    }
}