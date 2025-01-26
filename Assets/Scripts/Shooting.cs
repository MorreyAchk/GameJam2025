using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform gun;
    public GameObject bulletPrefab;
    public Color color;
    public string skill;

    public void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        GameObject bullet = Instantiate(bulletPrefab, gun.position, gun.rotation);
        BulletTrigger bi = bullet.GetComponent<BulletTrigger>();
        bi.color = color;
        bi.skill = skill;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * 5;
        Destroy(bullet, 5);
      }
    }
}
