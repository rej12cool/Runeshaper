using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController player;
    private Vector2 respawnPosition;

    void Start()
    {
        // Get reference to the PlayerController script
        player = GetComponent<PlayerController>();
        respawnPosition = GameObject.FindWithTag("Respawn").transform.position;
    }

    void Update()
    {
        // Get input 
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Specify which direction the player is going to move
        player.SetDirectionalInput(directionalInput);

        if (Input.GetButton("Jump"))
        {
            player.OnJumpInput();
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKey("r"))
        {
            player.transform.position = respawnPosition;
        }
    }
}