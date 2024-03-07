using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponStatusHandler : MonoBehaviour
{
    Weapon prevWeapon;
    WeaponController _weaponController;
    [SerializeField] TextMeshProUGUI ammoText;
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
            prevWeapon?.OnAmmoChanged.AddListener(DrawAmmo);
            ammoText.text = prevWeapon?.CurrentAmmo.ToString();
        }
    }
    private void ApplyChangeWeapon(Weapon nextWeapon)
    {
        if (_weaponController != null)
        {
            prevWeapon?.OnAmmoChanged.RemoveListener(DrawAmmo);
            nextWeapon?.OnAmmoChanged.AddListener(DrawAmmo);
            ammoText.text = nextWeapon?.CurrentAmmo.ToString();
            prevWeapon = nextWeapon;
        }
    }
    private void DrawAmmo(int ammoAmount)
    {
        if (_weaponController != null)
        {
            ammoText.text = ammoAmount.ToString();
        }
    }
}
