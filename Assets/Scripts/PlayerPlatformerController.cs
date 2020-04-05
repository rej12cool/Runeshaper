using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    //private Animator animator;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        // Get input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Check if the jump command was given
        if (Input.GetButtonDown("Jump"))
        {
            // If the player is grounded, add the jump velocity
            if (grounded)
            {
                velocity.y = jumpTakeOffSpeed;
            }
            // Otherwise, half the velocity?
            else if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        // Flip the sprite if moving left vs. right
        if (input.x != 0)
        {
            spriteRenderer.flipX = input.x == 1;
        }

        //animator.SetBool("grounded", grounded);
        //animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = input * maxSpeed;
    }
}