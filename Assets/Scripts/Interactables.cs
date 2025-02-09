using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    public bool finalDoor;
    public bool isReversed;
    private bool hasToggled;
    public List<InteractFlags> flags;
    public string onAnimationName= "Open";
    public string offAnimationName= "Close";

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
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

        if (isAllOn)
        {
            hasToggled = true;
            string animation = !isReversed ^ isAllOn ? offAnimationName : onAnimationName;
            animator.Play(animation);
        }

        else if(hasToggled)
        {
            string animation = !isReversed ^ !isAllOn ? onAnimationName : offAnimationName;
            animator.Play(animation);
        }

    }

}
