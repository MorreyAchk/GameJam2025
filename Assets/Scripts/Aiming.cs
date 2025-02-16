using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Aiming : NetworkBehaviour
{
    [Header("Aiming")]
    public Transform playerTransform;
    public float gunDistance = 1.5f;
    [SerializeField] private SpriteRenderer gunSprite;
    private readonly NetworkVariable<bool> isFacingRightNetwork = new(true);
    private float aimingAngle;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    [Header("Shooting")]
    public float gunRadius = 0.5f;
    public Transform shootingPoint;
    public GameObject bulletPrefab;
    public float bulletForce = 7f;
    public Color color;
    public Powers power;
    public LayerMask groundLayer;
    public ParticleSystem shootingParticles;
    [SerializeField] private Cooldown cooldown;

    [Header("Trajectory")]
    public int maxBounces = 5;
    public float maxDistance = 20f;

    private readonly List<Vector2> pointsOfReflection = new();
    private Vector2 mousePosition;

    private new LineRenderer renderer;
    private Controller2d controller;
    private Vector2 direction;
    private BulletEffects bulletEffects;
    private PlayerSpawner playerSpawner;
    private void Start()
    {
        renderer = GetComponent<LineRenderer>();
        bulletEffects = GetComponentInParent<BulletEffects>();  
        controller = playerTransform.GetComponent<Controller2d>();
        isFacingRightNetwork.OnValueChanged += OnFacingDirectionChanged;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerSpawner = FindObjectOfType<PlayerSpawner>();
    }


    public override void OnDestroy()
    {
        isFacingRightNetwork.OnValueChanged -= OnFacingDirectionChanged;
    }

    public void Update()
    {
        if (bulletEffects.isInBubble.Value) {
            gunSprite.enabled = false;
            return;
        }

        if (gunSprite.enabled == false)
            gunSprite.enabled = true;

        if (IsOwner)
        {
            ShootBullet();
            MoveGun();
            Trajectory();
            if (playerSpawner.isSceneChaniging.Value)
                return;
            SentAngleAndRotationToServerRpc(aimingAngle, transform.rotation);
        }
        else {
            transform.position = networkPosition;
            transform.rotation = networkRotation;
        }
    }

    #region Moving
    private void MoveGun() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - playerTransform.position;

        Quaternion newRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        aimingAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.position = playerTransform.position + Quaternion.Euler(0, 0, aimingAngle) * new Vector3(gunDistance, 0, 0);
        transform.rotation = newRotation;

        GunFlipController(mousePos);
    }

    [ServerRpc]
    private void SentAngleAndRotationToServerRpc(float angle,Quaternion rotation)
    {
        SentAngleAndRotationFromClientRpc(angle, rotation);
    }

    [ClientRpc]
    private void SentAngleAndRotationFromClientRpc(float angle, Quaternion rotation)
    {
        if (IsOwner)
            return;

        networkPosition = playerTransform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunDistance, 0, 0);
        networkRotation = rotation;
    }

    private void OnFacingDirectionChanged(bool oldValue, bool newValue)
    {
        gunSprite.flipY = !newValue;
    }

    private void GunFlipController(Vector3 mousePos)
    {
        bool isFacingRight = mousePos.x > transform.position.x;

        if (isFacingRight != isFacingRightNetwork.Value)
        {
            if (IsServer)
            {
                isFacingRightNetwork.Value = isFacingRight;
            }
            else
            {
                UpdateFacingDirectionServerRpc(isFacingRight);
            }
        }
    }

    [ServerRpc]
    private void UpdateFacingDirectionServerRpc(bool newFacingRight)
    {
        isFacingRightNetwork.Value = newFacingRight;
    }

    #endregion

    #region Aiming
    private bool IsInGround()
    {
        return Physics2D.OverlapCircle(shootingPoint.position, gunRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(shootingPoint.position, gunRadius);
    }

    private void Trajectory()
    {
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
               groundLayer);

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

        if (pointsOfReflection.Count > 0 && Input.GetMouseButton(0))
        {
            renderer.positionCount = pointsOfReflection.Count;
            renderer.SetPositions(
                pointsOfReflection
                .Select(v => new Vector3(v.x, v.y, 0))
                .ToArray());
        }
        else
        {
            renderer.positionCount = 0; // Clear the line if no points
        }
    }

    #endregion

    #region Shooting
    private void ShootBullet()
    {
        if (cooldown.IsCoolingDown)
            return;
        if (Input.GetMouseButtonDown(1) && !IsInGround())
        {
            cooldown.StartCooldown();
            SpawnBulletServerRpc();
        }
    }
    [ClientRpc]
    private void ShootingParticlesClientRpc() {
        shootingParticles.Play();
    }

    [ServerRpc]
    private void SpawnBulletServerRpc()
    {
        Vector2 direction = (mousePosition - new Vector2(shootingPoint.position.x, shootingPoint.position.y)).normalized;
        ShootingParticlesClientRpc();
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        bullet.GetComponent<NetworkObject>().Spawn();

        BulletTrigger bi = bullet.GetComponent<BulletTrigger>();
        bi.Set(power, color);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletForce;
    }

    #endregion
}
