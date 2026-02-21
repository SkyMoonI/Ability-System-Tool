namespace AbilitySystemTool
{
    public readonly struct AbilityCastContext
    {
        public readonly AbilitySystemComponent CasterAbilitySystemComponent;
        public readonly AbilityTarget Caster;
        public readonly AbilityTarget Target;
        public readonly AbilitySO Ability;
        public readonly ResourceComponent CasterResourceComponent;

        public AbilityCastContext(
                AbilitySystemComponent casterAbilitySystemComponent,
                AbilityTarget caster,
                AbilityTarget target,
                AbilitySO ability,
                ResourceComponent casterResourceComponent)
        {
            CasterAbilitySystemComponent = casterAbilitySystemComponent;
            Caster = caster;
            Target = target;
            Ability = ability;
            CasterResourceComponent = casterResourceComponent;
        }
    }
}