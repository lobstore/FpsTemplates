using UnityEngine;
using UnityEngine.Events;

public enum WeaponType
{
    Auto,
    Manual,
    Semiauto
}
public abstract class Weapon : MonoBehaviour
{
    [field: SerializeField]
    public WeaponType WeaponType { get; set; }
    protected int maxAmmo;
    protected int currentAmmo;
    [field: SerializeField]
    public GameObject WeaponRoot { get; protected set; }
    public UnityEvent<float> OnShot { get; } = new();
    public UnityEvent OnAmmoExpired { get; } = new();

    public float ShotDelay { get; protected set; }
    public float MaxRange { get; protected set; }
    public int MaxAmmo { get=>maxAmmo; protected set=>maxAmmo=value; }
    public int CurrentAmmo
    {
        get { return currentAmmo; }
        protected set
        {
            if (value > maxAmmo)
            {
                currentAmmo = maxAmmo;
            }
            else if (value <= 0)
            {
                currentAmmo = 0;
            }
            else
            {
                currentAmmo = value;
            }

        }
    }
    public float MinRecoil { get; protected set; }
    public float MaxRecoil { get; protected set; }
    protected int Damage { get; set; }

    public abstract void Reload();
    public abstract void Shoot();
}
