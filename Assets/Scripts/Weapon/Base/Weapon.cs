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
    protected Camera FpsCamera { get; set; }

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
            FpsCamera = Camera.main;
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
            _recoilPattern.Reset();
            _recoilPattern.CurrentSpread = _recoilPattern.MinSpread;
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
            Physics.Raycast(FpsCamera.transform.position, FpsCamera.transform.forward + shootDir, out hit, MaxRange);
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
            _audioSource.PlayOneShot(shot_Clip);
            _recoilPattern.CurrentSpread += 0.05f;
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
        if (_recoilPattern.CurrentSpread > _recoilPattern.MinSpread)
        {
            if (TimeSinceLastShot > DelayBeforDecreaseSpread)
            {
                float reductionAmount = _recoilPattern.SpreadEasing * Time.deltaTime;
                _recoilPattern.CurrentSpread = Mathf.Max(_recoilPattern.CurrentSpread - reductionAmount, _recoilPattern.MinSpread);

            }
        }
    }
    public void CreateBulletHole(Vector3 position)
    {

        GameObject bulletHole = Instantiate(HolePrefab, position, Quaternion.identity);
    }
}
