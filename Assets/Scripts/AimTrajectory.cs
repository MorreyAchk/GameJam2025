using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AimTrajectory : NetworkBehaviour
{
    private int mask;

    public int maxBounces = 5;
    public float maxDistance = 20f;

    public Transform gunShootTransform;
    private List<Vector2> pointsOfReflection = new List<Vector2>();
    private Vector2 mousePosition;

    private Aiming aiming;
    private new LineRenderer renderer;
    [SerializeField] private Controller2d controller;
    public Vector2 direction;

    public void Start()
    {
        aiming = GetComponent<Aiming>();
        renderer = GetComponent<LineRenderer>();
        mask = LayerMask.GetMask("Ground");
    }

    public void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pointsOfReflection.Clear();
            pointsOfReflection.Add(gunShootTransform.position);

            int bounceCount = 0;
            direction = (mousePosition - new Vector2(gunShootTransform.position.x, gunShootTransform.position.y)).normalized;

            while (bounceCount < maxBounces)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    pointsOfReflection[pointsOfReflection.Count - 1] + direction * 0.01f, // Offset to avoid self-hit
                    direction,
                    maxDistance,
                    mask);

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
        }

        if (controller.isMoving)
        {
            pointsOfReflection.Clear();
        }

        if (pointsOfReflection.Count > 0)
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
}
