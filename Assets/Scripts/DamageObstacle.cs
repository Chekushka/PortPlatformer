using Player;
using UnityEngine;

public class DamageObstacle : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageCooldown = 1f; // Пауза між ударами
    [SerializeField] private bool destroyOnTouch = false; // Чи зникає перепона (як снаряд)

    private float _nextDamageTime;
    
    private void OnTriggerStay(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        Debug.Log($"Collided with {other.gameObject.name}");
        TryDealDamage(other.gameObject);
    }

    private void TryDealDamage(GameObject target)
    {
        // Перевіряємо, чи пройшов час відкату
        if (Time.time < _nextDamageTime) return;
        // Шукаємо компонент Health на об'єкті, з яким зіткнулися
        if (target.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
            _nextDamageTime = Time.time + damageCooldown;

            if (destroyOnTouch)
            {
                Destroy(gameObject);
            }
        }
    }
}