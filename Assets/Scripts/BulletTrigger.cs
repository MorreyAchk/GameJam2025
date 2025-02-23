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
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        if (IsServer)
        {
            MoveAlongTrajectory();
            SentPositionFromClientRpc(transform.position);
            return;
        }
        else {
            transform.SetPositionAndRotation(networkPosition, Quaternion.Euler(0, 0, networkRotationAngle));
        }
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
            SentAngleFromClientRpc(angle);
        }

        transform.position = Vector2.MoveTowards(transform.position, target, bulletSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.1f)
            currentPointIndex++;
    }


    //[ClientRpc]
    //private void RicochetSoundClientRpc() => RicochetSound();
    //private void RicochetSound() => audioSource.PlayOneShot(audioSource.clip);

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