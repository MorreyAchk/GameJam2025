using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingKeys : MonoBehaviour
{
    private Vector3 axis;
    public Transform gun;
    public AimTrajectoryKeys trajectory;

    public void Update()
    {
        if (Input.GetKey(KeyCode.Q))
          axis = new(0, 0, 1);
        else if (Input.GetKey(KeyCode.E))
          axis = new(0, 0, -1);
        else axis = default;

        if (axis != default)
          gun.RotateAround(transform.position, axis, Time.deltaTime * 200);
        trajectory.Fire(gun);
    }
}
