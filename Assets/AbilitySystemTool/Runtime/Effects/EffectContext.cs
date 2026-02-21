namespace AbilitySystemTool
{
    public readonly struct EffectContext
    {
        public readonly AbilityTarget Source;
        public readonly AbilityTarget Target;
        public readonly AbilitySO Ability;
        public readonly EffectSO Effect;
        public readonly int InstanceId;
        public readonly int StackCount;

        public EffectContext(
        AbilityTarget abilitySource,
        AbilityTarget abilityTarget,
        AbilitySO abilitySO,
        EffectSO effectSO,
        int instanceId,
        int stackCount)
        {
            Source = abilitySource;
            Target = abilityTarget;
            Ability = abilitySO;
            Effect = effectSO;
            InstanceId = instanceId;
            StackCount = stackCount;
        }
    }
}