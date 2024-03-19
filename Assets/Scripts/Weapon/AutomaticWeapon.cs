using UnityEngine;
using UnityEngine.UIElements;

public class AutomaticWeapon : Weapon
{
    public override void Shoot(bool isShooting)
    {
        if (!IsOnCooldown && isShooting)
        {
            IsShooting = true;
            Shooting();
        }
        else
        {
            IsShooting = false;
        }
    }
    private async void Shooting()
    {
        while ( IsShooting &&!IsReloading)
        {
            if (CurrentAmmo > 0)
            {
                Debug.DrawRay(CameraTransform.position + new Vector3(Random.Range(-CurrentSpread, CurrentSpread), Random.Range(-CurrentSpread, CurrentSpread)), CameraTransform.transform.forward * MaxRange, Color.red);
                RaycastHit hit;
                Physics.Raycast(CameraTransform.position + new Vector3(Random.Range(-CurrentSpread,CurrentSpread), Random.Range(-CurrentSpread, CurrentSpread)), CameraTransform.transform.forward, out hit, MaxRange);
                if (hit.collider != null && hit.collider.GetComponent<Character>()!=Owner)
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
            }
            else
            {
                TimeSinceLastShot = 0;
                OnAmmoExpired.Invoke();
            }
            await SetShootCooldown();
        }

    }

}
