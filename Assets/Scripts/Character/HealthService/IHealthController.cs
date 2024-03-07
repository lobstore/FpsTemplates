using UnityEngine.Events;

public interface IHealthController
{
    UnityEvent OnHealthGone { get; set; }
    UnityEvent<float> OnHealthChanged { get; set; }
    float MaxHP { get; }
    float CurrentHP { get; }
}