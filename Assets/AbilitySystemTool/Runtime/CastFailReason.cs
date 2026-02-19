namespace AbilitySystemTool
{
  public enum CastFailReason : byte
  {
    None = 0,
    Invalid = 1,
    NoTargetAbilitySystem = 2,
    OnCooldown = 3
  }
}