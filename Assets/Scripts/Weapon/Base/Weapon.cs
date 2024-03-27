using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]

public abstract class Weapon : MonoBehaviour, IInteractable
{

    //public bool IsDropped = false;
    public WeaponType WeaponType;
    protected WeaponRecoil _recoilPattern;
    protected WeaponMagazine _magazine;
    protected Animator _animator;
    protected AudioSource _audioSource;
    protected AudioClip shot_Clip;
    protected AudioClip reload_Clip;
    protected CinemachineImpulseSource _cameraShake;
    protected GameObject HolePrefab { get; set; }
    public Character Owner;
    public UnityEvent<int> OnCurrentAmmoChanged { get; } = new();
    public UnityEvent<int> OnAmountAmmoChanged { get; } = new();
    public string WeaponName { get; protected set; }
    [field: SerializeField]

    public UnityEvent<Transform> OnShot { get; } = new();
    public UnityEvent OnAmmoExpired { get; } = new();
    public UnityEvent OnWeaponNotAvailable { get; } = new();

    public float ShotDelay { get; protected set; }
    public float ReloadTime { get; protected set; }
    public float MaxRange { get; protected set; }
    public int MaxAmmoInMagazine { get => _magazine.MaxAmmoInMagazine; }
    public int AmmoAmount { get => _magazine.AmmoAmount; }
    public int CurrentAmmo
    {
        get { return _magazine.CurrentAmmo; }
    }

    protected int Damage { get; set; }
    public bool IsShooting = false;
    public bool IsOnCooldown = false;
    [SerializeField] protected bool IsReloading = false;

    public bool IsAvailable = true;
    private Rigidbody _weaponRigidbody;
    private Collider _weaponColider;

    [SerializeField]
    WeaponConfig config;
    protected float TimeSinceLastShot { get; set; }

    private void Awake()
    {

        if (config != null)
        {
            enabled = false;
            _weaponRigidbody = GetComponent<Rigidbody>();
            _weaponColider = GetComponent<Collider>();
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _magazine = new WeaponMagazine(config);
            HolePrefab = config.HolePrefab;

            MaxRange = config.MaxRange;
            Damage = config.Damage;
            WeaponName = config.WeaponName;
            ShotDelay = config.ShotDelay;

            shot_Clip = config.ShotClip;
            reload_Clip = config.ReloadClip;
            ReloadTime = config.ReloadTime;
            _recoilPattern = new WeaponRecoil(config);
            _cameraShake = GetComponent<CinemachineImpulseSource>();
            IsAvailable = true;
        }

    }
    private void OnEnable()
    {
        IsReloading = false;
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
            _recoilPattern.ResetRecoil();
            _recoilPattern.ResetSpread();
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
            RaycastHit hit;
            Vector3 nextPosition = _recoilPattern.NextPosition();
            Vector3 horizontalDir = nextPosition.x * Owner.cam.transform.right;
            Vector3 verticalDir = nextPosition.y * Owner.cam.transform.up;
            Vector3 shootDir = horizontalDir + verticalDir;
            Physics.Raycast(Owner.cam.transform.position, Owner.cam.transform.forward + shootDir, out hit, MaxRange);
            if (hit.collider != null && hit.collider.GetComponent<Character>() != Owner)
            {
                CreateBulletHole(hit);
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().TakeDamage(Damage);
                }
            }
            _magazine.CurrentAmmo--;
            OnCurrentAmmoChanged.Invoke(_magazine.CurrentAmmo);
            TimeSinceLastShot = 0;
            _audioSource.PlayOneShot(shot_Clip);
            _recoilPattern.CurrentSpread += _recoilPattern.RecoilForce;
            if (_cameraShake != null)
            {
                _cameraShake.GenerateImpulse(Owner.cam.transform.up);
            }
            else
            {
                Debug.LogWarning("There is no CinemachineImpulseSource on this Weapon");
            }

        }
        else if (_magazine.CurrentAmmo <= 0 && !IsReloading)
        {
            Reload();
            OnAmmoExpired.Invoke();
        }
    }
    virtual protected void Update()
    {
        TimeSinceLastShot += Time.deltaTime;

        if (Owner.Velocity.magnitude > 0)
        {
            _recoilPattern.CurrentSpread += (_recoilPattern.SpreadEasing + Owner.Velocity.magnitude) * Time.deltaTime;
        }
        else if (_recoilPattern.CurrentSpread > _recoilPattern.MinSpread)
        {
            if (TimeSinceLastShot > _recoilPattern.DelayBeforDecreaseSpread)
            {
                _recoilPattern.ResetRecoil();
                float reductionAmount = _recoilPattern.SpreadEasing * Time.deltaTime;
                _recoilPattern.CurrentSpread = Mathf.Max(_recoilPattern.CurrentSpread - reductionAmount, _recoilPattern.MinSpread);

            }
        }
    }
    public void CreateBulletHole(RaycastHit hit)
    {
        if (hit.collider.GetComponent<ShotHolesSpawner>() != null)
        {

            hit.collider.GetComponent<ShotHolesSpawner>().TakeShot(hit);
        }
        else
        {
            // GameObject bulletHole = Instantiate(HolePrefab, hit.point, Quaternion.identity);

        }

    }
}
