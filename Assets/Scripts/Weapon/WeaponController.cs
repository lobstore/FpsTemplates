using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public struct CrosshairData
{
    [Tooltip("The image that will be used for this weapon's crosshair")]
    public Sprite CrosshairSprite;

    [Tooltip("The size of the crosshair image")]
    public int CrosshairSize;

    [Tooltip("The color of the crosshair image")]
    public Color CrosshairColor;
}
public class WeaponController : MonoBehaviour
{
    public Transform _camera;
    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();
    Weapon[] m_WeaponSlots = new Weapon[9];
    private int activeWeaponIndex = 0;
    public Weapon activeWeapon { get; private set; }
    [SerializeField] private Transform weaponHolder;
    IWeaponInput _weaponInput;
    bool IsShooting = false;
    bool IsOnCooldown = false;

    public UnityEvent<int> OnAmmoChanged { get; } = new();
    public UnityEvent OnWeaponSwitched {get; } = new();
    public int ActiveWeaponIndex
    {
        get { return activeWeaponIndex; }
        set
        {
            if (value > _weapons.Count)
            {
                activeWeaponIndex = 0;
            }
            else if (value < 0)
            {
                activeWeaponIndex = _weapons.Count - 1;
            }
            else
            {
                activeWeaponIndex = value;
            }
        }
    }

    void Awake()
    {
        _weaponInput = GetComponent<IWeaponInput>();
        _weaponInput.OnWeaponSwitch.AddListener(ScrollWeapon);
        _weaponInput.IsShooting.AddListener(Shoot);
        _weaponInput.OnAmmoReload.AddListener(Reload);
        foreach (var weapon in _weapons)
        {
            AddWeapon(weapon);
        }
        activeWeapon = m_WeaponSlots[activeWeaponIndex];
        ScrollWeapon(0);
    }


    // Update is called once per frame
    void Update()
    {

    }
    private void Reload()
    {
        activeWeapon.Reload();
        OnAmmoChanged.Invoke(activeWeapon.CurrentAmmo);
    }
    private void Shoot(bool buttonPressed)
    {
        if (buttonPressed && !IsOnCooldown)
        {
            IsShooting = true;
            switch (activeWeapon.WeaponType)
            {
                case WeaponType.Auto:
                    Shooting();
                    break;
                case WeaponType.Manual:
                    activeWeapon.Shoot();
                    OnAmmoChanged.Invoke(activeWeapon.CurrentAmmo);
                    _ = SetCooldown();
                    break;
                case WeaponType.Semiauto:
                    break;
                default:
                    break;
            }
        }
        else
        {
            IsShooting = false;
        }
    }
    private async void Shooting()
    {
        while (IsShooting)
        {
            activeWeapon.Shoot();
            OnAmmoChanged.Invoke(activeWeapon.CurrentAmmo);
            await SetCooldown();
        }
    }
    private async Task SetCooldown()
    {
        if (IsOnCooldown == false)
        {
            IsOnCooldown = true;
            await Task.Delay(TimeSpan.FromSeconds(activeWeapon.ShotDelay));
            IsOnCooldown = false;
        }
    }
    private void ScrollWeapon(float direction)
    {
        if (direction > 0)
        {
            ++ActiveWeaponIndex;

        }
        else if (direction < 0)
        {
            --ActiveWeaponIndex;

        }

        while (m_WeaponSlots[ActiveWeaponIndex] == null) { ActiveWeaponIndex++; }
        SwitchWeapon(m_WeaponSlots[ActiveWeaponIndex]);
    }
    public void SwitchWeapon(Weapon nextWeapon)
    {
        activeWeapon.WeaponRoot.SetActive(false);
        nextWeapon.WeaponRoot.SetActive(true);
        activeWeapon = nextWeapon;
        OnWeaponSwitched.Invoke();
    }

    public bool AddWeapon(Weapon weaponPrefab)
    {


        // search our weapon slots for the first free one, assign the weapon to it, and return true if we found one. Return false otherwise
        for (int i = 0; i < m_WeaponSlots.Length; i++)
        {
            // only add the weapon if the slot is free
            if (m_WeaponSlots[i] == null)
            {
                // spawn the weapon prefab as child of the weapon socket
                Weapon weaponInstance = Instantiate(weaponPrefab, weaponHolder);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;

                weaponInstance.WeaponRoot.SetActive(false);


                m_WeaponSlots[i] = weaponInstance;

                return true;
            }
        }

        return false;
    }
}
