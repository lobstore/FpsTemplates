using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponStatusHandler : MonoBehaviour
{
    Weapon prevWeapon;
    WeaponController _weaponController;
    [SerializeField] TextMeshProUGUI currentAmmoText;
    [SerializeField] TextMeshProUGUI amountAmmoText;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Initialize();
    }
    private void OnDisable()
    {
        _weaponController?.OnWeaponSwitched.RemoveListener(ApplyChangeWeapon);
    }
    private void Initialize()
    {
        _weaponController = GetComponent<WeaponController>();
        if (_weaponController != null)
        {
            _weaponController.OnWeaponSwitched.AddListener(ApplyChangeWeapon);
            prevWeapon = _weaponController.ActiveWeapon;
            prevWeapon?.OnCurrentAmmoChanged.AddListener(DrawCurrentAmmo);
            prevWeapon?.OnAmountAmmoChanged.AddListener(DrawAmountAmmo);
            currentAmmoText.text = prevWeapon?.CurrentAmmo.ToString();
            amountAmmoText.text = prevWeapon?.AmmoAmount.ToString();
        }
    }
    private void ApplyChangeWeapon(Weapon nextWeapon)
    {
        if (_weaponController != null)
        {
            prevWeapon?.OnCurrentAmmoChanged.RemoveListener(DrawCurrentAmmo);
            prevWeapon?.OnAmountAmmoChanged.RemoveListener(DrawAmountAmmo);
            nextWeapon?.OnCurrentAmmoChanged.AddListener(DrawCurrentAmmo);
            nextWeapon?.OnAmountAmmoChanged.AddListener(DrawAmountAmmo);
            currentAmmoText.text = nextWeapon?.CurrentAmmo.ToString();
            amountAmmoText.text = nextWeapon?.AmmoAmount.ToString();
            prevWeapon = nextWeapon;
        }
    }
    private void DrawCurrentAmmo(int ammoAmount)
    {
        if (_weaponController != null)
        {
            currentAmmoText.text = ammoAmount.ToString();
        }
    }
    private void DrawAmountAmmo(int ammoAmount)
    {
        if (_weaponController != null)
        {
            amountAmmoText.text = ammoAmount.ToString();
        }
    }
}
