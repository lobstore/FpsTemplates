using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour
{

    protected AudioSource _audioSource;
    protected AudioClip shot_Clip;
    protected AudioClip reload_Clip;
    private float DelayBeforDecreaseSpread { get; set; } = 0.2f;
    protected GameObject HolePrefab { get; set; }
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
    public int MaxAmmo { get => maxAmmo; protected set => maxAmmo = value; }
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
    public float MinSpread { get; protected set; }
    public float MaxSpread { get; protected set; }
    private float currentSpread;
    public float CurrentSpread
    {
        get { return currentSpread; }
        protected set
        {
            if (value > MaxSpread)
            {
                currentSpread = MaxSpread;
            }
            else if (value <= MinSpread)
            {
                currentSpread = MinSpread;
            }
            else
            {
                currentSpread = value;
            }
        }
    }
    protected int Damage { get; set; }
    public bool IsShooting { get; set; } = false;
    public bool IsOnCooldown { get; set; } = true;
    protected bool IsReloading { get; set; } = false;

    public float SpreadEasing { get; protected set; } = 1f;

    [SerializeField]
    WeaponConfig config;
    protected float TimeSinceLastShot { get; set; }

    private void Awake()
    {
        if (config != null)
        {
            HolePrefab = config.HolePrefab;
            MaxAmmo = config.MaxAmmo;
            MinSpread = config.MinSpread;
            MaxSpread = config.MaxSpread;
            MaxRange = config.MaxRange;
            Damage = config.Damage;
            WeaponName = config.WeaponName;
            ShotDelay = config.ShotDelay;
            CurrentAmmo = MaxAmmo;
            CurrentSpread = MinSpread;
            shot_Clip = config.ShotClip;
            reload_Clip = config.ReloadClip;
        }
        CameraTransform = Camera.main.transform;
        _audioSource = GetComponent<AudioSource>();
    }
    public virtual async void Reload()
    {
        if (!IsReloading && CurrentAmmo < MaxAmmo)
        {
            if (reload_Clip != null)
            {
                _audioSource.PlayOneShot(reload_Clip);
                IsReloading = true;
                await Task.Delay(TimeSpan.FromSeconds(reload_Clip.length));

            }
            CurrentAmmo = MaxAmmo;
            OnAmmoChanged.Invoke(CurrentAmmo);
            IsReloading = false;
        }
    }
    public abstract void Shoot(bool isShooting);
    public virtual async Task SetShootCooldown()
    {
        if (IsOnCooldown == false)
        {
            IsOnCooldown = true;
            await Task.Delay(TimeSpan.FromSeconds(ShotDelay));
            IsOnCooldown = false;
        }
    }
    private void Update()
    {
        if (Owner.Velocity.magnitude>0) {
            CurrentSpread += (1f+Owner.Velocity.magnitude) * Time.deltaTime;
            Debug.Log(CurrentSpread);
        }
        if (CurrentSpread > MinSpread)
        {

            TimeSinceLastShot += Time.deltaTime;

            if (TimeSinceLastShot > DelayBeforDecreaseSpread)
            {
                float reductionAmount = SpreadEasing * Time.deltaTime;
                CurrentSpread = Mathf.Max(CurrentSpread - reductionAmount, MinSpread);

            }
        }
    }
    public void CreateBulletHole(Vector3 position)
    {

        GameObject bulletHole = Instantiate(HolePrefab, position, Quaternion.identity);
    }
}
