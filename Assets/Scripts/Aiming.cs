using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Aiming : NetworkBehaviour
{
    public Transform gun;
    public float gunDistance = 1.5f;

    private bool facingLeft = false;
    [SerializeField] private SpriteRenderer gunSprite;

    public void Update()
    {
        if (!IsOwner)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;

        gun.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunDistance, 0, 0);

        GunFlipController(mousePos);
    }

    private void GunFlipController(Vector3 mousePos)
    {
        bool shouldFaceLeft = mousePos.x < gun.position.x;

        if (shouldFaceLeft != facingLeft)
        {
            facingLeft = shouldFaceLeft;
            gunSprite.flipY = facingLeft;
        }
    }
}
