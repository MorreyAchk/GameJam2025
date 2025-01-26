using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform gun;
    public GameObject bulletPrefab;
    public float bulletForce=7f;
    public Color color;
    public Powers power;

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
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
