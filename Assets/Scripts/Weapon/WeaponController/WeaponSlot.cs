using System.Linq;
using UnityEngine;
public class WeaponSlot : MonoBehaviour
{
    public Character Owner;
    [SerializeField] private int MaxWeapons;
    public bool IsEmpty = true;
    public WeaponType WeaponType;
    public GameObject weaponGO;
    public Weapon Weapon;
    public void DropWeapon()
    {
        Weapon.enabled = false;
        Weapon.Owner = null;
        Weapon = null;
        weaponGO.SetActive(true);
        weaponGO.layer = 0;
        weaponGO.transform.SetParent(null, true);
        weaponGO.GetComponent<Rigidbody>().isKinematic = false;
        weaponGO.GetComponent<Collider>().enabled = true;
        weaponGO = null;
        IsEmpty = true;
    }
    public void AddWeapon(GameObject weapon)
    {
        if (GetComponentsInChildren<Weapon>().Count() >= MaxWeapons)
        {
            DropWeapon();
        }
        weapon.transform.SetParent(gameObject.transform);
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.GetComponent<Collider>().enabled = false;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        weaponGO = weapon;
        weaponGO.layer=12;
        Weapon = weapon.GetComponent<Weapon>();
        Weapon.Owner = Owner;
        Weapon.enabled = true;
        IsEmpty = false;
    }
}
