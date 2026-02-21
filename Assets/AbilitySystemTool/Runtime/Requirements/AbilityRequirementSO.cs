using UnityEngine;

namespace AbilitySystemTool
{
    public abstract class AbilityRequirementSO : ScriptableObject
    {
        public abstract bool IsMet(in AbilityCastContext castContext, out CastFailReason failReason);
    }
}