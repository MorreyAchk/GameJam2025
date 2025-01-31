using UnityEngine;
using UnityEditor;

public class BulletTrigger : MonoBehaviour
{
    public Powers power;
    public Color color;
    public int maxBounces = 8;
    private int bounceCounter;
    private Rigidbody2D rb;

    [HideInInspector] public Transform graphics;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Set(Powers power, Color color)
    {
        this.power = power;
        this.color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceCounter++;
        if (!collision.collider.tag.ToLower().Contains("wall") || bounceCounter == maxBounces)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (power == Powers.Wind) {
            // Rotate the graphics to face the movement direction
            if (rb.velocity.sqrMagnitude > 0.01f) // Ensure there's some movement
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                graphics.rotation = Quaternion.Euler(0, 0, angle);
            }
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