using System.Threading;
using UnityEngine;
using UnityEngine.Events;
public class StaminaController : MonoBehaviour, IStaminaController
{
    public UnityEvent<float> OnStaminaChanged { get; } = new();
    public UnityEvent<float> OnStaminaDepleted { get; } = new();
    private Stamina stamina;
    public float MaxStamina { get => stamina.MaxStamina; }
    public float CurrentStamina
    {
        get => stamina.CurrentStamina;
    }
    public float TimeSinceLastConsume { get; private set; }

    private float regenStamina;
    private float rechargeCooldown;
    private CancellationTokenSource cancellationTokensource = new CancellationTokenSource();
    CancellationToken cancellationToken;

    [SerializeField]
    PlayerConfig playerConfig;
    public void Awake()
    {
        rechargeCooldown = playerConfig.RechargeCooldown;
        regenStamina = playerConfig.StaminaRegen;
        stamina = new Stamina(playerConfig.MaxStamina);
        cancellationToken = cancellationTokensource.Token;
    }

    public void Update()
    {
        if (CurrentStamina < MaxStamina)
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
        if (stamina.CurrentStamina < stamina.MaxStamina)
        {
            stamina.CurrentStamina += regenStamina * Time.deltaTime;
            OnStaminaChanged.Invoke(stamina.CurrentStamina);
        }
    }
    public bool HasEnoughStamina(float requiredStamina)
    {
        return CurrentStamina > requiredStamina;
    }
    public void ReduceStyamina(float requiredStamina)
    {
        stamina.CurrentStamina -= requiredStamina;
        OnStaminaChanged.Invoke(stamina.CurrentStamina);
        TimeSinceLastConsume = 0;
    }

}
