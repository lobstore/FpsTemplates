using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class StaminaController : MonoBehaviour, IStaminaController ,IDisposable
{
    public UnityEvent<float> OnStaminaChanged { get; } = new();
    public UnityEvent<float> OnStaminaDepleted { get; } = new();
    private Stamina stamina;
    public float MaxStamina { get => stamina.MaxStamina; }
    public float CurrentStamina
    {
        get => stamina.CurrentStamina;
    }
    private float regenStamina;
    private float rechargeCooldown;
    private bool isRechargeColdown = false;
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
        InitializeStaminaRecharge();
    }
    public void InitializeStaminaRecharge()
    {
        StaminaReachargeCooldownWaiter();
    }
    public void StopStaminaRecharge()
    {
        cancellationTokensource.Cancel();
    }
    public void Dispose()
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            StopStaminaRecharge();

        }
    }
    public void Update()
    {
        RegenerateStamina();
    }
    async void StaminaReachargeCooldownWaiter()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!isRechargeColdown)
            {
                await Task.Yield();
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(rechargeCooldown));
                isRechargeColdown = false;
            }
        }
    }
    private void RegenerateStamina()
    {
        if (stamina.CurrentStamina < stamina.MaxStamina && !isRechargeColdown)
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
        isRechargeColdown = true;
        stamina.CurrentStamina -= requiredStamina;
        OnStaminaChanged.Invoke(stamina.CurrentStamina);
    }

}
