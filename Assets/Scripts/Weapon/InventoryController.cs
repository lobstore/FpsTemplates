using UnityEngine;
using UnityEngine.Events;
public class InventoryController : MonoBehaviour, IInventoryController
{
    private int money = 24000000;
    public int Money
    {
        get => money;
        set
        {
            if (value < 0)
            {
                money = 0;
            }
            else
            {
                money = value;
            }

        }
    }
    public UnityEvent<Weapon> OnWeaponChanged { get; } = new();
    IWeaponInput InputHandler;
    public GameObject[] weapons; // Массив префабов оружия
    public GameObject currentWeaponGO; // Текущее оружие игрока
    Weapon currentWeapon;

    [SerializeField]
    GameObject weaponHolder;
    int currentWeaponIndex = 0;
    int CurrentWeaponIndex
    {
        get { return currentWeaponIndex; }
        set
        {
            if (value > weapons.Length - 1)
            {
                currentWeaponIndex = 0;
            }
            else if (value < 0)
            {
                currentWeaponIndex = weapons.Length - 1;
            }
            else
            {
                currentWeaponIndex = value;
            }
        }
    }
    private void Awake()
    {
        InputHandler = GetComponent<IWeaponInput>();
        InputHandler.OnWeaponSwitch.AddListener(ScrollWeapon);
        InputHandler.OnWeaponDrop.AddListener(DropWeapon);
        CurrentWeaponIndex = 0;
        currentWeaponGO = weapons[CurrentWeaponIndex];
    }
    private void Start()
    {
        ScrollWeapon(0);
    }
    public void SwitchWeapon(GameObject newWeaponPrefab)
    {
        if (currentWeaponGO != null)
        {
            Debug.Log(currentWeaponGO);
            DestroyImmediate(currentWeaponGO);
        }
        GameObject newWeapon = Instantiate(newWeaponPrefab, weaponHolder.transform);

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.SetActive(true);
        currentWeaponGO = newWeapon;
        currentWeapon = currentWeaponGO.GetComponent<Weapon>();
    }
    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public void DropWeapon()
    {
        if (CurrentWeaponIndex != 0 && currentWeaponGO != null)
        {
            weapons[CurrentWeaponIndex] = null;
            GameObject droppedInstance = Instantiate(currentWeaponGO, weaponHolder.transform.position, weaponHolder.transform.rotation);
            droppedInstance.GetComponent<Collider>().enabled = true;
            droppedInstance.GetComponent<Rigidbody>().useGravity = true;
            droppedInstance.GetComponent<Rigidbody>().AddForce(weaponHolder.transform.forward * 5f, ForceMode.Impulse);

            DestroyImmediate(currentWeaponGO);
            ScrollWeapon(-1);
        }
    }


    public void AddWeapon(GameObject newWeapon)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = newWeapon;
                CurrentWeaponIndex = i;
                break;
            }
            else if (weapons[i] != null && i == weapons.Length - 1)
            {
                DropWeapon();
                weapons[i] = newWeapon;
                CurrentWeaponIndex = i;
                break;
            }
        }
        SwitchWeapon(newWeapon);
    }
    private void ScrollWeapon(float direction)
    {
        if (direction > 0)
        {
            ++CurrentWeaponIndex;

        }
        else if (direction < 0)
        {
            --CurrentWeaponIndex;

        }

        while (weapons[CurrentWeaponIndex] == null) { CurrentWeaponIndex++; }
        SwitchWeapon(weapons[CurrentWeaponIndex]);
    }
}
