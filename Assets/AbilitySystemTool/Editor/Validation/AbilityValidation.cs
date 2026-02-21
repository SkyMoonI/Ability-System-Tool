using System.Collections.Generic;

namespace AbilitySystemTool
{
    public static class AbilityValidation
    {
        public static void Validate(AbilitySO ability, List<ValidationMessage> outMessages)
        {
            if (outMessages == null) return;

            if (ability == null)
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Error, $"AbilitySO '{ability.name}': is null.", ability));
                return;
            }

            if (ability.EffectList == null || ability.EffectList.Count == 0)
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Error, $"AbilitySO '{ability.name}' has no effects.", ability));
                return;
            }

            HashSet<EffectSO> effectSet = new HashSet<EffectSO>();
            for (int i = 0; i < ability.EffectList.Count; i++)
            {
                EffectSO effect = ability.EffectList[i];

                // ability has no effect. it will not do any effect
                if (effect == null)
                {
                    outMessages.Add(new ValidationMessage(ValidationSeverity.Warning, $"AbilitySO '{ability.name}': effect at index {i} is null.", ability));
                }
                else
                {
                    // contains same effect
                    if (effectSet.Contains(effect))
                    {
                        outMessages.Add(new ValidationMessage(ValidationSeverity.Info, $"EffectSO '{effect.name}': is duplicated.", ability));
                    }
                    else
                    {
                        effectSet.Add(effect);
                    }
                }
            }

            if (ability.CooldownSeconds < 0f)
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Error, $"AbilitySO '{ability.name}': CooldownSeconds cannot be negative.", ability));
            }

            if (ability.CooldownSeconds > 300f)
            {
                outMessages.Add(new ValidationMessage(ValidationSeverity.Warning, $"AbilitySO '{ability.name}': CooldownSeconds is too high ({ability.CooldownSeconds})s.", ability));
            }
        }
    }
}