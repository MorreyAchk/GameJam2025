using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Controller2d controller2D = collision.GetComponentInParent<Controller2d>();
        if (controller2D != null)
        {
            controller2D.OnSpike();
        }
    }
}
