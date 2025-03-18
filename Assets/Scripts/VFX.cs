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
        AudioReverbFilter audioReverb = gameObject.AddComponent<AudioReverbFilter>();
        audioReverb.dryLevel = 0;
        audioReverb.room = -1000;
        audioReverb.roomHF = -100;
        audioReverb.roomLF = 0;
        audioReverb.decayTime = 1.49f;
        audioReverb.decayHFRatio = 0.83f;
        audioReverb.reflectionsLevel = -2602;
        audioReverb.reflectionsDelay = 0;
        audioReverb.reverbLevel = 25;
        audioReverb.reverbDelay = 0.011f;
        audioReverb.hfReference = 5000;
        audioReverb.lfReference = 250;
        audioReverb.diffusion = 100;
        audioReverb.density = 50;
    }

    internal void UpdateVolume(float value)
    {
        if(audioSource != null)
            audioSource.volume = value;
    }
}
