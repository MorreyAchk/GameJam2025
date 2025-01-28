using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BulletTrigger : MonoBehaviour
{
    public Powers power;
    public Color color;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.tag.ToLower().Contains("wall")) {
            Destroy(gameObject);
        }
    }
}
