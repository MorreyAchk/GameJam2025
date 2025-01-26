using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTrajectory : MonoBehaviour
{
    private int mask;

    public int maxBounces = 5;
    public float maxDistance = 20f;

    private List<Vector2> pointsOfReflection = new List<Vector2>();
    private Vector2 playerPosition, mousePosition;

    public new LineRenderer renderer;
    [SerializeField]private Controller2d controller;

    public void Start()
    {
        mask = LayerMask.GetMask("Ground");
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerPosition = transform.position;
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pointsOfReflection.Clear();
            pointsOfReflection.Add(playerPosition);

            int bounceCount = 0;
            Vector2 direction = (mousePosition - playerPosition).normalized;

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
