using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusHandler : MonoBehaviour, IDisposable
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider staminaSlider;
    private IStaminaController staminaController;
    private IHealthController healthController;
    private void OnEnable()
    {
        Initialize();
    }
    private void OnDisable()
    {
        healthController?.OnHealthChanged.RemoveListener(DrawHealth);
        staminaController?.OnStaminaChanged.RemoveListener(DrawStamina);
    }

    public void Dispose()
    {
        healthController?.OnHealthChanged.RemoveListener(DrawHealth);
        staminaController?.OnStaminaChanged.RemoveListener(DrawStamina);
    }
    public void DrawStamina(float stamina)
    {
        if (staminaController != null)
            staminaSlider.value = stamina / staminaController.MaxStamina;

    }
    public void DrawHealth(float health)
    {
        if (healthController != null)
            healthSlider.value = health / healthController.MaxHP;
    }
    private void Initialize()
    {
        staminaController = GetComponent<IStaminaController>();
        healthController = GetComponent<IHealthController>();
        if (healthController != null)
        {
            healthController.OnHealthChanged.AddListener(DrawHealth);
            healthSlider.value = healthController.CurrentHP / healthController.MaxHP;
        }
        if (staminaController != null)
        {
            staminaController.OnStaminaChanged.AddListener(DrawStamina);
            staminaSlider.value = staminaController.CurrentStamina / staminaController.MaxStamina;
        }
    }
}
