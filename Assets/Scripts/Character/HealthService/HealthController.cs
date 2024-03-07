using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour, IHealthController, IDisposable
{
    public UnityEvent OnHealthGone { get; set; } = new();
    public UnityEvent<float> OnHealthChanged { get; set; } = new();
    private Health _health;
    [SerializeField]
    private PlayerConfig _playerConfig;
    private IDamageable _damageable;
    public float MaxHP { get => _health.MaxHP; }
    public float CurrentHP
    {
        get => _health.CurrentHP;
    }
    public void Awake()
    {
        _damageable = GetComponent<IDamageable>();
        _health = new Health(_playerConfig.MaxHealth);
        _damageable.OnTakeDamage.AddListener(TakeDamage);
    }


    public void Dispose()
    {
        _damageable.OnTakeDamage.RemoveListener(TakeDamage);
    }
    private void TakeDamage(float damage)
    {
        _health.CurrentHP -= damage;
        OnHealthChanged.Invoke(_health.CurrentHP);
        if (CurrentHP <= 0)
        {
            OnHealthGone.Invoke();
        }
    }


}
