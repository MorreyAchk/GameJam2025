using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class BulletTrigger : NetworkBehaviour
{
    public Powers power;
    public Color color;
    public int maxBounces=8;
    private int bounceCounter;
    private NetworkObject networkObject;

    private void Start()
    {
        networkObject = GetComponent<NetworkObject>();  
    }

    public void Set(Powers power,Color color) {
        this.power = power;
        this.color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceCounter++;
        if (!collision.collider.tag.ToLower().Contains("wall") || bounceCounter == maxBounces) {
            networkObject.Despawn();
        }
    }
}
