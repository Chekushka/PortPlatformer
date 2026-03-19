using System;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int _currentHealth;

        public event Action OnDamageTaken;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnDamageTaken?.Invoke();
            if (_currentHealth <= 0) Die();
        }
        
        private void Die()
        {
            Debug.Log("Player has died.");
        }
    }
}
