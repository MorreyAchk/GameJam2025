using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    private AudioSource audioSource;
    private float previousValue;
    public AudioClip[] variations;

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

    public void PlaySound()
    {
        AudioClip clip = variations[Random.Range(0, variations.Length)];
        audioSource.pitch = Random.Range(0.8f, 1.2f);

        audioSource.PlayOneShot(clip);

        StartCoroutine(ResetPitch());
    }

    private IEnumerator ResetPitch()
    {
        yield return new WaitForSeconds(0.1f);
        audioSource.pitch = 1.0f;
    }
}
