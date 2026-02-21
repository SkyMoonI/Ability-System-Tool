using System.Collections.Generic;

namespace AbilitySystemTool
{
    public static class EffectValidation
    {
        public static void Validate(EffectSO effect, List<ValidationMessage> outMessages)
        {
            if (outMessages == null) return;
            if (effect == null)
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Error, $"EffectSO: is null.", effect));
                return;
            }

            // effect has no effect action. it will not do any effect
            if (effect.EffectActionSO == null)
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Warning, $"EffectSO '{effect.name}': has no effect action.", effect));
            }

            if (effect.HasTick)
            {
                if (effect.EffectDuration == 0f)
                {
                    outMessages.Add(new ValidationMessage(ValidationSeverity.Warning, $"EffectSO '{effect.name}': has tick enabled but has no duration.", effect));
                }

                if (effect.TickInterval <= 0f)
                {
                    outMessages.Add(new ValidationMessage(ValidationSeverity.Warning, $"EffectSO '{effect.name}': has tick enabled but has no tick interval.", effect));
                }
            }

            if (effect.StackingPolicy != StackingPolicy.Replace && effect.StackingPolicy != StackingPolicy.Refresh && effect.StackingPolicy != StackingPolicy.Stack)
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Warning, $"EffectSO '{effect.name}': has unknown stacking policy.", effect));
            }

            if (effect.EffectDuration <= 0f && effect.StackingPolicy != StackingPolicy.Stack) // Stack için duration 0 belki anlamlı olabilir? senin design’a göre
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Warning, $"EffectSO '{effect.name}': has {effect.StackingPolicy} but duration is 0.", effect));
            }
        }
    }
}