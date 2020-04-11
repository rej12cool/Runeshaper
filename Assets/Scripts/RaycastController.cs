using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask; // LayerMask that specifies the layers in which the boxCollider interacts
    public RaycastOrigins raycastOrigins;

    protected const float SkinWidth = .015f; // Define inset distance into the bounds to start with raycasting
    protected const float DistanceBetweenRays = .25f; // Set distance between adjacent rays

    protected BoxCollider2D boxCollider; // Reference to Game Object's boxCollider
    protected float horizontalRaySpacing, verticalRaySpacing; // Calculated spacing between rays
    protected int horizontalRayCount, verticalRayCount; // Numbers of rays on a face

    public virtual void Awake()
    {
        // Get boxCollider object in awake to avoid issues with order of getting components in Start
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void CalculateRaySpacing()
    {
        // Get the bounds of the object and move them inwards by the SkinWidth for better spacing for raycasting
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-2 * SkinWidth);

        // Calculate the number of rays based on the size of the bounds and the spacing
        horizontalRayCount = Mathf.RoundToInt(bounds.size.y / DistanceBetweenRays);
        verticalRayCount = Mathf.RoundToInt(bounds.size.x / DistanceBetweenRays);

        // Recalculate the ray spacing to avoid math issues
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    public void UpdateRayCastOrigins()
    {
        // Get the bounds of the object and move them inwards by the SkinWidth for better spacing for raycasting
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-2 * SkinWidth);

        // Specify the origins of the raycasts by the corners of the modified bounds
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // Struct that holds the information about the corners of the boxCollider, which translates to the origin of the Raycasts
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}