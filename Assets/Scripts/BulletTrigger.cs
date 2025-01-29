using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class BulletTrigger : MonoBehaviour
{
    public Powers power;
    public Color color;
    public int maxBounces=8;
    private int bounceCounter;

    public void Set(Powers power,Color color) {
        this.power = power;
        this.color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceCounter++;
        if (!collision.collider.tag.ToLower().Contains("wall") || bounceCounter == maxBounces) {
            Destroy(gameObject);
        }
    }
}
