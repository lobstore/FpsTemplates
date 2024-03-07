using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EyeTrigger : MonoBehaviour
{
    public UnityEvent<Vector3> OnPlayerContact { get; set; } = new();
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Character>() != null)
        {
            OnPlayerContact.Invoke(other.transform.position);
        }
    }
}
