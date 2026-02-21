using UnityEngine;

namespace AbilitySystemTool
{
    [CreateAssetMenu(menuName = "Ability System Tool/Costs/Flat Resource Cost", fileName = "newFlatResourceCost")]
    public class FlatResourceCostSO : AbilityCostSO
    {
        [SerializeField] private ResourceIdSO _resourceId;
        [SerializeField] private float _costAmount;

        public override bool CanPay(in AbilityCastContext castContext, out CastFailReason failReason)
        {
            if (_resourceId == null || _costAmount < 0f)
            {
                failReason = CastFailReason.MisconfiguredAbility;
                return false;
            }

            if (castContext.CasterResourceComponent == null)
            {
                failReason = CastFailReason.MissingResourceComponent;
                return false;
            }

            if (!castContext.CasterResourceComponent.HasAndEnoughResource(_resourceId, _costAmount))
            {
                failReason = CastFailReason.NotEnoughResource;
                return false;
            }

            failReason = CastFailReason.None;
            return true;
        }

        public override void Pay(in AbilityCastContext castContext)
        {
            if (castContext.CasterResourceComponent == null) return;
            castContext.CasterResourceComponent.TryConsume(_resourceId, _costAmount, out float _, out float _);
        }
    }
}