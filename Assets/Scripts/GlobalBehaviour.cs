using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalBehaviour : MonoBehaviour
{
    private static GlobalBehaviour instance;
    public AudioSource audioSource, audioSourceBubble;
    public Image brightness;

    public static GlobalBehaviour Instance => instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
