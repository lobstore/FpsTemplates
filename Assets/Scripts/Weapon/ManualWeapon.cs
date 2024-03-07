using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualWeapon : Weapon
{
    public override void Shoot(bool isShooting)
    {
        if (!IsOnCooldown && isShooting)
        {
            if (CurrentAmmo > 0)
            {
                Debug.DrawRay(CameraTransform.position, CameraTransform.transform.forward * MaxRange, Color.red);
                RaycastHit hit;
                Physics.Raycast(CameraTransform.position, CameraTransform.transform.forward, out hit, MaxRange);
                if (hit.collider != null && hit.collider.GetComponent<IDamageable>() != null && hit.collider.GetComponent<Character>() != Owner)
                {
                    hit.collider.GetComponent<IDamageable>().TakeDamage(Damage);
                }
                CurrentAmmo--;
                OnAmmoChanged.Invoke(CurrentAmmo);
                _ = SetCooldown();
            }
            else
            {
                OnAmmoExpired.Invoke();
            }
        }
    }
}
