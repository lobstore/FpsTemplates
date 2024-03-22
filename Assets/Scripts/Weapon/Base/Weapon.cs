using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public abstract class Weapon : MonoBehaviour
{
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
    protected Transform FpsCamera { get; set; }

    [field: SerializeField]
    public GameObject WeaponRoot { get; protected set; }
    public UnityEvent<Transform> OnShot { get; } = new();
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

    protected int Damage { get; set; }
    public bool IsShooting { get; set; } = false;
    public bool IsOnCooldown { get; set; } = true;
    protected bool IsReloading { get; set; } = false;



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

            MaxRange = config.MaxRange;
            Damage = config.Damage;
            WeaponName = config.WeaponName;
            ShotDelay = config.ShotDelay;

            shot_Clip = config.ShotClip;
            reload_Clip = config.ReloadClip;
            ReloadTime = config.ReloadTime;
            FpsCamera = Camera.main.transform;
            _recoilPattern = new WeaponRecoil(config);
            _cameraShake = GetComponent<CinemachineImpulseSource>();
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
            Vector3 horizontalDir = nextPosition.x * FpsCamera.transform.right;
            Vector3 verticalDir = nextPosition.y * FpsCamera.transform.up;
            Vector3 shootDir = horizontalDir + verticalDir;
            Physics.Raycast(FpsCamera.position, FpsCamera.forward + shootDir, out hit, MaxRange);
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
            _cameraShake.GenerateImpulse(FpsCamera.up);
        }
        else if (_magazine.CurrentAmmo <= 0 && !IsReloading)
        {
            Reload();
            OnAmmoExpired.Invoke();
        }
    }
    virtual protected void Update()
    {
        Debug.Log(_recoilPattern.CurrentSpread);
        TimeSinceLastShot += Time.deltaTime;
        if (Owner.Velocity.magnitude > 0)
        {
            _recoilPattern.CurrentSpread += (_recoilPattern.SpreadEasing + Owner.Velocity.magnitude) * Time.deltaTime;
        }else if (_recoilPattern.CurrentSpread > _recoilPattern.MinSpread)
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
        if (hit.collider.GetComponent<ShotHolesSpawner>()!=null)
        {

         hit.collider.GetComponent<ShotHolesSpawner>().TakeShot(hit.point);
        }
        else
        {
           // GameObject bulletHole = Instantiate(HolePrefab, hit.point, Quaternion.identity);

        }
      
    }
}
