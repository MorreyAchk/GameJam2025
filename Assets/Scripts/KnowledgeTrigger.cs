using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KnowledgeTrigger : MonoBehaviour
{
	private bool isTriggered;
	public string skill;

	private void OnTriggerEnter2D(Collider2D collision)
    {
        //Controller2d controller2D = collision.GetComponentInParent<Controller2d>();
        //if (controller2D != null && !isTriggered)
        //{
        //    isTriggered = true;
        //    controller2D.OnKnowledge(skill);
        //    SpriteRenderer sr = GetComponent<SpriteRenderer>();
        //    Color c = sr.color;
        //    c.a = .3f;
        //    sr.color = c;
        //}
    }
}
