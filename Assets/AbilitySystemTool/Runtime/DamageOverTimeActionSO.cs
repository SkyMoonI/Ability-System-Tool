using UnityEngine;

namespace AbilitySystemTool
{
    [CreateAssetMenu(menuName = "Ability System Tool/Effect Actions/Damage Over Time Action", fileName = "newDamageOverTimeAction")]
    public sealed class DamageOverTimeActionSO : EffectActionSO
    {
        [SerializeField] private float _damagePerTick = 1f;
        public float DamagePerTick => _damagePerTick;

        public override void OnApply(AbilityTarget target, int instanceId, EffectSO effectSO) { }

        public override void OnTick(AbilityTarget target, int instanceId, EffectSO effectSO)
        {
            if (target == null) return;
            HealthComponent healthComponent = target.HealthComponent;
            if (healthComponent == null) return;


            healthComponent.TakeDamage(DamagePerTick);
        }

        public override void OnExpire(AbilityTarget target, int instanceId, EffectSO effectSO) { }
    }
}