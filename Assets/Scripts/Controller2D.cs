using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    public CollisionInfo collisions; // Reference to the collision information from the collider
    public Vector2 playerInput; // Vector representing the input for the player - used for the camera

    private const float MaxClimbAngle = 80; // Max angle the object can climb
    private const float MaxDescendAngle = 75; // Max angle the object can descend without falling off
    private const float HoistSpeed = .05f; // Speed at which the object climbs up a wall over an edge
    private const float PostHoistSpeed = .1f; // Speed at which the object moves after finishing hoisting

    public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        UpdateRayCastOrigins();

        // Do resetting for the collision info
        collisions.Reset();
        collisions.oldMoveAmount = moveAmount;

        // Check for slope descent if moving down
        if (moveAmount.y < 0)
            CheckSlopeDescent(ref moveAmount);

        // Check collisions if moving in either vertical or horizontal direction
        if (moveAmount.x != 0)
            CheckHorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
            VerticalCollisions(ref moveAmount);

        // If finished hoisting, move the player a little bit in the same direction and don't move up/down
        if (collisions.hoistingOld && !collisions.hoisting)
        {
            moveAmount.x = PostHoistSpeed * collisions.hoistDirection;
            moveAmount.y = 0;
        }

        // Perform the actual translation here
        transform.Translate(moveAmount);
        // Sync the movement of transforms to avoid small oscillations in position
        Physics2D.SyncTransforms();

        // Check if the object is standing on a interactable and indicate that there is something below
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    void CheckHorizontalCollisions(ref Vector2 moveAmount)
    {
        // Get direction of movement
        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;

        // Loop through all of the rays that are related to the RaycastController
        for (int i = 0; i < horizontalRayCount; i++)
        {
            // Get the starting position of the ray as it goes up the length of the object
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            // Get info about the hit
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            // Draw a line in the direction of movement to check for collisions
            if (i != horizontalRayCount - 1)
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            }
            else
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green);
            }

            if (hit)
            {
                // No need to calculate anything if the object is at a wall
                if (hit.distance == 0)
                {
                    continue;
                }

                // Get the slope angle from the object in the environment that was hit
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // If it is the bottom ray checking and the slope angle of the incline is less than the maximum
                if (i == 0)
                {
                    // Check for climbing
                    if (slopeAngle <= MaxClimbAngle)
                    {
                        // If previously descending
                        if (collisions.descendingSlope)
                        {
                            // No longer descending
                            collisions.descendingSlope = false;
                            moveAmount = collisions.oldMoveAmount;
                        }

                        // Get the distance to the start of the slope
                        float distanceToSlopeStart = 0;
                        // If the slope angle changes, we are climbing a new slope
                        if (slopeAngle != collisions.slopeAngleOld)
                        {
                            // Add the small distance to the start of the slope to avoid a gap before the slope
                            distanceToSlopeStart = hit.distance - SkinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }

                        // Actually do the moving
                        ClimbSlope(ref moveAmount, slopeAngle);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }
                }

                // Only check the other rays if we are not climbing the slope
                if (!collisions.climbingSlope || slopeAngle > MaxClimbAngle)
                {
                    // Move up to where the collision occurs (if moving left/right would cause a clip into the object)
                    moveAmount.x = (hit.distance - SkinWidth) * directionX;

                    // Make the collisions detected based on direction of movement
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
            else
            {
                // If the top ray had nothing to hit and the rest of it is blocked
                if (i == horizontalRayCount - 1 && (collisions.left && directionX == -1 || collisions.right && directionX == 1))
                {
                    // If on a platform or moving downwards
                    if (collisions.below || Mathf.Sign(moveAmount.y) == -1)
                    {
                        // Move upwards with the hoist speed
                        moveAmount.y = HoistSpeed;
                        // Object is now hoisting
                        collisions.hoisting = true;
                        // Capture direction for post-hoist movement in x
                        collisions.hoistDirection = directionX;
                    }
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + SkinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            // Do raycasting along the top or bottom of the space
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                // If a hit is detected, reduce the movement to the amount it would take the reach the collision
                moveAmount.y = (hit.distance - SkinWidth) * directionY;

                // Change the x move amount to account for collisions that are above while climbing a slope
                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) *
                                   Mathf.Sign(moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope)
        {
            // We want to check if there is a new slope in the horizontal direction to avoid clipping into a new slope
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;
            Vector2 rayOrigin = (directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
                                Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                // Get the angle of the slope that was hit
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                // Then check to see if the new angle is different from the previous one
                if (slopeAngle != collisions.slopeAngle)
                {
                    // Collided with new slope, but will move the new distance to prevent clipping into the new slope
                    moveAmount.x = (hit.distance - SkinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        // Want the speed of the movement to be the same on the incline as on flat ground
        // Perform a small transformation to rotate the movement vector so it is aligned with the slope angle
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbVeloctyY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        // Check if the amount we are moving up while climbing the slope to allow for jumping while moving on the slope
        if (moveAmount.y <= climbVeloctyY)
        {
            moveAmount.y = climbVeloctyY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);

            // Update collisions
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void CheckSlopeDescent(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        // Ray origin is the opposite direction of movement
        Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        // Cast the ray downward, using infinity because we don't know how far down the slope might be
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            // Allow descent if between 0 and the maximum descent angle
            if (slopeAngle != 0 && slopeAngle <= MaxDescendAngle)
            {
                // If moving in the same direction as the normal of the slope
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    // Then we are moving down the slope
                    if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        // Then descend the slope will go into effect
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendVeloctyY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendVeloctyY;

                        // Update collisions
                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    // Struct that contains info about collisions and slopes
    public struct CollisionInfo
    {
        public Vector2 oldMoveAmount;

        public float slopeAngle, slopeAngleOld;
        public float hoistDirection;

        // If any of these 4 bools are true, then there is an object in the direction of the variable name
        public bool above, below;
        public bool left, right;
        public bool climbingSlope, descendingSlope;
        public bool hoisting, hoistingOld;

        // Reset every time
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
            hoistingOld = hoisting;
            hoisting = false;
        }
    }
}