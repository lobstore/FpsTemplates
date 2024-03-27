using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] public Camera cam;
    IWeaponInput _weaponInput;
    public Vector3 Velocity { get => _controller.velocity; }
    CharacterController _controller;
    WController _weaponController;
    public UnityEvent<float> OnTakeDamage { get; set; } = new UnityEvent<float>();
    private void Start ()
    {
        _weaponController= GetComponent<WController>();
        _controller = GetComponent<CharacterController>();
        GetComponent<IHealthController>()?.OnHealthGone.AddListener(PlayDead);
    }
    private void FixedUpdate()
    {


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
