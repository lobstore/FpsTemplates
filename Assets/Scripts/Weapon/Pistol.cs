using UnityEngine;

public class Pistol : Weapon
{
    private void Awake()
    {
        Damage = 50;
        MaxRange = 1000f;
        MaxAmmo = 50;
        MaxRecoil = 0.5f;
        MinRecoil = 0.02f;
        ShotDelay = 0.05f;
        CurrentAmmo = MaxAmmo;
    }
    public override void Shoot()
    {
        if (CurrentAmmo > 0)
        {
            OnShot.Invoke(Damage);
            CurrentAmmo--;
        }
    }
    public override void Reload()
    {
        CurrentAmmo = MaxAmmo;
    }


}
