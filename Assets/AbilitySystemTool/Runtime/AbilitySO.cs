using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    [CreateAssetMenu(menuName = "Ability System Tool/Ability", fileName = "newAbility")]
    public class AbilitySO : ScriptableObject
    {
        [SerializeField] private List<EffectSO> _effectList = new List<EffectSO>();
        public IReadOnlyList<EffectSO> EffectList => _effectList;

        [SerializeField] private float _cooldownSeconds = 0f;
        public float CooldownSeconds => _cooldownSeconds;

        [SerializeField] private List<AbilityRequirementSO> _requirementList = new List<AbilityRequirementSO>();
        [SerializeField] private List<AbilityCostSO> _costList = new List<AbilityCostSO>();
        public IReadOnlyList<AbilityRequirementSO> RequirementList => _requirementList;
        public IReadOnlyList<AbilityCostSO> CostList => _costList;

        private void OnValidate()
        {
            _cooldownSeconds = Mathf.Max(0f, _cooldownSeconds);
        }
    }
}