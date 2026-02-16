using System.Collections.Generic;
using UnityEngine;


namespace AbilitySystemTool
{

    [CreateAssetMenu(menuName = "Ability System Tool/Ability", fileName = "newAbility")]
    public class AbilitySO : ScriptableObject
    {
        public List<EffectSO> effectList = new List<EffectSO>();
    }
}