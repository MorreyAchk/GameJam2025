using UnityEngine;
using UnityEditor;
using Unity.Netcode;

public class BulletTrigger : NetworkBehaviour
{
    public Powers power;
    public Color color;
    public int maxBounces = 8;
    private int bounceCounter;
    private Rigidbody2D rb;
    private Vector3 networkPosition;
    private float networkRotationAngle;
    private NetworkObject networkObject;

    [HideInInspector] public Transform graphics;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        networkObject = GetComponent<NetworkObject>();
    }

    public void Set(Powers power, Color color)
    {
        this.power = power;
        this.color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
            return;
        bounceCounter++;
        if (bounceCounter == maxBounces)
        {
            DespawnBullet();
        }
    }

    public void DespawnBullet() {
        networkObject.Despawn(true);
    }

    private void Update()
    {
        if (IsServer) {
            SentPositionFromClientRpc(transform.position);
            return;
        }
        transform.position = networkPosition;
    }

    [ClientRpc]
    private void SentPositionFromClientRpc(Vector3 position)
    {
        networkPosition = position;
    }

    [ClientRpc]
    private void SentAngleFromClientRpc(float angle)
    {
        networkRotationAngle = angle;
    }

    private void FixedUpdate()
    {
        if (power == Powers.Wind && graphics)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            if (IsServer) {
                graphics.rotation = Quaternion.Euler(0, 0, angle);
                SentAngleFromClientRpc(angle);
                return;
            }
            graphics.rotation = Quaternion.Euler(0, 0, networkRotationAngle);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BulletTrigger))]
public class BulletTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target object
        BulletTrigger bulletTrigger = (BulletTrigger)target;

        // Draw the default fields
        DrawDefaultInspector();

        // Conditionally show the graphics field if the power is Wind
        if (bulletTrigger.power == Powers.Wind)
        {
            bulletTrigger.graphics = (Transform)EditorGUILayout.ObjectField("Graphics", bulletTrigger.graphics, typeof(Transform), true);
        }
    }
}
#endif