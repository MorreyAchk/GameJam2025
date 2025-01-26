using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BulletTrigger : MonoBehaviour
{
  public string skill;
  public Color color;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    Controller2d controller2D = collision.GetComponentInParent<Controller2d>();
    if (controller2D != null)
    {
      if (skill == "Bubble")
        controller2D.SetInBubble();

      Destroy(this.gameObject);
    }

        MemoryStoneTrigger ms = collision.GetComponentInParent<MemoryStoneTrigger>();
        if (ms != null)
        {
          ms.Set(skill, color);
          Destroy(this.gameObject);
        }

        BulletTrigger b = collision.GetComponentInParent<BulletTrigger>();
        if (b != null)
          Destroy(this.gameObject);
  }
}
