using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryStoneTrigger : MonoBehaviour
{
    public string skill;
    public Color color = Color.white;

    public void Awake()
    {
      Set(skill, color);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Controller2d controller2D = collision.GetComponentInParent<Controller2d>();
        if (controller2D != null)
        {
          if (skill == "Bubble")
            controller2D.SetInBubble();
        }
    }

    public void Set(string skill, Color color)
    {
      this.skill = skill;
      SpriteRenderer sr = GetComponent<SpriteRenderer>();
      sr.color = this.color = color;
    }
}
