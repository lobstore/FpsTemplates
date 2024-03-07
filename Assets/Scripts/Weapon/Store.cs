using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField]
    List<GameObject> weaponList;
    [SerializeField]
    InventoryController _inventoryController;

    Dictionary<string, GameObject> weapDict = new();
    public void Buy(string weaponName)
    {
        _inventoryController.AddWeapon(weapDict[weaponName]);
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var weapon in weaponList)
        {
            weapDict[weapon.name] = weapon;
        }
    }

}
