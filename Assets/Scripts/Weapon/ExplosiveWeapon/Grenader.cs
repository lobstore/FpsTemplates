using System.Collections;
using UnityEngine;

public class Grenader : Weapon
{

    public GameObject Grenade;
    [SerializeField] float minThrowForce;
    [SerializeField] float maxThrowForce;
    [SerializeField] float currentThrowForce;
    private bool IsCharging;

    public float CurrentThrowForce
    {
        get => currentThrowForce;
        set
        {
            if (value > maxThrowForce)
            {
                currentThrowForce = maxThrowForce;
            }
            else if (value < minThrowForce)
            {
                currentThrowForce = minThrowForce;
            }
            else
            {
                currentThrowForce = value;
            }
        }
    }

    public override void Shoot(bool isShooting)
    {
        if (CurrentAmmo > 0)
        {
            if (isShooting)
            {
                IsCharging = true;
            }
            else
            {
                IsCharging = false;
                Throw();
            }
        }
    }
    private void OnEnable()
    {
        CurrentThrowForce = minThrowForce;
    }
    private void OnDisable()
    {
        CurrentThrowForce = minThrowForce;

    }
    private void Throw()
    {
        _magazine.CurrentAmmo--;
        OnCurrentAmmoChanged.Invoke(_magazine.CurrentAmmo);
        TimeSinceLastShot = 0;
        GameObject grnd = Instantiate(Grenade, Owner.cam.transform.position + Owner.cam.transform.forward * 1f, Owner.cam.transform.rotation);
        var rb = grnd.GetComponent<Rigidbody>();
        rb.AddForce(Owner.cam.transform.forward * currentThrowForce, ForceMode.VelocityChange);
        CurrentThrowForce = minThrowForce;
        if (CurrentAmmo<=0)
        {
            IsAvailable = false;
            OnWeaponNotAvailable.Invoke();
        }
    }
    override protected void Update()
    {
        TimeSinceLastShot += Time.deltaTime;
        if (IsCharging)
        {
            CurrentThrowForce += Time.deltaTime * 10;
        }
    }


}
