using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shooting : NetworkBehaviour
{
    public Transform gun;
    public float gunRadius=0.5f;
    public GameObject bulletPrefab;
    public float bulletForce=7f;
    public Color color;
    public Powers power;
    public LayerMask groundLayer;

    private bool IsInGround()
    {
        return Physics2D.OverlapCircle(gun.position, gunRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gun.position, gunRadius);
    }


    public void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetMouseButtonDown(1) && !IsInGround())
        {
            GameObject bullet = Instantiate(bulletPrefab, gun.position, gun.rotation);
            BulletTrigger bi = bullet.GetComponent<BulletTrigger>();
            bi.color = color;
            bi.power = power;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = transform.right * bulletForce;
            Destroy(bullet, 5);
        }
    }
}
