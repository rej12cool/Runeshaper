using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Controller2D target; // Controller2D reference for the object that will be followed with the camera
    public Vector2 focusAreaSize; // Size of the focus area that contains the target
    public float verticalOffset; // Amount by which the camera should auto-translate up/down as a baseline
    public float lookAheadDistX; // Amount the camera offsets in the x dir when the player is moving in the x dir
    public float lookSmoothTimeX; // Time it takes for the camera to move to target position in x direction
    public float verticalSmoothTime; // Time it takes for the camera to move to target position in x direction

    private FocusArea focusArea; // Reference to the focus area
    private float currentLookAheadX; // Current position the camera is looking ahead
    private float targetLookAheadX; // Target position the camera should go to for looking ahead
    private float lookAheadDirectionX; // Direction (left or right) of looking
    private float smoothLookVelocityX; // Rate at which the camera smooths in the x direction
    private float smoothLookVelocityY; // Rate at which the camera smooths in the y direction

    private bool lookAheadStopped;

    void Start()
    {
        focusArea = new FocusArea(target.GetComponent<BoxCollider2D>().bounds, focusAreaSize);
    }

    // Use LateUpdate for camerawork
    void LateUpdate()
    {
        // Update the focus area based on the target's new position/bounds
        focusArea.Update(target.GetComponent<BoxCollider2D>().bounds);
        // The focus position is the center of the area plus a vertical offset
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            // Get the direction that camera should be moving in based on the sign of the velocity
            lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);
            // Check if the target is moving in the same direction as the focus area (which occurs at the edge)
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                // This means that the camera needs to continue to move in that direction
                targetLookAheadX = lookAheadDirectionX * lookAheadDistX;
                lookAheadStopped = false;
            }
            else
            {
                // This means that the target is stopped or moving in the opposite direction from the camera
                if (!lookAheadStopped)
                {
                    // This means that the camera still needs to move so recalculate what the new position will be
                    targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDirectionX - currentLookAheadX) / 4f;
                    lookAheadStopped = true;
                }
            }
        }

        // Smooth the movement from the current amount that the camera is looking ahead to the target value
        currentLookAheadX =
            Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        // Smooth the movement in the y direction for the centered + offset position based on the current position
        focusPosition.y =
            Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothLookVelocityY, verticalSmoothTime);
        // Move the camera to the left/right based on how far it should currently be looking ahead
        focusPosition += Vector2.right * currentLookAheadX;

        // Move the camera back in space
        transform.position = (Vector3) focusPosition + Vector3.forward * -10;
    }

    // Draw a slightly transparent red square around focusArea
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    // Struct that has information about the focus area, which is where the 
    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        private float left, right;
        private float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            // Calculate the coordinates of the sides of the focus area
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.max.y + size.y;

            velocity = Vector2.zero;
            // The center is the average of the bounds
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            // Determine what the new bounds should be if the target moves to the left or right too much
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            // Add the shift to the left and right bounds of the focus area
            left += shiftX;
            right += shiftX;

            // Repeat for vertical changes
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            bottom += shiftY;
            top += shiftY;

            // Recalculate the center and determine the velocity based on the shift
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}