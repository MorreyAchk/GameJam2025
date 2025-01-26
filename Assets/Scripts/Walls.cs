using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Walls : MonoBehaviour
{
    public bool finalDoor;
    public bool isReversed;
    private bool hasToggled;
    public List<DoorFlags> flags;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool isAllOn = flags.All(flag => flag.isOn);

        if (flags.Count == 0 || (finalDoor && isAllOn))
        {
            animator.Play("Open");
            flags.Clear();
            return;
        }

        if (isAllOn)
        {
            hasToggled = true;
            string animation = !isReversed ^ isAllOn ? "Close" : "Open";
            animator.Play(animation);
        }

        else if(hasToggled)
        {
            string animation = !isReversed ^ !isAllOn ? "Open":"Close" ;
            animator.Play(animation);
        }

    }

}
