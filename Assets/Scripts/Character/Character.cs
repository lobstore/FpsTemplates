using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable
{
    public Vector3 Velocity { get => _controller.velocity; }
    CharacterController _controller;
    public UnityEvent<float> OnTakeDamage { get; set; } = new UnityEvent<float>();
    private void Start ()
    {
        _controller = GetComponent<CharacterController>();
        GetComponent<IHealthController>()?.OnHealthGone.AddListener(PlayDead);
    }

    private void PlayDead()
    {
        gameObject.SetActive(false);
    }
    public void TakeDamage(float damageTaken)
    {
        Debug.Log("TakenDamage");
        OnTakeDamage?.Invoke(damageTaken);
    }
}
