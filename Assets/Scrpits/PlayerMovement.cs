using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;

    private bool isGrounded;

    // Animator for handling animations
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Right is the red axis, forward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;

        // Move the player character
        controller.Move(move * speed * Time.deltaTime);

        // Set animation parameters
        animator.SetFloat("Speed", move.magnitude);

        // Check if the player is on the ground and can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // The equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump"); // Trigger jump animation
        }

        velocity.y += gravity * Time.deltaTime;

        // Move the player with gravity applied
        controller.Move(velocity * Time.deltaTime);
    }
}
