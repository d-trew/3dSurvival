using System;
using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    public LayerMask layerMask;
    public bool grounded;
    public float jumpForce = 5f;
    public float rotationSpeed = 720f;
    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Grounded();
        Jump();
        Move();
        Dance();
    }

    private void Move()
    {
        // Capture player input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Create movement vector
        Vector3 movement = this.transform.forward * vertical + this.transform.right * horizontal;
        movement.Normalize();

        // multiplier depends on animation
        this.transform.position += movement * 0.1f;

        this.anim.SetFloat("vertical", vertical);
        this.anim.SetFloat("horizontal", horizontal);
    }

    private void Grounded()
    {
        if (Physics.CheckSphere(this.transform.position + Vector3.down, 0.2f, layerMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        this.anim.SetBool("jump", !this.grounded);
    }

    private void Jump()
    {
        // Check if the player is on the ground and has pressed the jump button
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Apply a force to make the player jump
            this.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Dance()
    {
        // Check if the 'Y' key is pressed to trigger a dance
        if (Input.GetKeyDown(KeyCode.Y))
        {
            anim.SetTrigger("Dance");
        }
    }
}
