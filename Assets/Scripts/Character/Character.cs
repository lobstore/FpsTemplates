using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable
{
    public UnityEvent<float> OnTakeDamage { get; set; } = new UnityEvent<float>();
    private void Start ()
    {
        GetComponent<IHealthController>().OnHealthGone.AddListener(PlayDead);
    }

    private void PlayDead()
    {
        gameObject.SetActive(false);
    }
    public void TakeDamage(float damageTaken)
    {
        Debug.Log("TakenDamage");
        OnTakeDamage.Invoke(damageTaken);
    }
}
