using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
  public Transform gun;
  public float gunDistance = 1.5f;

  private bool gunFacingRight;

  public void Update()
  {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 direction = mousePos - transform.position;

    gun.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    gun.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunDistance, 0, 0);

    GunFlipController(mousePos);
  }

  private void GunFlipController(Vector3 mousePos)
  {
    if (mousePos.x < gun.position.x && gunFacingRight)
      GunFlip();
    else if (mousePos.x > gun.position.x && !gunFacingRight)
      GunFlip();
  }

  private void GunFlip()
  {
      gunFacingRight = !gunFacingRight;
      gun.localScale = new Vector3(gun.localScale.x, gun.localScale.y * -1, gun.localScale.z);
  }


}
