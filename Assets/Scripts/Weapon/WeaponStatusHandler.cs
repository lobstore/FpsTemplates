using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponStatusHandler : MonoBehaviour
{
    WeaponController _weaponController;
    [SerializeField] TextMeshProUGUI ammoText;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Initialize();
    }
    private void OnDisable()
    {
        _weaponController?.OnAmmoChanged.RemoveListener(DrawAmmo);
        _weaponController?.OnWeaponSwitched.RemoveListener(DrawNextWeaponStatus);
    }
    private void Initialize()
    {
        _weaponController = GetComponent<WeaponController>();
        if (_weaponController != null)
        {
            _weaponController.OnWeaponSwitched.AddListener(DrawNextWeaponStatus);
            _weaponController.OnAmmoChanged.AddListener(DrawAmmo);
            ammoText.text = _weaponController.activeWeapon.CurrentAmmo.ToString();
        }
    }
    private void DrawNextWeaponStatus()
    {
        if (_weaponController != null)
        {
            ammoText.text = _weaponController.activeWeapon.CurrentAmmo.ToString();
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
