#if UNITY_EDITOR
using UnityEditor;
#endif
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

        private const float MinTickInterval = 0.01f;

        private void OnValidate()
        {
            bool changed = false;

            if (_effectDuration < 0f)
            {
                _effectDuration = 0f;
                changed = true;
                Debug.LogWarning($"[EffectSO] Duration was < 0. Clamped to 0 on '{name}'.", this);
            }

            if (_hasTick)
            {
                if (_tickInterval < MinTickInterval)
                {
                    _tickInterval = MinTickInterval;
                    changed = true;
                    Debug.LogWarning($"[EffectSO] TickInterval was < {MinTickInterval}. Clamped to {MinTickInterval} on '{name}'.", this);
                }
            }
            else
            {
                if (_tickInterval != 0f)
                {
                    _tickInterval = 0f;
                    changed = true;
                }
            }

#if UNITY_EDITOR
            if (changed)
            {
                EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}