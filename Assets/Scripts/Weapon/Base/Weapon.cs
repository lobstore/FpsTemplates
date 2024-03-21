using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Random = UnityEngine.Random;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public abstract class Weapon : MonoBehaviour
{
    protected RecoilPattern _recoilPattern;
    protected WeaponMagazine _magazine;
    protected Animator _animator;
    protected AudioSource _audioSource;
    protected AudioClip shot_Clip;
    protected AudioClip reload_Clip;
    private float DelayBeforDecreaseSpread { get; set; } = 0.2f;
    protected GameObject HolePrefab { get; set; }
    public Character Owner;
    public UnityEvent<int> OnCurrentAmmoChanged { get; } = new();
    public UnityEvent<int> OnAmountAmmoChanged { get; } = new();
    public string WeaponName { get; protected set; }
    [field: SerializeField] 
    protected Transform CameraTransform { get; set; }

    [field: SerializeField]
    public GameObject WeaponRoot { get; protected set; }
    public UnityEvent<float> OnShot { get; } = new();
    public UnityEvent OnAmmoExpired { get; } = new();

    public float ShotDelay { get; protected set; }
    public float ReloadTime { get; protected set; }
    public float MaxRange { get; protected set; }
    public int MaxAmmoInMagazine { get => _magazine.MaxAmmoInMagazine; }
    public int AmmoAmount { get => _magazine.AmmoAmount; }
    public int CurrentAmmo
    {
        get { return _magazine.CurrentAmmo; }
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
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _magazine = new WeaponMagazine(config);
            HolePrefab = config.HolePrefab;
            MinSpread = config.MinSpread;
            MaxSpread = config.MaxSpread;
            MaxRange = config.MaxRange;
            Damage = config.Damage;
            WeaponName = config.WeaponName;
            ShotDelay = config.ShotDelay;
            CurrentSpread = MinSpread;
            shot_Clip = config.ShotClip;
            reload_Clip = config.ReloadClip;
            ReloadTime = config.ReloadTime;
            CameraTransform = Camera.main.transform;
            _recoilPattern = new RecoilPattern(config);
        }

    }
    private void OnEnable()
    {
        _animator.SetBool("Reloading", false);
    }
    private void OnDisable()
    {
        IsReloading = false;
        StopAllCoroutines();
    }
    public virtual void Reload()
    {
        if (!IsReloading && _magazine.CurrentAmmo < _magazine.MaxAmmoInMagazine)
        {
            StartCoroutine(Reloading());
        }
    }
    protected virtual IEnumerator Reloading()
    {
        if (_magazine.AmmoAmount > 0)
        {
            _audioSource.PlayOneShot(reload_Clip);
            IsReloading = true;
            _animator?.SetBool("Reloading", true);
            yield return new WaitForSeconds(reload_Clip.length - 0.25f);
            _animator?.SetBool("Reloading", false);
            yield return new WaitForSeconds(0.25f);
            IsReloading = false;
            _magazine.ReloadMagazine();
            OnCurrentAmmoChanged.Invoke(_magazine.CurrentAmmo);
            OnAmountAmmoChanged.Invoke(_magazine.AmmoAmount);
        }
    }
    public virtual void Shoot(bool isShooting)
    {
        if (_magazine.CurrentAmmo > 0 && TimeSinceLastShot >= ShotDelay && !IsOnCooldown && !IsReloading)
        {
            Debug.DrawRay(CameraTransform.position + _recoilPattern.NextPosition(), CameraTransform.transform.forward * MaxRange, Color.red);
            RaycastHit hit;
            Physics.Raycast(CameraTransform.position + _recoilPattern.NextPosition(), CameraTransform.transform.forward, out hit, MaxRange);
            if (hit.collider != null && hit.collider.GetComponent<Character>() != Owner)
            {
                CreateBulletHole(hit.point);
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().TakeDamage(Damage);
                }
            }
            _magazine.CurrentAmmo--;
            OnCurrentAmmoChanged.Invoke(_magazine.CurrentAmmo);
            TimeSinceLastShot = 0;
            CurrentSpread += 0.05f;
            _audioSource.PlayOneShot(shot_Clip);
        }
        else if (_magazine.CurrentAmmo <= 0 && !IsReloading)
        {
            StartCoroutine(Reloading());
            OnAmmoExpired.Invoke();
        }
    }
    virtual protected void Update()
    {
        TimeSinceLastShot += Time.deltaTime;
        if (Owner.Velocity.magnitude > 0)
        {
            CurrentSpread += (1f + Owner.Velocity.magnitude) * Time.deltaTime;
        }
        if (CurrentSpread > MinSpread)
        {
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
