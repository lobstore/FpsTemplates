using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] shot;
    AudioSource shotSource;
    // Start is called before the first frame update
    private void Awake()
    {
        shotSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        if (shot!=null)
        {
            shotSource.PlayOneShot(shot[Random.Range(0,shot.Length)]);

        }
    }
    
}
