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
    [SerializeField] private List<Weapon> _weaponsOnStart = new List<Weapon>();
    Weapon[] m_WeaponSlots = new Weapon[9];
    private int activeWeaponIndex = 0;
    public Weapon ActiveWeapon { get; private set; }
    [SerializeField] private Transform weaponHolder;
    IWeaponInput _weaponInput;
    public UnityEvent<Weapon> OnWeaponSwitched { get; } = new();
    public int ActiveWeaponIndex
    {
        get { return activeWeaponIndex; }
        set
        {
            if (value > m_WeaponSlots.Length - 1)
            {
                activeWeaponIndex = 0;
            }
            else if (value < 0)
            {
                activeWeaponIndex = m_WeaponSlots.Length - 1;
            }
            else
            {
                activeWeaponIndex = value;
            }
        }
    }

    void Awake()
    {
        Character Owner = GetComponent<Character>();
        _weaponInput = GetComponent<IWeaponInput>();
        _weaponInput.OnWeaponSwitch.AddListener(ScrollWeapon);
        _weaponInput.IsShooting.AddListener(Shoot);
        _weaponInput.OnAmmoReload.AddListener(Reload);
        if (_weaponsOnStart.Count > 0)
        {
            foreach (var weapon in _weaponsOnStart)
            {
                weapon.Owner = Owner;
                AddWeapon(weapon);
            }
            ActiveWeapon = m_WeaponSlots[activeWeaponIndex];
            ScrollWeapon(0);
        }

    }

    private void Reload()
    {
        if (ActiveWeapon != null)
            ActiveWeapon.Reload();
    }
    private void Shoot(bool buttonPressed)
    {
        if (ActiveWeapon != null)
            ActiveWeapon.Shoot(buttonPressed);
    }

    private void ScrollWeapon(float direction)
    {
        bool InventoryIsNotEmpty = false;
        for (int i = 0; i < m_WeaponSlots.Length; i++)
        {
            if (m_WeaponSlots[i] != null)
            {
                InventoryIsNotEmpty = true;
            }
        }
        if (InventoryIsNotEmpty)
        {


            if (direction > 0)
            {
                ++ActiveWeaponIndex;
                while (m_WeaponSlots[ActiveWeaponIndex] == null) { ++ActiveWeaponIndex; }
            }
            else if (direction < 0)
            {
                --ActiveWeaponIndex;
                while (m_WeaponSlots[ActiveWeaponIndex] == null) { --ActiveWeaponIndex; }
            }

            SwitchWeapon(m_WeaponSlots[ActiveWeaponIndex]);
        }
        else
        {
            return;
        }
    }
    public void SwitchWeapon(Weapon nextWeapon)
    {
        ActiveWeapon.WeaponRoot.SetActive(false);
        GettingUpWeapon();
        nextWeapon.WeaponRoot.SetActive(true);
        ActiveWeapon = nextWeapon;
        OnWeaponSwitched.Invoke(ActiveWeapon);
    }
    private async void GettingUpWeapon()
    {
        ActiveWeapon.IsOnCooldown = true;
        await Task.Delay(TimeSpan.FromSeconds(1f));
        ActiveWeapon.IsOnCooldown = false;
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
