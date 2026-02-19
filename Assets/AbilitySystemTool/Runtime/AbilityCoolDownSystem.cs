using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystemTool
{
    public sealed class AbilityCooldownSystem
    {
        private readonly List<CooldownEntry> _activeCooldownList;
        private readonly Dictionary<AbilitySO, int> _indexByAbilityDictionary;

        public AbilityCooldownSystem(int initialCapacity = 16)
        {
            _activeCooldownList = new List<CooldownEntry>(initialCapacity);
            _indexByAbilityDictionary = new Dictionary<AbilitySO, int>(initialCapacity);
        }

        public bool IsOnCooldown(AbilitySO ability)
        {
            if (ability == null) return false;

            if (_indexByAbilityDictionary.TryGetValue(ability, out int index))
            {
                return _activeCooldownList[index].remaining > 0f;
            }

            return false;
        }

        public float GetRemainingCooldown(AbilitySO ability)
        {
            if (ability == null) return 0f;

            if (_indexByAbilityDictionary.TryGetValue(ability, out int index))
            {
                float remaining = _activeCooldownList[index].remaining;
                return remaining > 0f ? remaining : 0f;
            }

            return 0f;
        }

        /// <summary>
        /// Starts cooldown if ability has cooldown > 0 and not already on cooldown.
        /// Returns true if cooldown is considered started (or cooldown is 0).
        /// </summary>
        public bool TryStartCooldown(AbilitySO ability)
        {
            if (ability == null) return false;

            float cooldown = ability.CooldownSeconds;
            if (cooldown <= 0f) return true; // no cooldown, always available

            if (_indexByAbilityDictionary.ContainsKey(ability)) return false; // already on cooldown

            int index = _activeCooldownList.Count;
            _activeCooldownList.Add(new CooldownEntry(ability, cooldown));
            _indexByAbilityDictionary.Add(ability, index);

            return true;
        }

        public void Update(float deltaTime)
        {
            if (deltaTime <= 0f) return;
            if (_activeCooldownList.Count == 0) return;

            UpdateActiveCooldowns(deltaTime);
        }

        private void UpdateActiveCooldowns(float deltaTime)
        {
            for (int i = _activeCooldownList.Count - 1; i >= 0; i--)
            {
                CooldownEntry cooldownEntry = _activeCooldownList[i];
                cooldownEntry.remaining -= deltaTime;

                if (cooldownEntry.remaining > 0f)
                {
                    _activeCooldownList[i] = cooldownEntry;
                    continue;
                }

                // Expired: remove via swap-back
                RemoveAtSwapBack(i);
            }
        }

        private void RemoveAtSwapBack(int index)
        {
            int lastIndex = _activeCooldownList.Count - 1;
            AbilitySO removedAbility = _activeCooldownList[index].ability;

            if (index != lastIndex)
            {
                CooldownEntry lastEntry = _activeCooldownList[lastIndex];
                _activeCooldownList[index] = lastEntry;
                _indexByAbilityDictionary[lastEntry.ability] = index;
            }

            _activeCooldownList.RemoveAt(lastIndex);
            _indexByAbilityDictionary.Remove(removedAbility);
        }

        private struct CooldownEntry
        {
            public AbilitySO ability;
            public float remaining;

            public CooldownEntry(AbilitySO ability, float remaining)
            {
                this.ability = ability;
                this.remaining = remaining;
            }
        }

    }

}