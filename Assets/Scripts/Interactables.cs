using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    public bool finalDoor;
    private bool previousValue;
    public List<InteractFlags> flags;
    public string onAnimationName= "Open";
    public string offAnimationName= "Close";

    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.Play(offAnimationName);
    }

    private void Update()
    {
        bool isAllOn = flags.All(flag => flag.isOn.Value);

        if (flags.Count == 0 || (finalDoor && isAllOn))
        {
            animator.Play(onAnimationName);
            flags.Clear();
            return;
        }
        if (isAllOn == previousValue)
            return;

        previousValue = isAllOn;
        animator.Play(isAllOn ? onAnimationName : offAnimationName);
        audioSource.PlayOneShot(audioSource.clip);
    }

}
