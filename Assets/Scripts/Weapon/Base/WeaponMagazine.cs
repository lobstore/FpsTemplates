public class WeaponMagazine
{
    private int ammoAmount;
    private int maxAmmoInMagazine;
    private int currentAmmo;
    public int AmmoAmount
    {
        get => ammoAmount;
        private set
        {
            if (value <= 0)
            {
                ammoAmount = 0;
            }
            else
            {
                ammoAmount = value;
            }
        }
    }
    public int MaxAmmoInMagazine { get => maxAmmoInMagazine; private set => maxAmmoInMagazine = value; }
    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set
        {
            if (value > maxAmmoInMagazine)
            {
                currentAmmo = maxAmmoInMagazine;
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
    public void ReloadMagazine()
    {
        int lack = MaxAmmoInMagazine - CurrentAmmo;
        
        if (AmmoAmount>=lack)
        {
            CurrentAmmo += lack;
        }
        else
        {
            CurrentAmmo = AmmoAmount;
        }
        AmmoAmount -= lack;
    }
    public WeaponMagazine(WeaponConfig config)
    {
        AmmoAmount = config.AmmoAmount;
        MaxAmmoInMagazine = config.MaxAmmoInMagazine;
        CurrentAmmo = MaxAmmoInMagazine;
    }
}