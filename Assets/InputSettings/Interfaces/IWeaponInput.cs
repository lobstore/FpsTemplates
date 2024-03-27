using UnityEngine.Events;

public interface IWeaponInput
{
    public UnityEvent<bool> IsShooting { get; }
    public UnityEvent<float> OnWeaponSwitch { get; }
    public UnityEvent OnWeaponDrop { get; }
    public UnityEvent OnWeaponPickUp { get; }
    public UnityEvent OnAmmoReload { get; }
    
}