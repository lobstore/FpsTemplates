using UnityEngine.Events;

public interface IStaminaController
{
    UnityEvent<float> OnStaminaChanged { get; }
    UnityEvent<float> OnStaminaDepleted { get; }
    float MaxStamina { get; }
    float CurrentStamina { get; }

    public void ReduceStyamina(float requiredStamina);
    public bool HasEnoughStamina(float requiredStamina);
}