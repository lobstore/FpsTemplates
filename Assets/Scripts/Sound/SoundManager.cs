using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> audioClipList;
    private AudioSource mainAudioSource;
    // Update is called once per frame
    private void Awake()
    {
        mainAudioSource = GetComponent<AudioSource>();
    }
    private void FixedUpdate()
    {
        if (mainAudioSource.isPlaying == false)
        {
            mainAudioSource.PlayOneShot(audioClipList[Random.Range(0,3)]);
        }
    }
}
