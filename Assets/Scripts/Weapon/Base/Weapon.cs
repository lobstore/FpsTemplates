using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    public Character Owner;
    public UnityEvent<int> OnAmmoChanged { get; } = new();
    public string WeaponName { get; protected set; }
    protected Transform CameraTransform { get; set; }
    protected int maxAmmo;
    protected int currentAmmo;
    [field: SerializeField]
    public GameObject WeaponRoot { get; protected set; }
    public UnityEvent<float> OnShot { get; } = new();
    public UnityEvent OnAmmoExpired { get; } = new();

    public float ShotDelay { get; protected set; }
    public float MaxRange { get; protected set; }
    public int MaxAmmo { get=>maxAmmo; protected set=>maxAmmo=value; }
    public int CurrentAmmo
    {
        get { return currentAmmo; }
        protected set
        {
            if (value > maxAmmo)
            {
                currentAmmo = maxAmmo;
            }
            else if (value <= 0)
            {
                currentAmmo = 0;
            }
            else
            {
                currentAmmo = value;
            }

        }
    }
    public float MinRecoil { get; protected set; }
    public float MaxRecoil { get; protected set; }
    protected int Damage { get; set; }
    protected bool IsShooting { get; set; } = false;
    protected bool IsOnCooldown {get;set; } = false;
    [SerializeField]
    WeaponConfig config;

    private void Awake()
    {
        if (config != null)
        {
            MaxAmmo = config.MaxAmmo;
            MinRecoil = config.MinRecoil;
            MaxRecoil = config.MaxRecoil;
            MaxRange = config.MaxRange;
            Damage = config.Damage;
            WeaponName = config.WeaponName;
            ShotDelay = config.ShotDelay;
            CurrentAmmo = MaxAmmo;
        }
        CameraTransform = Camera.main.transform;
    }
    public virtual void Reload()
    {
        CurrentAmmo = MaxAmmo;
        OnAmmoChanged.Invoke(CurrentAmmo);
    }
    public abstract void Shoot(bool isShooting);
    public virtual async Task SetCooldown()
    {
        if (IsOnCooldown == false)
        {
            IsOnCooldown = true;
            await Task.Delay(TimeSpan.FromSeconds(ShotDelay));
            IsOnCooldown = false;
        }
    }
}
