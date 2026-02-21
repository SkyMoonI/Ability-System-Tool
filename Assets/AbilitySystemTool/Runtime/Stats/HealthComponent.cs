using UnityEngine;

namespace AbilitySystemTool
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField][Min(1f)] private float _maxHealth;
        private float _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void Reset()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(float damageAmount)
        {
            if (damageAmount <= 0f) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damageAmount);
            RuntimeLogger.Log($"Health: {_currentHealth} / {_maxHealth}");
        }
    }
}