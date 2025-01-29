using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Shooting : NetworkBehaviour
{
    public float gunRadius = 0.5f;
    public Transform shootingPoint;
    public GameObject bulletPrefab;
    public float bulletForce = 7f;
    public Color color;
    public Powers power;
    public LayerMask groundLayer;

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

        if (Input.GetMouseButtonDown(1) && !IsInGround())
        {
            SpawnBulletServerRpc(shootingPoint.position, shootingPoint.rotation);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        BulletTrigger bi = bullet.GetComponent<BulletTrigger>();
        bi.Set(power, color); // Use a method to set synced properties

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = shootingPoint.right * bulletForce;
    }

}
