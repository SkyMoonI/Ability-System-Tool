using UnityEngine;

namespace AbilitySystemTool
{
    public sealed class StatsComponent : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float _baseMoveSpeed = 10f;

        private float _moveSpeedMultiplier = 1f;

        public float BaseMoveSpeed => _baseMoveSpeed;
        public float MoveSpeedMultiplier => _moveSpeedMultiplier;
        public float CurrentMoveSpeed => _baseMoveSpeed * _moveSpeedMultiplier;

        // Multiplies current move speed by a factor (e.g. 0.7f for slow).
        public void MultiplyMoveSpeed(float factor)
        {
            if (factor <= 0f) return;
            _moveSpeedMultiplier *= factor;
        }

        // Reverts a previous MultiplyMoveSpeed by dividing the multiplier.
        public void DivideMoveSpeed(float factor)
        {
            if (factor <= 0f) return;
            _moveSpeedMultiplier /= factor;
        }
    }
}