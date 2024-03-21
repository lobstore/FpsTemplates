using UnityEngine;

public class AutomaticWeapon : Weapon
{
    public override void Shoot(bool isShooting)
    {

        if (isShooting)
        {
            IsShooting = true;
        }
        else
        {
            IsShooting = false;
        }

    }

    override protected void Update()
    {
        base.Update();
        if (IsShooting )
        {
            base.Shoot(IsShooting);

        }
    }

}
