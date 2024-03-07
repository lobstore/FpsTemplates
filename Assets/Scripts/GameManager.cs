using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject store;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (store.activeInHierarchy)
            {
                store.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                store.SetActive(true);
            }
        }
    }
}
