using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{
    public Vector2 velocity;

    private Vector2 directionalInput; // Reference to the input

    private float jumpHeight = 1.3f; // How high the jump is at it's maximum
    private float timeToJumpApex = .3f; // How long it takes to get to the apex of the jump
    private float moveSpeed = 6; // X velocity
    private float fallMultiplier = 1.4f; // How fast the player falls relative to rising (for better jump)
    private float accelerationTimeAirborne = .2f; // How long it takes to reverse direction while in the air
    private float accelerationTimeGrounded = .1f; // How long it takes to reverse direction while on the ground

    private float gravity; // The rate at which the y velocity decreases while player is airborne
    private float jumpVelocity; // Actual velocity that is set to the player when the jump command is given
    private float velocityXSmoothing; // Reference value for smoothing the transition for movement in the x direction

    private Controller2D controller;

    void Awake()
    {
        controller = GetComponent<Controller2D>();
    }

    void Start()
    {
        // Calculate the gravity and jump velocity
        gravity = -2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        // Get input from PlayerInput script
        directionalInput = input;
    }

    // Method called when the jump input is given
    public void OnJumpInput()
    {
        // If the player is grounded, then allow them to add the jump velocity
        if (controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
    }

    void FixedUpdate()
    {
        // First calculate the velocity with no other issues
        CalculateVelocity();

        // Move the player
        controller.Move(velocity * Time.deltaTime);

        // Stop the vertical movement if something is detected above or below
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    // Helper method to calculate the velocity
    void CalculateVelocity()
    {
        // The target velocity is the maximized velocity in the direction of the input
        float targetVelocityX = directionalInput.x * moveSpeed;
        // Make the actual velocity in the x direction smoothed from the current velocity to the target value
        // This rate of smoothing changes whether the player is grounded or airborne
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        // Implement the better jump if the player is moving down. Otherwise just use the gravity.
        velocity.y += (velocity.y < 0 ? fallMultiplier : 1) * gravity * Time.deltaTime;
    }
}