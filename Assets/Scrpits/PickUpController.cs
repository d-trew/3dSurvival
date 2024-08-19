using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GunSystem gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;
    public Animator playerAnimator; // Reference to the player's Animator

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private int defaultLayerIndex = 0; // Index of the default animation layer (unarmed)
    private int weaponLayerIndex = 1; // Index of the animation layer for holding a weapon

    private void Start()
    {
        // Setup
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
            // Ensure the weapon layer is inactive and default layer is active at the start
            playerAnimator.SetLayerWeight(weaponLayerIndex, 0);
            playerAnimator.SetLayerWeight(defaultLayerIndex, 1);
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
            // Activate weapon layer and deactivate default layer if already equipped at start
            playerAnimator.SetLayerWeight(weaponLayerIndex, 1);
            playerAnimator.SetLayerWeight(defaultLayerIndex, 0);
        }
    }

    private void Update()
    {
        // Check if player is in range and "E" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        // Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        // Make weapon a child of the camera and move it to default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        // Make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;

        // Enable script
        gunScript.enabled = true;

        // Switch to the weapon animation layer
        playerAnimator.SetLayerWeight(1, 1f);

    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        // Set parent to null
        transform.SetParent(null);

        // Make Rigidbody not kinematic and BoxCollider normal
        rb.isKinematic = false;
        coll.isTrigger = false;

        // Gun carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        // AddForce
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        // Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        // Disable script
        gunScript.enabled = false;

        // Switch back to the default animation layer
        playerAnimator.SetLayerWeight(1, 0f);
    }
}
