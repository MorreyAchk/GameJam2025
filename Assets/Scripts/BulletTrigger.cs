using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using System.Collections.Generic;

public class BulletTrigger : NetworkBehaviour
{
    public Powers power;
    public Color color;
    public Vector2 direction;
    public float bulletSpeed;
    private List<Vector2> trajectoryPoints;
    private int bounceCounter, currentPointIndex;
    private Vector3 networkPosition;
    private float networkRotationAngle;
    private NetworkObject networkObject;

    [HideInInspector] public Transform graphics;

    private void Start()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    public void Set(Powers power, Color color, List<Vector2> trajectoryPoints)
    {
        this.power = power;
        this.color = color;
        this.trajectoryPoints = trajectoryPoints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
            return;

        bounceCounter++;
        if (bounceCounter == trajectoryPoints.Count)
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
            MoveAlongTrajectory();
            SentPositionFromClientRpc(transform.position);
            return;
        }
        transform.position = networkPosition;
    }

    private void MoveAlongTrajectory()
    {
        if (currentPointIndex >= trajectoryPoints.Count)
        {
            DespawnBullet();
            return;
        }

        Vector2 target = trajectoryPoints[currentPointIndex];
        direction = target - (Vector2)transform.position;

        if (direction.sqrMagnitude > Mathf.Epsilon)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.position = Vector2.MoveTowards(transform.position, target, bulletSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            currentPointIndex++;
            if (power == Powers.Wind && graphics)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (IsServer)
                {
                    graphics.rotation = Quaternion.Euler(0, 0, angle);
                    SentAngleFromClientRpc(angle);
                    return;
                }
                graphics.rotation = Quaternion.Euler(0, 0, networkRotationAngle);
            }
        }
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