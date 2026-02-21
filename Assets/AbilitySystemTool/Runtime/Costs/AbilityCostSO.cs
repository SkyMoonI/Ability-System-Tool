using UnityEngine;

namespace AbilitySystemTool
{
    public abstract class AbilityCostSO : ScriptableObject
    {
        public abstract bool CanPay(in AbilityCastContext castContext, out CastFailReason failReason);
        public abstract void Pay(in AbilityCastContext castContext);
    }
}