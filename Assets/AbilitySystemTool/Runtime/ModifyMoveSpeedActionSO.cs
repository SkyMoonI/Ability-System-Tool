using UnityEngine;

namespace AbilitySystemTool
{
    [CreateAssetMenu(menuName = "Ability System Tool/Effect Actions/Modify Move Speed Action", fileName = "ModifyMoveSpeedAction")]
    public sealed class ModifyMoveSpeedActionSO : EffectActionSO
    {
        [SerializeField] private float _multiplier = 0.7f;
        public float Multiplier => _multiplier;

        private void OnValidate()
        {
            _multiplier = Mathf.Clamp(_multiplier, 0.01f, 10f);
        }

        public override void OnApply(AbilityTarget target, int instanceId, EffectSO effectSO)
        {
            if (target == null) return;
            StatsComponent statsComponent = target.StatsComponent;
            if (statsComponent == null) return;

            statsComponent.MultiplyMoveSpeed(_multiplier);
            Debug.Log($"[SLOW APPLY] {effectSO.name} (id={instanceId}, mult={_multiplier}, speed={statsComponent.CurrentMoveSpeed})");
        }

        public override void OnTick(AbilityTarget target, int instanceId, EffectSO effectSO) { }

        public override void OnExpire(AbilityTarget target, int instanceId, EffectSO effectSO)
        {
            if (target == null) return;
            StatsComponent statsComponent = target.StatsComponent;
            if (statsComponent == null) return;

            statsComponent.DivideMoveSpeed(_multiplier);
            Debug.Log($"[SLOW EXPIRE] {effectSO.name} (id={instanceId}, mult={_multiplier}, speed={statsComponent.CurrentMoveSpeed})");
        }
    }
}