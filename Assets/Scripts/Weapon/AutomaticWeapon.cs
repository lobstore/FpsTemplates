using UnityEngine;

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
        while ( IsShooting)
        {
            if (CurrentAmmo > 0)
            {
                Debug.DrawRay(CameraTransform.position, CameraTransform.transform.forward * MaxRange, Color.red);
                RaycastHit hit;
                Physics.Raycast(CameraTransform.position, CameraTransform.transform.forward, out hit, MaxRange);
                if (hit.collider != null && hit.collider.GetComponent<IDamageable>() != null && hit.collider.GetComponent<Character>()!=Owner)
                {
                    hit.collider.GetComponent<IDamageable>().TakeDamage(Damage);
                }
                CurrentAmmo--;
                OnAmmoChanged.Invoke(CurrentAmmo);

            }
            else
            {
                OnAmmoExpired.Invoke();
            }
            await SetCooldown();
        }
    }
}
