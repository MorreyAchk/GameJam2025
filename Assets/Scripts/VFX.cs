using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    private AudioSource audioSource;
    private float previousValue;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("vfx");
        previousValue = audioSource.volume;
    }

    private void Update()
    {
        float settingValue = PlayerPrefs.GetFloat("vfx");
        if (previousValue != settingValue)
        {
            audioSource.volume = settingValue;
            previousValue = settingValue;
        }
    }
}
