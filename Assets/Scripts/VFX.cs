using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("vfx");
    }

    internal void UpdateVolume(float value)
    {
        if(audioSource != null)
            audioSource.volume = value;
    }
}
