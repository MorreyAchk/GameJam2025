using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Shooting : NetworkBehaviour
{
    [Header("Shooting")]
    public float gunRadius = 0.5f;
    public Transform shootingPoint;
    public GameObject bulletPrefab;
    public float bulletForce = 7f;
    public Color color;
    public Powers power;
    public LayerMask groundLayer;

    [Header("Trajectory")]
    public int maxBounces = 5;
    public float maxDistance = 20f;

    private List<Vector2> pointsOfReflection = new ();
    private Vector2 mousePosition;

    private new LineRenderer renderer;
    [SerializeField] private Controller2d controller;
    private Vector2 direction;

    private void Start()
    {
        renderer = GetComponent<LineRenderer>();
    }

    private bool IsInGround()
    {
        return Physics2D.OverlapCircle(shootingPoint.position, gunRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(shootingPoint.position, gunRadius);
    }


    private void Update()
    {
        if (!IsOwner)
            return;

        Trajectory();
        if (Input.GetMouseButtonDown(1) && !IsInGround())
        {
            if (IsServer)
            {
                SpawnBulletClientRpc(shootingPoint.position);
            }
            else {
                SpawnBulletServerRpc(shootingPoint.position);
                InstantiateBullet(shootingPoint.position);
            }
        }
    }

    private void Trajectory() {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pointsOfReflection.Clear();
        pointsOfReflection.Add(shootingPoint.position);

        int bounceCount = 0;
        direction = (mousePosition - new Vector2(shootingPoint.position.x, shootingPoint.position.y)).normalized;

        while (bounceCount < maxBounces)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                pointsOfReflection[pointsOfReflection.Count - 1] + direction * 0.01f, // Offset to avoid self-hit
                direction,
            maxDistance,
                LayerMask.GetMask("Ground"));

            if (hit.collider != null)
            {
                if (Vector2.Distance(pointsOfReflection[pointsOfReflection.Count - 1], hit.point) > 0.1f)
                {
                    pointsOfReflection.Add(hit.point);
                    Vector2 reflection = Vector2.Reflect(direction, hit.normal);
                    direction = reflection.normalized;
                    bounceCount++;
                }
                else break; // Stop if the hit is too close to avoid dense points
            }
            else break; // Stop if no collision
        }

        if (controller.isMoving)
        {
            pointsOfReflection.Clear();
        }

        if (pointsOfReflection.Count > 0)
        {
            if (Input.GetMouseButton(0)) {
                renderer.positionCount = pointsOfReflection.Count;
                renderer.SetPositions(
                    pointsOfReflection
                    .Select(v => new Vector3(v.x, v.y, 0))
                    .ToArray());
            }
        }
        else
        {
            renderer.positionCount = 0; // Clear the line if no points
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector3 position) => InstantiateBullet(position);

    [ClientRpc]
    private void SpawnBulletClientRpc(Vector3 position) => InstantiateBullet(position);

    private void InstantiateBullet(Vector3 position)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);

        BulletTrigger bi = bullet.GetComponent<BulletTrigger>();
        bi.Set(power, color); // Use a method to set synced properties

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Vector2 firstDirection = (pointsOfReflection[1] - pointsOfReflection[0]).normalized;

        rb.velocity = firstDirection * bulletForce;
    }

}
