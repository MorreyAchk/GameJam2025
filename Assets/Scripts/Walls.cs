using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Walls : MonoBehaviour
{
    public List<DoorFlags> flags;
    public bool startOpen;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (startOpen) {
            animator.Play("Open");
        }
        else
        {
            animator.Play("Close");
        }
    }

    private void Update()
    {
        if (flags.Count>0)
        {
            if (flags.All(flag => flag.isOn))
            {
                animator.Play("Open");
            }
            else
            {
                animator.Play("Close");
            }
        }
    }

}
