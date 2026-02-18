using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    [CreateAssetMenu(menuName = "Ability System Tool/Ability", fileName = "newAbility")]
    public class AbilitySO : ScriptableObject
    {
        [SerializeField] private List<EffectSO> _effectList = new List<EffectSO>();
        public List<EffectSO> EffectList => _effectList;
    }
}