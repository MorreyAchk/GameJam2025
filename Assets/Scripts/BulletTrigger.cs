using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BulletTrigger : MonoBehaviour
{
    public Powers power;
    public Color color;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BubbleInteractable bubbleInteractable = collision.GetComponentInParent<BubbleInteractable>();
        if (bubbleInteractable != null)
        {
            if (power == Powers.Bubble)
            {
                bubbleInteractable.isInBubble = true;
                Destroy(this.gameObject);
            }
            if (power == Powers.Wind)
            {
                bubbleInteractable.MoveBubble(collision.transform.position);
                Destroy(this.gameObject);
            }

        }

        MemoryStoneTrigger ms = collision.GetComponentInParent<MemoryStoneTrigger>();
        if (ms != null)
        {
            ms.Set(power, color);
            Destroy(this.gameObject);
        }

    }
}
