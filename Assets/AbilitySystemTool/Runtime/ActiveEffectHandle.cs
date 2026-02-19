namespace AbilitySystemTool
{
    public readonly struct ActiveEffectHandle
    {
        public readonly int InstanceId;
        public readonly EffectSO Effect;

        public ActiveEffectHandle(int instanceId, EffectSO effect)
        {
            InstanceId = instanceId;
            Effect = effect;
        }

        public bool IsValid => InstanceId > 0 && Effect != null;
    }
}
