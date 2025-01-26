using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFlags : MonoBehaviour
{
    public bool isOn;
    public bool isPlate;
    private Animator animator;

    private AudioClip clip;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            isOn = !isOn;
            animator.SetBool("ToggleValue", isOn);
            //GlobalBehaviour.Instance.audioSource.PlayOneShot(GlobalBehaviour.Instance.GetLeverClip(), .2f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isPlate && collision.CompareTag("Player")) {
            isOn=false;
            animator.SetBool("ToggleValue", false);
            //GlobalBehaviour.Instance.audioSource.PlayOneShot(GlobalBehaviour.Instance.GetLeverClip(), .2f);
        }
    }
}
