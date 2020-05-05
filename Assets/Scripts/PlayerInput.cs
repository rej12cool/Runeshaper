using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    Animator animator;
    private PlayerController player;
    private Vector2 respawnPosition;
    private CameraFollow mainCamera;

    // Boolean that determines whether or not the player is in rune placement mode (astral form)
    private bool projecting = false;
    // The astral form of the player
    private GameObject astralForm;
    // Bool used to determine if the F event has already been handled and
    // any further pressing should be ignored
    private bool pressingF = false;

    void Start()
    {
        // Get reference to the PlayerController script
        player = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        respawnPosition = GameObject.FindWithTag("Respawn").transform.position;
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>();
        astralForm = GameObject.FindWithTag("AstralForm");
    }

    void Update()
    {
        // Default animation settings
        animator.SetBool("idle", true);
        animator.SetBool("move_right", false);
        animator.SetBool("move_left", false);
        animator.SetBool("jump", false);


        // Reset level signal
        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Move the player if not projecting
        if (!projecting)
        {
            // Get axis input 
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // Specify which direction the player is going to move
            player.SetDirectionalInput(directionalInput);

            if (Input.GetButton("Jump"))
            {
                player.OnJumpInput();
                animator.SetBool("jump", true);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool("move_right", true);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool("move_left", true);
            }
        }

        // Astral project key
        if (Input.GetKey("f") && !pressingF)
        {
            pressingF = true;

            // If not currently in astral form, go into astral form
            if (!projecting)
            {
            	// Nullify player's movement
            	player.SetDirectionalInput(new Vector2(0f,0f));

                astralForm.transform.position = player.transform.position + new Vector3(0f, 1f, 0f);
                astralForm.GetComponent<AstralProject>().Begin();
                // Focus camera on astral form
                mainCamera.target = astralForm.GetComponent<Controller2D>();

                projecting = true;
            }
            // Otherwise, come out of astral form
            else
            {
                // Tell astral form that it should stop
                astralForm.GetComponent<AstralProject>().End();

                // Focus camera on player
                mainCamera.target = GetComponent<Controller2D>();

                projecting = false;
            }
        }
        else if (!(Input.GetKey("f")))
        {
            pressingF = false;
        }
    }
}