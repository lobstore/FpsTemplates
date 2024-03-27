using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public enum WeaponType
{
    Melee,
    GrenadeF1,
    GrenadeSmoke,
    GrenadeFlash,
    GrenadeMolotov,
    Pistol,
    Main
}
public class WController : MonoBehaviour
{
    [SerializeField] Character Owner;
    [SerializeField] private List<Weapon> _weaponsOnStart = new List<Weapon>();
    [SerializeField] GameObject WeaponHolder;
    WeaponSlot[] m_WeaponSlots;
    public WeaponSlot ActiveWeaponSlot;
    [SerializeField] private int activeWeaponIndex = 0;
    IWeaponInput _weaponInput;
    [SerializeField] private Weapon pickedweapon;
    
    public UnityEvent<Weapon> OnWeaponSwitched { get; } = new();
    public UnityEvent<bool> IsWeaponInFocus { get; } = new();
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
    // Start is called before the first frame update
    void Awake()
    {
        m_WeaponSlots = WeaponHolder.GetComponentsInChildren<WeaponSlot>();
        Owner = GetComponent<Character>();
        for (int i = 0; i < m_WeaponSlots.Length; i++)
        {
            m_WeaponSlots[i].Owner = Owner;
        }
        _weaponInput = GetComponent<IWeaponInput>();
        _weaponInput.OnWeaponSwitch.AddListener(ScrollWeapon);
        _weaponInput.IsShooting.AddListener(Shoot);
        _weaponInput.OnAmmoReload.AddListener(Reload);
        _weaponInput.OnWeaponDrop.AddListener(DropWeapon);
        _weaponInput.OnWeaponPickUp.AddListener(PickUp);
        ActiveWeaponSlot = m_WeaponSlots[ActiveWeaponIndex];
        for (int i = 0; i < _weaponsOnStart.Count; i++)
        {
            pickedweapon = Instantiate(_weaponsOnStart[i]);
            PickUp();
        }
        pickedweapon = null;
    }

    private void Update()
    {

    }
    private void FixedUpdate()
    {
        RaycastHit hit;

        Physics.Raycast(Owner.cam.transform.position, Owner.cam.transform.forward, out hit, 3f);
        Debug.DrawRay(Owner.cam.transform.position, Owner.cam.transform.forward * 3f);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Weapon>() != null)
            {
                IsWeaponInFocus.Invoke(true);
                pickedweapon = hit.collider.GetComponent<Weapon>();
            }
            else
            {
                IsWeaponInFocus.Invoke(false);
                pickedweapon = null;
            }
        }
        else
        {
            IsWeaponInFocus.Invoke(false);
            pickedweapon = null;
        }
    }
    private void PickUp()
    {
        if (pickedweapon == null)
        {
            return;
        }
        // Если слот такого типа пустой, добавлять оружие, в противном случае выбрасывать оружие и подбирать новое
        for (int i = 0; i < m_WeaponSlots.Length; i++)
        {
            if (m_WeaponSlots[i].WeaponType == pickedweapon.WeaponType)
            {
                if (!m_WeaponSlots[i].IsEmpty)
                {
                    m_WeaponSlots[i].DropWeapon();
                    m_WeaponSlots[i].AddWeapon(pickedweapon.gameObject);
                    
                    if (m_WeaponSlots[i] != ActiveWeaponSlot)
                    {
                        m_WeaponSlots[i].weaponGO.SetActive(false);
                    }
                    else
                    {
                        ActiveWeaponIndex = i;
                        ActiveWeaponSlot = m_WeaponSlots[ActiveWeaponIndex];
                        OnWeaponSwitched.Invoke(m_WeaponSlots[i].Weapon);
                    }
                }
                else
                {
                    m_WeaponSlots[i].AddWeapon(pickedweapon.gameObject);
                    ActiveWeaponIndex = i;
                    SwitchWeapon(m_WeaponSlots[ActiveWeaponIndex]);
                }

                break;
            }
        }

    }
    private void DropWeapon()
    {
        ActiveWeaponSlot.DropWeapon();
        ScrollWeapon(-1);
    }
    private void ScrollWeapon(float direction)
    {
        int emptyslots = 0;
        foreach (var item in m_WeaponSlots)
        {
            if (item.IsEmpty)
            {
                emptyslots++;
            }
        }
        if (emptyslots == m_WeaponSlots.Length)
            return;

        if (direction > 0)
        {
            ++ActiveWeaponIndex;
            while (m_WeaponSlots[ActiveWeaponIndex].IsEmpty) { ++ActiveWeaponIndex; }
        }
        else if (direction < 0)
        {
            --ActiveWeaponIndex;
            while (m_WeaponSlots[ActiveWeaponIndex].IsEmpty) { --ActiveWeaponIndex; }
        }



        SwitchWeapon(m_WeaponSlots[ActiveWeaponIndex]);

    }

    private async void GettingUpWeapon()
    {
        ActiveWeaponSlot.Weapon.IsOnCooldown = true;
        await Task.Delay(TimeSpan.FromSeconds(1f));
        ActiveWeaponSlot.Weapon.IsOnCooldown = false;
    }
    public void SwitchWeapon(WeaponSlot nextWeapon)
    {
        if (ActiveWeaponSlot.Weapon != null)
        {
            ActiveWeaponSlot.Weapon.gameObject.SetActive(false);
        }
        ActiveWeaponSlot = nextWeapon;
       // GettingUpWeapon();
        ActiveWeaponSlot.Weapon.gameObject.SetActive(true);
        OnWeaponSwitched.Invoke(ActiveWeaponSlot.Weapon);
    }
    private void Reload()
    {
        if (ActiveWeaponSlot != null)
            ActiveWeaponSlot.Weapon.Reload();
    }
    private void Shoot(bool buttonPressed)
    {
        if (ActiveWeaponSlot.Weapon != null)
            ActiveWeaponSlot.Weapon.Shoot(buttonPressed);
    }

}

public interface IInteractable
{
}