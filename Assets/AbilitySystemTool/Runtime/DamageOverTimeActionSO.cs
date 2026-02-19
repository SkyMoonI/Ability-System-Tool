using UnityEngine;

namespace AbilitySystemTool
{
    [CreateAssetMenu(menuName = "Ability System Tool/Effect Actions/Damage Over Time Action", fileName = "newDamageOverTimeAction")]
    public sealed class DamageOverTimeActionSO : EffectActionSO
    {
        [SerializeField] private float _damagePerTick = 1f;
        public float DamagePerTick => _damagePerTick;

        public override void OnApply(in EffectContext context) { }

        public override void OnTick(in EffectContext context)
        {
            if (context.Target == null) return;
            HealthComponent healthComponent = context.Target.HealthComponent;
            if (healthComponent == null) return;


            healthComponent.TakeDamage(DamagePerTick);
        }

        public override void OnExpire(in EffectContext context) { }
    }
}