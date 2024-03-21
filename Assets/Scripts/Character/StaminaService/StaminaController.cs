using System.Threading;
using UnityEngine;
using UnityEngine.Events;
public class StaminaController : MonoBehaviour, IStaminaController
{
    public UnityEvent<float> OnStaminaChanged { get; } = new();
    public UnityEvent<float> OnStaminaDepleted { get; } = new();
    private Stamina _stamina;
    public float MaxStamina { get => _stamina.MaxStamina; }
    public float CurrentStamina
    {
        get => _stamina.CurrentStamina;
    }
    public float TimeSinceLastConsume { get; private set; }

    private float regenStamina;
    private float rechargeCooldown;

    [SerializeField]
    PlayerConfig playerConfig;
    public void Awake()
    {
        rechargeCooldown = playerConfig.RechargeCooldown;
        regenStamina = playerConfig.StaminaRegen;
        _stamina = new Stamina(playerConfig);

    }

    public void Update()
    {
        if (_stamina.CurrentStamina < _stamina.MaxStamina)
        {
            TimeSinceLastConsume += Time.deltaTime;

            if (TimeSinceLastConsume > rechargeCooldown)
            {
                RegenerateStamina();

            }
        }

    }
    private void RegenerateStamina()
    {
        if (_stamina.CurrentStamina < _stamina.MaxStamina)
        {
            _stamina.CurrentStamina += regenStamina * Time.deltaTime;
            OnStaminaChanged.Invoke(_stamina.CurrentStamina);
        }
    }
    public bool HasEnoughStamina(float requiredStamina)
    {
        return _stamina.CurrentStamina > requiredStamina;
    }
    public void ReduceStyamina(float requiredStamina)
    {
        _stamina.CurrentStamina -= requiredStamina;
        OnStaminaChanged.Invoke(_stamina.CurrentStamina);
        TimeSinceLastConsume = 0;
    }

}
