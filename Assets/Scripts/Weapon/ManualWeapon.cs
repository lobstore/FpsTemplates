using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualWeapon : Weapon
{
    public override void Shoot(bool isShooting)
    {
        if (!IsOnCooldown && !IsReloading && isShooting)
        {
            IsShooting = true;
            if (CurrentAmmo > 0)
            {
                Debug.DrawRay(CameraTransform.position + new Vector3(Random.Range(-CurrentSpread, CurrentSpread), Random.Range(-CurrentSpread, CurrentSpread)), CameraTransform.transform.forward * MaxRange, Color.red);
                RaycastHit hit;
                Physics.Raycast(CameraTransform.position + new Vector3(Random.Range(-CurrentSpread, CurrentSpread), Random.Range(-CurrentSpread, CurrentSpread)), CameraTransform.transform.forward, out hit, MaxRange);
                if (hit.collider != null && hit.collider.GetComponent<Character>() != Owner)
                {
                    CreateBulletHole(hit.point);
                    if (hit.collider.GetComponent<IDamageable>() != null)
                    {
                        hit.collider.GetComponent<IDamageable>().TakeDamage(Damage);
                    }
                }
                CurrentAmmo--;
                OnAmmoChanged.Invoke(CurrentAmmo);
                TimeSinceLastShot = 0;
                CurrentSpread += 0.05f;
                _audioSource.PlayOneShot(shot_Clip);
                _ = SetShootCooldown();
            }
            else
            {
                TimeSinceLastShot = 0;
                OnAmmoExpired.Invoke();
            }
        }
        else
        {
            IsShooting = false;
        }
    }
}
