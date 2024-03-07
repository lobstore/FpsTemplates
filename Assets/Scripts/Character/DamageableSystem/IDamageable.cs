using UnityEngine.Events;

public interface IDamageable
{
    public UnityEvent<float> OnTakeDamage { get; set; } 
    public void TakeDamage(float damageTaken);
}