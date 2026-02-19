using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    [CreateAssetMenu(menuName = "Ability System Tool/Ability", fileName = "newAbility")]
    public class AbilitySO : ScriptableObject
    {
        [SerializeField] private List<EffectSO> _effectList = new List<EffectSO>();
        public List<EffectSO> EffectList => _effectList;

        [SerializeField] private float _cooldownSeconds = 0f;
        public float CooldownSeconds => _cooldownSeconds;

        private void OnValidate()
        {
            _cooldownSeconds = Mathf.Max(0f, _cooldownSeconds);
        }
    }
}