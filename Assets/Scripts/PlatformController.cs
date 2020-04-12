using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask; // Layer for the passenger (whose velocity will be affected)
    public Vector3[] localWaypoints; // Waypoints relative to the platform's starting position - can be edited

    [Range(0, 2)]
    public float easeAmount; // Amount by which the platform's movement accelerates/decelerates
    public float speed; // Speed of the platform, on average
    public float waitTime; // Amount of time the platform pauses at each waypoint
    public bool cyclic; // True if the platform moves back to the first waypoint vs. reverses order

    // Dictionary that keeps track of the passenger's controller to avoid using GetComponent too often
    private readonly Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();
    private Vector3[] globalWaypoints; // Coordinates of the local waypoints in global scale
    private List<PassengerMovement> passengerMovements; // List of the passengers' structs on the platform

    private float percentBetweenWayPoints; // Variable to keep track of how far between waypoints the platform is
    private float nextMoveTime; // Variable to keep track of when the platform should next move
    private int fromWayPointIndex; // Index of the waypoint that the platform just left from
    
    public override void Start()
    {
        base.Start();

        // Calculate the global position of the waypoints
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    void FixedUpdate()
    {
        // Recalculate the origin of each ray because the platform is moving
        UpdateRayCastOrigins();

        // Get the velocity of the platform
        Vector3 velocity = CalculatePlatformMovement() * Time.deltaTime;

        // Determine how much the passengers should move
        CalculatePassengerMovement(velocity);
        // Move the passengers if they should move first
        MovePassengers(true);
        // Move the platform
        transform.Translate(velocity);
        // Move the passengers if the platform should move first
        MovePassengers(false);
    }

    Vector3 CalculatePlatformMovement()
    {
        // If not enough time has moved before the next move time, don't move and escape
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        // Get the previous waypoint index that the platform was at
        fromWayPointIndex %= globalWaypoints.Length;

        // Get the destination waypoint's index
        int toWayPointIndex = (fromWayPointIndex + 1) % globalWaypoints.Length;

        // Calculate the values about being in between the two waypoints
        float distanceBetweenWaypoints =
            Vector3.Distance(globalWaypoints[fromWayPointIndex], globalWaypoints[toWayPointIndex]);
        percentBetweenWayPoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWayPoints = Mathf.Clamp01(percentBetweenWayPoints); // Percentage must between 0 and 1
        // Recalculate the percentage based on the easing curve in the Ease function
        float easedPercentBetweenWaypoints = Ease(percentBetweenWayPoints);

        // Linearly interpolate between the two waypoints based on the percentage
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWayPointIndex], globalWaypoints[toWayPointIndex],
            percentBetweenWayPoints);

        // Check if the platform has reached the destination waypoint
        if (percentBetweenWayPoints >= 1)
        {
            // Reset the percentage and increase the indices
            percentBetweenWayPoints = 0;
            fromWayPointIndex++;
            if (fromWayPointIndex >= globalWaypoints.Length - 1 && !cyclic)
            {
                // Reached end of waypoints. If not cyclic moving platform, then reverse the order of the array
                fromWayPointIndex = 0;
                System.Array.Reverse(globalWaypoints);
            }

            // Calculate when the platform should move next
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    // Function to perform easing of the platform movement
    float Ease(float x)
    {
        float a = easeAmount + 1;
        // This function still maps 0, .5, and 1 to itself; but adds curvature based on the value a
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        // Create a HashSet to prevent duplicates in the passengers, since they may be detected multiple times
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        // Reset the list of passenger movements
        passengerMovements = new List<PassengerMovement>();

        // Get the directions of the movement
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // If the platform is moving vertically
        if (velocity.y != 0)
        {
            // The length of the ray should be how far it will move in the next frame and the SkinWidth inset
            float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                // The rayOrigin will be the on the bottom if the platform is moving down or on the top if moving up
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                // Move the x coordinate of the ray origin so they are evenly spaced along the top or bottom edge
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                // Get the hit information after raycasting
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (hit)
                {
                    // If the passenger is hit and has not already been moved
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        // Add the transform to the list of passengers that were moved
                        movedPassengers.Add(hit.transform);

                        // Determine how much the passenger should move
                        // If the platform is moving diagonally and the passenger is standing on top of the platform,
                        // make the passenger move with the additional velocity of the platform
                        // Otherwise, make it 0 (if the passenger is below the platform)
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        // Move the passenger by the amount the platform is moving with the information from the ray
                        float pushY = velocity.y - (hit.distance - SkinWidth) * directionY;

                        // Add new passenger movement struct instance - standingOnPlatform is true when moving up
                        passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY),
                            directionY == 1, true));
                    }
                }
            }
        }

        // If the platform is moving horizontally
        if (velocity.x != 0)
        {
            // The length of the ray should be how far it will move in the next frame and the SkinWidth inset
            float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                // Do the same rayOrigin check, but using the left or right side and going up
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                // Create a ray i * spacing up the side of the bounds
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                // Get hit information
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit)
                {
                    // If the passenger has not been moved yet
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        // Move the passenger by its velocity without the difference between the distance of the
                        // passenger and the SkinWidth
                        float pushX = velocity.x - (hit.distance - SkinWidth) * directionX;
                        // Move down by the SkinWidth
                        float pushY = -SkinWidth;

                        // Add new passenger movement struct instance - standingOnPlatform is true when moving up
                        passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY),
                            false, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            // Make the ray slightly larger than the SkinWidth
            float rayLength = 2 * SkinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        // Move the passenger with the same velocity as the platform
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        // Here, this indicates that the platform should move first
                        passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY),
                            true, false));
                    }
                }
            }
        }
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        // Add movement to each passenger that was detected
        foreach (PassengerMovement passenger in passengerMovements)
        {
            // Only add a passenger's transform and controller once if it is not already present in the dictionary
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }

            // Determine if the passenger should move based on its position relative to the platform
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    // Struct to account for the movement of the passenger while affected by the platform
    struct PassengerMovement
    {
        public Transform transform; // Transform of the passenger
        public Vector3 velocity; // Velocity that the passenger will move due to the platform
        public bool standingOnPlatform;
        public bool moveBeforePlatform; // Check whether the passenger or the platform should move first

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform,
            bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }


    }

    // Draw extra lines based on the rays, which represent the interactions based on the direction of input
    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWayPointPosition = (Application.isPlaying) ? globalWaypoints[i] :
                    localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWayPointPosition - Vector3.up * size,
                    globalWayPointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWayPointPosition - Vector3.left * size,
                    globalWayPointPosition + Vector3.left * size);
            }
        }
    }
}
