using UnityEngine.Events;

public interface IInventoryController
{
    UnityEvent<Weapon> OnWeaponChanged { get; }

    Weapon GetCurrentWeapon();
}