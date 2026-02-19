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

        public override void OnApply(in EffectContext context)
        {
            if (context.Target == null) return;
            StatsComponent statsComponent = context.Target.StatsComponent;
            if (statsComponent == null) return;

            statsComponent.MultiplyMoveSpeed(_multiplier);
            Debug.Log($"[SLOW APPLY] {context.Effect.name} (id={context.InstanceId}, mult={_multiplier}, speed={statsComponent.CurrentMoveSpeed})");
        }

        public override void OnTick(in EffectContext context) { }

        public override void OnExpire(in EffectContext context)
        {
            if (context.Target == null) return;
            StatsComponent statsComponent = context.Target.StatsComponent;
            if (statsComponent == null) return;

            statsComponent.DivideMoveSpeed(_multiplier);
            Debug.Log($"[SLOW EXPIRE] {context.Effect.name} (id={context.InstanceId}, mult={_multiplier}, speed={statsComponent.CurrentMoveSpeed})");
        }
    }
}