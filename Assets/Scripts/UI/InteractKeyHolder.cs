using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractKeyHolder : MonoBehaviour
{
    [SerializeField] Image interactKeyImage;
    WController _wController;
    // Start is called before the first frame update
    private void OnEnable()
    {
        _wController = GetComponent<WController>();
        _wController.IsWeaponInFocus.AddListener(Switch);
    }

    private void Switch(bool isInFocus)
    {
        if (isInFocus)
        {
            interactKeyImage.gameObject.SetActive(true);
        }
        else
        {
            interactKeyImage.gameObject.SetActive(false);
        }
    }
}
