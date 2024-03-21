using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualWeapon : Weapon
{
    public override void Shoot(bool isShooting)
    {

        _recoilPattern.Reset();
        if (isShooting)
        {
            IsShooting = true;
            base.Shoot(isShooting);
        }
        else
        {
            IsShooting = false;
        }
    }
}
