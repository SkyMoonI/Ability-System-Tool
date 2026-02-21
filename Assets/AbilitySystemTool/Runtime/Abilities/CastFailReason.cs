namespace AbilitySystemTool
{
  public enum CastFailReason : byte
  {
    None = 0,
    Invalid = 1,
    NoTargetAbilitySystem = 2,
    OnCooldown = 3,
    MisconfiguredAbility = 4,
    RequirementFailed = 5,
    MissingResourceComponent = 6,
    NotEnoughResource = 7,
    CostFailed = 8
  }
}