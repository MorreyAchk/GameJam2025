using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private BubbleInteractable bubbleInteractable;
    private bool wasInBubble;
    private void Start()
    {
        bubbleInteractable = GetComponent<BubbleInteractable>();
    }

    private void Update()
    {
        if (bubbleInteractable != null && !wasInBubble) {
            wasInBubble = bubbleInteractable.isInBubble;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (wasInBubble && collision.collider.CompareTag("DestroyableWalls"))
        {
            Destroy(collision.gameObject);
        }
    }
}
